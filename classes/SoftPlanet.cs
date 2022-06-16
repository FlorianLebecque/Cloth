using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Color;
using Raylib_cs;

namespace PhysicObject.classes {
    public class SoftPlanet {
        
        public Vector3 position;

        public Vector3 velocity;
        public int subdivision;
        public float mass;
        public float radius;
        public float bounciness;
        public float roughness;

        public float particule_radius;
        public float pressure;

        public SoftPlanet(Vector3 position_,Vector3 speed_,float mass_,float radius_,float particule_size,int subdivision_,float pressure_){
            
            position = position_;
            velocity = speed_;

            radius = radius_;
            subdivision = subdivision_;

            mass = mass_;
            particule_radius = particule_size;

            pressure = pressure_;

            bounciness = 0.5f;
            roughness = 1f;
        }
    }
}