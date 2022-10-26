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
    float max_distance;
    float k;
    float cd;

    int particul_1;
    int particul_2;

    int broken;
};

struct Spring_force{
    int p1;
    int p2;
    struct Vector3 force;
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

__kernel void ComputeSpringForce(__global struct Particule_obj *input,__global struct Spring *springs, __global struct Spring_force *sp){
    
	int index = get_global_id(0);

    int i_p1 = springs[index].particul_1;
    int i_p2 = springs[index].particul_2;

    float dist = V3Distance(input[i_p2].position,input[i_p1].position);

    if (dist > springs[index].max_distance)
        springs[index].broken = 0;

    float dl = dist - springs[index].rest_distance;

    struct Vector3 normal = V3Normalize(V3Sub(input[i_p2].position,input[i_p1].position));

    float in_direction_velocity_1 = V3Dot(normal,V3Sub(input[i_p2].velocity,input[i_p1].velocity));

    float spring_force  = dl * (springs[index].k/2) * springs[index].broken;

    float damping_force_1 = in_direction_velocity_1 * springs[index].cd * springs[index].broken;

    if(damping_force_1 > spring_force){
        damping_force_1 = spring_force;
    }

    struct Vector3 hooks_p1   = V3fmul(normal,spring_force);
    struct Vector3 damping_p1 = V3fmul(normal,damping_force_1);


    sp[index].force = V3Add(hooks_p1,damping_p1);
    sp[index].p1 = i_p1;
    sp[index].p2 = i_p2;
};