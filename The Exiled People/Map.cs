using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

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

        private TileSet _tileSetTexture;

        public Map(Vector2u layerSize, Vector2u displaySize)
        {
            _layerSize = layerSize;

            _target = new RenderTexture(displaySize.X, displaySize.Y) {Smooth = true};
            _targetSpr = new Sprite() {Position = new Vector2f(0,20)};

            _tileSetTexture = new TileSet(@"testTileSet.png", new Vector2u(12, 12));

            _entireMap = new List<MapLayer>();
            
        }

        public void Initialize()
        {
            AddNewLayer(); // Add inital layer
            Program.ActiveGame.Win.MouseButtonReleased += Win_MouseButtonReleased;
            Program.ActiveGame.Win.KeyReleased += Win_KeyReleased;
        }

        public void ChangeTileSet(TileSet ts)
        {
            _tileSetTexture = ts;
            _entireMap.ForEach(layer => layer.Tileset = ts);
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
            var toInsert = new MapLayer(_layerSize, _tileSetTexture);
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
            var mapCoords = new Vector2u((uint)Math.Floor(coords.X/_tileSetTexture.TileSize.X),
                (uint)Math.Floor(coords.Y/_tileSetTexture.TileSize.Y));
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
                    if (_entireMap[_activeLayer].TopLeft.X + _target.Size.X < _entireMap[_activeLayer].LayerSize.X)
                        move.X += 1;
                    break;
                case Keyboard.Key.Up:
                    if (_entireMap[_activeLayer].TopLeft.Y > 0)
                        move.Y -= 1;
                    break;
                case Keyboard.Key.Down:
                    if (_entireMap[_activeLayer].TopLeft.Y + _target.Size.Y < _entireMap[_activeLayer].LayerSize.Y)
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
