using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noise;
using Noise.Modules;
using Noise.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using The_Exiled_People.TileContents;

namespace The_Exiled_People
{
    class Map : Drawable, IUpdateable, IDisposable
    {
        /// <summary>
        /// Holds the entire map, in order, with each element being a map layer
        /// List is fine because most insertions are downward (end) and not upward (front)
        /// </summary>
        private readonly List<MapLayer> _entireMap;
        private readonly Vector2u _layerSize;

        private int _activeLayer = 0;

        private readonly RenderTexture _target;
        private readonly Sprite _targetSpr;

        private TileSetCollection _tileSetCollection;

        private bool zoomed = false;

        #region Noiselib variables
        private Perlin _perlin;
        #endregion

        public Map(Vector2u layerSize, Vector2u displaySize, Vector2f position)
        {
            _layerSize = layerSize;

            _target = new RenderTexture(displaySize.X, displaySize.Y) {Smooth = true};
            _targetSpr = new Sprite() {Position = position};

            _tileSetCollection = new TileSetCollection();

            _entireMap = new List<MapLayer>();
            
        }

        public void Initialize()
        {
            //AddNewLayer(); // Add inital layer
            GenerateTerrain(11);
            Program.ActiveGame.Win.MouseButtonReleased += Win_MouseButtonReleased;
            Program.ActiveGame.Win.KeyReleased += Win_KeyReleased;
            Program.ActiveGame.Win.KeyPressed += Win_KeyPressed;
        }

        private void GenerateTerrain(int mapDepth)
        {
            _perlin = new Perlin(2, 1, NoiseQuality.Standard, 4, 0.4, Program.ActiveGame.Rand.Next(1024));
            
            //TODO: Not yet capable of multiple generations for deeper stuff
            
            var noiseValues = new List<double[,]>();
            for (var depth = 0; depth < mapDepth; depth++)
            {
                var layerValues = new double[_layerSize.X, _layerSize.Y];

                for (var x = 0; x < _layerSize.X; x++)
                {
                    for (var y = 0; y < _layerSize.Y; y++)
                    {
                        layerValues[x, y] = _perlin.GetValue(x/20.0, y/20.0, (depth+1)/20.0);
                    }
                }
                noiseValues.Add(layerValues);
            }

            // cutoff for each layer, increasing
            var cutoff = -0.9;
            var seperation = 2.0 / mapDepth;

            for (var layerNum = 0; layerNum < mapDepth; layerNum++)
            {
                _entireMap.Add(new MapLayer(_layerSize, _tileSetCollection, _target.Size));
            }

            for (var layerNum = 0; layerNum < noiseValues.Count; layerNum++)
            {
                var layer = noiseValues[layerNum];
                for (var x = 0; x < _layerSize.X; x++)
                {
                    for (var y = 0; y < _layerSize.Y; y++)
                    {
                        //Debug.WriteLine("layer {0}  (x,y):({1},{2})", layerNum, x, y);
                        if (layer[x, y] < cutoff)
                        {
                            if (layer[x, y] < 0)
                            {
                                
                                if (layer[x, y] < 0.5 && Program.ActiveGame.Rand.Next(4) == 0)
                                    _entireMap[layerNum][x, y] = new MapSpot(FloorType.Stone, PersonProfession.None);
                                else
                                    _entireMap[layerNum][x, y] = new MapSpot(FloorType.Dirt, PersonProfession.None);
                            }
                            else
                                _entireMap[layerNum][x, y] = new MapSpot(FloorType.Water, PersonProfession.None);
                        }
                        else
                        {
                            _entireMap[layerNum][x, y] = new MapSpot(FloorType.SmoothedStone, PersonProfession.None);
                        }
                    }
                }              

                cutoff += seperation;
            }
        }

        public void ChangeTileSet(TileSetCollection tsc)
        {
            _tileSetCollection = tsc;
            _entireMap.ForEach(layer => layer.TileSetCollection = tsc);
        }

        /// <summary>
        /// Move up or down a layer
        /// </summary>
        /// <param name="down"></param>
        private void MoveUpDownLayer(bool down)
        {
            var oldTopLeft = _entireMap[_activeLayer].TopLeft;
            if (down)
            {
                if (_activeLayer == _entireMap.Count-1)
                    AddNewLayer();
                _activeLayer++;
            }
            else
            {
                if (_activeLayer != 0)
                    _activeLayer--;
            }
            //Todo: there's a redundant redraw here from addnewlayer and assigning topleft
            _entireMap[_activeLayer].TopLeft = oldTopLeft;
            Debug.WriteLine("Moved to layer {0}", _activeLayer);
        }

        /// <summary>
        /// Adds a new map layer to the entire map
        /// </summary>
        /// <param name="downward">If true, add layer to bottom of map</param>
        private void AddNewLayer(bool downward = true)
        {
            Debug.WriteLine("Added new Layer");
            var toInsert = new MapLayer(_layerSize, _tileSetCollection, _target.Size);
            if (downward)
            {
                _entireMap.Add(toInsert);

            }
            else
            {
                _entireMap.Insert(0, toInsert);
                _activeLayer++;
            }
        }

        public void ResizeTarget(Vector2u newSize)
        {
            _entireMap.ForEach(layer => layer.DrawTargetSize = newSize);
            throw new NotImplementedException();
        }

        void Drawable.Draw(RenderTarget target, RenderStates states)
        {
            _target.Clear();
            _target.Draw(_entireMap[_activeLayer]);
            _target.Display();
            _targetSpr.Texture = _target.Texture;

            target.Draw(_targetSpr);
        }

        public void Update()
        {
            _entireMap.ForEach(layer => layer.Update());
        }

        void Win_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            // check mouse release happened inside _target bounds
            if (
                !(e.X >= _targetSpr.Position.X && e.Y >= _targetSpr.Position.Y &&
                  e.X < _targetSpr.Position.X + _targetSpr.Texture.Size.X &&
                  e.Y < _targetSpr.Position.Y + _targetSpr.Texture.Size.Y))
                return;

            var coords = _target.MapPixelToCoords(new Vector2i(e.X - (int)_targetSpr.Position.X, e.Y - (int)_targetSpr.Position.Y));
            var mapCoords = new Vector2u((uint)Math.Floor(coords.X/_tileSetCollection.TileSize.X),
                (uint)Math.Floor(coords.Y/_tileSetCollection.TileSize.Y));
            mapCoords.X += (uint)_entireMap[_activeLayer].TopLeft.X;
            mapCoords.Y += (uint)_entireMap[_activeLayer].TopLeft.Y;
            if (zoomed)
            {
                //buggy
                mapCoords.X *= 2;
                mapCoords.Y *= 2;
            }
            Debug.WriteLine("clicked on {0} | {1}", mapCoords, _entireMap[_activeLayer].GetSpotAt(mapCoords.X, mapCoords.Y));
        }

        void Win_KeyReleased(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Q:
                    MoveUpDownLayer(false);
                    break;
                case Keyboard.Key.E:
                    MoveUpDownLayer(true);
                    break;
                case Keyboard.Key.Equal: // plus
                    if (!zoomed)
                    {
                        zoomed = true;
                        var v =_target.GetView();
                        v.Zoom(0.5f);
                        _target.SetView(v);
                    }
                    break;
                case Keyboard.Key.Dash: // minus
                    if (zoomed)
                    {
                        zoomed = false;
                        var v = _target.GetView();
                        v.Zoom(2f);
                        _target.SetView(v);
                    }
                    break;
            }
        }

        void Win_KeyPressed(object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                case Keyboard.Key.Left:
                case Keyboard.Key.Right:
                case Keyboard.Key.Up:
                case Keyboard.Key.Down:
                    MoveMap(e.Code);
                    break;
            }
        }

        void MoveMap(Keyboard.Key key)
        {
            var move = new Vector2i(0, 0);
            switch (key)
            {
                case Keyboard.Key.Left:
                    if (_entireMap[_activeLayer].TopLeft.X > 0)
                        move.X -= 1;
                    break;
                case Keyboard.Key.Right:
                    if (_entireMap[_activeLayer].TopLeft.X + _target.Size.X/_tileSetCollection.TileSize.X < _entireMap[_activeLayer].LayerSize.X)
                        move.X += 1;
                    break;
                case Keyboard.Key.Up:
                    if (_entireMap[_activeLayer].TopLeft.Y > 0)
                        move.Y -= 1;
                    break;
                case Keyboard.Key.Down:
                    if (_entireMap[_activeLayer].TopLeft.Y + _target.Size.Y/_tileSetCollection.TileSize.Y < _entireMap[_activeLayer].LayerSize.Y)
                        move.Y += 1;
                    break;

            }
            _entireMap[_activeLayer].TopLeft += move;
        }

        public void Dispose()
        {
            _targetSpr.Dispose();
            _target.Dispose();
        }
    }
}
