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
        public RenderWindow Win { get; private set; }
        private Vector2u screensize;

        private readonly RenderTexture _target;
        private readonly Sprite _targetSpr;

        private readonly Map _map;

        public Random Rand { get; private set; }

        public TepGame()
        {
            screensize = new Vector2u(1100, 900);
            var cs = new ContextSettings()
            {
                DepthBits = 24,
                StencilBits = 8,
                AntialiasingLevel = 4,
                MajorVersion = 0,
                MinorVersion = 1
            };
            Win = new RenderWindow(new VideoMode(screensize.X, screensize.Y), "The Exiled People", Styles.Close, cs);
            Win.SetFramerateLimit(60);
            Win.SetVerticalSyncEnabled(true);
            Win.SetKeyRepeatEnabled(true);

            Win.Closed += WinClosed;
            Win.KeyReleased += Win_KeyReleased;

            Rand = new Random();

            _target = new RenderTexture(screensize.X, screensize.Y) { Smooth = true };
            _targetSpr = new Sprite();

            _map = new Map(new Vector2u(90, 90), new Vector2u(960, 884), new Vector2f(8, 16));
        }

        public void Initialize()
        {
            _map.Initialize();
        }

        public void Run()
        {
            var lastFPS = 0;
            while (Win.IsOpen())
            {
                var frameStart = DateTime.Now;

                Update();
                Draw();

                // Not super accurate, espcially if frame is working too fast
                var fps = (int)Math.Round(1 / (DateTime.Now - frameStart).TotalSeconds);
                if (fps == lastFPS) { continue; }
                Win.SetTitle(string.Format("The Exiled People | FPS: {0}", fps));
                lastFPS = fps;
            }
        }

        void Update()
        {
            Win.DispatchEvents();
        }

        void Draw()
        {
            _target.Clear();
            _target.Draw(_map);
            _target.Display();
            _targetSpr.Texture = _target.Texture;

            Win.Clear();
            Win.Draw(_targetSpr);
            Win.Display();
        }

        void Win_KeyReleased(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
                Win.Close();
        }

        void WinClosed(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(RenderWindow))
                ((RenderWindow)sender).Close();
        }

        void IDisposable.Dispose()
        {
            _map.Dispose();
            _targetSpr.Dispose();
            Win.Dispose();
        }
    }
}
