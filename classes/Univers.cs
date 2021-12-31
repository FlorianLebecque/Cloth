using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes
{
    public struct Univers {
        public float G {get;set;}
        public float dt {get;set;}

        public Univers(float G_,float dt_){
            G = G_;
            dt = dt_;
        }
    }
}