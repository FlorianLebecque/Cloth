using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhysicObject.classes.tree
{
    public struct Region {
        
        public int capacity;
        public int count;
        public bool subdivided;

        public int offset;  //represente the starting coordinate inside the particule array
        //region coordinate
        public Cube region;

        //childs (if exist)
        public int child_s_nw = -1;
        public int child_s_ne = -1;
        public int child_s_se = -1;
        public int child_s_sw = -1;
        public int child_t_nw = -1;
        public int child_t_ne = -1;
        public int child_t_se = -1;
        public int child_t_sw = -1;

        public Region(Vector3 center,float size,int region_capacity,int offset_){
            capacity = region_capacity;
            count = 0;
            subdivided = false;
            offset = offset_;

            child_s_nw = -1;
            child_s_ne = -1;
            child_s_se = -1;
            child_s_sw = -1;
            child_t_nw = -1;
            child_t_ne = -1;
            child_t_se = -1;
            child_t_sw = -1;


            region = new Cube(center,size);
        }


    }
}