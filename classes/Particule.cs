using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes{
    public class Particule:PhysicObject{
        public Raylib_cs.Color cl;
        
        public Particule(Vector3 pos_,Vector3 vel_ ,float mass_,float radius_):base(pos_,vel_,mass_,radius_){
            cl = new Raylib_cs.Color(GetRandomValue(100,255),GetRandomValue(100,255),GetRandomValue(100,255),255);

        }
        public void Draw(){
            DrawSphere(position, radius, cl);

        }


    }
}