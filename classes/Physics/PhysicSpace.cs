using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes
{
    public class PhysicSpace {
        
        public List<PhysicObject> SpaceObjects;
        
        public float G { get; set; }
        float div = 2;
        float cur_dt = 0.01f;
        
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
                        Vector3 unitVector = Vector3.Normalize(so2.position - so1.position);

                        
                        Force1 += G * (so1.mass * so2.mass) * unitVector / (distanceSquared);                        
                    }
                }

                so1.AddForce(Force1);

            }
        }

        public void UpdatePosition(){
            float dt = cur_dt;
            foreach(PhysicObject PO in SpaceObjects){
                PO.UpdatePosition(dt/div);
            }
        }

        public void UpdateVelocity(){
            float dt = cur_dt;
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

                            float totalMass = so1.mass + so2.mass;
                            float mass_factor = so1.mass/totalMass;

                            Vector3 Normal = Vector3.Normalize(so2.position - so1.position);  //V1 -> V2
                            Vector3 ExitVector = Normal * ((so1.radius+so2.radius)-distance); //V1 -> V2

                            float bounciness = (so1.bounciness + so2.bounciness)/2;

                            float ViSO1 = Vector3.Dot(so1.velocity,Normal) * bounciness;
                            float ViSO2 = Vector3.Dot(so2.velocity,Normal) * bounciness;
                            
                            so1.velocity -= Normal * ViSO1;
                            so2.velocity -= Normal * ViSO2;

                            float VfSO1 = ViSO2;
                            float VfSO2 = ViSO1;
                            if(so1.mass != so2.mass){
                                VfSO1 = (((so1.mass-so2.mass)/(totalMass))  * ViSO1) + (((2*so2.mass)/(totalMass))*ViSO2);
                                VfSO2 = (((2*so1.mass)/(so1.mass-so2.mass)) * ViSO1) + (((so1.mass-so2.mass)/(totalMass))*ViSO2);
                            }

                            so1.velocity +=  Normal * (VfSO1);
                            so2.velocity += -Normal * (VfSO2);

                            float SO1InwardVectorValue = Vector3.Dot(so1.acceleration * so1.mass,Normal);  //projection du vector de force sur la normal
                            Vector3 SO1ReactionForce = -Normal * SO1InwardVectorValue; 

                            float SO2InwardVectorValue = Vector3.Dot(so2.acceleration * so2.mass,Normal);  //projection du vector de force sur la normal
                            Vector3 SO2ReactionForce = -Normal * SO2InwardVectorValue; 



                            so1.AddForce(SO1ReactionForce);
                            so2.AddForce(SO2ReactionForce);

                            so1.position -= (ExitVector * (1-mass_factor));
                            so2.position += (ExitVector * (mass_factor));
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