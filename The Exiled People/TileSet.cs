using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using The_Exiled_People.TileContents;

namespace The_Exiled_People
{
    /// <summary>
    /// Contains all the data for a particular texture map
    /// TODO: make dictionaries more flexible instead of taking a specific enum
    /// </summary>
    class TileSet
    {
        public Texture Tex { get; private set; }

        public TileSet(string texPath)
        {
            Tex = new Texture(texPath);
        }
    }
}
