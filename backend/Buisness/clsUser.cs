using System;
using System.Data;
using InventoryDB_DataAccess;


namespace InventoryDB_Business
{
    public class clsUser
    {

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public clsUserData.UserDTO UDTO
        {
            get { return (new clsUserData.UserDTO(this.UserID, this.UserName, this.RoleInfo.Name, this.IsActive)); }
        }

        public int UserID { set; get; }
        public int AccountID { set; get; }
        public int RoleID { set; get; }
        public clsRole RoleInfo;
        public string UserName { set; get; }
        public string PasswordHash { private set; get; }
        public bool IsActive { set; get; }

        public clsUser()
        {
            this.UserID = -1;
            this.UserName = "";
            this.PasswordHash = "";
            this.IsActive = true;
            Mode = enMode.AddNew;
        }

        public clsUser(clsUserData.UserDTO UDTO,int AccountID,string Password, enMode cMode = enMode.AddNew)
        {
            this.UserID = UDTO.Id;
            this.AccountID = AccountID;
            this.RoleID = clsRole.FindByName(UDTO.RoleName).RoleID;
            this.RoleInfo = clsRole.Find(RoleID);
            this.UserName = UDTO.UserName;
            this.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password); ;
            this.IsActive = UDTO.IsActive;
            Mode = cMode;
        }
        private clsUser(int UserID, int AccountID, int RoleID, string UserName,
            string Password, bool IsActive)
        {
            this.UserID = UserID;
            this.AccountID = AccountID;
            this.RoleID = RoleID;
            this.RoleInfo = clsRole.Find(RoleID);
            this.UserName = UserName;
            this.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password); ;
            this.IsActive = IsActive;
            Mode = enMode.Update;
        }

        // Call this before Save() on a new user
        public void SetPassword(string Password)
        {
            this.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
        }

        public bool VerifyPassword(string Password)
        {
            return BCrypt.Net.BCrypt.Verify(Password, this.PasswordHash);
        }

        private bool _AddNewUser()
        {
            this.UserID = clsUserData.AddNewUser(this.AccountID, this.RoleID, this.UserName,
                this.PasswordHash, this.IsActive);
            return (this.UserID != -1);
        }

        private bool _UpdateUser()
        {
            return clsUserData.UpdateUser(this.UserID, this.RoleID, this.UserName, this.IsActive);
        }

        public static clsUser FindByUserID(int UserID)
        {

            int AccountID = -1, RoleID = -1;
            string UserName = "", PasswordHash = "";
            bool IsActive = false;

            if (clsUserData.GetUserInfoByUserID(UserID, ref AccountID, ref RoleID,
                ref UserName, ref PasswordHash, ref IsActive))
                return new clsUser(UserID, AccountID, RoleID, UserName, PasswordHash, IsActive);
            else
                return null;
        }

        public static clsUser FindByUserName(string UserName)
        {
            int UserID = -1, AccountID = -1, RoleID = -1;
            string PasswordHash = "";
            bool IsActive = false;

            if (clsUserData.GetUserInfoByUserName(UserName, ref UserID, ref AccountID,
                ref RoleID, ref PasswordHash, ref IsActive))
                return new clsUser(UserID, AccountID, RoleID, UserName, PasswordHash, IsActive);
            else
                return null;
        }

        // Returns the user if the login is valid, otherwise null
        public static clsUser Login(string UserName, string Password)
        {
            clsUser User = FindByUserName(UserName);

            if (User != null && User.IsActive && User.VerifyPassword(Password))
                return User;
            else
                return null;
        }

        public bool ChangePassword(string NewPassword)
        {
            if (Mode != enMode.Update)
                return false;

            string NewHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);

            if (clsUserData.ChangePassword(this.UserID, NewHash))
            {
                this.PasswordHash = NewHash;
                return true;
            }
            return false;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (string.IsNullOrEmpty(this.PasswordHash))
                        return false;   // SetPassword() was not called

                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateUser();
            }
            return false;
        }

        public static List<clsUserData.UserDTO> GetAllUsers(int AccountID)
        {
            return clsUserData.GetAllUsers(AccountID);
        }

        //public static bool DeleteUser(int UserID)
        //{
        //    return clsUserData.DeleteUser(UserID);
        //}
        public static bool DeactivateUser(int AccountID, int UserID, int ActingUserID)
        {
            return clsUserData.DeactivateUser(AccountID, UserID, ActingUserID);
        }

        public static bool isUserExist(int UserID)
        {
            return clsUserData.IsUserExist(UserID);
        }

        public static bool isUserExist(string UserName)
        {
            return clsUserData.IsUserExist(UserName);
        }
        public bool Can(clsPermissions.enPermission Permission)
        {
            return clsPermissions.HasPermission(this.RoleID, Permission);
        }
    }
}