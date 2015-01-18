using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace The_Exiled_People
{
    class MapLayer : Drawable , IUpdateable , IDisposable
    {
        private MapSpot[,] _layer;
        private VertexArray _vertices;
        private Vector2u _layerSize;

        private TileSet _texture;

        public MapLayer(Vector2u layerSize)
        {
            _layerSize = layerSize;
            _layer = new MapSpot[layerSize.X, layerSize.Y];
            
            _texture = new TileSet(@"testTileSet.png", new Vector2u(12,12));

            SetupVertexArray();
        }

        void SetupVertexArray()
        {
            _vertices = new VertexArray(PrimitiveType.Quads);
            // Faster, but too lazy to math
            //_vertices.Resize(_layerSize.X * _layerSize.Y * 4);

            
            var tilesize = _texture.TileSize;

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
            // placeholder until tiles have real data
            var rand = new Random();

            for (uint col = 0; col < _layerSize.X; col++)
            {
                for (uint row = 0; row < _layerSize.Y; row++)
                {
                    var tc1 = new Vector2f(
                        rand.Next((int)Math.Round(((double)_texture.Tex.Size.X/_texture.TileSize.X) - 1)) * _texture.TileSize.X,
                        rand.Next((int)Math.Round(((double)_texture.Tex.Size.X/_texture.TileSize.X) - 1)) * _texture.TileSize.Y
                        );
                    var tc2 = tc1 + new Vector2f(_texture.TileSize.X, 0);
                    var tc3 = tc1 + new Vector2f(_texture.TileSize.X, _texture.TileSize.Y);
                    var tc4 = tc1 + new Vector2f(0, _texture.TileSize.Y);

                    //tc1 = new Vector2f(0, 0);
                    //tc2 = new Vector2f(100, 0);
                    //tc3 = new Vector2f(100, 100);
                    //tc4 = new Vector2f(0, 100);

                    //Debug.WriteLine("{0} {1} {2} {3}", tc1, tc2, tc3, tc4);

                    //_vertices[row * _layerSize.X + col * 4 + 0] = new Vertex() { Position = _vertices[row * _layerSize.X + col * 4 + 0].Position, TexCoords = tc1, Color = Color.Blue };
                    //_vertices[row * _layerSize.X + col * 4 + 1] = new Vertex() { Position = _vertices[row * _layerSize.X + col * 4 + 1].Position, TexCoords = tc2, Color = Color.Green };
                    //_vertices[row * _layerSize.X + col * 4 + 2] = new Vertex() { Position = _vertices[row * _layerSize.X + col * 4 + 2].Position, TexCoords = tc3, Color = Color.Red };
                    //_vertices[row * _layerSize.X + col * 4 + 3] = new Vertex() { Position = _vertices[row * _layerSize.X + col * 4 + 3].Position, TexCoords = tc4, Color = Color.Yellow };

                    var index = (_layerSize.X*row + col)*4;

                    _vertices[index + 0] = new Vertex(_vertices[index + 0].Position){ TexCoords = tc1 };
                    _vertices[index + 1] = new Vertex(_vertices[index + 1].Position){ TexCoords = tc2 };
                    _vertices[index + 2] = new Vertex(_vertices[index + 2].Position){ TexCoords = tc3 };
                    _vertices[index + 3] = new Vertex(_vertices[index + 3].Position){ TexCoords = tc4 };
                }
            }
        }

        void Drawable.Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_vertices, new RenderStates(_texture.Tex));
        }

        public void Update()
        {
            UpdateTexCoords();
        }

        void IDisposable.Dispose()
        {
            _vertices.Dispose();
        }
    }
}
