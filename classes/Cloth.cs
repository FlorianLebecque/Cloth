using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes {
    public class Tissue {

        public List<Spring> springs;

        public Cloth_settings settings;

        private float rest_distance = 3f;


        public Tissue(Vector3 position,int n,int m,List<Particule_obj> entites){

            int offset = entites.Count();

            settings = new Cloth_settings(offset,n*m,0);

            float start_pos_x = position.X - (n*rest_distance)/2;
            float start_pos_z = position.Z - (m*rest_distance)/2;

            springs = new List<Spring>();


            float k1 = 250f;
            float k2 = 200f;
            float k3 = 150f;

            float cd = 1f;

            for(int i = 0; i < n; i++){
                for(int j = 0; j < m ;j++){

                    Particule_obj pt = new Particule_obj(
                        new Vector3(start_pos_x+i*rest_distance,position.Y,start_pos_z+j*rest_distance),
                        new Vector3(0),
                        0.2f,
                        rest_distance/4f,
                        0.01f,
                        0.01f
                    );
                    entites.Add(pt);

                    int index = GetIndex(offset,i,j,m);


                    int l_i = i - 1; //left
                    int r_i = i + 1; //right
                    int u_j = j - 1; //up
                    int d_j = j + 1; //down

                    //left
                    if ( l_i >= 0){
                        int left_index = GetIndex(offset,l_i,j,m);
                        springs.Add(new Spring(rest_distance,k1,index,left_index,cd));
                    }

                    //right
                    if ( r_i < n){
                        int right_index = GetIndex(offset,r_i,j,m);
                        springs.Add(new Spring(rest_distance,k1,index,right_index,cd));
                    }

                    //up
                    if ( u_j >= 0){
                        int up_index = GetIndex(offset,i,u_j,m);
                        springs.Add(new Spring(rest_distance,k1,index,up_index,cd));   
                    }

                    //down
                    if ( d_j < m){
                        int down_index = GetIndex(offset,i,d_j,m);
                        springs.Add(new Spring(rest_distance,k1,index,down_index,cd));
                    }

                    float sqrt_2 = (float)Math.Sqrt(2);

                    //diag up left
                    if((l_i >= 0)&&(u_j >= 0)){
                        int ul_index = GetIndex(offset,l_i,u_j,m);
                        springs.Add(new Spring(sqrt_2*rest_distance,k2,index,ul_index,cd));
                    } 

                    //diag up right
                    if((r_i < n)&&(u_j >= 0)){
                        int ur_index = GetIndex(offset,r_i,u_j,m);
                        springs.Add(new Spring(sqrt_2*rest_distance,k2,index,ur_index,cd));
                    } 

                    //diag down left
                    if((l_i >= 0)&&(d_j < m)){
                        int dl_index = GetIndex(offset,l_i,d_j,m);
                        springs.Add(new Spring(sqrt_2*rest_distance,k2,index,dl_index,cd));
                    } 

                    //diag down right
                    if((r_i < n)&&(d_j < m)){
                        int dr_index = GetIndex(offset,r_i,d_j,m);
                        springs.Add(new Spring(sqrt_2*rest_distance,k2,index,dr_index,cd));
                    } 

                    int dl_i = i - 2; //left
                    int dr_i = i + 2; //right
                    int du_j = j - 2; //up
                    int dd_j = j + 2; //down

                    //left double
                    if ( dl_i >= 0){
                        int left_index = GetIndex(offset,dl_i,j,m);
                        springs.Add(new Spring(2*rest_distance,k3,index,left_index,2f));
                    }

                    //right double
                    if ( dr_i < n){
                        int right_index = GetIndex(offset,dr_i,j,m);
                        springs.Add(new Spring(2*rest_distance,k3,index,right_index,2f));
                    }

                    //up double
                    if ( du_j >= 0){
                        int up_index = GetIndex(offset,i,du_j,m);
                        springs.Add(new Spring(2*rest_distance,k3,index,up_index,2f));   
                    }

                    //down double
                    if ( dd_j < m){
                        int down_index = GetIndex(offset,i,dd_j,m);
                        springs.Add(new Spring(2*rest_distance,k3,index,down_index,2f));
                    }

                }
            }

            settings.nbr_spring = springs.Count();
        }

        private int GetIndex(int offset,int i,int j,int l){
            return offset + i + (j*l);
        }

        /*
        public void UpdateForce(){
            foreach(string pt_key in links.Keys){
                List<SpringClass> springs = links[pt_key];

                Particule pt1 = nodes[pt_key];

                foreach(SpringClass sp in springs){
                    Particule pt2 = nodes[sp.key];

                    Vector3 Hooks_force;   // F_h = k * d
                    Vector3 Armts_force;   // f_a = -Cd * V

                    float dist = Vector3.Distance(pt2.position,pt1.position);
                    float dl = dist-sp.distance;

                    Vector3 normal = Vector3.Normalize(pt2.position - pt1.position);

                    float in_direction_velocity = Vector3.Dot(normal,pt2.velocity-pt1.velocity);

                    float spring_force = (dl*(sp.k/2));
                    //float damping_force = in_direction_velocity * cd;

                    

                    Hooks_force =  normal * spring_force;
                    //Armts_force = normal * damping_force;

                    

                    pt1.AddForce(Hooks_force);
                    //pt1.AddForce(Armts_force);

                }

            }
        }
        */
    }
}