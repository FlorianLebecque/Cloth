using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cloth.classes {
    public class Tissue {
        public Dictionary<String,Particule> nodes;
        public Dictionary<String,List<Spring>> links;

        public float K = 150f;
        public float cd = 2f;

        private float rest_distance = 3f;
        private Vector3 starting_pos; 

        public Tissue(Vector3 position,int n,int m){

            starting_pos = position;

            nodes = new Dictionary<string, Particule>();

            float start_pos_x = position.X - (n*rest_distance)/2;
            float start_pos_z = position.Z - (m*rest_distance)/2;

            for(int i = 0; i < n; i++){
                for(int j = 0; j < m ;j++){

                    Particule pt = new Particule(
                        new Vector3(start_pos_x+i*rest_distance,starting_pos.Y,start_pos_z+j*rest_distance),
                        new Vector3(0),
                        0.2f,
                        rest_distance/4f,
                        0.01f
                    );

                    String key = String.Format("{0}-{1}",i,j);

                    nodes.Add(key,pt);
                }
            }

            links = new();

            foreach(String pt_key in nodes.Keys){

                Particule current = nodes[pt_key];

                String[] index = pt_key.Split("-");
                int i,j;
                
                i = Convert.ToInt32(index[0]);
                j = Convert.ToInt32(index[1]);

                List<String> potential_links = new();
                List<Spring> correct_links = new();


                    //diagonal link
                potential_links.Add(String.Format("{0}-{1}",i - 1,j - 1));  //  left up
                potential_links.Add(String.Format("{0}-{1}",i + 1,j - 1));  //  right up
                potential_links.Add(String.Format("{0}-{1}",i - 1,j + 1));  //  left down
                potential_links.Add(String.Format("{0}-{1}",i + 1,j + 1));  //  right down

                AddLink(correct_links,current,pt_key,potential_links,150f);
                potential_links.Clear();

                    //direct link
                potential_links.Add(String.Format("{0}-{1}",i + 1,j));  //  right
                potential_links.Add(String.Format("{0}-{1}",i - 1,j));  //  left
                potential_links.Add(String.Format("{0}-{1}",i,j - 1));  //  up
                potential_links.Add(String.Format("{0}-{1}",i,j + 1));  //  down

                AddLink(correct_links,current,pt_key,potential_links,500f);
                potential_links.Clear();

                    //double link
                potential_links.Add(String.Format("{0}-{1}",i + 2,j));  //  right
                potential_links.Add(String.Format("{0}-{1}",i - 2,j));  //  left
                potential_links.Add(String.Format("{0}-{1}",i,j - 2));  //  up
                potential_links.Add(String.Format("{0}-{1}",i,j + 2));  //  down
                
                AddLink(correct_links,current,pt_key,potential_links,500f);
                potential_links.Clear();

                links.Add(pt_key,correct_links);
            }


        }

        private void AddLink(List<Spring> correct_links,Particule current,String currentKey,List<String> potential_links,float k){
            foreach(String potential_link in potential_links){
                if(nodes.ContainsKey(potential_link)){
                    
                    Particule link_node = nodes[potential_link];


                    Spring sp = new(potential_link,Vector3.Distance(link_node.position,current.position),k);



                    correct_links.Add(sp);
                }
            }                

            //links.Add(currentKey,correct_links);
        }

        public void UpdateForce(){
            foreach(string pt_key in links.Keys){
                List<Spring> springs = links[pt_key];

                Particule pt1 = nodes[pt_key];

                foreach(Spring sp in springs){
                    Particule pt2 = nodes[sp.key];

                    Vector3 Hooks_force;   // F_h = k * d
                    Vector3 Armts_force;   // f_a = -Cd * V

                    float dist = Vector3.Distance(pt2.position,pt1.position);
                    float dl = dist-sp.distance;

                    Vector3 normal = Vector3.Normalize(pt2.position - pt1.position);

                    float in_direction_velocity = Vector3.Dot(normal,pt2.velocity-pt1.velocity);

                    float spring_force = (dl*(sp.k/2));
                    float damping_force = in_direction_velocity * cd;

                    

                    Hooks_force =  normal * spring_force;
                    Armts_force = normal * damping_force;

                    

                    pt1.AddForce(Hooks_force);
                    pt1.AddForce(Armts_force);

                }

            }
        }

        public void Draw(){
            foreach(string pt_key in links.Keys){
                List<Spring> springs = links[pt_key];

                Particule pt1 = nodes[pt_key];

                foreach(Spring sp in springs){
                    Particule pt2 = nodes[sp.key];

                    DrawLine3D(pt1.position,pt2.position,pt1.cl);

                }
            }
                
        }

    }
}