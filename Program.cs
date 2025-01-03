using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using Simulator;
using Univers;
using Renderer;

namespace ClothSimulator;

class Program
{

    static void Main(string[] args)
    {

        var windowSettings = GameWindowSettings.Default;
        windowSettings.RenderFrequency = 0;
        windowSettings.UpdateFrequency = 60;

        var nativeWindowSettings = NativeWindowSettings.Default;
        nativeWindowSettings.Size = new OpenTK.Mathematics.Vector2i(1280, 720);
        nativeWindowSettings.Title = "Cloth Simulator";


        using (var window = new SimulatorWindows(GameWindowSettings.Default, nativeWindowSettings))
        {
            window.Run();
        }
    }

}

