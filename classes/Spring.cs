using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes {
    public class Spring {
        
        public float distance;
        public string key;

        public float k;

        public Spring(String key_,float distance_,float k_){
            key = key_;
            distance = distance_;
            k = k_;
        }

    }
}