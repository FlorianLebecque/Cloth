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

namespace PhysicObject.classes {
    public struct Light {
        
        public int enabled;
        int type;
        public Vector3 position;
        Vector3 target;
        public Raylib_cs.Color color;

        static int lightsCount = 0;
        int enabledLoc;
        int typeLoc;
        int posLoc;
        int targetLoc;
        int colorLoc;

        public static float[] f3position = new float[3];
        public static float[] f3target = new float[3];
        public static float[] f4color = new float[4];


        public Light(Vector3 position_,Raylib_cs.Color color_){
            enabled = 0;
            type = 1;
            position = position_;
            target = Vector3.One;
            color = color_;
            enabledLoc  = 0;
            typeLoc     = 0;
            posLoc      = 0;
            targetLoc   = 0;
            colorLoc    = 0;
        }

        public static Light CreateLight(Shader shader,Vector3 position_,Raylib_cs.Color color_){
            
            Light l = new Light(position_,color_);
            l.enabled = 1;

            string enabledName  = "lights[x].enabled\0";
            string typeName     = "lights[x].type\0";
            string posName      = "lights[x].position\0";
            string targetName   = "lights[x].target\0";
            string colorName    = "lights[x].color\0";

            enabledName = enabledName.Replace("x",lightsCount.ToString());
            typeName = typeName.Replace("x",lightsCount.ToString());
            posName = posName.Replace("x",lightsCount.ToString());
            targetName = targetName.Replace("x",lightsCount.ToString());
            colorName = colorName.Replace("x",lightsCount.ToString());

            l.enabledLoc = GetShaderLocation(shader, enabledName);
            l.typeLoc = GetShaderLocation(shader, typeName);
            l.posLoc = GetShaderLocation(shader, posName);
            l.targetLoc = GetShaderLocation(shader, targetName);
            l.colorLoc = GetShaderLocation(shader, colorName);

            lightsCount++;

            Light.UpdateLightValues(shader, l);

            return l;
        }

        public static void UpdateLightValues(Shader shader,Light l){
            RaylibUtils.Utils.SetShaderValue<int>(shader,l.enabledLoc,l.enabled,ShaderUniformDataType.SHADER_UNIFORM_INT);
            RaylibUtils.Utils.SetShaderValue<int>(shader,l.typeLoc,l.type,ShaderUniformDataType.SHADER_UNIFORM_INT);

            f3position[0] = l.position.X;
            f3position[1] = l.position.Y;
            f3position[2] = l.position.Z;
            RaylibUtils.Utils.SetShaderValue<float[]>(shader,l.posLoc,f3position,ShaderUniformDataType.SHADER_UNIFORM_VEC3);

            f3target[0] = l.target.X;
            f3target[1] = l.target.Y;
            f3target[2] = l.target.Z;
            RaylibUtils.Utils.SetShaderValue<float[]>(shader,l.targetLoc,f3target,ShaderUniformDataType.SHADER_UNIFORM_VEC3);

            f4color[0] = l.color.r / 255;
            f4color[1] = l.color.g / 255;
            f4color[2] = l.color.b / 255;
            f4color[3] = l.color.a / 255;
            RaylibUtils.Utils.SetShaderValue<float[]>(shader,l.colorLoc,f4color,ShaderUniformDataType.SHADER_UNIFORM_VEC4);
        }
    }
}