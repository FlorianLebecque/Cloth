
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
        camera.position = new Vector3(250f, 50f, 0f);    // Camera position
        camera.target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
        camera.up = WORLD_UP;    
        camera.fovy = 90.0f;                                // Camera field-of-view Y
        camera.projection = CAMERA_PERSPECTIVE;

        SetCameraMode(camera, CameraMode.CAMERA_THIRD_PERSON);
        SetTargetFPS(120);

        //useless initialisation -> go check Load function
        UniversCreator.CreateUnivers3(WORLD_UP);
        RaylibRenderer renderer = new RaylibRenderer(camera,UniversCreator.colors);
        UniverSimulation univer_simulation = new UniverSimulation(UniversCreator.univers,UniversCreator.entities,UniversCreator.clothList);

        Menu menu = new Menu(camera);
        menu.Add("2 Rings with cloth and planet collision");
        menu.Add("Cloth on a sphere");


        /*
            Main loop
        */   
        int selected = -1;
        bool loaded = false;
        while (!WindowShouldClose()) {

            selected = menu.Run();
            
            if(selected != -1){

                if(loaded == false){
                    loaded = true;
                    Load(selected,WORLD_UP,camera,ref renderer,ref univer_simulation);
                }     

                if(IsKeyPressed(KeyboardKey.KEY_BACKSPACE)){
                    menu.reRun();
                    loaded = false;
                    selected = -1;
                    //univer_simulation.Clear();
                }

                univer_simulation.Run();
                renderer.Run(univer_simulation);
            }
        }

    }

    public static void Load(int selected,Vector3 WORLD_UP,Camera3D camera, ref RaylibRenderer renderer,ref UniverSimulation univer_simulation){
        
        switch(selected){
            case 0:
                UniversCreator.CreateTestUniver(WORLD_UP);
                break;
            case 1:
                UniversCreator.CreateUnivers2(WORLD_UP);
                break;
            case 2:
                UniversCreator.CreateUnivers3(WORLD_UP);
                break;
            case 3:
                UniversCreator.CreateMiniUniver(WORLD_UP);
                break;
        }

        SetCameraMode(camera,CameraMode.CAMERA_THIRD_PERSON);

        renderer = new RaylibRenderer(camera,UniversCreator.colors);
        univer_simulation = new UniverSimulation(UniversCreator.univers,UniversCreator.entities,UniversCreator.clothList);
    }

}
    
