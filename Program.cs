// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using OpenTK.Compute.OpenCL;
using OPENCL;

namespace ClothSimulator{

    class Program{

        static void Main(string[] args){

#region  RAYLIB
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

#endregion

#region MODEL_INIT
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
            Particule.model = model;

#endregion

#region UNIVERS
            /*
                Univer initialisation
            */
            ParticuleDrawer.model = model;
            Random rnd = new Random();
            List<Particule_obj> entities = new List<Particule_obj>();

            entities.Add(new Particule_obj(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,50f,0.95f));
            entities.Add(new Particule_obj(new Vector3(0f, 110f, 0f), new Vector3(0f, -20f, 0), 1000,10f,0.95f));

            for(int i = 0; i < 6400; i++){
                entities.Add(new Particule_obj(
                    new Vector3(rnd.Next(-1000,1000),rnd.Next(-1000,1000),rnd.Next(-1000,1000)),
                    new Vector3(rnd.Next(-100,100),rnd.Next(-100,100),rnd.Next(-100,100)),
                    rnd.Next(1,10),
                    rnd.Next(1,10),
                    0.9f
                ));
            }

            Particule_obj[] output_enties = new Particule_obj[entities.Count()];
            output_enties = entities.ToArray();

            Raylib_cs.Color[] colorArray = new Raylib_cs.Color[entities.Count()];
            for(int i = 0; i < entities.Count();i++){
                colorArray[i] = new Raylib_cs.Color(GetRandomValue(100,255),GetRandomValue(100,255),GetRandomValue(100,255),255);
            }

#endregion

#region GPU_INIT
            /*
                GPU initialisation
            */
            
            GPU computeGPU = new GPU();
            CLBuffer B1 = computeGPU.CreateBuffer<Particule_obj>(MemoryFlags.ReadWrite,entities.ToArray());
            CLBuffer B2 = computeGPU.CreateBuffer<Particule_obj>(MemoryFlags.ReadWrite,entities.ToArray());

            CLKernel gravity_applier   = computeGPU.CreateKernel("OpenCl/particul_gravity.cl","ComputeGravity");
            CLKernel velocity_applier  = computeGPU.CreateKernel("OpenCl/particul_velocity.cl","ComputeVelocity");
            CLKernel position_applier  = computeGPU.CreateKernel("OpenCl/particul_position.cl","ComputePosition");
            CLKernel collision_applier = computeGPU.CreateKernel("OpenCl/particul_collision.cl","ComputeCollision");

            computeGPU.SetKernelArg(gravity_applier,0,B1);
            computeGPU.SetKernelArg(gravity_applier,1,B2);

            computeGPU.SetKernelArg(velocity_applier,0,B1);
            computeGPU.SetKernelArg(velocity_applier,1,B2);

            computeGPU.SetKernelArg(position_applier,0,B1);
            computeGPU.SetKernelArg(position_applier,1,B2);
            
            computeGPU.SetKernelArg(collision_applier,0,B1);
            computeGPU.SetKernelArg(collision_applier,1,B2);

            computeGPU.Upload<Particule_obj>(B1,entities.ToArray());
#endregion


            /*
                Main loop
            */
            while (!WindowShouldClose()) {
                //Console.WriteLine(GetFrameTime());

                UpdateCamera(ref camera);

                camera.target = output_enties[0].position;

                if(IsKeyDown(KEY_RIGHT)){

                    computeGPU.Execute(gravity_applier ,1,entities.Count());

                    computeGPU.Download<Particule_obj>(B2,output_enties);
                    computeGPU.Upload<Particule_obj>(B1,output_enties);
                    
                    computeGPU.Execute(velocity_applier,1,entities.Count());

                    computeGPU.Download<Particule_obj>(B2,output_enties);
                    computeGPU.Upload<Particule_obj>(B1,output_enties);

                    computeGPU.Execute(position_applier,1,entities.Count());

                    computeGPU.Download<Particule_obj>(B2,output_enties);
                    computeGPU.Upload<Particule_obj>(B1,output_enties);

                    computeGPU.Execute(collision_applier,1,entities.Count());

                    computeGPU.Download<Particule_obj>(B2,output_enties);
                    computeGPU.Upload<Particule_obj>(B1,output_enties);
                }
                
                BeginDrawing();
                    ClearBackground(BLACK);

                    BeginMode3D(camera);

                        ParticuleDrawer.Draw(output_enties,colorArray);

                        DrawGrid(100, 10.0f);
                    EndMode3D();

                    DrawFPS(10, 10);

                EndDrawing();
            }

        }

    }
    
}