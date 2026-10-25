using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsCustomer
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public clsCustomerData.CustomerDTO DTO
        {
            get { return new clsCustomerData.CustomerDTO(this.CustomerID,this.AccountID , this.Name, this.Phone); }
        }
        public int CustomerID { set; get; }
        public int AccountID { set; get; }
        public string Name { set; get; }
        public string Phone { set; get; }

        public clsCustomer()
        {
            this.CustomerID = -1;
            this.Name = "";
            this.Phone = "";
            Mode = enMode.AddNew;
        }

        public clsCustomer(clsCustomerData.CustomerDTO DTO)
        {
            this.CustomerID = DTO.CustomerID;
            this.AccountID = DTO.AccountID;
            this.Name = DTO.Name;
            this.Phone = DTO.Phone;
            Mode = enMode.Update;
        }
        private clsCustomer(int CustomerID, int AccountID, string Name, string Phone)
        {
            this.CustomerID = CustomerID;
            this.AccountID = AccountID;
            this.Name = Name;
            this.Phone = Phone;
            Mode = enMode.Update;
        }

        private bool _AddNewCustomer()
        {
            this.CustomerID = clsCustomerData.AddNewCustomer(this.AccountID, this.Name, this.Phone);
            return (this.CustomerID != -1);
        }

        private bool _UpdateCustomer()
        {
            return clsCustomerData.UpdateCustomer(this.CustomerID, this.Name, this.Phone);
        }

        public static clsCustomer Find(int CustomerID)
        {
            int AccountID = -1;
            string Name = "", Phone = "";

            if (clsCustomerData.GetCustomerInfoByID(CustomerID, ref AccountID, ref Name, ref Phone))
                return new clsCustomer(CustomerID, AccountID, Name, Phone);
            else
                return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewCustomer())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateCustomer();
            }
            return false;
        }

        public static DataTable GetAllCustomers(int AccountID)
        {
            return clsCustomerData.GetAllCustomers(AccountID);
        }

        public static bool DeleteCustomer(int CustomerID)
        {
            return clsCustomerData.DeleteCustomer(CustomerID);
        }

        public static bool isCustomerExist(int CustomerID)
        {
            return clsCustomerData.IsCustomerExist(CustomerID);
        }
    }
}