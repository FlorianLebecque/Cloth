#include "../headers/structs.h"
#include "../headers/vector.h"

__kernel void ComputePos(__global struct Univers *uni,
                         __global struct Particule_obj *input,
                         __global struct Particule_obj *output) {

  int index = get_global_id(0);

  output[index] = input[index];

  // output[index].velocity =
  // V3Add(input[index].velocity,V3fmul(input[index].acceleration,uni[0].dt));

  output[index].position =
      V3Add(input[index].position, V3fmul(output[index].velocity, uni[0].dt));
};