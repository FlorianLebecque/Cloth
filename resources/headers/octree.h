#pragma once

#include "structs.h"

struct OctreeSettings
{
    int particules_count;
    int regions_count;
};

struct Cube
{
    struct Vector3 center;
    float size;
    struct Vector3 p0;
    struct Vector3 p1;
    struct Vector3 p2;
    struct Vector3 p3;
    struct Vector3 p4;
    struct Vector3 p5;
    struct Vector3 p6;
    struct Vector3 p7;
};

struct Region
{

    int capacity;
    int count;
    bool subdivided;
    int offset;
    struct Cube region;
    int child_s_nw;
    int child_s_ne;
    int child_s_se;
    int child_s_sw;
    int child_t_nw;
    int child_t_ne;
    int child_t_se;
    int child_t_sw;
};

bool Intersect(struct Cube c1, struct Cube c2)
{
    float total_size = c1.size + c2.size;
    return (fabs(c1.center.X - c2.center.X) <= total_size) &&
           (fabs(c1.center.Y - c2.center.Y) <= total_size) &&
           (fabs(c1.center.Z - c2.center.Z) <= total_size);
}

bool IsInCube(struct Cube c, struct Particule_obj p)
{
    float size_with_radius = c.size + p.radius;
    return (fabs(c.center.X - p.position.X) <= size_with_radius) &&
           (fabs(c.center.Y - p.position.Y) <= size_with_radius) &&
           (fabs(c.center.Z - p.position.Z) <= size_with_radius);
}