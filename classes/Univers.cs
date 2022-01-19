using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhysicObject.classes
{
    public struct UniversSettings {
        public float G {get;set;}
        public float dt {get;set;}

        public UniversSettings(float G_,float dt_){
            G = G_;
            dt = dt_;
        }
    }
    
}