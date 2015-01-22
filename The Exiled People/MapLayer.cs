﻿using System;
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
        private VertexArray _floorVertices;
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
            for (var row = 0; row < _layerSize.Y; row++)
            {
                for (var col = 0; col < _layerSize.X; col++)
                {
                    _layer[row, col] = new MapSpot(
                        (FloorType)floorTypes.GetValue(Program.ActiveGame.Rand.Next(floorTypes.Length))
                        );
                }
            }

            TileSetCollection = initalTileSetCollection;

            _topLeft = new Vector2i(0, 0);

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
            _floorVertices = new VertexArray(PrimitiveType.Quads);
            _floorVertices.Resize((uint)_gridPositions.Length * 4);

            for (var col = (uint)0; col < DrawTargetSize.X / TileSetCollection.TileSize.X; col++)
            {
                for (var row = (uint)0; row < DrawTargetSize.Y/TileSetCollection.TileSize.Y; row++)
                {

                    var tup = TileSetCollection.GetTexCoordOf(_layer[col + TopLeft.X, row + TopLeft.Y].FloorType);
                    var tc1 = tup.Item1;
                    var tc2 = tup.Item2;
                    var tc3 = tup.Item3;
                    var tc4 = tup.Item4;

                    //Debug.WriteLine("{0} {1} {2} {3}", tc1, tc2, tc3, tc4);

                    var index = (_layerSize.X*(row) + (col))*4;

                    _floorVertices[index + 0] = new Vertex(_gridPositions[col, row].Item1) { TexCoords = tc1 };
                    _floorVertices[index + 1] = new Vertex(_gridPositions[col, row].Item2) { TexCoords = tc2 };
                    _floorVertices[index + 2] = new Vertex(_gridPositions[col, row].Item3) { TexCoords = tc3 };
                    _floorVertices[index + 3] = new Vertex(_gridPositions[col, row].Item4) { TexCoords = tc4 };
                }
            }
        }

        public MapSpot GetSpotAt(uint x, uint y)
        {
            return _layer[x,y];
        }

        void Drawable.Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(_floorVertices, new RenderStates(TileSetCollection.Floor.Tex));
        }

        public void Update()
        {
            
        }

        void IDisposable.Dispose()
        {
            _floorVertices.Dispose();
        }
    }
}
