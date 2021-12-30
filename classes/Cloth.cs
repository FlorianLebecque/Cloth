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

namespace Cloth.classes {
    public class Tissue {

        
        public Spring[] springs;

        public Cloth_settings settings;

        private float rest_distance;
        private float max_distance;
        private float roughtness;


        public Tissue(Vector3 position,int n,int m,List<Particule> entites,List<Raylib_cs.Color> colors){

            rest_distance = 3f;
            max_distance = 15f*rest_distance;

            roughtness = 1f;


            int offset = entites.Count();

            settings = new Cloth_settings(offset,n*m,0);

            float start_pos_x = position.X - (n*rest_distance)/2;
            float start_pos_z = position.Z - (m*rest_distance)/2;

            List<Spring> spring_list = new List<Spring>();



            float k1 = 90;
            float k2 = 75;
            float k3 = 50;

            float cd = 2f;

            for(int i = 0; i < n; i++){
                for(int j = 0; j < m ;j++){

                    Particule pt = new Particule(
                        new Vector3(start_pos_x+i*rest_distance,position.Y,start_pos_z+j*rest_distance),
                        new Vector3(0),
                        0.2f,
                        rest_distance/4f,
                        0.01f,
                        roughtness
                    );
                    entites.Add(pt);

                    colors.Add(new Raylib_cs.Color(50,50,GetRandomValue(100,255),255));

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

                    int dl_i = i - 2; //left
                    int dr_i = i + 2; //right
                    int du_j = j - 2; //up
                    int dd_j = j + 2; //down

                    //left double
                    if ( dl_i >= 0){
                        int left_index = GetIndex(offset,dl_i,j,m);
                        spring_list.Add(new Spring(2*rest_distance,max_distance,k3,index,left_index,2f));
                    }

                    //right double
                    if ( dr_i < n){
                        int right_index = GetIndex(offset,dr_i,j,m);
                        spring_list.Add(new Spring(2*rest_distance,max_distance,k3,index,right_index,2f));
                    }

                    //up double
                    if ( du_j >= 0){
                        int up_index = GetIndex(offset,i,du_j,m);
                        spring_list.Add(new Spring(2*rest_distance,max_distance,k3,index,up_index,2f));   
                    }

                    //down double
                    if ( dd_j < m){
                        int down_index = GetIndex(offset,i,dd_j,m);
                        spring_list.Add(new Spring(2*rest_distance,max_distance,k3,index,down_index,2f));
                    }

                }
            }

            settings.nbr_spring = spring_list.Count();

            springs = spring_list.ToArray();
        }

        private int GetIndex(int offset,int i,int j,int l){
            return offset + i + (j*l);
        }

    }
}