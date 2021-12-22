using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes
{
    public class ParticuleDrawer{
        
        public static Model model;
        public static void Draw(Particule_obj[] entities){
            
            foreach(Particule_obj p in entities){
                DrawModel(model,p.position,p.radius,Color.BEIGE);
            }
        }

    }
}