using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Cloth.classes.windows {
    public class SimulationWindows : GameWindow {

        private int vertexBufferHandle;
        private int shaderProgramHandle;

        private int vertexArrayHandle;

        public SimulationWindows(int width,int height,string title)
            : base(GameWindowSettings.Default,NativeWindowSettings.Default){

            this.CenterWindow(new Vector2i(width,height));
            this.Title = title;
        }

        protected override void OnLoad() {
            GL.ClearColor(Color4.AliceBlue);

            float[] vertices = new float[]{
                0.0f  ,  0.5f , 0.0f,
                0.5f  , -0.5f , 0.0f,
               -0.5f  , -0.5f , 0.0f
            };

            this.vertexBufferHandle = GL.GenBuffer();                           //create a buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer,this.vertexBufferHandle);    //bind the buffer -> tells OpenGL we work on it
            GL.BufferData(BufferTarget.ArrayBuffer,vertices.Length * sizeof(float),vertices,BufferUsageHint.StaticDraw);    // Upload data to GPU
            GL.BindBuffer(BufferTarget.ArrayBuffer,0);  // stop working on a buffer


            this.vertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(this.vertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer,this.vertexBufferHandle);
            GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,3 * sizeof(float),0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);


            string vertexShaderCode = File.ReadAllText("resources/shaders/base.vs");
            string FragmentShaderCode = File.ReadAllText("resources/shaders/base.fs");

                //create a vertex shader
            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle,vertexShaderCode);
            GL.CompileShader(vertexBufferHandle);

                //create fragment shader
            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle,FragmentShaderCode);
            GL.CompileShader(fragmentShaderHandle);

                //create the shader program
            this.shaderProgramHandle = GL.CreateProgram();
            GL.AttachShader(shaderProgramHandle,vertexBufferHandle);
            GL.AttachShader(shaderProgramHandle,fragmentShaderHandle);

            GL.LinkProgram(this.shaderProgramHandle);

                //clear the ressources
            GL.DetachShader(shaderProgramHandle,vertexBufferHandle);
            GL.DetachShader(shaderProgramHandle,fragmentShaderHandle);
            GL.DeleteShader(vertexBufferHandle);
            GL.DeleteShader(fragmentShaderHandle);


            base.OnLoad();

        }

        protected override void OnUnload() {
            GL.BindBuffer(BufferTarget.ArrayBuffer,0);
            GL.DeleteBuffer(this.vertexBufferHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(this.shaderProgramHandle);

            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e) {
            GL.Viewport(0,0,e.Width,e.Height);
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args) {

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(this.shaderProgramHandle);

            GL.BindVertexArray(this.vertexArrayHandle);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            this.Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

    }
}