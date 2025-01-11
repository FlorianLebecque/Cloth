#include "../headers/structs.h"
#include "../headers/vector.h"

__kernel void ComputeSpring(__global struct Particule_obj *input,
                            __global struct Spring_force *sp,
                            __global struct Cloth_settings *cloth) {
  int index = get_global_id(0) + cloth[0].offset;

  // ouput[index] = input[index];

  struct Vector3 force = V3fmul(input[index].acceleration, input[index].mass);

  for (int i = 0; i < cloth[0].nbr_spring; i++) {

    if (sp[i].p1 == index) {
      force = V3Add(force, sp[i].force);
    }

    if (sp[i].p2 == index) {
      force = V3Sub(force, sp[i].force);
    }
  }

  input[index].acceleration = V3fdiv(force, input[index].mass);
}