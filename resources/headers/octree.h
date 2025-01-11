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
    bool IsX = ((c1.center.X + c1.size) >= (c2.center.X - c2.size)) &&
               ((c1.center.X - c1.size) <= (c2.center.X + c2.size));
    bool IsY = ((c1.center.Y + c1.size) >= (c2.center.Y - c2.size)) &&
               ((c1.center.Y - c1.size) <= (c2.center.Y + c2.size));
    bool IsZ = ((c1.center.Z + c1.size) >= (c2.center.Z - c2.size)) &&
               ((c1.center.Z - c1.size) <= (c2.center.Z + c2.size));

    return IsX && IsY && IsZ;
}

bool IsInCube(struct Cube c, struct Particule_obj p)
{
    bool IsX = ((c.center.X + c.size) >= (p.position.X - p.radius)) &&
               ((c.center.X - c.size) <= (p.position.X + p.radius));
    bool IsY = ((c.center.Y + c.size) >= (p.position.Y - p.radius)) &&
               ((c.center.Y - c.size) <= (p.position.Y + p.radius));
    bool IsZ = ((c.center.Z + c.size) >= (p.position.Z - p.radius)) &&
               ((c.center.Z - c.size) <= (p.position.Z + p.radius));

    return IsX && IsY && IsZ;
}