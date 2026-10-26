using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsRole
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public clsRoleData.RoleDTO DTO
        {
            get { return new clsRoleData.RoleDTO(this.RoleID, this.Name); }
        }
        public int RoleID { set; get; }
        public string Name { set; get; }

        public clsRole()
        {
            this.RoleID = -1;
            this.Name = "";
            Mode = enMode.AddNew;
        }

        public clsRole(clsRoleData.RoleDTO DTO)
        {
            this.RoleID = DTO.RoleID;
            this.Name = DTO.Name;
            Mode = enMode.Update;
        }
        private clsRole(int RoleID, string Name)
        {
            this.RoleID = RoleID;
            this.Name = Name;
            Mode = enMode.Update;
        }

        private bool _AddNewRole()
        {
            this.RoleID = clsRoleData.AddNewRole(this.Name);
            return (this.RoleID != -1);
        }

        private bool _UpdateRole()
        {
            return clsRoleData.UpdateRole(this.RoleID, this.Name);
        }

        public static clsRole Find(int RoleID)
        {
            string Name = "";

            if (clsRoleData.GetRoleInfoByID(RoleID, ref Name))
                return new clsRole(RoleID, Name);
            else
                return null;
        }
        public static clsRole FindByName(string Name)
        {
            int RoleID = -1;

            if (clsRoleData.GetRoleInfoByName(Name, ref RoleID))
                return new clsRole(RoleID, Name);
            else
                return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewRole())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateRole();
            }
            return false;
        }

        public static List<clsRoleData.RoleDTO> GetAllRoles()
        {
            return clsRoleData.GetAllRoles();
        }

        public static bool DeleteRole(int RoleID)
        {
            return clsRoleData.DeleteRole(RoleID);
        }

        public static bool isRoleExist(int RoleID)
        {
            return clsRoleData.IsRoleExist(RoleID);
        }
    }
}