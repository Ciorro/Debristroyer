using HlyssMG;
using Microsoft.Xna.Framework;
using System;
using TrijamJDS.Scenes;

namespace TrijamJDS
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var config = new GameConfig()
            {
                StartScene = new IntroScene(),
                DesiredWidth = 1920,
                DesiredHeight = 1080,
                IsPixelArtGame = true,
                ClearColor = Color.Black
            };

            if(args.Length > 2)
            {
                config.IsFullscreen = args[0] == "fullscreen";
                config.WindowWidth = int.Parse(args[1]);
                config.WindowHeight = int.Parse(args[2]);
            }

            using (var game = new GameBase(config))
                game.Run();
        }
    }
}
