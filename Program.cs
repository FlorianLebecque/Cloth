// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using OpenTK.Compute.OpenCL;
using OPENCL;

namespace ClothSimulator{

    class Program{

        static void Main(string[] args){

             /*
                Raylib initialisation
            */

            const int ScreenWidth = 1920;
            const int ScreenHeight = 1080;

            SetConfigFlags(FLAG_MSAA_4X_HINT);
            SetConfigFlags(FLAG_WINDOW_HIGHDPI);

            InitWindow(ScreenWidth, ScreenHeight, "Gravity");

            Camera3D camera = new Camera3D();
            camera.position = new Vector3(300f, 100, 0f);    // Camera position
            camera.target = new Vector3(0.0f, 0.0f, 0.0f);      // Camera looking at point
            camera.up = new Vector3(0.0f, 1.0f, 0.0f);    
            camera.fovy = 90.0f;                                // Camera field-of-view Y
            camera.projection = CAMERA_PERSPECTIVE;

            SetCameraMode(camera, CAMERA_THIRD_PERSON);
            SetTargetFPS(120);

            /*
                Space initialisation
            */

            PhysicSpace ST = new(10f);
            List<Particule> Particuls = new();

            Particule Earth = new(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,50f);
            Particule moon = new(new Vector3(0f, 110f, 0f), new Vector3(0f, 0f, 100), 500,10f);
            Tissue drape = new Tissue(new Vector3(0f,00f,150f),20,20);

            ST.Add(Earth);
            ST.Add(moon);
            Particuls.Add(Earth);
            Particuls.Add(moon);


            foreach(Particule pt in drape.nodes.Values){
                ST.Add(pt);
                Particuls.Add(pt);
            }

            /*
                Model initialisation
            */
            
            Mesh sphere = GenMeshSphere(1f,50,50);
            Model model = LoadModelFromMesh(sphere);//LoadModel("resources/models/bunny.obj");

            Material mt = LoadMaterialDefault();
            Shader shader = LoadShader("resources/shaders/base.vs", "resources/shaders/base.fs");
            int lightPosLoc = GetShaderLocation(shader, "lightPos");
            Vector3 lightPos = new Vector3(0.0f, 0.0f, 0.0f );
            Texture2D texture = LoadTexture("resources/textures/texel_checker.png");

            //RaylibUtils.Utils.SetMaterialShader(ref model,0,ref shader);
            //RaylibUtils.Utils.SetMaterialTexture(ref model,0,MATERIAL_MAP_DIFFUSE,ref texture);
            //RaylibUtils.Utils.MeshTangents(ref model);
            //RaylibUtils.Utils.SetShaderLocation(ref shader,SHADER_LOC_MATRIX_MODEL,"matModel");
            //RaylibUtils.Utils.SetShaderValue(shader,lightPosLoc,lightPos,ShaderUniformDataType.SHADER_UNIFORM_VEC3);

            Matrix4x4[] transforms = new Matrix4x4[Particuls.Count()];

            Particule.model = model;

            /*
                GPU initialisation
            */
            
            Vector3[] acceleration  = new Vector3[Particuls.Count()];
            Vector3[] velocities    = new Vector3[Particuls.Count()];
            Vector3[] positions     = new Vector3[Particuls.Count()];

            float[] dts = new float[Particuls.Count()];

            

            GPU computeGPU = new GPU();
            
            CLBuffer acceleration_buffer  = computeGPU.CreateBuffer<Vector3>(MemoryFlags.ReadWrite ,acceleration);
            CLBuffer velocity_buffer      = computeGPU.CreateBuffer<Vector3>(MemoryFlags.ReadWrite ,velocities);
            CLBuffer positions_buffer     = computeGPU.CreateBuffer<Vector3>(MemoryFlags.ReadWrite ,positions);
            CLBuffer dt_buffer            = computeGPU.CreateBuffer<float>(MemoryFlags.ReadOnly  ,dts);

            CLKernel UpdateVelocity = computeGPU.CreateKernel("OpenCl/velocity.cl","UpdateVelocity");
            CLKernel updatePosition = computeGPU.CreateKernel("OpenCl/position.cl","UpdatePosition");

            computeGPU.SetKernelArg(UpdateVelocity,0,velocity_buffer);
            computeGPU.SetKernelArg(UpdateVelocity,1,acceleration_buffer);
            computeGPU.SetKernelArg(UpdateVelocity,2,dt_buffer);

            computeGPU.SetKernelArg(updatePosition,0,positions_buffer);
            computeGPU.SetKernelArg(updatePosition,1,velocity_buffer);
            computeGPU.SetKernelArg(updatePosition,2,dt_buffer);

            dts = (from i in Enumerable.Range(0, Particuls.Count()) select 0.01f).ToArray();

            computeGPU.Upload<float>(dt_buffer, dts);
            
            /*
                Main loop
            */
            bool use_gpu = true;
            while (!WindowShouldClose()) {
                //Console.WriteLine(GetFrameTime());

                UpdateCamera(ref camera);

                camera.target = Earth.position;

                if(IsKeyDown(KEY_RIGHT)){
                    ST.UpdateForce();
//                    drape.UpdateForce();

                    if(use_gpu){

                        acceleration = ST.SpaceObjects.Select(p => (p.acceleration)).ToArray();
                        velocities   = ST.SpaceObjects.Select(p => (p.velocity)).ToArray();
                        positions    = ST.SpaceObjects.Select(p => (p.position)).ToArray();

                        computeGPU.Upload<Vector3>(acceleration_buffer , acceleration);
                        computeGPU.Upload<Vector3>(velocity_buffer      , velocities);
                        computeGPU.Upload<Vector3>(positions_buffer     , positions);

                        computeGPU.Execute(UpdateVelocity,1,Particuls.Count());
                        computeGPU.Download<Vector3>(velocity_buffer,velocities);

                        computeGPU.Execute(updatePosition,1,Particuls.Count());
                        computeGPU.Download<Vector3>(positions_buffer,positions);


                        for(int i = 0; i < Particuls.Count();i++){
                            Particuls[i].velocity = velocities[i];
                            Particuls[i].position = positions[i];
                        }

                    }else{
                        ST.UpdateVelocity();
                        ST.UpdatePosition();
                    }                    

                    ST.CheckCollision();
                    ST.ClearForce();
                }
                
                BeginDrawing();
                    ClearBackground(BLACK);

                    BeginMode3D(camera);
                        //drape.Draw();    

                        for(int i = 0;i < Particuls.Count();i++){
                            
                            Particuls[i].Draw();
                            //model.transform = transforms[i];
                        }

                        DrawGrid(100, 10.0f);
                    EndMode3D();

                    DrawFPS(10, 10);

                EndDrawing();
            }

        }

    }
    
}