using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes
{
    public struct Particule{

        public Particule(Vector3 pos_,Vector3 vel_ ,float mass_,float radius_,float bounc_,float roughness_){
            position = pos_;
            velocity = vel_;
            mass     = mass_;
            radius   = radius_;
            bounciness = bounc_;
            roughness = roughness_;
            acceleration = new Vector3();
        }
        public Vector3 position { get; set; }
        public Vector3 velocity { get; set; }
        public Vector3 acceleration {get; set;}
        public float mass { get; set; }
        public float bounciness { get; set; }
        public float radius {get; set; }

        public float roughness {get;set;}

        public static Vector3 GetOrbitalSpeed(Particule p1,Particule p2, Vector3 normal,Univers univers){
                float dist = Vector3.Distance(p2.position,p1.position);
                float total_mass = p1.mass + p2.mass;
                float speed = (float)Math.Sqrt((univers.G*total_mass)/dist);

                return p1.velocity + Vector3.Normalize(Vector3.Cross(p2.position - p1.position,normal)) * speed;  
        }
    }
}