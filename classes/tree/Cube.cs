using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Raymath;
using static Raylib_cs.Color;
using static Raylib_cs.ConfigFlags;
using static Raylib_cs.CameraMode;
using static Raylib_cs.CameraProjection;
using static Raylib_cs.KeyboardKey;
using static Raylib_cs.ShaderLocationIndex;
using static Raylib_cs.MaterialMapIndex;

namespace PhysicObject.classes.tree {
    public struct Cube {

        public Vector3 center;
        public float size;

        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
        public Vector3 p4;
        public Vector3 p5;
        public Vector3 p6;
        public Vector3 p7;

        public Cube(Vector3 center,float size){

            this.size = size;
            this.center = center;

            Vector2 xy0 = new Vector2(center.X - size,center.Y - size);
            Vector2 xy1 = new Vector2(center.X + size,center.Y - size);
            Vector2 xy2 = new Vector2(center.X + size,center.Y + size);
            Vector2 xy3 = new Vector2(center.X - size,center.Y + size);

            float z_sub = center.Z - size;
            float z_top = center.Z + size;

            p0 = new Vector3(xy0,z_sub);
            p1 = new Vector3(xy1,z_sub);
            p2 = new Vector3(xy2,z_sub);
            p3 = new Vector3(xy3,z_sub);

            p4 = new Vector3(xy0,z_top);
            p5 = new Vector3(xy1,z_top);
            p6 = new Vector3(xy2,z_top);
            p7 = new Vector3(xy3,z_top);

        }

        public void recompute(){
            Vector2 xy0 = new Vector2(center.X - size,center.Y - size);
            Vector2 xy1 = new Vector2(center.X + size,center.Y - size);
            Vector2 xy2 = new Vector2(center.X + size,center.Y + size);
            Vector2 xy3 = new Vector2(center.X - size,center.Y + size);

            float z_sub = center.Z - size;
            float z_top = center.Z + size;

            p0 = new Vector3(xy0,z_sub);
            p1 = new Vector3(xy1,z_sub);
            p2 = new Vector3(xy2,z_sub);
            p3 = new Vector3(xy3,z_sub);

            p4 = new Vector3(xy0,z_top);
            p5 = new Vector3(xy1,z_top);
            p6 = new Vector3(xy2,z_top);
            p7 = new Vector3(xy3,z_top);
        }

        public bool Inside(Vector3 p){

            bool isX = ((p.X >= (center.X-size))&&(p.X <= (center.X + size)));
            bool isY = ((p.Y >= (center.Y-size))&&(p.Y <= (center.Y + size)));
            bool isZ = ((p.Z >= (center.Z-size))&&(p.Z <= (center.Z + size)));

            return isX && isY && isZ;
        }

        public void Draw(Color cl){
                

                DrawLine3D(p0,p1,cl);
                DrawLine3D(p1,p2,cl);
                DrawLine3D(p2,p3,cl);
                DrawLine3D(p3,p0,cl);

                DrawLine3D(p0,p4,cl);
                DrawLine3D(p1,p5,cl);
                DrawLine3D(p2,p6,cl);
                DrawLine3D(p3,p7,cl);

                DrawLine3D(p4,p5,cl);
                DrawLine3D(p5,p6,cl);
                DrawLine3D(p6,p7,cl);
                DrawLine3D(p7,p4,cl);
        }

    }
}