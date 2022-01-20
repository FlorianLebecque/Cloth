using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Color;
using static Raylib_cs.ConfigFlags;
using static Raylib_cs.CameraMode;
using static Raylib_cs.CameraProjection;
using static Raylib_cs.KeyboardKey;
using static Raylib_cs.ShaderLocationIndex;
using static Raylib_cs.MaterialMapIndex;



namespace PhysicObject.classes.tree {
    public class Octree {
        
        private int[] particules_index;
        private Region[] regions;

        private int particul_counter;
        private int region_counter;

        private int region_capacity;
        private int nbr_particul;

        public int[] ParticulesArray;
        public Region[] RegionsArray;
        
        public OctreeSettings settings;

        static Color cl = new Color(255,0,0,255);
        public float capacity {get {
            return (float)region_counter / ((float)Math.Ceiling((double)nbr_particul/(double)region_capacity) * 4);
        }}

        public int particulCount{get{
            return particul_counter;
        }}

        public Octree(int region_cap,int nbr_particul){
            region_capacity = region_cap;
            this.nbr_particul = nbr_particul;

            int nbr_region = (int)Math.Ceiling((double)nbr_particul/(double)region_cap) * 4;
            regions = new Region[nbr_region];
            RegionsArray = new Region[nbr_region];
            ParticulesArray = new int[nbr_particul];

            particul_counter = 0;
            region_counter = 0;

            particules_index = new int[nbr_region * region_capacity];

            regions[0] = new Region(new Vector3(0,0,0),5000,region_capacity,0);

            settings = new OctreeSettings(0,0);
            
        }

        public void insert(int index,Particule p){
            insert(index,p,0);
        }

        private bool insert(int index,Particule p,int region_index){

            if(region_index == -1)
                return false;
                //check si la particule est dans le cube
            if(!regions[region_index].region.Inside(p.position))
                return false;   //n'est pas dans la bonne région


                //check si y a de la place dans la regions actuel
            if(regions[region_index].count < regions[region_index].capacity){
                particules_index[regions[region_index].offset+regions[region_index].count] = index;

                regions[region_index].count++;
                particul_counter++;

                return true;
            }

            if(regions[region_index].subdivided){
                bool result = false;
                result = insert(index,p,regions[region_index].child_s_nw);
                if(result)
                    return true;
                result = insert(index,p,regions[region_index].child_s_ne);
                if(result)
                    return true;
                result = insert(index,p,regions[region_index].child_s_se);
                if(result)
                    return true;
                result = insert(index,p,regions[region_index].child_s_sw);
                if(result)
                    return true;
                result = insert(index,p,regions[region_index].child_t_nw);
                if(result)
                    return true;
                result = insert(index,p,regions[region_index].child_t_ne);
                if(result)
                    return true;
                result = insert(index,p,regions[region_index].child_t_se);
                if(result)
                    return true;
                result = insert(index,p,regions[region_index].child_t_sw);
                if(result)
                    return true;

                
            }
            int new_index = Subdivide(region_index,p);
            
            return insert(index,p,new_index);
        }

        public void inserts(Particule[] entities){

            //this = new Octree(region_capacity,nbr_particul);

            for(int i = 0; i < entities.Count(); i++){
                insert(i,entities[i]);
            }

        }

        public int Subdivide(int region_index,Particule p){

            regions[region_index].subdivided = true;

            Vector3 curCenter = regions[region_index].region.center;
            float size = regions[region_index].region.size;

            float newSize = regions[region_index].region.size / 2;

            int x_plus = (p.position.X > curCenter.X)? 1 : 0; 
            int y_plus = (p.position.Y > curCenter.Y)? 1 : 0; 
            int z_plus = (p.position.Z > curCenter.Z)? 1 : 0; 

            float nx = curCenter.X - newSize + (size * x_plus);
            float ny = curCenter.Y - newSize + (size * y_plus);
            float nz = curCenter.Z - newSize + (size * z_plus);
            
            Vector3 correspondantCenter = new Vector3(nx,ny,nz);

            region_counter++;


            regions[region_counter] = new Region(correspondantCenter,newSize,region_capacity,(region_counter)*region_capacity);

            SetChild(region_index,x_plus,y_plus,z_plus);


            return region_counter;
        }

        private void SetChild(int region_index,int x_plus,int y_plus,int z_plus){
            if(z_plus == 0){    //bottom
                if(x_plus == 0){
                    if(y_plus == 0){
                        regions[region_index].child_s_nw = region_counter;
                    }else{
                        regions[region_index].child_s_sw = region_counter;
                    }
                }else{
                    if(y_plus == 0){
                        regions[region_index].child_s_ne = region_counter;
                    }else{
                        regions[region_index].child_s_se = region_counter;
                    }
                }
            }else{  //top
                if(x_plus == 0){
                    if(y_plus == 0){
                        regions[region_index].child_t_nw = region_counter;
                    }else{
                        regions[region_index].child_t_sw = region_counter;
                    }
                }else{
                    if(y_plus == 0){
                        regions[region_index].child_t_ne = region_counter;
                    }else{
                        regions[region_index].child_t_se = region_counter;
                    }
                }
            }
        }

        public void Clear(){
            particul_counter = 0;
            region_counter = 0;

            regions[0].child_s_nw = -1;
            regions[0].child_s_ne = -1;
            regions[0].child_s_se = -1;
            regions[0].child_s_sw = -1;
            regions[0].child_t_nw = -1;
            regions[0].child_t_ne = -1;
            regions[0].child_t_se = -1;
            regions[0].child_t_sw = -1;

            regions[0].count = 0;
            regions[0].subdivided = false;

        }

        public void GenParticulesArray(){
            
            ParticulesArray = new int[nbr_particul];
            RegionsArray    = new Region[(int)Math.Ceiling((double)nbr_particul/(double)region_capacity) * 4];
            int p_counter = 0;
            

            for(int i = 0; i <= region_counter;i++){
                Region cur_region = regions[i];

                RegionsArray[i] = new Region(cur_region.region.center,cur_region.region.size,cur_region.count,p_counter);
                RegionsArray[i].count = cur_region.count;
                RegionsArray[i].child_s_nw = cur_region.child_s_nw;
                RegionsArray[i].child_s_ne = cur_region.child_s_ne;
                RegionsArray[i].child_s_se = cur_region.child_s_se;
                RegionsArray[i].child_s_sw = cur_region.child_s_sw;
                RegionsArray[i].child_t_nw = cur_region.child_t_nw;
                RegionsArray[i].child_t_ne = cur_region.child_t_ne;
                RegionsArray[i].child_t_se = cur_region.child_t_se;
                RegionsArray[i].child_t_sw = cur_region.child_t_sw;

                RegionsArray[i].subdivided = cur_region.subdivided;

                for(int j = regions[i].offset; j < regions[i].offset + regions[i].count; j++){
                    ParticulesArray[p_counter] = particules_index[j];
                    p_counter++;
                }
            }

            settings.particules_count = p_counter;
            settings.regions_count = region_counter;
        }

        public void Draw(){
            for(int i = 0; i < region_counter; i++){
                regions[i].region.Draw(Color.DARKPURPLE);
            }
        }

    }


    public struct OctreeSettings{
        public int particules_count;
        public int regions_count;

        public OctreeSettings(int pc,int rc){
            particules_count = pc;
            regions_count = rc;
        }

    }
}