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

    
};


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


__kernel void ComputeVelocity(__global struct Particule_obj *input, __global struct Particule_obj *output){

	int index = get_global_id(0);

    output[index].velocity = V3Add(input[index].velocity,V3fmul(input[index].acceleration,0.01f));
};