#pragma once

struct Vector3
{
    float X;
    float Y;
    float Z;
};

struct Univers
{
    float G;
    float dt;
};

struct Particule_obj
{
    struct Vector3 position;
    struct Vector3 velocity;
    struct Vector3 acceleration;
    float mass;
    float bounciness;
    float radius;
    float roughness;
};

struct Cloth_settings
{
    int offset;
    int count;
    int nbr_spring;
};

struct Spring
{
    float rest_distance;
    float max_distance;
    float k;
    float cd;

    int particul_1;
    int particul_2;

    int broken;
};

struct Spring_force
{
    int p1;
    int p2;
    struct Vector3 force;
};
