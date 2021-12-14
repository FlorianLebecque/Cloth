// See https://aka.ms/new-console-template for more information


namespace ClothSimulator{

    class Program{

        static void Main(string[] args){

             /*
                Raylib initialisation
            */

            const int ScreenWidth = 1280;
            const int ScreenHeight = 720;

            SetConfigFlags(FLAG_MSAA_4X_HINT);
            SetConfigFlags(FLAG_WINDOW_HIGHDPI);

            InitWindow(ScreenWidth, ScreenHeight, "Gravity");

            Camera3D camera = new Camera3D();
            camera.position = new Vector3(0f, 60f, 1.0f);    // Camera position
            camera.target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
            camera.up = new Vector3(0.0f, 1.0f, 0.0f);    
            camera.fovy = 90.0f;                                // Camera field-of-view Y
            camera.projection = CAMERA_PERSPECTIVE;

            SetCameraMode(camera, CAMERA_THIRD_PERSON);
            SetTargetFPS(60);


            PhysicSpace ST = new(1f);
            List<Particule> DrawingParticul = new();

            Particule Earth = new(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 10000f,5f);
            //Particule moon = new(new Vector3(30f, 0f, 0f), new Vector3(0f, 0f, 38f), 1f,1f);


            Tissue drape = new Tissue(new Vector3(0f,20f,0f),15,15);

            

            foreach(Particule pt in drape.nodes.Values){
                ST.Add(pt);
                DrawingParticul.Add(pt);
            }

            ST.Add(Earth);
            //ST.Add(moon);
            DrawingParticul.Add(Earth);
            //DrawingParticul.Add(moon);

            while (!WindowShouldClose()) {
                //Console.WriteLine(GetFrameTime());

                UpdateCamera(ref camera);

                BeginDrawing();
                ClearBackground(BLACK);

                BeginMode3D(camera);                
                
                if(IsKeyDown(KEY_RIGHT)){
                    ST.UpdateForce();
                    drape.UpdateForce();

                    ST.UpdateVelocity();

                    ST.UpdatePosition();

                    ST.CheckCollision();
                   
                }
                ST.ClearForce();
                

                drape.Draw();    
                foreach(Particule p in DrawingParticul){
                    p.Draw();
                }

                

                DrawGrid(100, 10.0f);
                EndMode3D();

                DrawFPS(10, 10);

                EndDrawing();
            }

        }

    }
    
}