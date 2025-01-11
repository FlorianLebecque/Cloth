#pragma once

#include "structs.h"

/*
    This header file contains the functions for the Vector3 struct. used in OpenCL
*/

float V3LengthSquared(struct Vector3 V1)
{
    return (V1.X * V1.X) + (V1.Y * V1.Y) + (V1.Z * V1.Z);
}

float V3Length(struct Vector3 V1)
{
    return sqrt(V3LengthSquared(V1));
}

struct Vector3 V3Sub(struct Vector3 V1, struct Vector3 V2)
{
    struct Vector3 resutl;

    resutl.X = V1.X - V2.X;
    resutl.Y = V1.Y - V2.Y;
    resutl.Z = V1.Z - V2.Z;

    return resutl;
}

struct Vector3 V3Add(struct Vector3 V1, struct Vector3 V2)
{
    struct Vector3 resutl;

    resutl.X = V1.X + V2.X;
    resutl.Y = V1.Y + V2.Y;
    resutl.Z = V1.Z + V2.Z;

    return resutl;
}

struct Vector3 V3fmul(struct Vector3 V1, float v)
{
    struct Vector3 resutl;

    resutl.X = V1.X * v;
    resutl.Y = V1.Y * v;
    resutl.Z = V1.Z * v;

    return resutl;
}

struct Vector3 V3fdiv(struct Vector3 V1, float v)
{
    struct Vector3 resutl;

    if (v != 0)
    {
        resutl.X = V1.X / v;
        resutl.Y = V1.Y / v;
        resutl.Z = V1.Z / v;
    }

    return resutl;
}

float V3Distance(struct Vector3 V1, struct Vector3 V2)
{
    return V3Length(V3Sub(V1, V2));
}

float V3Dot(struct Vector3 V1, struct Vector3 V2)
{
    return (V1.X * V2.X) + (V1.Y * V2.Y) + (V1.Z * V2.Z);
}

struct Vector3 V3Normalize(struct Vector3 V1)
{
    struct Vector3 resutl;
    float l = V3Length(V1);

    if (l != 0)
    {
        resutl.X = V1.X / l;
        resutl.Y = V1.Y / l;
        resutl.Z = V1.Z / l;
    }

    return resutl;
}