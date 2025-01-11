using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenTK.Compute.OpenCL;


namespace OPENCL
{
    public class GPU
    {

        CLEvent event0;
        CLDevice Gpu_1;
        CLContext Gpu_context;
        CLCommandQueue commandQueue;


        public GPU()
        {
            Gpu_1 = GetGpu();
            Gpu_context = GetContext(Gpu_1);
            commandQueue = InitCommandQueue(Gpu_context, Gpu_1);
        }

        private CLDevice GetGpu()
        {
            //ErrorCode errorCode;
            CLPlatform[] platforms;
            CL.GetPlatformIds(out platforms);
            var Lst_Devices = new List<CLDevice>();

            foreach (CLPlatform platform in platforms)
            {
                CLDevice[] list;
                CL.GetDeviceIds(platform, DeviceType.Gpu, out list);
                //GPU uniquement 
                foreach (CLDevice device in list)
                {
                    Lst_Devices.Add(device);
                }
            }

            //Si aucun equipement 
            if (Lst_Devices.Count <= 0)
            {
                throw new Exception("Aucun GPU CUDA");
            }

            return Lst_Devices[0];
        }

        private CLContext GetContext(CLDevice Gpu_1)
        {
            CLResultCode errorCode;
            CLContext Gpu_context = CL.CreateContext((IntPtr)null, 1, new CLDevice[] { Gpu_1 }, (IntPtr)null, IntPtr.Zero, out errorCode);
            if (errorCode != CLResultCode.Success)
            {
                throw new Exception("Impossible de créer le contexte");
            }

            return Gpu_context;
        }

        private unsafe CLCommandQueue InitCommandQueue(CLContext Gpu_context, CLDevice Gpu_1)
        {
            CLResultCode errorCode;
            CommandQueueProperty cqp = new CommandQueueProperty();

            CLCommandQueue commandQueue = CL.CreateCommandQueueWithProperties(Gpu_context, Gpu_1, new IntPtr(&cqp), out errorCode);
            if (errorCode != CLResultCode.Success)
            {
                throw new Exception("Impossible de créer la liste de traitement");
            }

            return commandQueue;
        }

        public CLBuffer CreateBuffer<T>(MemoryFlags mFlag, T[] array) where T : unmanaged
        {
            CLResultCode err = new CLResultCode();

            int size_t = Marshal.SizeOf(typeof(T));
            int sizes = Marshal.SizeOf(typeof(T)) * array.Count();
            UIntPtr size = new UIntPtr((uint)sizes);


            CLBuffer t = CL.CreateBuffer(Gpu_context, MemoryFlags.ReadOnly, size, (IntPtr)null, out err);
            //CLBuffer t = CL.CreateBuffer<T>(Gpu_context,mFlag,array,out err);

            if (err != CLResultCode.Success)
            {
                throw new Exception(String.Format("Unable to load memory: {0}", err.ToString()));
            }

            return t;
        }

        private CLProgram CreateProgram(String path)
        {
            return GetProgram(path, Gpu_context, Gpu_1);
        }

        private CLProgram GetProgram(String path, CLContext Gpu_context, CLDevice Gpu_1)
        {
            String Program_Source_Code = File.ReadAllText(path);
            string kernelDirectory = Path.GetDirectoryName(Path.GetFullPath(path));

            // Parse include paths from source and make them relative to kernel path
            var includeDirectives = Program_Source_Code
                .Split('\n')
                .Where(line => line.Trim().StartsWith("#include"))
                .Select(line => Path.Combine(
                    kernelDirectory,
                    Path.GetDirectoryName(line.Split('"')[1].Trim())
                ))
                .Distinct()
                .ToList();

            CLResultCode err;
            CLProgram program = CL.CreateProgramWithSource(Gpu_context, Program_Source_Code, out err);

            if (err != CLResultCode.Success)
            {
                throw new Exception($"Failed to create program: {err}");
            }

            // Build program with include paths
            string buildOptions = string.Join(" ", includeDirectives.Select(dir => $"-I \"{dir}\""));
            err = CL.BuildProgram(program, 1, new[] { Gpu_1 }, buildOptions, IntPtr.Zero, IntPtr.Zero);

            if (err != CLResultCode.Success)
            {
                // Get build log
                UIntPtr logSize;
                CL.GetProgramBuildInfo(program, Gpu_1, ProgramBuildInfo.Log, UIntPtr.Zero, null, out logSize);
                byte[] buildLog = new byte[logSize.ToUInt32()];
                UIntPtr returnSize;
                CL.GetProgramBuildInfo(program, Gpu_1, ProgramBuildInfo.Log, new UIntPtr((uint)buildLog.Length), buildLog, out returnSize);
                string buildLogStr = System.Text.Encoding.ASCII.GetString(buildLog);
                throw new Exception($"Build failed: {err}\nBuild Log:\n{buildLogStr}");
            }

            // Verify build status
            byte[] result;
            err = CL.GetProgramBuildInfo(program, Gpu_1, ProgramBuildInfo.Status, out result);
            if (err != CLResultCode.Success)
            {
                throw new Exception($"Failed to get build info: {err}");
            }

            return program;
        }

        public CLKernel CreateKernel(String path, string name)
        {
            CLResultCode err = new CLResultCode();

            CLKernel kernel = CL.CreateKernel(CreateProgram(path), name, out err);
            if (err != CLResultCode.Success)
            {
                throw new Exception(err.ToString());
            }

            return kernel;
        }

        public unsafe void Upload<T>(CLBuffer buffer, T[] data) where T : unmanaged
        {

            CL.EnqueueWriteBuffer<T>(
                commandQueue,
                buffer,
                true,
                UIntPtr.Zero,
                data,
                null,
                out event0
            );

        }

        public unsafe void SetKernelArg(CLKernel kernel, uint index, CLBuffer buffer)
        {

            CLResultCode err = CL.SetKernelArg(kernel, index, (UIntPtr)Marshal.SizeOf(typeof(CLBuffer)), new IntPtr(&buffer));

            if (err != CLResultCode.Success)
            {
                throw new Exception(err.ToString());
            }
        }

        private int GetMAXGROUPESIZE()
        {
            byte[] MAX_WORK;
            CL.GetDeviceInfo(Gpu_1, DeviceInfo.MaximumWorkGroupSize, out MAX_WORK);
            return BitConverter.ToInt32(MAX_WORK);
        }
        public unsafe void Execute(CLKernel kernel, uint dim, int count)
        {
            UIntPtr[] global = new UIntPtr[dim];
            global[0] = new UIntPtr((uint)count);

            CLResultCode err;
            err = CL.EnqueueNDRangeKernel(
                commandQueue,
                kernel,
                dim,
                null,
                global, // total number of execution
                null,   // can be null -> let the gpu decide
                0,
                null,
                out event0
            );

            if (err != CLResultCode.Success)
            {
                throw new Exception(err.ToString());
            }

            CL.Finish(commandQueue);
        }

        public unsafe void Download<T>(CLBuffer buffer, T[] output) where T : unmanaged
        {

            fixed (T* pointer = output)
                CL.EnqueueReadBuffer(commandQueue, buffer, true, UIntPtr.Zero, (UIntPtr)(ulong)(output.Length * Marshal.SizeOf<T>()), (IntPtr)(void*)pointer, 0U, null, out _);

        }

        public void Flush()
        {
            CL.Flush(commandQueue);
        }
        public void Clear()
        {
            CL.ReleaseCommandQueue(commandQueue);
            CL.ReleaseContext(Gpu_context);
            CL.ReleaseDevice(Gpu_1);
        }

        public void ClearBuffer(CLBuffer b)
        {
            CL.EnqueueUnmapMemoryObject(commandQueue, b, IntPtr.Zero, 0, null, out event0);
            CL.ReleaseMemoryObject(b);

        }

    }
}