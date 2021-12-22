using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes
{
    public class PhysicDataObject {
        public Vector3 position { get; set; }
        public Vector3 velocity { get; set; }
        public Vector3 acceleration {get; set;}
        public float mass { get; set; }
        public float bounciness { get; set; }
        public float radius {get; set; }

        public PhysicDataObject(Vector3 pos_,Vector3 vel_ ,float mass_,float radius_,float bounc_){
            position = pos_;
            velocity = vel_;
            mass     = mass_;
            radius   = radius_;
            bounciness = bounc_;
        }

    }

    public struct Particule_obj{

        public Particule_obj(Vector3 pos_,Vector3 vel_ ,float mass_,float radius_,float bounc_){
            position = pos_;
            velocity = vel_;
            mass     = mass_;
            radius   = radius_;
            bounciness = bounc_;
            acceleration = new Vector3();
        }
        public Vector3 position { get; set; }
        public Vector3 velocity { get; set; }
        public Vector3 acceleration {get; set;}
        public float mass { get; set; }
        public float bounciness { get; set; }
        public float radius {get; set; }
    }
}