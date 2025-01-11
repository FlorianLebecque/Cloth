// #include "../headers/structs.h"
#include "../headers/octree.h"
#include "../headers/vector.h"
/*
    -> uni : univers | contains the time step and other universal constants
    -> input : array of particules | read buffer for the particules
    -> output : array of particules | write buffer for the particules
    -> index : int | index of the current particule
    -> i : int | index of the particule to check collision with
*/
void collision(__global struct Univers *uni,
               __global struct Particule_obj *input,
               __global struct Particule_obj *output, int index, int i) {

  float dist = V3Distance(input[index].position, input[i].position);

  if (dist > (input[index].radius + input[i].radius)) {
    return;
  }

  float totalMass = input[index].mass + input[i].mass;
  float totalFactor = input[index].mass / totalMass;
  float bounciness = (input[index].bounciness + input[i].bounciness) / 2;
  float roughness = (input[index].roughness + input[i].roughness) / 2;

  float inv_m1 = 0;
  if (input[index].mass != 0) {
    inv_m1 = 1 / input[index].mass;
  } else {
    totalFactor = 1;
  }

  float inv_m2 = 0;
  if (input[i].mass != 0) {
    inv_m2 = 1 / input[i].mass;

  } else {
    totalFactor = 0;
  }

  struct Vector3 normal =
      V3Normalize(V3Sub(input[i].position, input[index].position));
  struct Vector3 VELrelativ =
      V3Sub(input[index].velocity, input[i].velocity); // relative speed

  float sepVel = V3Dot(normal, VELrelativ);
  float new_sepvel = sepVel * bounciness;
  float vsep_diff = new_sepvel + sepVel;
  float impulse = vsep_diff / (inv_m1 + inv_m2);

  struct Vector3 impulseVec = V3fmul(normal, impulse);

  output[index].velocity =
      V3Sub(output[index].velocity, V3fmul(impulseVec, inv_m1));

  struct Vector3 VELrelativ_n =
      V3fmul(normal, V3Dot(normal, VELrelativ)); // normal relative speed
  struct Vector3 VELrelativ_t =
      V3Sub(VELrelativ, VELrelativ_n); // tengent relative speed

  float resitance = -V3Length(VELrelativ_t) * roughness * inv_m1;
  struct Vector3 resitanceForce = V3fmul(V3Normalize(VELrelativ_t), resitance);
  output[index].velocity =
      V3Add(output[index].velocity, V3fmul(resitanceForce, uni[0].dt));

  // position adjustement
  struct Vector3 ExitVector =
      V3fmul(normal, ((input[index].radius + input[i].radius) - dist));
  output[index].position =
      V3Sub(output[index].position, V3fmul(ExitVector, (1 - totalFactor)));
}

int getParticules(__global struct OctreeSettings *ts,
                  __global struct Region *regions, __global int *treeData,
                  __global struct Univers *uni,
                  __global struct Particule_obj *input,
                  __global struct Particule_obj *output, struct Cube space) {
  int region_queue[10000]; // array of the regions indexes (only the one we
                           // should check)
  region_queue[0] =
      0; // set the first region to check (the one whose index is 0)

  int region_cursor =
      0; // cursor who give the position in the region_queue queue
  int region_counter = 1; // number of element in the queue

  int particules_counter =
      0; // number and index of the cursor in the queue of particule

  int index = get_global_id(0);

  while ((region_cursor < region_counter) &&
         (particules_counter < ts[0].particules_count)) {
    int region_index = region_queue[region_cursor];

    // on check les particules dans la région
    for (int i = regions[region_index].offset;
         i < regions[region_index].offset + regions[region_index].capacity;
         i++) {
      // if(IsInCube(space,input[i])){
      if (index != treeData[i])
        collision(uni, input, output, index, treeData[i]);
      // particules_queue[particules_counter] = treeData[i];
      particules_counter++;
      //}
    }

    if (regions[region_index].subdivided) {

      if (regions[region_index].child_s_nw != -1) {
        if (Intersect(regions[regions[region_index].child_s_nw].region,
                      space)) {
          region_queue[region_counter] = regions[region_index].child_s_nw;
          region_counter++;
        }
      }
      if (regions[region_index].child_s_ne != -1) {
        if (Intersect(regions[regions[region_index].child_s_ne].region,
                      space)) {
          region_queue[region_counter] = regions[region_index].child_s_ne;
          region_counter++;
        }
      }
      if (regions[region_index].child_s_se != -1) {
        if (Intersect(regions[regions[region_index].child_s_se].region,
                      space)) {
          region_queue[region_counter] = regions[region_index].child_s_se;
          region_counter++;
        }
      }
      if (regions[region_index].child_s_sw != -1) {
        if (Intersect(regions[regions[region_index].child_s_sw].region,
                      space)) {
          region_queue[region_counter] = regions[region_index].child_s_sw;
          region_counter++;
        }
      }

      if (regions[region_index].child_t_nw != -1) {
        if (Intersect(regions[regions[region_index].child_t_nw].region,
                      space)) {
          region_queue[region_counter] = regions[region_index].child_t_nw;
          region_counter++;
        }
      }
      if (regions[region_index].child_t_ne != -1) {
        if (Intersect(regions[regions[region_index].child_t_ne].region,
                      space)) {
          region_queue[region_counter] = regions[region_index].child_t_ne;
          region_counter++;
        }
      }
      if (regions[region_index].child_t_se != -1) {
        if (Intersect(regions[regions[region_index].child_t_se].region,
                      space)) {
          region_queue[region_counter] = regions[region_index].child_t_se;
          region_counter++;
        }
      }
      if (regions[region_index].child_t_sw != -1) {
        if (Intersect(regions[regions[region_index].child_t_sw].region,
                      space)) {
          region_queue[region_counter] = regions[region_index].child_t_sw;
          region_counter++;
        }
      }
    }

    region_cursor++;
  }

  return particules_counter;
}

__kernel void ComputeCollision(__global struct Univers *uni,
                               __global struct Particule_obj *input,
                               __global struct Particule_obj *output,
                               __global struct OctreeSettings *treeSettings,
                               __global struct Region *treeRegions,
                               __global int *treeData) {
  int total = get_global_size(0);
  int index = get_global_id(0);

  output[index] = input[index];

  struct Cube space;
  space.center = input[index].position;
  space.size = 100;

  int nbr_particule = getParticules(treeSettings, treeRegions, treeData, uni,
                                    input, output, space);

  struct Vector3 center;
  center.X = center.Y = center.Z = 0;
  struct Cube c;
  c.center = center;
  c.size = 5000;

  if (!IsInCube(c, output[index])) {
    output[index].position = V3Sub(output[index].position,
                                   V3fmul(output[index].velocity, uni[0].dt));

    output[index].velocity.X = -output[index].velocity.X;
    output[index].velocity.Y = -output[index].velocity.Y;
    output[index].velocity.Z = -output[index].velocity.Z;
  }
};