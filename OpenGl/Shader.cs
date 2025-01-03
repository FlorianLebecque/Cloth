using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace OPENGL
{
    public class Shader
    {
        int handle;

        public Shader(string vertexPath, string fragmentPath)
        {

            /*

                Load the source code of the vertex and fragment shaders from files

            */
            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            /*  
                
                Create the shaders

            */
            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            /*

                Compile the shaders

            */

            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int successFragment);
            if (successFragment == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            /*

                Link the shaders

            */

            handle = GL.CreateProgram();

            GL.AttachShader(handle, VertexShader);
            GL.AttachShader(handle, FragmentShader);

            GL.LinkProgram(handle);

            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int successProgram);
            if (successProgram == 0)
            {
                string infoLog = GL.GetProgramInfoLog(handle);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(handle, VertexShader);
            GL.DetachShader(handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            if (disposedValue == false)
            {
                Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}