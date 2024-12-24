using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using System;
using System.Runtime.InteropServices;

namespace WindowSize;

public sealed unsafe class Plugin : IDalamudPlugin
{
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

    private const string CommandName = "/windowsize";

    public const int HWND_NOTOPMOST = -2;

    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public struct POINT
    {
        public int X;
        public int Y;
    }


    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern bool GetWindowRect(nint hWnd, RECT* lpRect);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern bool GetClientRect(nint hWnd, RECT* lpRect);

    [DllImport("user32.dll", ExactSpelling = true)]
    static extern bool ClientToScreen(IntPtr hWnd, POINT* lpPoint);


    public Plugin()
    {
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) { });
        ResizeWindow();
    }

    public void Dispose()
    {
        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        ResizeWindow();
    }

    private void ResizeWindow()
    {
        var hWnd = Framework.Instance()->GameWindow->WindowHandle;
        RECT window;
        RECT client;

        GetWindowRect(hWnd, &window);
        GetClientRect(hWnd, &client);

        POINT p = new() { X = client.Left, Y = client.Top };
        POINT p2 = new() { X = client.Right, Y = client.Bottom };
        ClientToScreen(hWnd, &p);
        ClientToScreen(hWnd, &p2);

        var border_left = p.X - window.Left;
        var  border_top = p.Y - window.Top;
        var border_right = window.Right - p2.X;

        SetWindowPos(hWnd, HWND_NOTOPMOST, -border_left, -border_top, 3840 + border_right + 11, 2160 + border_top + 11, 0);
    }
}
