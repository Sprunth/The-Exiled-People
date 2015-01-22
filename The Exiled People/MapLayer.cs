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
        private VertexArray _vertices;
        private Vector2u _layerSize;
        public Vector2u LayerSize { get { return _layerSize; } set { _layerSize = value; } }

        public Vector2u DrawTargetSize { get; set; }

        private TileSet _tileSet;
        public TileSet Tileset
        {
            get { return _tileSet; }
            set
            {
                _tileSet = value;
                SetupVertexArray();
            }
        }

        private Vector2i _topLeft;
        public Vector2i TopLeft
        {
            get { return _topLeft; }
            set { _topLeft = value; Debug.WriteLine("TopLeft now at {0}", _topLeft); SetupVertexArray(); }
        }

        public MapLayer(Vector2u layerSize, TileSet initalTileSet, Vector2u initalDrawTargetSize)
        {
            _layerSize = layerSize;
            DrawTargetSize = initalDrawTargetSize;

            _layer = new MapSpot[_layerSize.X, _layerSize.Y];
            var floorTypes = Enum.GetValues(typeof (FloorType));
            for (var row = 0; row < _layerSize.Y; row++)
            {
                for (var col = 0; col < _layerSize.X; col++)
                {
                    _layer[row, col] = new MapSpot((FloorType)floorTypes.GetValue(Program.ActiveGame.Rand.Next(floorTypes.Length)));
                }
            }

            Tileset = initalTileSet;

            _topLeft = new Vector2i(0, 0);

            SetupVertexArray();
        }

        void SetupVertexArray()
        {
            _vertices = new VertexArray(PrimitiveType.Quads);
            // Faster if we resize, but too lazy to math
            //_vertices.Resize(_layerSize.X * _layerSize.Y * 4);

            
            var tilesize = Tileset.TileSize;

            for (var row = 0; row < _layerSize.X; row++)
            {
                for (var col = 0; col < _layerSize.Y; col++)
                {
                    var v1 = new Vertex(new Vector2f((col )      * tilesize.X, (row )      * tilesize.Y));
                    var v2 = new Vertex(new Vector2f((col + 1 )  * tilesize.X, (row )      * tilesize.Y));
                    var v3 = new Vertex(new Vector2f((col + 1 )  * tilesize.X, (row + 1 )  * tilesize.Y));
                    var v4 = new Vertex(new Vector2f((col )      * tilesize.X, (row + 1 )  * tilesize.Y));
                   

                    _vertices.Append(v1);
                    _vertices.Append(v2);
                    _vertices.Append(v3);
                    _vertices.Append(v4);

                    //Debug.WriteLine("{0} {1} {2} {3}", v1.Position, v2.Position, v3.Position ,v4.Position);
                }
            }

            UpdateTexCoords();
        }

        /// <summary>
        /// Looks up the object at every map spot and changes the texcoords for the corresponding vertex if needed
        /// </summary>
        void UpdateTexCoords()
        {
            //for (var col = (uint)TopLeft.Y; col < DrawTargetSize.X/Tileset.TileSize.X; col++)
            for (var col = (uint)0; col < DrawTargetSize.X / Tileset.TileSize.X; col++)
            {
                for (var row = (uint)0; row < DrawTargetSize.Y/Tileset.TileSize.Y; row++)
                {
                    /*
                    var tc1 = new Vector2f(
                        rand.Next((int)Math.Round(((double)Tileset.Tex.Size.X/Tileset.TileSize.X) - 1)) * Tileset.TileSize.X,
                        rand.Next((int)Math.Round(((double)Tileset.Tex.Size.X/Tileset.TileSize.X) - 1)) * Tileset.TileSize.Y
                        );
                    var tc2 = tc1 + new Vector2f(Tileset.TileSize.X, 0);
                    var tc3 = tc1 + new Vector2f(Tileset.TileSize.X, Tileset.TileSize.Y);
                    var tc4 = tc1 + new Vector2f(0, Tileset.TileSize.Y);
                    */

                    var tup = Tileset.GetTexCoordOf(_layer[row + TopLeft.X, col + TopLeft.Y].FloorType);
                    var tc1 = tup.Item1;
                    var tc2 = tup.Item2;
                    var tc3 = tup.Item3;
                    var tc4 = tup.Item4;

                    //tc1 = new Vector2f(0, 12);
                    //tc2 = new Vector2f(12, 0);
                    //tc3 = new Vector2f(12, 12);
                    //tc4 = new Vector2f(0, 12);

                    //Debug.WriteLine("{0} {1} {2} {3}", tc1, tc2, tc3, tc4);

                    var index = (uint)(_layerSize.X*(row) + (col))*4;

                    _vertices[index + 0] = new Vertex(_vertices[index + 0].Position){ TexCoords = tc1 };
                    _vertices[index + 1] = new Vertex(_vertices[index + 1].Position){ TexCoords = tc2 };
                    _vertices[index + 2] = new Vertex(_vertices[index + 2].Position){ TexCoords = tc3 };
                    _vertices[index + 3] = new Vertex(_vertices[index + 3].Position){ TexCoords = tc4 };
                }
            }
        }

        public MapSpot GetSpotAt(uint x, uint y)
        {
            return _layer[y, x];
        }

        void Drawable.Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_vertices, new RenderStates(Tileset.Tex));
        }

        public void Update()
        {
            
        }

        void IDisposable.Dispose()
        {
            _vertices.Dispose();
        }
    }
}
