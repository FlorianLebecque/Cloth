using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes
{
    public class ParticuleDrawer{
        
        public static Model model;
        public static void Draw(Particule_obj[] entities,Raylib_cs.Color[] colors){
            
            for(int i = 0; i < entities.Count();i++){
                DrawModel(model,entities[i].position,entities[i].radius,colors[i]);
            }
        }

    }
}