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
        public Vector2u TileSize { get; private set; }

        private Dictionary<FloorType, Vector2f> _tilePositions;  

        public TileSet(string texPath, Vector2u tileSize)
        {
            Tex = new Texture(texPath);
            TileSize = tileSize;

            // hardcoded for now, load by file in future
            _tilePositions = new Dictionary<FloorType, Vector2f>()
            {
                {FloorType.Grass, new Vector2f(0, 0)},
                {FloorType.Dirt, new Vector2f(0, 1)},
                {FloorType.Stone, new Vector2f(1, 0)},
                {FloorType.Water, new Vector2f(1, 1)}
            };


        }

        public Tuple<Vector2f, Vector2f, Vector2f, Vector2f> GetTexCoordOf(FloorType ft)
        {
            var p1 = _tilePositions[ft];
            p1.X *= TileSize.X;
            p1.Y *= TileSize.Y;
            var p2 = p1 + new Vector2f(TileSize.X, 0);
            var p3 = p1 + new Vector2f(TileSize.X, TileSize.Y);
            var p4 = p1 + new Vector2f(0, TileSize.Y);
            return new Tuple<Vector2f, Vector2f, Vector2f, Vector2f>(p1, p2, p3, p4);
        }
    }
}
