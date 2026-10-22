using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsAccount
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int AccountID { set; get; }
        public string Name { set; get; }
        public DateTime CreatedAt { set; get; }

        public clsAccount()
        {
            this.AccountID = -1;
            this.Name = "";
            this.CreatedAt = DateTime.Now;
            Mode = enMode.AddNew;
        }

        private clsAccount(int AccountID, string Name, DateTime CreatedAt)
        {
            this.AccountID = AccountID;
            this.Name = Name;
            this.CreatedAt = CreatedAt;
            Mode = enMode.Update;
        }

        private bool _AddNewAccount()
        {
            this.AccountID = clsAccountData.AddNewAccount(this.Name);
            return (this.AccountID != -1);
        }

        private bool _UpdateAccount()
        {
            return clsAccountData.UpdateAccount(this.AccountID, this.Name);
        }

        public static clsAccount Find(int AccountID)
        {
            string Name = "";
            DateTime CreatedAt = DateTime.Now;

            if (clsAccountData.GetAccountInfoByID(AccountID, ref Name, ref CreatedAt))
                return new clsAccount(AccountID, Name, CreatedAt);
            else
                return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewAccount())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateAccount();
            }
            return false;
        }

        public static DataTable GetAllAccounts()
        {
            return clsAccountData.GetAllAccounts();
        }

        public static bool DeleteAccount(int AccountID)
        {
            return clsAccountData.DeleteAccount(AccountID);
        }

        public static bool isAccountExist(int AccountID)
        {
            return clsAccountData.IsAccountExist(AccountID);
        }
    }
}