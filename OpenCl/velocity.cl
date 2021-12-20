__kernel void UpdateVelocity(__global float3 *velocity, __global float3 *acceleration, __global float *dt){
	int i = get_global_id(0);
	
	velocity[i] += (acceleration[i] * dt[i]);
};