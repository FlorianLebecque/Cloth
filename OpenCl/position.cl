struct vector3{
	float X;
	float Y;
	float Z;
};

__kernel void UpdatePosition(__global struct vector3 *position, __global struct vector3 *velocity, __global float *dt){
	int i = get_global_id(0);
    /*
	position[i*3]   +=   velocity[i*3] * dt[0];
    position[i*3+1] += velocity[i*3+1] * dt[0];
    position[i*3+2] += velocity[i*3+2] * dt[0];
    */
	position[i].X += (velocity[i].X * dt[i]);
	position[i].Y += (velocity[i].Y * dt[i]);
	position[i].Z += (velocity[i].Z * dt[i]);
};