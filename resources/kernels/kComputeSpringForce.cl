#include "../headers/structs.h"
#include "../headers/vector.h"

__kernel void ComputeSpringForce(__global struct Particule_obj *input,
                                 __global struct Spring *springs,
                                 __global struct Spring_force *sp) {

  int index = get_global_id(0);

  int i_p1 = springs[index].particul_1;
  int i_p2 = springs[index].particul_2;

  float dist = V3Distance(input[i_p2].position, input[i_p1].position);

  if (dist > springs[index].max_distance)
    springs[index].broken = 0;

  float dl = dist - springs[index].rest_distance;

  struct Vector3 normal =
      V3Normalize(V3Sub(input[i_p2].position, input[i_p1].position));

  float in_direction_velocity_1 =
      V3Dot(normal, V3Sub(input[i_p2].velocity, input[i_p1].velocity));

  float spring_force = dl * (springs[index].k / 2) * springs[index].broken;

  float damping_force_1 =
      in_direction_velocity_1 * springs[index].cd * springs[index].broken;

  if (damping_force_1 > spring_force) {
    damping_force_1 = spring_force;
  }

  struct Vector3 hooks_p1 = V3fmul(normal, spring_force);
  struct Vector3 damping_p1 = V3fmul(normal, damping_force_1);

  sp[index].force = V3Add(hooks_p1, damping_p1);
  sp[index].p1 = i_p1;
  sp[index].p2 = i_p2;
};