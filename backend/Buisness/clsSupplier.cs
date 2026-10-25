using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsSupplier
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public clsSupplierData.SupplierDTO DTO
        {
            get { return new clsSupplierData.SupplierDTO(this.SupplierID, this.AccountID, this.Name, this.Phone); }
        }
        public int SupplierID { set; get; }
        public int AccountID { set; get; }
        public string Name { set; get; }
        public string Phone { set; get; }

        public clsSupplier()
        {
            this.SupplierID = -1;
            this.Name = "";
            this.Phone = "";
            Mode = enMode.AddNew;
        }
        public clsSupplier(clsSupplierData.SupplierDTO DTO)
        {
            this.SupplierID = DTO.SupplierID;
            this.AccountID = DTO.AccountID;
            this.Name = DTO.Name;
            this.Phone = DTO.Phone;
            Mode = enMode.Update;
        }
        private clsSupplier(int SupplierID, int AccountID, string Name, string Phone)
        {
            this.SupplierID = SupplierID;
            this.AccountID = AccountID;
            this.Name = Name;
            this.Phone = Phone;
            Mode = enMode.Update;
        }

        private bool _AddNewSupplier()
        {
            this.SupplierID = clsSupplierData.AddNewSupplier(this.AccountID, this.Name, this.Phone);
            return (this.SupplierID != -1);
        }

        private bool _UpdateSupplier()
        {
            return clsSupplierData.UpdateSupplier(this.SupplierID, this.Name, this.Phone);
        }

        public static clsSupplier Find(int SupplierID)
        {
            int AccountID = -1;
            string Name = "", Phone = "";

            if (clsSupplierData.GetSupplierInfoByID(SupplierID, ref AccountID, ref Name, ref Phone))
                return new clsSupplier(SupplierID, AccountID, Name, Phone);
            else
                return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewSupplier())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateSupplier();
            }
            return false;
        }

        public static DataTable GetAllSuppliers(int AccountID)
        {
            return clsSupplierData.GetAllSuppliers(AccountID);
        }

        public static bool DeleteSupplier(int SupplierID)
        {
            return clsSupplierData.DeleteSupplier(SupplierID);
        }

        public static bool isSupplierExist(int SupplierID)
        {
            return clsSupplierData.IsSupplierExist(SupplierID);
        }
    }
}