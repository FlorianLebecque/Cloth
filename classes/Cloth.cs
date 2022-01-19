using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Color;
using static Raylib_cs.ConfigFlags;
using static Raylib_cs.CameraMode;
using static Raylib_cs.CameraProjection;
using static Raylib_cs.KeyboardKey;
using static Raylib_cs.ShaderLocationIndex;
using static Raylib_cs.MaterialMapIndex;

namespace PhysicObject.classes {
    public class Cloth {

        public Spring[] springs;

        public Spring_force[] spring_forces;     //create an array for each springs
         
        public Cloth_settings settings;

        private float rest_distance;
        private float max_distance;

        public Raylib_cs.Color color;

        public static ClothParameter parameter = new();

        public Cloth(Vector3 position,int n,int m,float rest_distance_,List<Particule> entites,List<Raylib_cs.Color> colors,Raylib_cs.Color c_){

            

            color = c_;

            Random rnd = new Random();

            
            rest_distance = rest_distance_;
            max_distance = parameter.max_distance_factor * rest_distance;

            int offset = entites.Count();

            settings = new Cloth_settings(offset,n*m,0);

            float start_pos_x = position.X - (n*rest_distance)/2;
            float start_pos_z = position.Z - (m*rest_distance)/2;

            List<Spring> spring_list = new List<Spring>();



            float k1 = parameter.k1;
            float k2 = parameter.k2;
            float k3 = parameter.k3;

            float cd = parameter.cd;

            for(int i = 0; i < n; i++){
                for(int j = 0; j < m ;j++){

                    Particule pt = new Particule(
                        new Vector3(start_pos_x+i*rest_distance,position.Y,start_pos_z+j*rest_distance),
                        new Vector3(0),
                        parameter.mass,
                        rest_distance/parameter.size_factor,
                        parameter.bounciness,
                        parameter.roughtness
                    );
                    entites.Add(pt);

                    int R = c_.r + rnd.Next(-10,10);
                    int G = c_.g + rnd.Next(-10,10);
                    int B = c_.b + rnd.Next(-10,10);

                    if(R >255) R = 255;
                    if(G >255) G = 255;
                    if(B >255) B = 255;

                    if(R < 0) R = 0;
                    if(G < 0) G = 0;
                    if(B < 0) B = 0;

                    colors.Add(new Raylib_cs.Color(R,G,B,255));
                    
                    int index = GetIndex(offset,i,j,m);


                    int l_i = i - 1; //left
                    int r_i = i + 1; //right
                    int u_j = j - 1; //up
                    int d_j = j + 1; //down

                    //left
                    if ( l_i >= 0){
                        int left_index = GetIndex(offset,l_i,j,m);
                        spring_list.Add(new Spring(rest_distance,max_distance,k1,index,left_index,cd));
                    }

                    //right
                    if ( r_i < n){
                        int right_index = GetIndex(offset,r_i,j,m);
                        spring_list.Add(new Spring(rest_distance,max_distance,k1,index,right_index,cd));
                    }

                    //up
                    if ( u_j >= 0){
                        int up_index = GetIndex(offset,i,u_j,m);
                        spring_list.Add(new Spring(rest_distance,max_distance,k1,index,up_index,cd));   
                    }

                    //down
                    if ( d_j < m){
                        int down_index = GetIndex(offset,i,d_j,m);
                        spring_list.Add(new Spring(rest_distance,max_distance,k1,index,down_index,cd));
                    }

                    float sqrt_2 = (float)Math.Sqrt(2);

                    //diag up left
                    if((l_i >= 0)&&(u_j >= 0)){
                        int ul_index = GetIndex(offset,l_i,u_j,m);
                        spring_list.Add(new Spring(sqrt_2*rest_distance,max_distance,k2,index,ul_index,cd));
                    } 

                    //diag up right
                    if((r_i < n)&&(u_j >= 0)){
                        int ur_index = GetIndex(offset,r_i,u_j,m);
                        spring_list.Add(new Spring(sqrt_2*rest_distance,max_distance,k2,index,ur_index,cd));
                    } 

                    //diag down left
                    if((l_i >= 0)&&(d_j < m)){
                        int dl_index = GetIndex(offset,l_i,d_j,m);
                        spring_list.Add(new Spring(sqrt_2*rest_distance,max_distance,k2,index,dl_index,cd));
                    } 

                    //diag down right
                    if((r_i < n)&&(d_j < m)){
                        int dr_index = GetIndex(offset,r_i,d_j,m);
                        spring_list.Add(new Spring(sqrt_2*rest_distance,max_distance,k2,index,dr_index,cd));
                    } 

                    int dist = 2;

                    int dl_i = i - dist; //left
                    int dr_i = i + dist; //right
                    int du_j = j - dist; //up
                    int dd_j = j + dist; //down

                    //left double
                    if ( dl_i >= 0){
                        int left_index = GetIndex(offset,dl_i,j,m);
                        spring_list.Add(new Spring(dist*rest_distance,max_distance,k3,index,left_index,2f));
                    }

                    //right double
                    if ( dr_i < n){
                        int right_index = GetIndex(offset,dr_i,j,m);
                        spring_list.Add(new Spring(dist*rest_distance,max_distance,k3,index,right_index,2f));
                    }

                    //up double
                    if ( du_j >= 0){
                        int up_index = GetIndex(offset,i,du_j,m);
                        spring_list.Add(new Spring(dist*rest_distance,max_distance,k3,index,up_index,2f));   
                    }

                    //down double
                    if ( dd_j < m){
                        int down_index = GetIndex(offset,i,dd_j,m);
                        spring_list.Add(new Spring(dist*rest_distance,max_distance,k3,index,down_index,2f));
                    }

                    

                }
            }

            settings.nbr_spring = spring_list.Count();

            springs = spring_list.ToArray();
            spring_forces = new Spring_force[springs.Count()];
        }

        private int GetIndex(int offset,int i,int j,int l){
            return offset + i + (j*l);
        }

        public static void SetOrbitalSpeed(Cloth t,List<Particule> entities,int target,UniversSettings univers,Vector3 normal){
            

            for(int i = t.settings.offset; i < t.settings.offset + t.settings.count ; i++){
                Particule a = entities[i];
                a.velocity = Particule.GetOrbitalSpeed(entities[i],entities[target],normal,univers);
                entities[i] = a;
            }

        }

    }

    public struct ClothParameter{
        public float k1;
        public float k2;
        public float k3;
        public float cd;
        public float mass;
        public float bounciness;
        public float roughtness;
        public float size_factor;
        public float max_distance_factor;

        public ClothParameter(){
            k1 = 100;
            k2 = 150;
            k3 = 200;
            cd = 2f;
            max_distance_factor = 7;
            size_factor = 4;
            mass = 0.2f;
            bounciness = 0;
            roughtness = 1;

        }

    }
}