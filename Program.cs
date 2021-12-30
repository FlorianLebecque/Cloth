// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;
using OpenTK.Compute.OpenCL;
using OPENCL;
using Cloth.classes.windows;

using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Color;
using static Raylib_cs.ConfigFlags;
using static Raylib_cs.CameraMode;
using static Raylib_cs.CameraProjection;
using static Raylib_cs.KeyboardKey;
using static Raylib_cs.ShaderLocationIndex;
using static Raylib_cs.MaterialMapIndex;

namespace ClothSimulator{

    class Program{

        static void Main(string[] args){


            const int ScreenWidth = 1920;
            const int ScreenHeight = 1080;

            //using(SimulationWindows sw = new SimulationWindows(ScreenWidth,ScreenHeight,"Gravity")){
            //    sw.Run();
            //}


#region  RAYLIB
             /*
                Raylib initialisation
            */

            

            SetConfigFlags(FLAG_MSAA_4X_HINT);
            SetConfigFlags(FLAG_WINDOW_HIGHDPI);

            InitWindow(ScreenWidth, ScreenHeight, "Gravity");

            Camera3D camera = new Camera3D();
            camera.position = new Vector3(400f, 100, 0f);    // Camera position
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
            

#endregion

#region UNIVERS
            /*
                Univer initialisation
            */
            ParticuleDrawer.model = model;
            Random rnd = new Random();
            List<Particule> entities = new List<Particule>();
            List<Raylib_cs.Color> colors = new List<Color>();

            entities.Add(new Particule(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,150f,1.5f,0.0f));

            entities.Add(new Particule(new Vector3(15f, 350, 15f), new Vector3(-20f, -180, 0), 500,15,0.95f,0.01f));

            entities.Add(new Particule(new Vector3(10f, -150, 0f), new Vector3(5f, 90f, 200), 500,15,0.95f,0.01f));

            entities.Add(new Particule(new Vector3(250, 0f, 0f), new Vector3(0f, 0f, 60), 1000,25f,0.95f,0.1f));

            colors.Add(new Raylib_cs.Color(237, 217, 0,255));
            colors.Add(new Raylib_cs.Color(0, 230, 207,255));
            colors.Add(new Raylib_cs.Color(0, 230, 207,255));
            colors.Add(new Raylib_cs.Color(49, 224, 0,255));


            Tissue drape = new Tissue(new Vector3(0,300,2),45,45,entities,colors);
            Spring_force[] spring_forces = new Spring_force[drape.springs.Count()];
            
            
            for(int i = 0; i < 500; i++){
                entities.Add(new Particule(
                    new Vector3(rnd.Next(-500,500),rnd.Next(-50,50),rnd.Next(-500,500)),
                    new Vector3(rnd.Next(-100,100),rnd.Next(-5,5),rnd.Next(-100,100)),
                    rnd.Next(1,10),
                    rnd.Next(1,5),
                    0.9f,
                    ((float)rnd.NextDouble())*0.1f
                ));
                colors.Add(new Raylib_cs.Color(GetRandomValue(200,255),GetRandomValue(10,50),GetRandomValue(10,50),255));
            }
            

            Particule[] output_enties = new Particule[entities.Count()];
            output_enties = entities.ToArray();

            Raylib_cs.Color[] colorArray = colors.ToArray();


#endregion

#region GPU_INIT
            /*
                GPU initialisation
            */
            
            GPU computeGPU = new GPU();
            CLBuffer B1 = computeGPU.CreateBuffer<Particule>(MemoryFlags.ReadWrite,entities.ToArray());
            CLBuffer B2 = computeGPU.CreateBuffer<Particule>(MemoryFlags.ReadWrite,entities.ToArray());
            CLBuffer B3 = computeGPU.CreateBuffer<Particule>(MemoryFlags.ReadWrite,entities.ToArray());

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

            computeGPU.SetKernelArg(springs_applier,0,B2);
            computeGPU.SetKernelArg(springs_applier,1,BSpring);
            computeGPU.SetKernelArg(springs_applier,2,BSpringF);

            computeGPU.SetKernelArg(springsF_applier,0,B1);
            computeGPU.SetKernelArg(springsF_applier,1,B2);
            computeGPU.SetKernelArg(springsF_applier,2,BSpringF);
            computeGPU.SetKernelArg(springsF_applier,3,BCloth);

            computeGPU.SetKernelArg(velocity_applier,0,B1);
            computeGPU.SetKernelArg(velocity_applier,1,B2);

            computeGPU.SetKernelArg(position_applier,0,B1);
            computeGPU.SetKernelArg(position_applier,1,B2);
            
            computeGPU.SetKernelArg(collision_applier,0,B1);
            computeGPU.SetKernelArg(collision_applier,1,B2);
            

            computeGPU.Upload<Particule>(B1,entities.ToArray());
            computeGPU.Upload<Spring>(BSpring,drape.springs);
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

                //if(IsKeyDown(KEY_RIGHT)){

                    computeGPU.Execute(gravity_applier ,1,entities.Count());     
                    computeGPU.Execute(springs_applier,1,drape.springs.Count());

                    computeGPU.Download<Particule>(B2,output_enties);
                    computeGPU.Upload<Particule>(B1,output_enties);

                    computeGPU.Execute(springsF_applier,1,drape.settings.count);

                    computeGPU.Download<Particule>(B2,output_enties);
                    computeGPU.Upload<Particule>(B1,output_enties);

                    computeGPU.Execute(velocity_applier,1,entities.Count());

                    computeGPU.Download<Particule>(B2,output_enties);
                    computeGPU.Upload<Particule>(B1,output_enties);

                    computeGPU.Execute(position_applier,1,entities.Count());

                    computeGPU.Download<Particule>(B2,output_enties);
                    computeGPU.Upload<Particule>(B1,output_enties);

                    computeGPU.Execute(collision_applier,1,entities.Count());

                    computeGPU.Download<Spring>(BSpring,drape.springs);
                    computeGPU.Download<Particule>(B2,output_enties);
                    computeGPU.Upload<Particule>(B1,output_enties);
                //}
                


                BeginDrawing();
                    ClearBackground(BLACK);

                    BeginMode3D(camera);
                        ParticuleDrawer.DrawSprings(output_enties,drape);
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