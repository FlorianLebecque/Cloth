using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Raylib_cs.Raylib;
using static Raylib_cs.ShaderLocationIndex;

using RaylibUtils;


namespace Gravity.utils {
    class PlanetModelGenerator {

        public static Shader sh;
        public static bool ShaderInit = false;
        public static unsafe Tuple<Model,Shader> getSphere(float radius) {

            Model md = LoadModelFromMesh(GenMeshSphere(radius, 32, 32));

            if(!ShaderInit){
                sh = LoadShader("resources\\shaders\\base.vs", "resources\\shaders\\base.fs");
                
                
                Utils.SetShaderLocation(ref sh,SHADER_LOC_MATRIX_MODEL,"matModel");
                
                ShaderInit = true;
            }

            Utils.SetMaterialShader(ref md, 0, ref sh);
            Utils.MeshTangents(ref md);
            

            return Tuple.Create(md,sh);
        }

    }
}
