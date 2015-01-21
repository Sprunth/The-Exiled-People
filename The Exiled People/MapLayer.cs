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
            set { _topLeft = value; SetupVertexArray(); }
        }

        public MapLayer(Vector2u layerSize, TileSet initalTileSet)
        {
            _layerSize = layerSize;
            _layer = new MapSpot[layerSize.X, layerSize.Y];

            Tileset = initalTileSet;

            _layer = new MapSpot[_layerSize.X, _layerSize.Y];
            for (var row = 0; row < _layerSize.Y; row++)
            {
                for (var col = 0; col < _layerSize.X; col++)
                {
                    _layer[row, col] = new MapSpot(FloorType.Dirt);
                }
            }

            TopLeft = new Vector2i(0, 0);

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
                    var v1 = new Vertex(new Vector2f(row        * tilesize.X, col       * tilesize.Y));
                    var v2 = new Vertex(new Vector2f((row + 1)  * tilesize.X, col       * tilesize.Y));
                    var v3 = new Vertex(new Vector2f((row + 1)  * tilesize.X, (col + 1) * tilesize.Y));
                    var v4 = new Vertex(new Vector2f(row        * tilesize.X, (col + 1) * tilesize.Y));
                   

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
            var rand = Program.ActiveGame.Rand;
            // placeholder until tiles have real data
            for (uint col = 0; col < _layerSize.X; col++)
            {
                for (uint row = 0; row < _layerSize.Y; row++)
                {
                    var tc1 = new Vector2f(
                        rand.Next((int)Math.Round(((double)Tileset.Tex.Size.X/Tileset.TileSize.X) - 1)) * Tileset.TileSize.X,
                        rand.Next((int)Math.Round(((double)Tileset.Tex.Size.X/Tileset.TileSize.X) - 1)) * Tileset.TileSize.Y
                        );
                    var tc2 = tc1 + new Vector2f(Tileset.TileSize.X, 0);
                    var tc3 = tc1 + new Vector2f(Tileset.TileSize.X, Tileset.TileSize.Y);
                    var tc4 = tc1 + new Vector2f(0, Tileset.TileSize.Y);

                    //tc1 = new Vector2f(0, 12);
                    //tc2 = new Vector2f(12, 0);
                    //tc3 = new Vector2f(12, 12);
                    //tc4 = new Vector2f(0, 12);

                    //Debug.WriteLine("{0} {1} {2} {3}", tc1, tc2, tc3, tc4);

                    var index = (_layerSize.X*row + col)*4;

                    _vertices[index + 0] = new Vertex(_vertices[index + 0].Position){ TexCoords = tc1 };
                    _vertices[index + 1] = new Vertex(_vertices[index + 1].Position){ TexCoords = tc2 };
                    _vertices[index + 2] = new Vertex(_vertices[index + 2].Position){ TexCoords = tc3 };
                    _vertices[index + 3] = new Vertex(_vertices[index + 3].Position){ TexCoords = tc4 };
                }
            }
        }

        public MapSpot GetSpotAt(uint x, uint y)
        {
            return _layer[x, y];
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
