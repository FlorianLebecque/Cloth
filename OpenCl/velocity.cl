struct vector3{
	float X;
	float Y;
	float Z;
};


__kernel void UpdateVelocity(__global float *velocity, __global float  *acceleration, __global float *dt){
	int i = get_global_id(0);
	
	velocity[i*3]   +=   acceleration[i*3] * dt[0];
    velocity[i*3+1] += acceleration[i*3+1] * dt[0];
    velocity[i*3+2] += acceleration[i*3+2] * dt[0];
	

	//velocity[i]+= (acceleration[i] * dt[0]);


};