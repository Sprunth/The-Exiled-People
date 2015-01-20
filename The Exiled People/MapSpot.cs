using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Exiled_People.TileContents;

namespace The_Exiled_People
{
    class MapSpot
    {
        private Floor _floor;

        public MapSpot(FloorType ft)
        {
            _floor = new Floor(ft);
        }

        public override string ToString()
        {
            return _floor.ToString();
        }
    }
}
