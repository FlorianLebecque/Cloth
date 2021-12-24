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

            entities.Add(new Particule_obj(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,50f,0.55f,0.1f));
            entities.Add(new Particule_obj(new Vector3(0f, 150, 0f), new Vector3(0f, 0f, 100), 500,15,0.95f,0.1f));
            entities.Add(new Particule_obj(new Vector3(500, 0f, 0f), new Vector3(0f, 0f, 50), 1000,25f,0.95f,0.1f));

            Tissue drape = new Tissue(new Vector3(0,2,300),50,50,entities);
            Spring_force[] spring_forces = new Spring_force[drape.springs.Count()];
            
            
            for(int i = 0; i < 500; i++){
                entities.Add(new Particule_obj(
                    new Vector3(rnd.Next(-500,500),rnd.Next(-500,500),rnd.Next(-500,500)),
                    new Vector3(rnd.Next(-50,50),rnd.Next(-50,50),rnd.Next(-50,50)),
                    rnd.Next(1,10),
                    rnd.Next(1,10),
                    0.9f,
                    (float)rnd.NextDouble()
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
            CLBuffer BSpring = computeGPU.CreateBuffer<Spring>(MemoryFlags.ReadWrite,drape.springs.ToArray());
            CLBuffer BSpringF = computeGPU.CreateBuffer<Spring_force>(MemoryFlags.ReadWrite,spring_forces);
            CLBuffer BCloth = computeGPU.CreateBuffer<Cloth_settings>(MemoryFlags.ReadWrite,new Cloth_settings[1]{drape.settings});

            CLKernel gravity_applier   = computeGPU.CreateKernel("OpenCl/particul_gravity.cl","ComputeGravity");
            CLKernel velocity_applier  = computeGPU.CreateKernel("OpenCl/particul_velocity.cl","ComputeVelocity");
            CLKernel position_applier  = computeGPU.CreateKernel("OpenCl/particul_position.cl","ComputePosition");
            CLKernel collision_applier = computeGPU.CreateKernel("OpenCl/particul_collision.cl","ComputeCollision");
            CLKernel springs_applier = computeGPU.CreateKernel("OpenCl/particul_spring.cl","ComputeSpring");
            CLKernel springsF_applier = computeGPU.CreateKernel("OpenCl/spring_applier.cl","ComputeSpringForce");

            computeGPU.SetKernelArg(gravity_applier,0,B1);
            computeGPU.SetKernelArg(gravity_applier,1,B2);

            computeGPU.SetKernelArg(velocity_applier,0,B1);
            computeGPU.SetKernelArg(velocity_applier,1,B2);

            computeGPU.SetKernelArg(position_applier,0,B1);
            computeGPU.SetKernelArg(position_applier,1,B2);
            
            computeGPU.SetKernelArg(collision_applier,0,B1);
            computeGPU.SetKernelArg(collision_applier,1,B2);

            computeGPU.SetKernelArg(springs_applier,0,B1);
            computeGPU.SetKernelArg(springs_applier,1,BSpring);
            computeGPU.SetKernelArg(springs_applier,2,BSpringF);

            computeGPU.SetKernelArg(springsF_applier,0,B1);
            computeGPU.SetKernelArg(springsF_applier,1,B2);
            computeGPU.SetKernelArg(springsF_applier,2,BSpringF);
            computeGPU.SetKernelArg(springsF_applier,3,BCloth);

            computeGPU.Upload<Particule_obj>(B1,entities.ToArray());
            computeGPU.Upload<Spring>(BSpring,drape.springs.ToArray());
            computeGPU.Upload<Cloth_settings>(BCloth,new Cloth_settings[1]{drape.settings});
#endregion


            /*
                Main loop
            */

            int current_view = 0;
            bool showgrid = false;
            while (!WindowShouldClose()) {
                //Console.WriteLine(GetFrameTime());

                UpdateCamera(ref camera);                

                if(IsKeyPressed(KEY_LEFT)){
                    showgrid = !showgrid;
                }

                if(IsKeyPressed(KEY_UP)){
                    current_view--;
                }
                if(IsKeyPressed(KEY_DOWN)){
                    current_view++;
                }

                if(current_view < 0) {
                    current_view = entities.Count()-1;
                }

                if(current_view == entities.Count()){
                    current_view = 0;
                }

                camera.target = output_enties[current_view].position;

                if(IsKeyDown(KEY_RIGHT)){

                    computeGPU.Execute(gravity_applier ,1,entities.Count());

                    computeGPU.Download<Particule_obj>(B2,output_enties);
                    computeGPU.Upload<Particule_obj>(B1,output_enties);

                    computeGPU.Execute(springs_applier,1,drape.springs.Count());

                    computeGPU.Execute(springsF_applier,1,drape.settings.count);

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
                        if(showgrid)
                            DrawGrid(100, 10.0f);
                    EndMode3D();

                    DrawFPS(10, 10);
                    DrawText(current_view.ToString(),10,50,20,Color.DARKGREEN);

                EndDrawing();
            }

        }

    }
    
}