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

namespace Cloth.classes
{
    public class ParticuleDrawer{
        
        public static Model model;
        public bool showDrapeParticul = true;

        public static Mesh m = GenMeshSphere(1f,75,50);

        public static Material mat = LoadMaterialDefault();
        
        public static Matrix4x4[] transforms;

        public static void Init(Shader shader){
            mat.shader = shader;
            unsafe
            {
                MaterialMap* maps = (MaterialMap*)mat.maps.ToPointer();
                maps[(int)MATERIAL_MAP_DIFFUSE].color = Color.GRAY;
            }
            //RaylibUtils.Utils.SetMaterialShader(ref model,(int)MaterialMapIndex.MATERIAL_MAP_DIFFUSE,ref shader);
        }
        public static void Draw(Particule[] entities,Raylib_cs.Color[] colors){
            
            //DrawSphere(entities[0].position,entities[0].radius,colors[0]);
            //DrawSphere(entities[1].position,entities[1].radius,colors[1]);

            for(int i = 0; i < entities.Count();i++){
                //model.transform = Matrix4x4.Transpose(Matrix4x4.CreateTranslation(entities[i].position));
                DrawModel(model,entities[i].position,entities[i].radius,colors[i]);
            }
        }

        public static void DrawInstance(Particule[] entities){
            if(transforms == null){
                transforms = new Matrix4x4[entities.Count()];
            }

            //DrawSphere(entities[0].position,entities[0].radius,Color.GOLD);

            for(int i = 0 ; i < entities.Count(); i++){

                Matrix4x4 scale = Matrix4x4.CreateScale(Vector3.One * entities[i].radius);

                transforms[i] = Matrix4x4.Transpose(scale * Matrix4x4.CreateTranslation(entities[i].position));
            }

            DrawMeshInstanced(m,mat,transforms,entities.Count());
        }

        public static void DrawSprings(Particule[] entities,Raylib_cs.Color[] colors,Tissue drape){
            foreach(Spring sp in drape.springs){
                if(sp.broken == 1){
                    DrawLine3D(entities[sp.particul_1].position,entities[sp.particul_2].position,colors[sp.particul_1]);

                }
            }
        }

    }

}