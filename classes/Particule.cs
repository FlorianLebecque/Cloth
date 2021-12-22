using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes{
    public class Particule:PhysicObject{
        public Raylib_cs.Color cl;

        public static Model model;
        
        public Particule(Vector3 pos_,Vector3 vel_ ,float mass_,float radius_,float bounciness_):base(pos_,vel_,mass_,radius_,bounciness_){
            cl = new Raylib_cs.Color(GetRandomValue(100,255),GetRandomValue(100,255),GetRandomValue(100,255),255);

        }
        public void Draw(){
            DrawModel(model,position,radius,cl);
        }

        public Matrix4x4 GetTransform(){
            Matrix4x4 trans = Matrix4x4.CreateTranslation(position);
            Matrix4x4 scale = Matrix4x4.CreateScale(new Vector3(radius));

            return trans * scale;
        }


    }
}