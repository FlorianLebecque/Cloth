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

using Cloth.classes.tree;

namespace ClothSimulator{

    class Program{

        static void Main(string[] args){


            const int ScreenWidth = 1920;
            const int ScreenHeight = 1080;


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
            
            Mesh sphere = GenMeshSphere(1f,75,50);
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
                ---------------------------------------------------------------------------------------------
                        Univer initialisation

                    The univer object is a struct containing the univers properties
                        - G
                        - dt

                ---------------------------------------------------------------------------------------------
            */

            Univers univers = new Univers(10f,0.01f);

            ParticuleDrawer.model = model;
            Random rnd = new Random(3);
            List<Particule> entities = new List<Particule>();
            List<Raylib_cs.Color> colors = new List<Color>();

            entities.Add(new Particule(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,50,1.1f,0.0f));           //sun
            entities.Add(new Particule(new Vector3(2500, 0f, 0f), new Vector3(-250, 0f, 0), 100000,50f,1f,0.0f));      //secondary sun (far away)

            entities.Add(new Particule(new Vector3(5f, 350, 5f), new Vector3(-1f, -250, 0), 500,15,0.5f,0.01f));        //first planet (colide with tissue)
            entities.Add(new Particule(new Vector3(550f, 0, 100f), new Vector3(0, 0f, 0), 500,15,0.6f,0.01f));        //seconde planet useless

                //color for each Particule (Sun 1 , sun 2, planet 1 and 2)
            colors.Add(new Raylib_cs.Color(237, 217, 200   ,255));
            colors.Add(new Raylib_cs.Color(255  , 150, 30 ,255));
            colors.Add(new Raylib_cs.Color(0  , 230, 207 ,255));
            colors.Add(new Raylib_cs.Color(49 , 224, 0   ,255));

            Tissue drape2 = new Tissue(new Vector3(40,310,50),2,2,5f,1f,entities,colors,Color.SKYBLUE);      //fill the entities array with all the tissue particule
                

            Tissue drape = new Tissue(new Vector3(0,300,2),2,2,3f,1f,entities,colors,Color.BROWN);      //fill the entities array with all the tissue particule

            int[] rings = {0};
            Vector3 up = new Vector3(0,1,0);        

            foreach(int k in rings){

                int nbr_particul = 7000;//(int)(1500 * (entities[k].radius/50));

                    //generation of a ring of particule arround the first sun
                for(int i = 0; i < nbr_particul; i++){

                    float xz_dist = rnd.Next((int)entities[k].radius * 4,(int)entities[k].radius*8);
                    float xz_angle = rnd.Next();

                    float x =  xz_dist * (float)Math.Cos(xz_angle);
                    float z = -xz_dist * (float)Math.Sin(xz_angle);

                    Vector3 pos = entities[k].position + new Vector3(x,rnd.Next(-(int)entities[k].radius/2,(int)entities[k].radius/2),z);  //v = √ G * M / r

                    float mass = rnd.Next(1,5);
                    float dist = Vector3.Distance(pos,entities[k].position);
                    float total_mass = mass + entities[k].mass;
                    float speed = (float)Math.Sqrt((univers.G*total_mass)/dist);

                    Vector3 vel =  entities[k].velocity + Vector3.Normalize(Vector3.Cross(pos - entities[k].position,up)) * speed;
                    entities.Add(new Particule(
                        pos,
                        vel,
                        mass,
                        mass*1.3f,
                        0.2f,
                        0.0f
                    ));
                    
                    colors.Add(new Raylib_cs.Color(GetRandomValue(200,255),GetRandomValue(200,255),GetRandomValue(200,255),255));
                }
            }

        
            Octree UniversTree = new Octree(32,entities.Count()); 
            UniversTree.inserts(entities.ToArray());
            UniversTree.GenParticulesArray();

                //get an array of particule
            Particule[] output_enties = entities.ToArray();
                //get an array of colors
            Raylib_cs.Color[] colorArray = colors.ToArray();


#endregion

#region GPU_INIT
            /*
                GPU initialisation
            */
            
            GPU computeGPU = new GPU();
            CLBuffer bUniver = computeGPU.CreateBuffer<Univers>(MemoryFlags.ReadWrite,new Univers[1]{univers});

            CLBuffer B1 = computeGPU.CreateBuffer<Particule>(MemoryFlags.ReadWrite,entities.ToArray());
            CLBuffer B2 = computeGPU.CreateBuffer<Particule>(MemoryFlags.ReadWrite,entities.ToArray());

            CLBuffer bSprings       = computeGPU.CreateBuffer<Spring>(MemoryFlags.ReadWrite,drape.springs.ToArray());
            CLBuffer bSpringsForce  = computeGPU.CreateBuffer<Spring_force>(MemoryFlags.ReadWrite,drape.spring_forces);
            CLBuffer bClothSettings = computeGPU.CreateBuffer<Cloth_settings>(MemoryFlags.ReadWrite,new Cloth_settings[1]{drape.settings});

            CLBuffer bSprings_2         = computeGPU.CreateBuffer<Spring>(MemoryFlags.ReadWrite,drape2.springs.ToArray());
            CLBuffer bSpringsForce_2    = computeGPU.CreateBuffer<Spring_force>(MemoryFlags.ReadWrite,drape2.spring_forces);
            CLBuffer bClothSettings_2   = computeGPU.CreateBuffer<Cloth_settings>(MemoryFlags.ReadWrite,new Cloth_settings[1]{drape2.settings});

            CLBuffer bOctree_data       = computeGPU.CreateBuffer<int>(MemoryFlags.ReadOnly,new int[entities.Count()]);
            CLBuffer bOctree_regions    = computeGPU.CreateBuffer<Region>(MemoryFlags.ReadOnly,UniversTree.RegionsArray);
            CLBuffer bOctree_settings   = computeGPU.CreateBuffer<OctreeSettings>(MemoryFlags.ReadOnly,new OctreeSettings[]{UniversTree.settings});

            CLKernel kComputeGravity   = computeGPU.CreateKernel("OpenCl/kComputeGravity.cl","ComputeGravity");
            CLKernel kComputeVel       = computeGPU.CreateKernel("OpenCl/kComputeVel.cl","ComputeVel");
            CLKernel kComputePos       = computeGPU.CreateKernel("OpenCl/kComputePos.cl","ComputePos");
            CLKernel kComputeCollision = computeGPU.CreateKernel("OpenCl/kComputeCollision.cl","ComputeCollision");

            CLKernel kComputeSpringForce    = computeGPU.CreateKernel("OpenCl/kComputeSpringForce.cl","ComputeSpringForce");
            CLKernel kComputeSpring         = computeGPU.CreateKernel("OpenCl/kComputeSpring.cl","ComputeSpring");

            CLKernel kComputeSpringForce_2  = computeGPU.CreateKernel("OpenCl/kComputeSpringForce.cl","ComputeSpringForce");
            CLKernel kComputeSpring_2       = computeGPU.CreateKernel("OpenCl/kComputeSpring.cl","ComputeSpring");


            computeGPU.SetKernelArg(kComputeGravity,0,bUniver);
            computeGPU.SetKernelArg(kComputeGravity,1,B1);
            computeGPU.SetKernelArg(kComputeGravity,2,B2);


            computeGPU.SetKernelArg(kComputeSpringForce,0,B2);
            computeGPU.SetKernelArg(kComputeSpringForce,1,bSprings);
            computeGPU.SetKernelArg(kComputeSpringForce,2,bSpringsForce);

            computeGPU.SetKernelArg(kComputeSpring,0,B2);
            computeGPU.SetKernelArg(kComputeSpring,1,bSpringsForce);
            computeGPU.SetKernelArg(kComputeSpring,2,bClothSettings);

            computeGPU.SetKernelArg(kComputeSpringForce_2,0,B2);
            computeGPU.SetKernelArg(kComputeSpringForce_2,1,bSprings_2);
            computeGPU.SetKernelArg(kComputeSpringForce_2,2,bSpringsForce_2);

            computeGPU.SetKernelArg(kComputeSpring_2,0,B2);
            computeGPU.SetKernelArg(kComputeSpring_2,1,bSpringsForce_2);
            computeGPU.SetKernelArg(kComputeSpring_2,2,bClothSettings_2);


            computeGPU.SetKernelArg(kComputeVel,0,bUniver);
            computeGPU.SetKernelArg(kComputeVel,1,B2);
            computeGPU.SetKernelArg(kComputeVel,2,B1);


            computeGPU.SetKernelArg(kComputePos,0,bUniver);
            computeGPU.SetKernelArg(kComputePos,1,B1);
            computeGPU.SetKernelArg(kComputePos,2,B2);
            

            computeGPU.SetKernelArg(kComputeCollision,0,bUniver);
            computeGPU.SetKernelArg(kComputeCollision,1,B2);
            computeGPU.SetKernelArg(kComputeCollision,2,B1);
            computeGPU.SetKernelArg(kComputeCollision,3,bOctree_settings);
            computeGPU.SetKernelArg(kComputeCollision,4,bOctree_regions);
            computeGPU.SetKernelArg(kComputeCollision,5,bOctree_data);
            

            computeGPU.Upload<Particule>(B1,entities.ToArray());

            computeGPU.Upload<Spring>(bSprings,drape.springs);
            computeGPU.Upload<Cloth_settings>(bClothSettings,new Cloth_settings[1]{drape.settings});

            computeGPU.Upload<Spring>(bSprings_2,drape2.springs);
            computeGPU.Upload<Cloth_settings>(bClothSettings_2,new Cloth_settings[1]{drape2.settings});
            
            computeGPU.Upload<Univers>(bUniver,new Univers[1]{univers});

            computeGPU.Upload<OctreeSettings>(bOctree_settings,new OctreeSettings[]{UniversTree.settings});
            computeGPU.Upload<Region>(bOctree_regions,UniversTree.RegionsArray);
            computeGPU.Upload<int>(bOctree_data,UniversTree.ParticulesArray);
#endregion

            /*
                Main loop
            */



            bool started = false;   // tells if the simulation is started
            int current_view = 0;   // tells witch particule the camera follow
            bool showgrid = false;  // debug info

            Vector3 CamTarget = new Vector3(entities[0].position.X,entities[0].position.Y,entities[0].position.Z);
            Vector3 CamObj = output_enties[current_view].position;

            Cube current_cube = new Cube(entities[current_view].position,80);
            Color cl = new Color(0,255,0,255);

            while (!WindowShouldClose()) {

                UpdateCamera(ref camera);                

#region CONTROL
                if(IsKeyPressed(KEY_LEFT)){
                    showgrid = !showgrid;
                }

                if(IsKeyDown(KEY_KP_ADD)){
                    if(IsKeyDown(KEY_RIGHT_CONTROL)){
                        univers.dt += 0.001f;
                    }else{
                        univers.dt += 0.0001f;
                    }
                    computeGPU.Upload<Univers>(bUniver,new Univers[1]{univers});
                }

                if(IsKeyDown(KEY_KP_SUBTRACT)){
                    if(IsKeyDown(KEY_RIGHT_CONTROL)){
                        univers.dt -= 0.001f;
                    }else{
                        univers.dt -= 0.0001f;
                    }
                    if(univers.dt <= 0){
                        univers.dt = 0;
                    }
                    computeGPU.Upload<Univers>(bUniver,new Univers[1]{univers});
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

                if(IsKeyPressed(KEY_SPACE)){
                    started = true;
                }

                CamObj = output_enties[current_view].position;
                Vector3 dir = Vector3.Normalize(CamObj - CamTarget);
                float speed =  (CamObj-CamTarget).Length()/8;

                if((CamObj != CamTarget)&&((CamObj-CamTarget).Length()>=1)){
                    CamTarget += dir*speed;
                }


                camera.target = CamTarget;
#endregion
                
#region SIMULATION                
                if(started){

                    UniversTree.GenParticulesArray();

                    computeGPU.Upload<OctreeSettings>(bOctree_settings,new OctreeSettings[]{UniversTree.settings});
                    computeGPU.Upload<Region>(bOctree_regions,UniversTree.RegionsArray);
                    computeGPU.Upload<int>(bOctree_data,UniversTree.ParticulesArray);

                    computeGPU.Execute(kComputeGravity ,1,entities.Count());    

                    computeGPU.Execute(kComputeSpringForce,1,drape.springs.Count());
                    computeGPU.Execute(kComputeSpring,1,drape.settings.count);

                    computeGPU.Execute(kComputeSpringForce_2,1,drape2.springs.Count());
                    computeGPU.Execute(kComputeSpring_2,1,drape2.settings.count);

                    computeGPU.Execute(kComputeVel,1,entities.Count());
                    computeGPU.Execute(kComputePos,1,entities.Count());
                    computeGPU.Execute(kComputeCollision,1,entities.Count());

                    computeGPU.Download<Spring>(bSprings,drape.springs);
                    computeGPU.Download<Spring>(bSprings_2,drape2.springs);
                    computeGPU.Download<Particule>(B1,output_enties);

                }
                
#endregion

                UniversTree = new Octree(32,output_enties.Count());//.Clear(); //= new Octree(32,entities.Count());
                UniversTree.inserts(output_enties);

                BeginDrawing();
                    ClearBackground(BLACK);

                    BeginMode3D(camera);
                        ParticuleDrawer.DrawSprings(output_enties,colorArray,drape);   //draw all springs
                        ParticuleDrawer.DrawSprings(output_enties,colorArray,drape2);   //draw all springs
                        ParticuleDrawer.Draw(output_enties,colorArray);     //draw all particule

                        //debug
                        if(showgrid){
                            UniversTree.Draw();
                            //DrawGrid(100, 50.0f);

                            //draw velocity and acceleration vector for selected particule
                            DrawLine3D(output_enties[current_view].position,output_enties[current_view].position + (Vector3.Add(output_enties[current_view].velocity,Vector3.Normalize(output_enties[current_view].velocity) * output_enties[current_view].radius)),Color.DARKPURPLE);
                            DrawLine3D(output_enties[current_view].position,output_enties[current_view].position + (Vector3.Add(output_enties[current_view].acceleration,Vector3.Normalize(output_enties[current_view].acceleration) * output_enties[current_view].radius)),Color.ORANGE);
                        
                            current_cube.center = output_enties[current_view].position;
                            current_cube.size = 100;//output_enties[current_view].radius*2;
                            current_cube.recompute();

                            current_cube.Draw(cl);
                        
                        }
                    EndMode3D();

                    if(showgrid){
                        DrawFPS(10, 10);
                        DrawText(univers.dt.ToString("F4"),75,50,20,Color.DARKGREEN);
                        DrawText(current_view.ToString(),10,50,20,Color.DARKGREEN);
                        
                        DrawText(output_enties[current_view].acceleration.ToString("F3"),10,75,20,Color.DARKGREEN);
                        DrawText(output_enties[current_view].velocity.ToString("F3"),10,100,20,Color.DARKGREEN);
                        DrawText(output_enties[current_view].position.ToString("F3"),10,125,20,Color.DARKGREEN);

                        
                        DrawText(UniversTree.capacity.ToString("F3"),10,150,20,Color.DARKGREEN);
                        DrawText(UniversTree.particulCount.ToString(),10,175,20,Color.DARKGREEN);
                        //DrawText(current_view.ToString(),10,50,175,Color.DARKGREEN);
                    }

                    

                EndDrawing();
            }

        }

    }
    
}