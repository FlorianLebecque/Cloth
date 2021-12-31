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

/*

    ---------------------------------------------------------------------------------------------

            Gravity and cloth simulation

        Force simulated :
            Gravity
            Springs with directional dampening
            Bouncing
            Resitance

        Simulation :

            Initialisation phase :
                1) Raylib
                2) 3D Model
                3) Univers
                4) GPU

            Main loop:

                1) Simulation
                    1) Gravity for every particule
                    2) Spring Forces for every springs
                    3) Apply spring forces on the particule
                    4) Update velocity for each particule
                    5) Update position for each particule
                    6) check collision for every particule

                    We download and send the data to the GPU between every action

                2) Display

                    1) Draw the spings
                    2) Draw the particule

                    to draw the particule we take the array of particule and the array of color
                    They are the same length and particule[i] -> color[i]

        CONTROLE

            [SPACE]                     -> start the simulation
            [LEFT]                      -> Display debug
            [UP] or [DOWN]              -> Change targeted particule
            [KEY PAD +] or [KEY PAD -]  -> Slow or accelerate the simulation (When accelerated -> can be unstable)


        OPENCL
            The Object GPU is an interface to communicate with the gpu
            All the kernel are in the opencl folder

            I had to reimplement a bunch of basic vector function because I am using Vector3 witch is from C# and differ from float3 in OpenCl

            Kernel :
                [particule_gravity]     :   Compute the gravity for each particule
                    0 : Univer struct
                    1 : Input buffer (array of particule)
                    2 : Output buffer (array of particuke)

                    exectuted once for each particule

                [particul_spring]       :   Compute the force for each springs
                    0 : Input buffer (array of particule)
                    1 : springs -> array containing all the springs
                    2 : sp -> array of springs_force

                    exectuted once for every springs

                [spring_applier]        :   Apply the force to each particule of the cloth
                    0 : Input buffer (array of particule)
                    1 : Output buffer (array of particule)
                    2 : sp -> array of springs_force
                    3 : cloth -> array of one element -> the cloth settings

                    Since all the cloth particule are in the same buffer as all the other particule
                    We need to know where they are in the array
                        -> offset : tells where the index start
                        -> count  : tells how many particule they are in the array

                    ex : [p,p,p,p,p,c,c,c,c,c,c,p,p];
                        -> offset 5
                        -> count  6

                        We know the cloth is from index 5 to index 10
                    
                    Executed once for every particule in the cloth

                [particule_velocity]    :   Apply the acceleration to the velocity
                [particule_position]    :   Apply the velocity to the position
                    0 : Univers (array of one element) contain the univer parameter G and dt
                    1 : Input (array of all the particules)
                    2 : Ouput (array of all the particules)

                    They both are exectuted once per particule

                [particul_collision]    :   Compute collision and adjust velocity and position
                    0 : Univers (array of one element) contain the univer parameter G and dt
                    1 : Input (array of all the particuls)
                    2 : Ouput (array of all the particuls)

                    Executed once for every particules

        The simulation works by sending an array of Particule to the GPU and do the calculation on it
                    We then download the array and display it
    ---------------------------------------------------------------------------------------------
*/


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
            Random rnd = new Random();
            List<Particule> entities = new List<Particule>();
            List<Raylib_cs.Color> colors = new List<Color>();

            entities.Add(new Particule(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,75,1.1f,0.0f));           //sun
            entities.Add(new Particule(new Vector3(2500, 0f, 0f), new Vector3(-250f, 0f, 0), 100000,50f,1f,0.0f));      //secondary sun (far away)

            entities.Add(new Particule(new Vector3(5f, 350, 5f), new Vector3(-1f, -250, 0), 500,15,0.5f,0.01f));        //first planet (colide with tissue)
            entities.Add(new Particule(new Vector3(550f, 0, 100f), new Vector3(0f, 0f, 75), 500,15,0.6f,0.01f));        //seconde planet useless

                //color for each Particule (Sun 1 , sun 2, planet 1 and 2)
            colors.Add(new Raylib_cs.Color(237, 217, 200   ,255));
            colors.Add(new Raylib_cs.Color(0  , 230, 207 ,255));
            colors.Add(new Raylib_cs.Color(0  , 230, 207 ,255));
            colors.Add(new Raylib_cs.Color(49 , 224, 0   ,255));

                
            Tissue drape = new Tissue(new Vector3(0,300,2),45,45,entities,colors);      //fill the entities array with all the tissue particule
            Spring_force[] spring_forces = new Spring_force[drape.springs.Count()];     //create an array for each springs
            

                //generation of a ring of particule arround the first sun
            Vector3 up = new Vector3(0,1,0);        
            for(int i = 0; i < 1500; i++){

                float xz_dist = rnd.Next((int)entities[0].radius * 4,(int)entities[0].radius*5);
                float xz_angle = rnd.Next();

                float x =  xz_dist * (float)Math.Cos(xz_angle);
                float z = -xz_dist * (float)Math.Sin(xz_angle);

                Vector3 pos = new Vector3(x,rnd.Next(-50,50),z);  //v = √ G * M / r

                float mass = rnd.Next(1,5);
                float dist = Vector3.Distance(pos,entities[0].position);
                float total_mass = mass + entities[0].mass;
                float speed = (float)Math.Sqrt((univers.G*total_mass)/dist);

                Vector3 vel =  Vector3.Normalize(Vector3.Cross(pos,up))*speed;
                entities.Add(new Particule(
                    pos,
                    vel,
                    mass,
                    mass*1.3f,
                    0.9f,
                    0.0f
                ));
                colors.Add(new Raylib_cs.Color(GetRandomValue(200,255),GetRandomValue(200,255),GetRandomValue(200,255),255));
            }
            
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
            CLBuffer Buniver = computeGPU.CreateBuffer<Univers>(MemoryFlags.ReadWrite,new Univers[1]{univers});

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


            computeGPU.SetKernelArg(gravity_applier,0,Buniver);
            computeGPU.SetKernelArg(gravity_applier,1,B1);
            computeGPU.SetKernelArg(gravity_applier,2,B2);


            computeGPU.SetKernelArg(springs_applier,0,B2);
            computeGPU.SetKernelArg(springs_applier,1,BSpring);
            computeGPU.SetKernelArg(springs_applier,2,BSpringF);


            computeGPU.SetKernelArg(springsF_applier,0,B1);
            computeGPU.SetKernelArg(springsF_applier,1,B2);
            computeGPU.SetKernelArg(springsF_applier,2,BSpringF);
            computeGPU.SetKernelArg(springsF_applier,3,BCloth);


            computeGPU.SetKernelArg(velocity_applier,0,Buniver);
            computeGPU.SetKernelArg(velocity_applier,1,B1);
            computeGPU.SetKernelArg(velocity_applier,2,B2);


            computeGPU.SetKernelArg(position_applier,0,Buniver);
            computeGPU.SetKernelArg(position_applier,1,B1);
            computeGPU.SetKernelArg(position_applier,2,B2);
            

            computeGPU.SetKernelArg(collision_applier,0,Buniver);
            computeGPU.SetKernelArg(collision_applier,1,B1);
            computeGPU.SetKernelArg(collision_applier,2,B2);
            

            computeGPU.Upload<Particule>(B1,entities.ToArray());
            computeGPU.Upload<Spring>(BSpring,drape.springs);
            computeGPU.Upload<Cloth_settings>(BCloth,new Cloth_settings[1]{drape.settings});
            computeGPU.Upload<Univers>(Buniver,new Univers[1]{univers});
#endregion


            /*
                Main loop
            */

            bool started = false;
            int current_view = 0;
            bool showgrid = false;
            while (!WindowShouldClose()) {
                //Console.WriteLine(GetFrameTime());

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
                    computeGPU.Upload<Univers>(Buniver,new Univers[1]{univers});
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
                    computeGPU.Upload<Univers>(Buniver,new Univers[1]{univers});
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

                camera.target = output_enties[current_view].position;
#endregion
                
#region SIMULATION                
                if(started){

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
                }
                
#endregion

                BeginDrawing();
                    ClearBackground(BLACK);

                    BeginMode3D(camera);
                        ParticuleDrawer.DrawSprings(output_enties,drape);   //draw all springs
                        ParticuleDrawer.Draw(output_enties,colorArray);     //draw all particule

                        //debug
                        if(showgrid){
                            DrawGrid(100, 10.0f);

                            //draw velocity and acceleration vector for selected particule
                            DrawLine3D(output_enties[current_view].position,output_enties[current_view].position + (Vector3.Add(output_enties[current_view].velocity,Vector3.Normalize(output_enties[current_view].velocity) * output_enties[current_view].radius)),Color.RED);
                            DrawLine3D(output_enties[current_view].position,output_enties[current_view].position + (Vector3.Add(output_enties[current_view].acceleration,Vector3.Normalize(output_enties[current_view].acceleration) * output_enties[current_view].radius)),Color.ORANGE);
                        }
                    EndMode3D();

                    if(showgrid){
                        DrawFPS(10, 10);
                        DrawText(univers.dt.ToString("F4"),50,50,20,Color.DARKGREEN);
                        DrawText(current_view.ToString(),10,50,20,Color.DARKGREEN);
                        
                        DrawText(output_enties[current_view].acceleration.ToString("F3"),10,75,20,Color.DARKGREEN);
                        DrawText(output_enties[current_view].velocity.ToString("F3"),10,100,20,Color.DARKGREEN);
                        DrawText(output_enties[current_view].position.ToString("F3"),10,125,20,Color.DARKGREEN);
                    }

                    

                EndDrawing();
            }

        }

    }
    
}