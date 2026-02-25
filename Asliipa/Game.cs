using SDL2;
using System.Numerics;

namespace Whizz
{
    public class Game
    {
        public static LoggerAgent VisualLoggerAgent;
        public static LoggerAgent StorageLoggerAgent;

        private nint MainRenderer;
        private nint MainWindow;

        private Logger MainLogger;
        private LoggerAgent MainLoggerAgent;

        public Game(Vector2 windowSize)
        {
            int result;
            MainLogger = new() { LogFilter = [LogLevel.Trace] };
            MainLoggerAgent = MainLogger.GetAgent("Main");
            VisualLoggerAgent = MainLogger.GetAgent("Visual");
            StorageLoggerAgent = MainLogger.GetAgent("Storage");

            result = SDL.SDL_CreateWindowAndRenderer(
                (int)windowSize.X,
                (int)windowSize.Y,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE,
                out MainWindow, out MainRenderer);

            if (result == 0)
                MainLoggerAgent.Log("SDL initialization successful", LogLevel.Trace);
            else
                MainLoggerAgent.Log("SDL initialization failure", LogLevel.Fatal);

            Material.AtlasTexture = SDL_image.IMG_LoadTexture(MainRenderer, "./Resources/tiles.png");
        }

        public void Start()
        {
            MainLoggerAgent.Log("Application Started", LogLevel.Info);

            Material grass = new("grass", Vector2.Zero);
            Tile t = new() { Material = grass };
            // Chunk c = new Chunk().DebugFill(t);
            Noise noise = new Noise(seed: 13667);
            Chunk c = new Chunk().GenerateChunk(noise);

            var stream = new MemoryStream();
            c.Serialize(stream);
            stream.Position = 0;
            c.Deserialize(stream);

            bool quit = false;
            SDL.SDL_Event e;
            while (!quit)
            {
                while (SDL.SDL_PollEvent(out e) != 0)
                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                        quit = true;

                SDL.SDL_RenderClear(MainRenderer);

                c.RenderChunkAt(MainRenderer, Vector2.Zero, 0);

                SDL.SDL_RenderPresent(MainRenderer);
            }


            SDL.SDL_DestroyWindow(MainWindow);
            SDL.SDL_Quit();
        }
    }
}
