using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Exiled_People.TileContents
{
    class Floor
    {
        public FloorType Type { get; set; }

        public Floor(FloorType ft)
        {   
            Type = ft;
        }
    }

    public enum FloorType { Dirt }
}
