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

        public ParticuleDrawer(){

        }
        public void Draw(Model model, Particule[] entities,Raylib_cs.Color[] colors){
            
            DrawSphere(entities[0].position,entities[0].radius,colors[0]);
            DrawSphere(entities[1].position,entities[1].radius,colors[1]);

            for(int i = 2; i < entities.Count();i++){
                DrawModel(model,entities[i].position,entities[i].radius,colors[i]);
            }
        }

    }

}