using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Color;
using Raylib_cs;

namespace PhysicObject.classes {
    public class Ring {
        
        public int target;

        public int nbr_particul;
        public float min_distance;
        public float max_distance;

        public float min_mass;
        public float max_mass;

        public float radius_factor;
        public float bounciness;
        public float roughness;
        public Vector3 normal;

        public Ring(List<Particule> entities,int target_,Vector3 normal_,int min_factor,int max_factor){
            target = target_;
            normal = Vector3.Normalize(normal_);
            min_distance = (int)entities[target].radius * min_factor;
            max_distance = (int)entities[target].radius * max_factor;;

            nbr_particul = 1500;

            min_mass = 1;
            max_mass = 5;

            radius_factor = 1.3f;

            bounciness = 0.5f;
            roughness = 0.1f;
        }
        public Ring(int target_,Vector3 normal_,float min_distance_,float max_distance_){
            target = target_;
            normal = Vector3.Normalize(normal_);
            min_distance = min_distance_;
            max_distance = max_distance_;

            nbr_particul = 1500;

            min_mass = 1;
            max_mass = 5;

            radius_factor = 1.3f;

            bounciness = 0.5f;
            roughness = 0.1f;
        }

    }
}