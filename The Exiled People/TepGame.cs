using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace The_Exiled_People
{
    class TepGame : IDisposable
    {
        private readonly RenderWindow _window;
        private Vector2u screensize;

        private readonly RenderTexture _target;
        private readonly Sprite _targetSpr;

        private readonly Map _map;

        public TepGame()
        {
            screensize = new Vector2u(1100, 1100);
            var cs = new ContextSettings()
            {
                DepthBits = 24,
                StencilBits = 8,
                AntialiasingLevel = 4,
                MajorVersion = 0,
                MinorVersion = 1
            };
            _window = new RenderWindow(new VideoMode(screensize.X, screensize.Y), "The Exiled People", Styles.Close, cs);
            _window.SetFramerateLimit(60);
            _window.SetVerticalSyncEnabled(true);

            _window.Closed += _window_Closed;

            _target = new RenderTexture(screensize.X, screensize.Y) { Smooth = true };
            _targetSpr = new Sprite();

            _map = new Map(new Vector2u(90, 90));
        }

        void _window_Closed(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(RenderWindow))
                ((RenderWindow)sender).Close();
        }

        public void Initialize()
        {
            
        }

        public void Run()
        {
            var lastFPS = 0;
            while (_window.IsOpen())
            {
                var frameStart = DateTime.Now;

                Update();
                Draw();

                // Not super accurate, espcially if frame is working too fast
                var fps = (int)Math.Round(1 / (DateTime.Now - frameStart).TotalSeconds);
                if (fps == lastFPS) { continue; }
                _window.SetTitle(string.Format("The Exiled People | FPS: {0}", fps));
                lastFPS = fps;
            }
        }

        void Update()
        {
            _window.DispatchEvents();
            if (DateTime.Now.Millisecond % 10 == 0)
                _map.Update();
        }

        void Draw()
        {
            _target.Clear();
            _target.Draw(_map);
            _target.Display();
            _targetSpr.Texture = _target.Texture;

            _window.Clear();
            _window.Draw(_targetSpr);
            _window.Display();
        }

        void IDisposable.Dispose()
        {
            _targetSpr.Dispose();
            _window.Dispose();
        }
    }
}
