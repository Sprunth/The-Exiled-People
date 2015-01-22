﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using The_Exiled_People.TileContents;

namespace The_Exiled_People
{
    class TileSetCollection
    {
        public TileSet Floor { get; private set; }
        public TileSet People { get; private set; }

        public Vector2u TileSize { get; private set; }

        private readonly Dictionary<FloorType, Vector2f> _floorTilePositions;

        public TileSetCollection()
        {
            TileSize = new Vector2u(16, 16);

            Floor = new TileSet("Graphics/Floor.png");
            // hardcoded for now, load by file in future
            _floorTilePositions = new Dictionary<FloorType, Vector2f>()
            {
                {FloorType.Grass, new Vector2f(0, 0)},
                {FloorType.Dirt, new Vector2f(0, 1)},
                {FloorType.Stone, new Vector2f(1, 0)},
                {FloorType.Water, new Vector2f(1, 1)}
            };
            People = new TileSet("Graphics/People.png");

        }

        public Tuple<Vector2f, Vector2f, Vector2f, Vector2f> GetTexCoordOf(FloorType ft)
        {
            var p1 = _floorTilePositions[ft];
            p1.X *= TileSize.X;
            p1.Y *= TileSize.Y;
            var p2 = p1 + new Vector2f(TileSize.X, 0);
            var p3 = p1 + new Vector2f(TileSize.X, TileSize.Y);
            var p4 = p1 + new Vector2f(0, TileSize.Y);
            return new Tuple<Vector2f, Vector2f, Vector2f, Vector2f>(p1, p2, p3, p4);
        }
    }
}