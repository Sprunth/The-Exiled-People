using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Exiled_People
{
    class Program
    {
        public static TepGame ActiveGame;
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

            ActiveGame = new TepGame();
            ActiveGame.Initialize();
            ActiveGame.Run();
        }
    }
}
