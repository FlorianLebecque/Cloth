struct Vector3{
	float X;
	float Y;
	float Z;
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

__kernel void ComputeGravity(__global struct Particule_obj *input, __global struct Particule_obj *output){
    int total = get_global_size(0);
	int index = get_global_id(0);

    struct Vector3 force;
    force.X = 0;
    force.Y = 0;
    force.Z = 0;

    output[index] = input[index];

    for(int i = 0; i < total;i++){

        if(i != index){
            float dist = V3Distance(input[index].position,input[i].position);
            struct Vector3 normal = V3Normalize(V3Sub(input[i].position,input[index].position));

            float force_value = 10 * (input[i].mass * input[index].mass) / (dist*dist);

            force = V3Add(force,V3fmul(normal,force_value));
        }

    }


    output[index].acceleration = V3fdiv(force,input[index].mass);
};