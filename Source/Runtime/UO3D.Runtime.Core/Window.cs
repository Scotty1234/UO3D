﻿using static SDL3.SDL;
using UO3D.Runtime.Platform;

namespace UO3D.Runtime.Core;

public class Window: IWindow
{
    public event Action<IWindow>? OnResized;

    public IntPtr Handle { get; private set; }

    public uint Width { get; private set; }
    public uint Height { get; private set; }

    public void Startup()
    {
        if (!SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO))
        {
            throw new Exception("SDL_Init failed: " + SDL_GetError());
        }

        string title = "UO3D";

        SDL_WindowFlags initFlags = SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

        Width = 1920;
        Height = 1080;

        Handle = SDL_CreateWindow(title, (int)Width, (int)Height, initFlags);

        if (Handle == IntPtr.Zero)
        {
            throw new Exception("SDL_CreateWindow failed: " + SDL_GetError());
        }

        // Win32 handle if needed
        //Handle = SDL.SDL_GetPointerProperty(SDL.SDL_GetWindowProperties(_sdlHandle), SDL.SDL_PROP_WINDOW_WIN32_HWND_POINTER, IntPtr.Zero);

    }

    public bool PollEvents()
    {
        SDL_Event evt;

        while (SDL_PollEvent(out evt))
        {
            switch ((SDL_EventType)evt.type)
            {
                case SDL_EventType.SDL_EVENT_QUIT:
                    {
                        return true;
                    }

                case SDL_EventType.SDL_EVENT_WINDOW_RESIZED:
                    {
                        Width = (uint)evt.window.data1;
                        Height = (uint)evt.window.data2;

                        OnResized?.Invoke(this);

                    }
                    break;

                default:
                    break;
            }
        }

        return false;
    }

    public void Dispose()
    {
        SDL_DestroyWindow(Handle);
        SDL_QuitSubSystem(SDL_InitFlags.SDL_INIT_VIDEO);
    }
}
