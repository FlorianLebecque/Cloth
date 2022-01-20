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

namespace Renderer
{
    public class ParticuleDrawer{

        public Model sunMd;

        float radius_factor = 1.05f;
        public ParticuleDrawer(){

        }
        public void Draw(Model model, Particule[] entities,Raylib_cs.Color[] colors){
            
            DrawModel(sunMd,entities[0].position,entities[0].radius*radius_factor,colors[0]);
            DrawModel(sunMd,entities[1].position,entities[1].radius*radius_factor,colors[1]);

            for(int i = 2; i < entities.Count();i++){
                DrawModel(model,entities[i].position,entities[i].radius*radius_factor,colors[i]);
            }
        }

    }

}