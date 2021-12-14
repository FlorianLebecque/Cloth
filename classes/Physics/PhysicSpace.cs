using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes
{
    public class PhysicSpace {
        
        private List<PhysicObject> SpaceObjects;
        
        public float G { get; set; }
        float div = 4;
        
        public PhysicSpace(float G_){
            G = G_;
            SpaceObjects = new();
        }

        public void Add(PhysicObject SO){
            SpaceObjects.Add(SO);
        }

        public void UpdateForce(){
            
            
            foreach(PhysicObject so1 in SpaceObjects) {

                Vector3 Force1 = new Vector3(0);

                foreach (PhysicObject so2 in SpaceObjects) {
                    if(so1 != so2) {

                        float distanceSquared = Vector3.DistanceSquared(so1.position,so2.position);
                        float distance = Vector3.Distance(so1.position,so2.position);
                        Vector3 unitVector = Vector3.Normalize(so2.position - so1.position);

                        
                        Force1 += G * (so1.mass * so2.mass) * unitVector / (distanceSquared);                        
                    }
                }

                so1.AddForce(Force1);

            }
        }

        public void UpdatePosition(){
            float dt = GetFrameTime();
            foreach(PhysicObject PO in SpaceObjects){
                PO.UpdatePosition(dt/div);
            }
        }

        public void UpdateVelocity(){
            float dt = GetFrameTime();
            foreach(PhysicObject PO in SpaceObjects){
                PO.UpdateVelocity(dt/div);
            }
        }

        public void CheckCollision(){
            foreach(PhysicObject so1 in SpaceObjects){
                foreach(PhysicObject so2 in SpaceObjects){
                    if(so1 != so2){
                        float distance = Vector3.Distance(so1.position,so2.position);
                        if(distance < (so1.radius + so2.radius)){   //collision
                            Vector3 Normal = Vector3.Normalize(so2.position - so1.position); //V1 -> V2
                            Vector3 ExitVector = Normal * ((so1.radius+so2.radius)-distance); //V1 -> V2

                            float SO1InWardVelocity = Vector3.Dot(so1.velocity,Normal);
                            Vector3 SO1VelocityCorrection = -Normal * SO1InWardVelocity;

                            float SO2InWardVelocity = Vector3.Dot(so2.velocity,Normal);
                            Vector3 SO2VelocityCorrection = -Normal * SO2InWardVelocity;

                            so1.velocity += SO1VelocityCorrection;
                            so2.velocity += SO2VelocityCorrection;

                            float SO1InwardVectorValue = Vector3.Dot(so1.acceleration * so1.mass,Normal);  //projection du vector de force sur la normal
                            Vector3 SO1ReactionForce = -Normal * SO1InwardVectorValue; 

                            float SO2InwardVectorValue = Vector3.Dot(so2.acceleration * so2.mass,Normal);  //projection du vector de force sur la normal
                            Vector3 SO2ReactionForce = -Normal * SO2InwardVectorValue; 

                            so1.AddForce(SO1ReactionForce);
                            so2.AddForce(SO2ReactionForce);

                            if(so1.mass < so2.mass){

                                so1.position -= (ExitVector);
                                
                            }else{

                                so2.position += (ExitVector);

                            }
                        }
                    }
                }
            }
        }

        public void ClearForce(){
            foreach(PhysicObject PO in SpaceObjects){
                PO.acceleration = new Vector3(0);
            }
        }

        public void Draw(){
            
        }

    }
}