using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPSample.Models.Inventory.Masters
{
    public class WarehouseMasterMain
    {
        private Locations _Locations;
        public Locations Locations
        {
            get
            {
                if (_Locations == null)
                {
                    _Locations = new Locations();
                }
                return _Locations;
            }
            set
            {
                _Locations = value;
            }
        }

        private LocationBranchList _LocationBranchList;
        public LocationBranchList LocationBranchList
        {
            get
            {
                if (_LocationBranchList == null)
                {
                    _LocationBranchList = new LocationBranchList();
                }
                return _LocationBranchList;
            }
            set
            {
                _LocationBranchList = value;
            }
        }
    }
}
