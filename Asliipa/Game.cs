using SDL2;
using System.Numerics;

namespace Whizz
{
    public class Game
    {
        private nint MainRenderer;
        private nint MainWindow;

        private Logger MainLogger;
        private LoggerAgent MainLoggerAgent;

        public Game(Vector2 windowSize)
        {
            int result;
            MainLogger = new() { LogFilter = [LogLevel.Trace] };
            MainLoggerAgent = MainLogger.GetAgent("Main");

            result = SDL.SDL_CreateWindowAndRenderer(
                (int)windowSize.X,
                (int)windowSize.Y,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN,
                out MainWindow, out MainRenderer);

            if (result == 0)
                MainLoggerAgent.Log("SDL initialization successful", LogLevel.Trace);
            else
                MainLoggerAgent.Log("SDL initialization failure", LogLevel.Fatal);
            
        }

        public void Start()
        {
            MainLoggerAgent.Log("Application Started", LogLevel.Info);

            bool quit = false;
            SDL.SDL_Event e;
            while (!quit)
                while (SDL.SDL_PollEvent(out e) != 0)
                    if (e.type == SDL.SDL_EventType.SDL_QUIT)
                        quit = true;

            SDL.SDL_DestroyWindow(MainWindow);
            SDL.SDL_Quit();
        }
    }
}
