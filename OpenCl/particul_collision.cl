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
    
    resutl.X = V1.X / v;
    resutl.Y = V1.Y / v;
    resutl.Z = V1.Z / v;

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
    resutl.X = V1.X / l;
    resutl.Y = V1.Y / l;
    resutl.Z = V1.Z / l;
    return resutl;
}

__kernel void ComputeCollision(__global struct Particule_obj *input, __global struct Particule_obj *output){
    int total = get_global_size(0);
	int index = get_global_id(0);

    output[index] = input[index];

    for(int i = 0; i < total;i++){

        if(i != index){

            float dist = V3Distance(input[index].position,input[i].position);

            if(dist < (input[index].radius + input[i].radius)){
                float totalMass   = input[index].mass + input[i].mass;
                float totalFactor = input[index].mass / totalMass;
                float bounciness  = (input[index].bounciness + input[i].bounciness) / 2;

                struct Vector3 normal = V3Normalize(V3Sub(input[i].position,input[index].position));
                struct Vector3 ExitVector = V3fmul(normal,((input[index].radius+input[i].radius)-dist));

                //speed adjustement
                float ViSO1 = V3Dot(input[index].velocity,normal) * bounciness;
                float ViSO2 = V3Dot(input[i].velocity,normal) * bounciness;

                output[index].velocity = V3Sub(output[index].velocity,V3fmul(normal,ViSO1));
                float VfSO1 = ViSO2;
                if(input[index].mass != input[i].mass){
                    VfSO1 = (((input[index].mass-input[i].mass)/(totalMass))  * ViSO1) + (((2*input[i].mass)/(totalMass))*ViSO2);
                }
                output[index].velocity = V3Add(output[index].velocity,V3fmul(normal,VfSO1));


                //force adjustement
                float ReactionForceValue = V3Dot(
                    V3fmul(input[index].acceleration,input[index].mass),
                    normal
                );
                struct Vector3 ReactionForce = V3fmul(normal,-ReactionForceValue);
                output[index].acceleration = V3Add(output[index].acceleration,V3fdiv(ReactionForce,output[index].mass));

                //position adjustement
                output[index].position = V3Sub(output[index].position,V3fmul(ExitVector,(1-totalFactor)));

            }

        }

    }


   
};