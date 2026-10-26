using System;
using System.Data;
using InventoryDB_DataAccess;

namespace InventoryDB_Business
{
    public class clsUnit
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public clsUnitData.UnitDTO DTO
        {
            get { return new clsUnitData.UnitDTO(this.UnitID, this.Name); }
        }
        public int UnitID { set; get; }
        public string Name { set; get; }

        public clsUnit()
        {
            this.UnitID = -1;
            this.Name = "";
            Mode = enMode.AddNew;
        }
        public clsUnit(clsUnitData.UnitDTO DTO)
        {
            this.UnitID = DTO.UnitID;
            this.Name = DTO.Name;
            Mode = enMode.Update;
        }
        private clsUnit(int UnitID, string Name)
        {
            this.UnitID = UnitID;
            this.Name = Name;
            Mode = enMode.Update;
        }

        private bool _AddNewUnit()
        {
            this.UnitID = clsUnitData.AddNewUnit(this.Name);
            return (this.UnitID != -1);
        }

        private bool _UpdateUnit()
        {
            return clsUnitData.UpdateUnit(this.UnitID, this.Name);
        }

        public static clsUnit Find(int UnitID)
        {
            string Name = "";

            if (clsUnitData.GetUnitInfoByID(UnitID, ref Name))
                return new clsUnit(UnitID, Name);
            else
                return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUnit())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateUnit();
            }
            return false;
        }

        public static List<clsUnitData.UnitDTO> GetAllUnits()
        {
            return clsUnitData.GetAllUnits();
        }

        public static bool DeleteUnit(int UnitID)
        {
            return clsUnitData.DeleteUnit(UnitID);
        }

        public static bool isUnitExist(int UnitID)
        {
            return clsUnitData.IsUnitExist(UnitID);
        }
    }
}