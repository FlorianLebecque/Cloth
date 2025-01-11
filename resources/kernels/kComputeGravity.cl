#include "../headers/structs.h"
#include "../headers/vector.h"

__kernel void ComputeGravity(__global struct Univers *uni,
                             __global struct Particule_obj *input,
                             __global struct Particule_obj *output) {
  int total = get_global_size(0);
  int index = get_global_id(0);

  struct Vector3 force;
  force.X = 0;
  force.Y = 0;
  force.Z = 0;

  output[index] = input[index];

  for (int i = 0; i < total; i++) {

    if (i != index) {
      float dist = V3Distance(input[index].position, input[i].position);
      struct Vector3 normal =
          V3Normalize(V3Sub(input[i].position, input[index].position));

      float force_value =
          uni[0].G * (input[i].mass * input[index].mass) / (dist * dist);

      force = V3Add(force, V3fmul(normal, force_value));
    }
  }

  output[index].acceleration = V3fdiv(force, input[index].mass);
};