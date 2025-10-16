using SDL3;
using UO3D.Runtime.Platform;

namespace UO3D.Runtime.Core;

public class Window: IWindow
{
    public IntPtr Handle { get; private set; }

    public uint Width { get; private set; }
    public uint Height { get; private set; }

    public void Startup()
    {
        if (!SDL.SDL_Init(SDL.SDL_InitFlags.SDL_INIT_VIDEO))
        {
            throw new Exception("SDL_Init failed: " + SDL.SDL_GetError());
        }

        string title = "UO3D";

        SDL.SDL_WindowFlags initFlags = SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

        Width = 1920;
        Height = 1080;

        Handle = SDL.SDL_CreateWindow(title, (int)Width, (int)Height, initFlags);

        if (Handle == IntPtr.Zero)
        {
            throw new Exception("SDL_CreateWindow failed: " + SDL.SDL_GetError());
        }

        // Win32 handle if needed
        //Handle = SDL.SDL_GetPointerProperty(SDL.SDL_GetWindowProperties(_sdlHandle), SDL.SDL_PROP_WINDOW_WIN32_HWND_POINTER, IntPtr.Zero);

    }

    public bool PollEvents()
    {
        SDL.SDL_Event evt;

        while (SDL.SDL_PollEvent(out evt))
        {
            switch ((SDL.SDL_EventType)evt.type)
            {
                case SDL.SDL_EventType.SDL_EVENT_QUIT:
                    {
                        return true;
                    }

                default:
                    break;
            }
        }

        return false;
    }

    public void Dispose()
    {
        SDL.SDL_DestroyWindow(Handle);
        SDL.SDL_QuitSubSystem(SDL.SDL_InitFlags.SDL_INIT_VIDEO);
    }
}
