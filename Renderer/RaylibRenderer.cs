using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

using Simulator;

namespace Renderer
{
    public class RaylibRenderer {

        Shader lightShader;
        Model sphereModel;

        Model skyboxModel;

        Shader BloomShader;

        int loc_vector_view;

        ParticuleDrawer PD;

        Color[] colors;

        Light[] ltable;
        Camera3D camera;
        RenderTexture2D render_target;
        Rectangle screenRec;
        int current_view;

        bool debug_render = false;

        Cube current_cube;

        Vector3 CamTarget;
        Vector3 CamObj;

        float[] cam_pos;

        public RaylibRenderer(Camera3D c, List<Color> color_l){
            camera = c;
            colors = color_l.ToArray();

            PD = new();
            render_target = LoadRenderTexture(GetScreenWidth(),GetScreenHeight());

            ltable = new Light[4];

            sphereModel = ModelInit();
            skyboxModel = SkyBoxInit();
            BloomShader = BloomInit();

            screenRec = new Rectangle(0,0,render_target.texture.width,-render_target.texture.height);
            current_cube = new Cube(Vector3.Zero,80);

            current_view = 0;

            CamTarget = Vector3.Zero;
            CamObj = Vector3.Zero;
            cam_pos = new float[3]{camera.position.X,camera.position.Y,camera.position.Z};
        }

        private Model ModelInit(){
            /*
            Model initialisation
            */
            
            Mesh sphere = GenMeshSphere(1f,20,20);
            Model model = LoadModelFromMesh(sphere);//LoadModel("resources/models/bunny.obj");

            lightShader = LoadShader("resources/shaders/base_lighting.vs","resources/shaders/lighting.fs");
            loc_vector_view = GetShaderLocation(lightShader,"viewPos");


            RaylibUtils.Utils.SetMaterialShader(ref model,0,ref lightShader);
            RaylibUtils.Utils.MeshTangents(ref model);
            RaylibUtils.Utils.SetShaderLocation(ref lightShader,SHADER_LOC_MATRIX_MODEL,"matModel");

            
            ltable[0] = Light.CreateLight(lightShader,new Vector3(0,0,0),Color.WHITE);
            ltable[1] = Light.CreateLight(lightShader,new Vector3(0,0,0),Color.WHITE);

            return model;
        }

        private Model SkyBoxInit(){
            Model skyBox = LoadModelFromMesh(GenMeshCube(1,1,1));
            Shader skyShader = LoadShader("resources/shaders/skybox.vs","resources/shaders/skybox.fs");
            
            Texture2D skyTexture = LoadTexture("resources/textures/sky3.png");//LoadTextureCubemap(img,CubemapLayout.CUBEMAP_LAYOUT_AUTO_DETECT);

            RaylibUtils.Utils.SetMaterialShader(ref skyBox,0,ref skyShader);
            RaylibUtils.Utils.SetMaterialTexture(ref skyBox,0,MATERIAL_MAP_DIFFUSE,ref skyTexture);
            RaylibUtils.Utils.MeshTangents(ref skyBox);
            RaylibUtils.Utils.SetShaderLocation(ref skyShader,SHADER_LOC_MATRIX_MODEL,"matModel");
            
            return skyBox;
        }
        private Shader BloomInit(){
            Shader BloomShader = LoadShader("","resources/shaders/bloom.fs");
            int loc_res = GetShaderLocation(BloomShader,"resolution");
            RaylibUtils.Utils.SetShaderValue<float[]>(BloomShader,loc_res,new float[2]{GetScreenWidth(),GetScreenHeight()},ShaderUniformDataType.SHADER_UNIFORM_VEC2);
            

            return BloomShader;
        }
        
        public void Draw(UniverSimulation us){
            BeginTextureMode(render_target);
                BeginMode3D(camera);
                    ClearBackground(BLACK);

                    Rlgl.rlDisableBackfaceCulling();
                        Rlgl.rlDisableDepthMask();
                            DrawModel(skyboxModel, camera.position, 750.0f, Color.SKYBLUE);
                        Rlgl.rlEnableBackfaceCulling();
                    Rlgl.rlEnableDepthMask();
                    
                    PD.Draw(sphereModel,us.output_enties,colors);     //draw all particule
                EndMode3D();
            EndTextureMode();

            BeginDrawing();

                ClearBackground(BLACK);
                
                BeginShaderMode(BloomShader);
                    DrawTextureRec(render_target.texture, screenRec, Vector2.Zero, Color.BLACK);
                EndShaderMode();


                if(debug_render){

                    BeginMode3D(camera);

                        us.UniversTree.Draw();

                        //draw velocity and acceleration vector for selected particule
                        DrawLine3D(us.output_enties[current_view].position,us.output_enties[current_view].position + (Vector3.Add(us.output_enties[current_view].velocity,Vector3.Normalize(us.output_enties[current_view].velocity) * us.output_enties[current_view].radius)),Color.DARKPURPLE);
                        DrawLine3D(us.output_enties[current_view].position,us.output_enties[current_view].position + (Vector3.Add(us.output_enties[current_view].acceleration,Vector3.Normalize(us.output_enties[current_view].acceleration) * us.output_enties[current_view].radius)),Color.ORANGE);
                    
                        current_cube.center = us.output_enties[current_view].position;
                        current_cube.size = 100;//output_enties[current_view].radius*2;
                        current_cube.recompute();

                        current_cube.Draw(Color.GREEN);
                    
                    EndMode3D();

                    DrawFPS(10, 10);
                    DrawText(us.univers.dt.ToString("F4"),75,50,20,Color.DARKGREEN);
                    DrawText(current_view.ToString(),10,50,20,Color.DARKGREEN);
                    
                    DrawText(us.output_enties[current_view].acceleration.ToString("F3"),10,75,20,Color.DARKGREEN);
                    DrawText(us.output_enties[current_view].velocity.ToString("F3"),10,100,20,Color.DARKGREEN);
                    DrawText(us.output_enties[current_view].position.ToString("F3"),10,125,20,Color.DARKGREEN);

                    
                    DrawText(us.UniversTree.capacity.ToString("F3"),10,150,20,Color.DARKGREEN);
                    DrawText(us.UniversTree.particulCount.ToString(),10,175,20,Color.DARKGREEN);
                }

            EndDrawing();
        }

        public void UpdateCamera(){
            Raylib.UpdateCamera(ref camera); 

            camera.target = CamTarget;
            cam_pos[0] = camera.position.X;
            cam_pos[1] = camera.position.Y;
            cam_pos[2] = camera.position.Z;
            RaylibUtils.Utils.SetShaderValue<float[]>(lightShader,loc_vector_view,cam_pos,ShaderUniformDataType.SHADER_UNIFORM_VEC3 );
        }

        public void CheckControl(UniverSimulation us){
            if(IsKeyPressed(KEY_LEFT)){
                debug_render = !debug_render;
            }

            if(IsKeyPressed(KEY_UP)){
                current_view--;
            }
            if(IsKeyPressed(KEY_DOWN)){
                current_view++;
            }

            if(current_view < 0) {
                current_view = us.output_enties.Count()-1;
            }

            if(current_view == us.output_enties.Count()){
                current_view = 0;
            }

            CamObj = us.output_enties[current_view].position;
            Vector3 dir = Vector3.Normalize(CamObj - CamTarget);
            float speed =  (CamObj-CamTarget).Length()/4;

            if((CamObj != CamTarget)&&((CamObj-CamTarget).Length()>=1)){
                CamTarget += dir*speed;
            }
        }

        public void UpdateLight(UniverSimulation us){
            ltable[0].position = us.output_enties[0].position;
            ltable[1].position = us.output_enties[1].position;
            Light.UpdateLightValues(lightShader,ltable[0]);
            Light.UpdateLightValues(lightShader,ltable[1]);
        }

    }
}