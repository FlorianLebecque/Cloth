// See https://aka.ms/new-console-template for more information
using Cloth.utils;

using System.Runtime.InteropServices;

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
            camera.position = new Vector3(300f, 0, 0f);    // Camera position
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
            Particule moon = new(new Vector3(0f, 110f, 0f), new Vector3(0f, 0f, 0), 500,10f);
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
            IntPtr size_vec3  = new IntPtr(System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3))*Particuls.Count());
            IntPtr size_float = new IntPtr(sizeof(float)*Particuls.Count());

            IMem acceleration_buffer  = computeGPU.CreateBuffer(MemFlags.ReadWrite ,size_vec3);
            IMem velocity_buffer      = computeGPU.CreateBuffer(MemFlags.ReadWrite ,size_vec3);
            IMem positions_buffer     = computeGPU.CreateBuffer(MemFlags.ReadWrite ,size_vec3);
            IMem dt_buffer            = computeGPU.CreateBuffer(MemFlags.ReadOnly  ,size_float);

            Kernel UpdateVelocity = computeGPU.CreateKernel("OpenCl/velocity.cl","UpdateVelocity");
            Kernel updatePosition = computeGPU.CreateKernel("OpenCl/position.cl","UpdatePosition");

            computeGPU.SetKernelArg(UpdateVelocity,0,velocity_buffer);
            computeGPU.SetKernelArg(UpdateVelocity,1,acceleration_buffer);
            computeGPU.SetKernelArg(UpdateVelocity,2,dt_buffer);

            computeGPU.SetKernelArg(updatePosition,0,positions_buffer);
            computeGPU.SetKernelArg(updatePosition,1,velocity_buffer);
            computeGPU.SetKernelArg(updatePosition,2,dt_buffer);

            dts = (from i in Enumerable.Range(0, Particuls.Count()) select 0.005f).ToArray();

            computeGPU.Upload(dt_buffer, size_float, dts);

            /*
                Main loop
            */
            bool use_gpu = false;
            while (!WindowShouldClose()) {
                //Console.WriteLine(GetFrameTime());

                UpdateCamera(ref camera);

                camera.target = Earth.position;

                if(IsKeyDown(KEY_RIGHT)){
                    ST.UpdateForce();
                    drape.UpdateForce();

                    if(use_gpu){

                        acceleration = ST.SpaceObjects.Select(p => (p.acceleration)).ToArray();
                        velocities   = ST.SpaceObjects.Select(p => (p.velocity)).ToArray();
                        positions    = ST.SpaceObjects.Select(p => (p.position)).ToArray();

                        computeGPU.Upload(acceleration_buffer , size_vec3 , acceleration);
                        computeGPU.Upload(velocity_buffer     , size_vec3 , velocities);
                        computeGPU.Upload(positions_buffer    , size_vec3 , positions);


                        computeGPU.Execute(UpdateVelocity,1,Particuls.Count());
                        computeGPU.Download(velocity_buffer,size_vec3,velocities);


                        computeGPU.Execute(updatePosition,1,Particuls.Count());
                        computeGPU.Download(positions_buffer,size_vec3,positions);


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
                        drape.Draw();    

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

        static OpenCL.Net.float3 toFloat3(Vector3 foo){
            return new OpenCL.Net.float3(foo.X,foo.Y,foo.Z);
        }

        static Vector3 toVect3(OpenCL.Net.float3 foo){
            return new Vector3(foo.x,foo.y,foo.z);
        }

        static void Test(){

        
            GPU myGpu = new GPU();

            const int count = 1024;
            IntPtr size = new IntPtr(sizeof(float)*count);
            IMem input  = myGpu.CreateBuffer(MemFlags.ReadOnly ,size);
            IMem output = myGpu.CreateBuffer(MemFlags.WriteOnly,size);

            Kernel myTestKernel = myGpu.CreateKernel("OpenCl/test.cl","My_Function");

            // Génération des données de tests aléatoires
            float[] data_in  = (from i in Enumerable.Range(0, count) select (float)1f).ToArray();
            float[] data_out = (from i in Enumerable.Range(0, count) select (float)0f).ToArray();

            //myGpu.Upload(myTestKernel,input ,0,size,data_in);
            //myGpu.Upload(myTestKernel,output,1,size,data_out);
            
            myGpu.Execute(myTestKernel,1,count);

        
            float[] results = new float[count];
            myGpu.Download(output,size,results);
        
            foreach(float f in results){
                Console.WriteLine(f);
            }
        
        }
    }
    
}