using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Dictionary<PersonProfession, Vector2f> _peopleTilePositions;

        public Dictionary<PersonProfession, Tuple<Vector2f, Vector2f, Vector2f, Vector2f>> _professionTexCoords;
        public Dictionary<FloorType, Tuple<Vector2f, Vector2f, Vector2f, Vector2f>> _floorTexCoords; 

        public TileSetCollection()
        {
            TileSize = new Vector2u(16, 16);

            _professionTexCoords = new Dictionary<PersonProfession, Tuple<Vector2f, Vector2f, Vector2f, Vector2f>>();
            _floorTexCoords = new Dictionary<FloorType, Tuple<Vector2f, Vector2f, Vector2f, Vector2f>>();

            Floor = new TileSet("Graphics/Floor.png");
            // hardcoded for now, load by file in future
            _floorTilePositions = new Dictionary<FloorType, Vector2f>()
            {
                {FloorType.Grass, new Vector2f(0, 0)},
                {FloorType.Dirt, new Vector2f(0, 1)},
                {FloorType.Stone, new Vector2f(1, 0)},
                {FloorType.Water, new Vector2f(1, 1)},
                {FloorType.Sky, new Vector2f(0, 2)},
                {FloorType.SmoothedStone, new Vector2f(1, 2)}
            };
            People = new TileSet("Graphics/People.png");
            _peopleTilePositions = new Dictionary<PersonProfession, Vector2f>()
            {
                {PersonProfession.None, new Vector2f(0, 0)},
                {PersonProfession.Baby, new Vector2f(2, 0)},
                {PersonProfession.Kid, new Vector2f(3, 0)},
                {PersonProfession.CoffeeMan, new Vector2f(1, 3)},
                {PersonProfession.Guard, new Vector2f(3, 3)}
            };

            RecalculateTexCoords();
        }

        private void RecalculateTexCoords()
        {
            var floorValues = Enum.GetValues(typeof(FloorType));
            foreach (FloorType fv in floorValues)
                _floorTexCoords[fv] = CalculateTexCoords(fv);
            var professionValues = Enum.GetValues(typeof(PersonProfession));
            foreach (PersonProfession pv in professionValues)
                _professionTexCoords[pv] = CalculateTexCoords(pv);
        }

        private Tuple<Vector2f, Vector2f, Vector2f, Vector2f> CalculateTexCoords(FloorType ft)
        {
            var p1 = _floorTilePositions[ft];
            p1.X *= TileSize.X;
            p1.Y *= TileSize.Y;
            var p2 = p1 + new Vector2f(TileSize.X, 0);
            var p3 = p1 + new Vector2f(TileSize.X, TileSize.Y);
            var p4 = p1 + new Vector2f(0, TileSize.Y);
            return new Tuple<Vector2f, Vector2f, Vector2f, Vector2f>(p1, p2, p3, p4);
        }

        private Tuple<Vector2f, Vector2f, Vector2f, Vector2f> CalculateTexCoords(PersonProfession pp)
        {
            var p1 = _peopleTilePositions[pp];
            p1.X *= TileSize.X;
            p1.Y *= TileSize.Y;
            var p2 = p1 + new Vector2f(TileSize.X, 0);
            var p3 = p1 + new Vector2f(TileSize.X, TileSize.Y);
            var p4 = p1 + new Vector2f(0, TileSize.Y);
            return new Tuple<Vector2f, Vector2f, Vector2f, Vector2f>(p1, p2, p3, p4);
        }

        public Tuple<Vector2f, Vector2f, Vector2f, Vector2f> GetTexCoordOf(FloorType ft)
        {
            return _floorTexCoords[ft];
        }
        public Tuple<Vector2f, Vector2f, Vector2f, Vector2f> GetTexCoordOf(PersonProfession pp)
        {
            return _professionTexCoords[pp];
        }
    }
}
