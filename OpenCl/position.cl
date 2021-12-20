__kernel void UpdatePosition(__global float3 *position, __global float3 *velocity, __global float *dt){
	int i = get_global_id(0);
    
	position[i] += (velocity[i] * dt[i]);
};