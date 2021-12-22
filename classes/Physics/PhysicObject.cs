using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes
{
    public class PhysicObject:PhysicDataObject{
        



        public PhysicObject(Vector3 pos_,Vector3 vel_ ,float mass_,float radius_,float bounciness_):base(pos_,vel_,mass_,radius_,bounciness_){
            
        }

        public void UpdateVelocity(float dt){
            velocity += acceleration  * dt;
        }
        public void UpdatePosition(float dt){

            position += velocity * dt ;
        }

        public void AddForce(Vector3 NewForce){
            acceleration += (NewForce / mass);
        }

    }
}