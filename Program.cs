
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.ConfigFlags;
using static Raylib_cs.CameraProjection;



using Simulator;
using Univers;
using Renderer;

namespace ClothSimulator;

class Program{

    static void Main(string[] args){


        const int ScreenWidth = 1920;
        const int ScreenHeight = 1080;
        Vector3 WORLD_UP = new Vector3(0,1,0);


        /*
            Raylib initialisation
        */
        SetConfigFlags(FLAG_MSAA_4X_HINT);
        SetConfigFlags(FLAG_WINDOW_HIGHDPI);

        InitWindow(ScreenWidth, ScreenHeight, "Gravity");

        Camera3D camera = new Camera3D();
        camera.position = new Vector3(2.5f, 400f, 3.0f);    // Camera position
        camera.target = new Vector3(0.0f, 0.0f, 0.7f);      // Camera looking at point
        camera.up = WORLD_UP;    
        camera.fovy = 90.0f;                                // Camera field-of-view Y
        camera.projection = CAMERA_PERSPECTIVE;

        SetCameraMode(camera, CameraMode.CAMERA_THIRD_PERSON);
        SetTargetFPS(120);


        UniversCreator.CreateUnivers1(WORLD_UP);

        RaylibRenderer RR = new RaylibRenderer(camera,UniversCreator.colors);

        UniverSimulation us = new UniverSimulation(UniversCreator.univers);

        us.Init(UniversCreator.entities,UniversCreator.clothList);


        /*
            Main loop
        */   
        while (!WindowShouldClose()) {
            RR.UpdateCamera();

            us.CheckControl();
            us.Simulate();
            
            RR.CheckControl(us);
            RR.UpdateLight(us);

            RR.Draw(us);
        }

    }

}
    
