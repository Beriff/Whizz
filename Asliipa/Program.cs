using SDL2;

var window = SDL.SDL_CreateWindow(
    "asliipa",
    SDL.SDL_WINDOWPOS_CENTERED,
    SDL.SDL_WINDOWPOS_CENTERED,
    800,
    600,
    SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

bool quit = false;
SDL.SDL_Event e;
while (!quit)
    while (SDL.SDL_PollEvent(out e) != 0)
        if (e.type == SDL.SDL_EventType.SDL_QUIT)
            quit = true;

SDL.SDL_DestroyWindow(window);
SDL.SDL_Quit();
