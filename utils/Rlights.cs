using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.ShaderUniformDataType;

namespace RaylibUtils {
    public struct Light {
        public bool enabled;
        public LightType type;
        public Vector3 position;
        public Vector3 target;
        public Color color;

        public int enabledLoc;
        public int typeLoc;
        public int posLoc;
        public int targetLoc;
        public int colorLoc;
    }

    public enum LightType {
        LIGHT_DIRECTIONAL,
        LIGHT_POINT
    }

    public static class Rlights {
        public static Light CreateLight(int lightsCount, LightType type, Vector3 pos, Vector3 targ, Color color, Shader shader) {
            Light light = new Light();

            light.enabled = true;
            light.type = type;
            light.position = pos;
            light.target = targ;
            light.color = color;

            string enabledName = "lights[" + lightsCount + "].enabled";
            string typeName = "lights[" + lightsCount + "].type";
            string posName = "lights[" + lightsCount + "].position";
            string targetName = "lights[" + lightsCount + "].target";
            string colorName = "lights[" + lightsCount + "].color";

            light.enabledLoc = GetShaderLocation(shader, enabledName);
            light.typeLoc = GetShaderLocation(shader, typeName);
            light.posLoc = GetShaderLocation(shader, posName);
            light.targetLoc = GetShaderLocation(shader, targetName);
            light.colorLoc = GetShaderLocation(shader, colorName);

            UpdateLightValues(shader, light);

            return light;
        }

        public static void UpdateLightValues(Shader shader, Light light) {
            // Send to shader light enabled state and type
            Utils.SetShaderValue(shader, light.enabledLoc, light.enabled ? 1 : 0, SHADER_UNIFORM_INT);
            Utils.SetShaderValue(shader, light.typeLoc, (int)light.type, SHADER_UNIFORM_INT);

            // Send to shader light target position values
            float[] position = new[] { light.position.X, light.position.Y, light.position.Z };
            Utils.SetShaderValue(shader, light.posLoc, position, SHADER_UNIFORM_VEC3);

            // Send to shader light target position values
            float[] target = { light.target.X, light.target.Y, light.target.Z };
            Utils.SetShaderValue(shader, light.targetLoc, target, SHADER_UNIFORM_VEC3);

            // Send to shader light color values
            float[] color = new[] { (float)light.color.r / (float)255, (float)light.color.g / (float)255, (float)light.color.b / (float)255, (float)light.color.a / (float)255 };
            Utils.SetShaderValue(shader, light.colorLoc, color, SHADER_UNIFORM_VEC4);
        }
    }
}