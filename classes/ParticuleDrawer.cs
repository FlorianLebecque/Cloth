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

        public static void Draw(Particule[] entities,Raylib_cs.Color[] colors){
            
            for(int i = 0; i < entities.Count();i++){
                DrawModel(model,entities[i].position,entities[i].radius,colors[i]);
            }
        }

        public static void Draw(Particule[] entities,Raylib_cs.Color[] colors,Tissue drape){
            
            for(int i = 0; i < entities.Count();i++){


                bool show = true;
                foreach(Spring sp in drape.springs){
                    if((sp.particul_1 == i)||(sp.particul_2 == i)){
                        show = false;
                        break;
                    }
                }
                
                if(show)
                    DrawModel(model,entities[i].position,entities[i].radius,colors[i]);
            }
        }

        public static void DrawSprings(Particule[] entities,Raylib_cs.Color[] colors,Tissue drape){
            foreach(Spring sp in drape.springs){
                if(sp.broken == 1){
                    DrawLine3D(entities[sp.particul_1].position,entities[sp.particul_2].position,colors[sp.particul_1]);

                }
            }
        }

    }

    public class ParticuleArray{

        public static List<float_3> particule_pos = new();
        public static List<float> particule_radius = new();

        public static void Generate(List<Particule> entites){

            foreach(Particule p in entites){


                particule_pos.Add(new float_3(p.position.X,p.position.Y,p.position.Z));
                particule_radius.Add(p.radius);
            }

        }



    }

    public struct float_3{
        float x;
        float y;
        float z;

        public float_3(float x_,float y_,float z_){
            x = x_;
            y = y_;
            z = z_;
        }

    }

}