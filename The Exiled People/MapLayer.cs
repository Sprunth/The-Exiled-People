using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using The_Exiled_People.TileContents;

namespace The_Exiled_People
{
    class MapLayer : Drawable , IUpdateable , IDisposable
    {
        private readonly MapSpot[,] _layer;
        private readonly VertexArray _floorVertices, _peopleVertices;
        public MapSpot this[int x, int y]
        {
            get { return _layer[x, y]; }
            set {
                _layer[x, y] = value;
                dirtyTexCoords = true;
            }
        }
        private bool dirtyTexCoords = false;

        /// <summary>
        /// Holds a tuple containing the 4 corner positions of each grid position
        /// </summary>
        private Tuple<Vector2f, Vector2f, Vector2f, Vector2f>[,] _gridPositions;

        private Vector2u _layerSize;
        public Vector2u LayerSize { get { return _layerSize; } set { _layerSize = value; } }

        private Vector2u _drawTargetSize;
        public Vector2u DrawTargetSize { get { return _drawTargetSize; } set { _drawTargetSize = value; UpdateTexCoords(); } }

        private TileSetCollection _tileSetCollection;
        public TileSetCollection TileSetCollection
        {
            get { return _tileSetCollection; }
            set
            {
                _tileSetCollection = value;
                SetupVertexArray();
            }
        }

        private Vector2i _topLeft;
        public Vector2i TopLeft
        {
            get { return _topLeft; }
            set { _topLeft = value; Debug.WriteLine("TopLeft now at {0}", _topLeft); UpdateTexCoords(); }
        }

        public MapLayer(Vector2u layerSize, TileSetCollection initalTileSetCollection, Vector2u initalDrawTargetSize)
        {
            _layerSize = layerSize;
            _drawTargetSize = initalDrawTargetSize;

            _layer = new MapSpot[_layerSize.X, _layerSize.Y];
            
            var floorTypes = Enum.GetValues(typeof (FloorType));
            var professions = Enum.GetValues(typeof (PersonProfession));
            for (var row = 0; row < _layerSize.Y; row++)
            {
                for (var col = 0; col < _layerSize.X; col++)
                {
                    var floor = (FloorType) floorTypes.GetValue(Program.ActiveGame.Rand.Next(floorTypes.Length));
                    var profession = Program.ActiveGame.Rand.Next(20) == 1 ? (PersonProfession)professions.GetValue(Program.ActiveGame.Rand.Next(professions.Length)) : PersonProfession.None;
                    
                    _layer[row, col] = new MapSpot(floor, profession);
                }
            }
            
            _topLeft = new Vector2i(0, 0);

            _tileSetCollection = initalTileSetCollection;

            

            _floorVertices = new VertexArray(PrimitiveType.Quads);
            _peopleVertices = new VertexArray(PrimitiveType.Quads);
            _floorVertices.Resize(_layerSize.X * _layerSize.Y * 4);
            _peopleVertices.Resize(_layerSize.X * _layerSize.Y * 4);

            SetupVertexArray();
        }

        /// <summary>
        /// Called whenever the positions to draw have changed.
        /// i.e. when the window is resized as therefore the target
        /// </summary>
        void SetupVertexArray()
        {
            _gridPositions = new Tuple<Vector2f, Vector2f, Vector2f, Vector2f>[_layerSize.X, _layerSize.Y];

            var tilesize = TileSetCollection.TileSize;

            for (var row = 0; row < _layerSize.X; row++)
            {
                for (var col = 0; col < _layerSize.Y; col++)
                {
                    var v1 = new Vector2f((col )      * tilesize.X, (row )      * tilesize.Y);
                    var v2 = new Vector2f((col + 1 )  * tilesize.X, (row )      * tilesize.Y);
                    var v3 = new Vector2f((col + 1 )  * tilesize.X, (row + 1 )  * tilesize.Y);
                    var v4 = new Vector2f((col )      * tilesize.X, (row + 1 )  * tilesize.Y);

                    _gridPositions[col, row] = new Tuple<Vector2f, Vector2f, Vector2f, Vector2f>(v1,v2,v3,v4);
                }
            }

            UpdateTexCoords();
        }

        /// <summary>
        /// Looks up the object at every map spot and changes the texcoords for the corresponding vertex if needed
        /// </summary>
        void UpdateTexCoords()
        {
            var floorTexCoords = TileSetCollection._floorTexCoords;
            var peopleTexCoords = TileSetCollection._professionTexCoords;

            for (var col = (uint)0; col < DrawTargetSize.X / TileSetCollection.TileSize.X; col++)
            {
                for (var row = (uint)0; row < DrawTargetSize.Y/TileSetCollection.TileSize.Y; row++)
                {
                    var index = (_layerSize.X * (row) + (col)) * 4;
                    var mapSpot = _layer[col + TopLeft.X, row + TopLeft.Y];

                    //var floorTup = TileSetCollection.GetTexCoordOf(mapSpot.FloorType);
                    var floorTup = floorTexCoords[mapSpot.FloorType];
                    
                    _floorVertices[index + 0] = new Vertex(_gridPositions[col, row].Item1) { TexCoords = floorTup.Item1 };
                    _floorVertices[index + 1] = new Vertex(_gridPositions[col, row].Item2) { TexCoords = floorTup.Item2 };
                    _floorVertices[index + 2] = new Vertex(_gridPositions[col, row].Item3) { TexCoords = floorTup.Item3 };
                    _floorVertices[index + 3] = new Vertex(_gridPositions[col, row].Item4) { TexCoords = floorTup.Item4 };


                    //var personTup = TileSetCollection.GetTexCoordOf(mapSpot.PersonProfession);
                    var personTup = peopleTexCoords[mapSpot.PersonProfession];
                    _peopleVertices[index + 0] = new Vertex(_gridPositions[col, row].Item1) { TexCoords = personTup.Item1 };
                    _peopleVertices[index + 1] = new Vertex(_gridPositions[col, row].Item2) { TexCoords = personTup.Item2 };
                    _peopleVertices[index + 2] = new Vertex(_gridPositions[col, row].Item3) { TexCoords = personTup.Item3 };
                    _peopleVertices[index + 3] = new Vertex(_gridPositions[col, row].Item4) { TexCoords = personTup.Item4 };
                }
            }
        }

        public MapSpot GetSpotAt(uint x, uint y)
        {
            return _layer[x,y];
        }

        void Drawable.Draw(RenderTarget target, RenderStates states)
        {
            if (dirtyTexCoords)
            {
                dirtyTexCoords = false;
                UpdateTexCoords();
            }
            target.Draw(_floorVertices, new RenderStates(TileSetCollection.Floor.Tex));
            target.Draw(_peopleVertices, new RenderStates(TileSetCollection.People.Tex));
        }

        public void Update()
        {
            
        }

        void IDisposable.Dispose()
        {
            _floorVertices.Dispose();
            _peopleVertices.Dispose();
        }
    }
}
