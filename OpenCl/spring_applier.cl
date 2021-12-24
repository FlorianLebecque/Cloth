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

struct Spring{
    float rest_distance;
    float k;
    float cd;

    int particul_1;
    int particul_2;
};

struct Spring_force{
    int p1;
    int p2;
    struct Vector3 force;
};

struct Cloth_settings{
    int offset;
    int count;
    int nbr_spring;
};

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
    
    resutl.X = V1.X / v;
    resutl.Y = V1.Y / v;
    resutl.Z = V1.Z / v;

    return resutl;
}


__kernel void ComputeSpringForce(__global struct Particule_obj *input, __global struct Particule_obj *ouput ,__global struct Spring_force *sp,__global struct Cloth_settings *cloth){
	int index = get_global_id(0) + cloth[0].offset;

    ouput[index] = input[index];

    struct Vector3 force = V3fmul(input[index].acceleration,input[index].mass);

    for(int i = 0; i < cloth[0].nbr_spring; i++){

        if(sp[i].p1 == index){
            force = V3Add(force,sp[i].force);
        }

        if(sp[i].p2 == index){
            force = V3Sub(force,sp[i].force);
        }

    }

    ouput[index].acceleration = V3fdiv(force,input[index].mass);

}