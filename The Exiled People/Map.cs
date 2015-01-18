using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace The_Exiled_People
{
    class Map : Drawable, IUpdateable
    {
        /// <summary>
        /// Holds the entire map, in order, with each element being a map layer
        /// List is fine because most insertions are downward (end) and not upward (front)
        /// </summary>
        private readonly List<MapLayer> entireMap;
        private readonly Vector2u _layerSize;

        private int _activeLayer = 0;

        public Map(Vector2u layerSize)
        {
            _layerSize = layerSize;
            
            entireMap = new List<MapLayer>();
            AddNewLayer(); // Add inital layer
        }

        /// <summary>
        /// Adds a new map layer to the entire map
        /// </summary>
        /// <param name="downward">If true, add layer to bottom of map</param>
        private void AddNewLayer(bool downward = true)
        {
            var toInsert = new MapLayer(_layerSize);
            if (downward)
            {
                entireMap.Add(toInsert);

            }
            else
            {
                entireMap.Insert(0, toInsert);
                _activeLayer++;
            }
                
        }

        void Drawable.Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(entireMap[_activeLayer]);
        }

        public void Update()
        {
            entireMap.ForEach(layer => layer.Update());
        }
    }
}
