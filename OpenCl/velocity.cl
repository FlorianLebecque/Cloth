struct vector3{
	float X;
	float Y;
	float Z;
};


__kernel void UpdateVelocity(__global struct vector3 *velocity, __global struct vector3  *acceleration, __global float *dt){
	int i = get_global_id(0);
	
	/*
	velocity[i*3]   +=   acceleration[i*3] * dt[0];
    velocity[i*3+1] += acceleration[i*3+1] * dt[0];
    velocity[i*3+2] += acceleration[i*3+2] * dt[0];
	*/

	velocity[i].X+= (acceleration[i].X * dt[0]);
	velocity[i].Y+= (acceleration[i].Y * dt[0]);
	velocity[i].Z+= (acceleration[i].Z * dt[0]);


};