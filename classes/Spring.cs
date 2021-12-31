using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes {

    public struct Spring{
        public float rest_distance {get;set;}
        public float max_distance {get;set;}
        public float k {get;set;}
        public float cd;
        public int particul_1 {get;set;}
        public int particul_2 {get;set;}

        public int broken;
        
        public Spring(float rest_,float max_,float k_,int p1,int p2,float cd_){
            rest_distance = rest_;
            max_distance = max_;
            k = k_;
            particul_1 = p1;
            particul_2 = p2;
            cd = cd_;
            broken = 1;
        }

    }

    public struct Spring_force{
        public int p1;
        public int p2;
        public Vector3 force;
    }

    public struct Cloth_settings{
        public int offset;
        public int count;
        public int nbr_spring;

        public Cloth_settings(int off_,int count_,int nbr_spring_){
            offset = off_;
            count = count_;
            nbr_spring = nbr_spring_;
        }
    }

}