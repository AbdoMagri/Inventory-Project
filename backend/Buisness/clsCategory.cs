using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsCategory
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int CategoryID { set; get; }
        public int AccountID { set; get; }
        public string Name { set; get; }

        public clsCategory()
        {
            this.CategoryID = -1;
            this.Name = "";
            Mode = enMode.AddNew;
        }

        private clsCategory(int CategoryID, int AccountID, string Name)
        {
            this.CategoryID = CategoryID;
            this.AccountID = AccountID;
            this.Name = Name;
            Mode = enMode.Update;
        }

        private bool _AddNewCategory()
        {
            this.CategoryID = clsCategoryData.AddNewCategory(this.AccountID, this.Name);
            return (this.CategoryID != -1);
        }

        private bool _UpdateCategory()
        {
            return clsCategoryData.UpdateCategory(this.CategoryID, this.Name);
        }

        public static clsCategory Find(int CategoryID)
        {
            int AccountID = -1;
            string Name = "";

            if (clsCategoryData.GetCategoryInfoByID(CategoryID, ref AccountID, ref Name))
                return new clsCategory(CategoryID, AccountID, Name);
            else
                return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewCategory())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateCategory();
            }
            return false;
        }

        public static DataTable GetAllCategories(int AccountID)
        {
            return clsCategoryData.GetAllCategories(AccountID);
        }

        public static bool DeleteCategory(int CategoryID)
        {
            return clsCategoryData.DeleteCategory(CategoryID);
        }

        public static bool isCategoryExist(int AccountID, string Name)
        {
            return clsCategoryData.IsCategoryExist(AccountID, Name);
        }
    }
}