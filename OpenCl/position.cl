__kernel void UpdatePosition(__global float *position, __global float *velocity, __global float *dt){
	int i = get_global_id(0);
    
	position[i*3]   +=   velocity[i*3] * dt[0];
    position[i*3+1] += velocity[i*3+1] * dt[0];
    position[i*3+2] += velocity[i*3+2] * dt[0];

	//position[i] += (velocity[i] * dt[i]);
};