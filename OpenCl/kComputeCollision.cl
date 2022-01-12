struct Vector3{
	float X;
	float Y;
	float Z;
};

struct Univers {
        float G;
        float dt;
};

struct Particule_obj{
    struct Vector3 position;
    struct Vector3 velocity;
    struct Vector3 acceleration;
    float mass;
    float bounciness;
    float radius;
    float roughness;
};

struct OctreeSettings{
    int particules_count;
    int regions_count;
};

struct Cube {
    struct Vector3 center;
    float size;
    struct Vector3 p0;
    struct Vector3 p1;
    struct Vector3 p2;
    struct Vector3 p3;
    struct Vector3 p4;
    struct Vector3 p5;
    struct Vector3 p6;
    struct Vector3 p7;
};

struct Region {
        
    int capacity;
    int count;
    bool subdivided;
    int offset;  
    struct Cube region;
    int child_s_nw;
    int child_s_ne;
    int child_s_se;
    int child_s_sw;
    int child_t_nw;
    int child_t_ne;
    int child_t_se;
    int child_t_sw;
};

float V3Length(struct Vector3 V1){
    return sqrt((V1.X*V1.X)+(V1.Y*V1.Y)+(V1.Z*V1.Z));
}

struct Vector3 V3Sub(struct Vector3 V1,struct Vector3 V2){
    struct Vector3 resutl;
    
    resutl.X = V1.X - V2.X;
    resutl.Y = V1.Y - V2.Y;
    resutl.Z = V1.Z - V2.Z;

    return resutl;
}

struct Vector3 V3Add(struct Vector3 V1,struct Vector3 V2){
    struct Vector3 resutl;
    
    resutl.X = V1.X + V2.X;
    resutl.Y = V1.Y + V2.Y;
    resutl.Z = V1.Z + V2.Z;

    return resutl;
}

struct Vector3 V3fmul(struct Vector3 V1,float v){
    struct Vector3 resutl;
    
    resutl.X = V1.X * v;
    resutl.Y = V1.Y * v;
    resutl.Z = V1.Z * v;

    return resutl;
}

struct Vector3 V3fdiv(struct Vector3 V1,float v){
    struct Vector3 resutl;
    
    if(v != 0){
        resutl.X = V1.X / v;
        resutl.Y = V1.Y / v;
        resutl.Z = V1.Z / v;
    }
    

    return resutl;
}

float V3Distance(struct Vector3 V1,struct Vector3 V2){
    return V3Length(V3Sub(V1,V2));
}

float V3Dot(struct Vector3 V1, struct Vector3 V2){
    return (V1.X * V2.X) + (V1.Y * V2.Y) + (V1.Z * V2.Z);
}

struct Vector3 V3Normalize(struct Vector3 V1){
    struct Vector3 resutl;
    float l = V3Length(V1);

    if(l != 0){
        resutl.X = V1.X / l;
        resutl.Y = V1.Y / l;
        resutl.Z = V1.Z / l;
    }
    
    return resutl;
}

bool Intersect(struct Cube c1,struct Cube c2){
    bool IsX = ((c1.center.X + c1.size) >= (c2.center.X - c2.size)) &&  ((c1.center.X - c1.size) <= (c2.center.X + c2.center.X));
    bool IsY = ((c1.center.Y + c1.size) >= (c2.center.Y - c2.size)) &&  ((c1.center.Y - c1.size) <= (c2.center.Y + c2.center.X));
    bool IsZ = ((c1.center.Z + c1.size) >= (c2.center.Z - c2.size)) &&  ((c1.center.Z - c1.size) <= (c2.center.Z + c2.center.X));

    return IsX && IsY && IsZ;
}

bool IsInCube(struct Cube c, struct Vector3 p){
    bool IsX = (p.X >= (c.center.X - c.size))&&((p.X <= c.center.X + c.size));
    bool IsY = (p.Y >= (c.center.Y - c.size))&&((p.Y <= c.center.Y + c.size));
    bool IsZ = (p.Z >= (c.center.Z - c.size))&&((p.Z <= c.center.Z + c.size));

    return IsX && IsY && IsZ;
}



int getParticules(struct OctreeSettings *ts,struct Region *regions,int *treeData,struct Particule_obj *input,struct Cube space,int *particules_queue){
    int children[10000];    //array of the regions indexes (only the one we should check)
    children[0] = 0;        //set the first region to check (the one whose index is 0)

    int region_cursor = 0;  // cursor who give the position in the children queue
    int region_counter = 1; // number of element in the queue

    int particules_counter = 0; //number and index of the cursor in the queue of particule

    while((region_cursor < region_counter)&&(particules_counter < ts[0].particules_count)){
        int region_index = children[region_cursor];

        //on check les particules dans la région
        for(int i = regions[region_index].offset; i < regions[region_index].offset + regions[region_index].capacity;i++){
            if(IsInCube(space,input[i].position)){
                particules_queue[particules_counter] = treeData[i];
                particules_counter++;

            }
        }

        if(regions[region_index].subdivided){

            if(regions[region_index].child_s_nw != -1){
                if(Intersect(regions[regions[region_index].child_s_nw].region,space)){
                    children[region_counter] = regions[region_index].child_s_nw;
                    region_counter++;
                }
            }
            if(regions[region_index].child_s_ne != -1){
                if(Intersect(regions[regions[region_index].child_s_ne].region,space)){
                    children[region_counter] = regions[region_index].child_s_ne;
                    region_counter++;
                }
            }
            if(regions[region_index].child_s_se != -1){
                if(Intersect(regions[regions[region_index].child_s_se].region,space)){
                    children[region_counter] = regions[region_index].child_s_se;
                    region_counter++;
                }
            }
            if(regions[region_index].child_s_sw != -1){
                if(Intersect(regions[regions[region_index].child_s_sw].region,space)){
                    children[region_counter] = regions[region_index].child_s_sw;
                    region_counter++;
                }
            }

            if(regions[region_index].child_t_nw != -1){
                if(Intersect(regions[regions[region_index].child_t_nw].region,space)){
                    children[region_counter] = regions[region_index].child_t_nw;
                    region_counter++;
                }
            }
            if(regions[region_index].child_t_ne != -1){
                if(Intersect(regions[regions[region_index].child_t_ne].region,space)){
                    children[region_counter] = regions[region_index].child_t_ne;
                    region_counter++;
                }
            }
            if(regions[region_index].child_t_se != -1){
                if(Intersect(regions[regions[region_index].child_t_se].region,space)){
                    children[region_counter] = regions[region_index].child_t_se;
                    region_counter++;
                }
            }
            if(regions[region_index].child_t_sw != -1){
                if(Intersect(regions[regions[region_index].child_t_sw].region,space)){
                    children[region_counter] = regions[region_index].child_t_sw;
                    region_counter++;
                }
            }
        }
        

        region_cursor++;
    }

    return particules_counter;
}

__kernel void ComputeCollision(__global struct Univers *uni,__global struct Particule_obj *input, __global struct Particule_obj *output,__global struct OctreeSettings *treeSettings,__global struct Region *treeRegions,__global int *treeData){
    int total = get_global_size(0);
	int index = get_global_id(0);

    output[index] = input[index];

    int particules[10000];

    struct Cube space;
    space.center = input[index].position;
    space.size = 100;
    
    int nbr_particule = getParticules(treeSettings,treeRegions,treeData,input,space,particules);
    //nbr_particule = -1;

    for(int k = 0; k <= nbr_particule;k++){
        int i = particules[k];
        if(i != index){

            float dist = V3Distance(input[index].position,input[i].position);

            if(dist < (input[index].radius + input[i].radius)){
                float totalMass   = input[index].mass + input[i].mass;
                float totalFactor = input[index].mass / totalMass;
                float bounciness  = (input[index].bounciness + input[i].bounciness) / 2;

                struct Vector3 normal = V3Normalize(V3Sub(input[i].position,input[index].position));
                struct Vector3 ExitVector = V3fmul(normal,((input[index].radius+input[i].radius)-dist));

                struct Vector3 VELn = V3fmul(normal,V3Dot(normal,input[index].velocity));
                struct Vector3 VELt = V3Sub(input[index].velocity,VELn);

                //speed adjustement
                float ViSO1 = V3Dot(normal,input[index].velocity) * bounciness;
                float ViSO2 = V3Dot(input[i].velocity,normal) * bounciness;

                struct Vector3 normalVelocity = V3fmul(normal,ViSO1);

                float resitance =  -V3Length(VELt)*(input[index].roughness);
                struct Vector3 resitanceForce = V3fmul(V3Normalize(VELt),resitance);

                output[index].velocity = V3Sub(output[index].velocity,normalVelocity);
                float VfSO1 = ViSO2;
                if(input[index].mass != input[i].mass){
                    VfSO1 = (((input[index].mass-input[i].mass)/(totalMass))  * ViSO1) + (((2*input[i].mass)/(totalMass))*ViSO2);
                }

                output[index].velocity = V3Add(output[index].velocity,V3fmul(normal,VfSO1));
                output[index].velocity = V3Add(output[index].velocity,V3fmul(resitanceForce,uni[0].dt));
                

                //position adjustement
                output[index].position = V3Sub(output[index].position,V3fmul(ExitVector,(1-totalFactor)));

            }

        }

    }

    struct Vector3 center;
    center.X = center.Y = center.Z = 0;
    struct Cube c;
    c.center = center;
    c.size = 5000;

    if(!IsInCube(c,output[index].position)){
        output[index].position = V3Sub(output[index].position,V3fmul(output[index].velocity,uni[0].dt));

        output[index].velocity.X = - output[index].velocity.X;
        output[index].velocity.Y = - output[index].velocity.Y;
        output[index].velocity.Z = - output[index].velocity.Z;
        
    }
};