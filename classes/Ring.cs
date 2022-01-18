using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Color;
using Raylib_cs;

namespace Cloth.classes {
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

    public class RingGenerator{

        public static Vector3 WORLD_UP = new Vector3(0,1,0);
        public static Random rnd = new Random(2);
        public static void CreateRing(Univers univers,List<Particule> entities,List<Color> colors,Ring ring){

            if(rnd == null){
                rnd = new Random(2);
            }

            Vector3 rotationAxis = Vector3.Normalize(Vector3.Cross(WORLD_UP,ring.normal));

            if(WORLD_UP == ring.normal){
                rotationAxis = WORLD_UP;
            }

            float angle = -(float)Math.Acos(Vector3.Dot(WORLD_UP,ring.normal));
            Matrix4x4 r = MatrixRotate(rotationAxis,angle);

                //generation of a ring of particule arround the first sun
            for(int i = 0; i < ring.nbr_particul; i++){

                float xz_dist = rnd.Next((int)ring.min_distance,(int)ring.max_distance);
                float theta = rnd.Next();

                float x =  xz_dist * (float)Math.Cos(theta);
                float z = -xz_dist * (float)Math.Sin(theta);
                float y = rnd.Next(-(int)entities[ring.target].radius/2,(int)entities[ring.target].radius/2);

                Vector3 pos =  new Vector3(x,y,z);
        
                pos = entities[ring.target].position + Vector3.Transform(pos,r);


                float mass = ring.min_mass +  (float)rnd.NextDouble() * ring.max_mass;
                Particule p = new Particule(
                    pos,
                    Vector3.One,
                    mass,
                    mass*ring.radius_factor,
                    ring.bounciness,
                    ring.roughness
                );
                p.velocity = Particule.GetOrbitalSpeed(entities[ring.target],p,ring.normal,univers);

                entities.Add(p);
                colors.Add(new Raylib_cs.Color(GetRandomValue(200,255),GetRandomValue(200,255),GetRandomValue(200,255),255));
            }
        }

    }
}