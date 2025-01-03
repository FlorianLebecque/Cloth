using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OPENGL;
using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace ClothSimulator
{
    public class SimulatorWindows : GameWindow
    {

        Stopwatch stopwatch = new Stopwatch();

        int VertexArrayObject;
        Shader shader;
        float[] sphereVertices;
        int[] sphereIndices;
        float[] positions;
        public SimulatorWindows(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {

        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            shader = new Shader("resources/shaders/base.vert", "resources/shaders/base.frag");

            // Naive OBJ parser for sphere.obj
            List<float> vertexList = new List<float>();
            List<int> indexList = new List<int>();

            string[] lines = File.ReadAllLines("resources/models/sphere.obj");
            foreach (string line in lines)
            {
                if (line.StartsWith("v "))
                {
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    vertexList.Add(float.Parse(parts[1], CultureInfo.InvariantCulture));
                    vertexList.Add(float.Parse(parts[2], CultureInfo.InvariantCulture));
                    vertexList.Add(float.Parse(parts[3], CultureInfo.InvariantCulture));
                }
                else if (line.StartsWith("f "))
                {
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < parts.Length; i++)
                    {
                        // Strip possible vertex/texcoord/normal references
                        string[] subParts = parts[i].Split('/');
                        indexList.Add(int.Parse(subParts[0]) - 1);
                    }
                }
            }

            sphereVertices = vertexList.ToArray();
            sphereIndices = indexList.ToArray();

            // Setup VAO -> VAO Store information about how to interpret vertex data
            int VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            // VBO -> VBO Store vertex data
            int VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sphereVertices.Length * sizeof(float), sphereVertices, BufferUsageHint.StaticDraw);
            // this describes how the data is stored in the buffer, it is stored in the VAO
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            // enable the vertex attribute, at location 0 (it is disabled by default)
            GL.EnableVertexAttribArray(0);

            // EBO
            int EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sphereIndices.Length * sizeof(int), sphereIndices, BufferUsageHint.StaticDraw);

            // Create instance positions
            List<float> instancePositions = new List<float>();
            for (int x = -5; x <= 5; x++)
            {
                for (int y = -5; y <= 5; y++)
                {
                    instancePositions.Add(x * 0.2f);
                    instancePositions.Add(y * 0.2f);
                    instancePositions.Add(0.0f);
                }
            }
            positions = instancePositions.ToArray();

            // Instance positions buffer
            int instanceVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, instanceVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribDivisor(1, 1); // Tell OpenGL this is an instanced vertex attribute
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            stopwatch.Restart();
            base.OnRenderFrame(e);


            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();
            int instanceCount = positions.Length / 3;
            GL.DrawElementsInstanced(PrimitiveType.Triangles, sphereIndices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero, instanceCount);


            SwapBuffers();
            stopwatch.Stop();

            Title = $"Cloth Simulator - FPS: {1f / e.Time:00000} - Frame Time: {stopwatch.Elapsed.TotalMilliseconds:0.00}ms";
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}