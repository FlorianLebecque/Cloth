using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenTK.Compute.OpenCL;


namespace OPENCL {
    public class GPU {
        
        CLEvent event0;
        CLDevice Gpu_1;
        CLContext Gpu_context;
        CLCommandQueue commandQueue;
        

        public GPU(){
            Gpu_1 = GetGpu();
            Gpu_context = GetContext(Gpu_1);
            commandQueue = InitCommandQueue(Gpu_context,Gpu_1);
        }

        private CLDevice GetGpu(){
            //ErrorCode errorCode;
            CLPlatform[] platforms;
            CL.GetPlatformIds(out platforms);
            var Lst_Devices = new List<CLDevice>();

            foreach (CLPlatform platform in platforms) {
                CLDevice[] list;
                CL.GetDeviceIds(platform, DeviceType.Gpu,out list);
                //GPU uniquement 
                foreach (CLDevice device in list)
                {
                    Lst_Devices.Add(device);
                }
            }

            //Si aucun equipement 
            if (Lst_Devices.Count <= 0) {
                throw new Exception("Aucun GPU CUDA");
            }

            return Lst_Devices[0];
        }

        private CLContext GetContext(CLDevice Gpu_1){
            CLResultCode errorCode;
            CLContext Gpu_context = CL.CreateContext((IntPtr)null, 1, new CLDevice[] { Gpu_1 }, (IntPtr)null, IntPtr.Zero, out errorCode);
            if (errorCode != CLResultCode.Success) {
                throw new Exception("Impossible de créer le contexte");
            }

            return Gpu_context;
        }

        private CLCommandQueue InitCommandQueue(CLContext Gpu_context,CLDevice Gpu_1){
            CLResultCode errorCode;
            CLCommandQueue commandQueue = CL.CreateCommandQueue(Gpu_context, Gpu_1, (CommandQueueProperty)0, out errorCode);
            if (errorCode != CLResultCode.Success) {
                throw new Exception("Impossible de créer la liste de traitement");
            }

            return commandQueue;
        }
    
        public CLBuffer CreateBuffer<T>(MemoryFlags mFlag, T[] array) where T:unmanaged {
            CLResultCode err = new CLResultCode();

            int size_t = Marshal.SizeOf(typeof(T));
            int sizes = Marshal.SizeOf(typeof(T))*array.Count();
            UIntPtr size = new UIntPtr((uint)sizes);


            CLBuffer t = CL.CreateBuffer(Gpu_context,MemoryFlags.ReadOnly,size,(IntPtr)null,out err);
            //CLBuffer t = CL.CreateBuffer<T>(Gpu_context,mFlag,array,out err);

            if (err != CLResultCode.Success) {
                throw new Exception(String.Format("Unable to load memory: {0}", err.ToString()));                
            }

            return  t;
        }
    
        private CLProgram CreateProgram(String path){
            return GetProgram(path,Gpu_context,Gpu_1);
        }

        private CLProgram GetProgram(String path,CLContext Gpu_context,CLDevice Gpu_1){
            String Program_Source_Code = File.ReadAllText(path);

            CLResultCode err;

            //Création du programme 
            CLProgram program = CL.CreateProgramWithSource(Gpu_context,Program_Source_Code,out err);
            
            CL.BuildProgram(program, 1, new[] {Gpu_1}, null, (IntPtr)null, (IntPtr)null);

            byte[] result;
            err = CL.GetProgramBuildInfo(program,Gpu_1,ProgramBuildInfo.Status,out result);

            //Récupérations des informations du build
            if (result is not null) {
                //Affichage si erreurs durant la compilation 
                if (err != CLResultCode.Success) {
                    String Output = "";
                    Output += String.Format("ERROR: " + "CL.GetProgramBuildInfo" + " (" + err.ToString() + ")");
                    Output += String.Format("CL.GetProgramBuildInfo != Success");
                }
            }

            return program;
        }

        public CLKernel CreateKernel(String path,string name){
            CLResultCode err = new CLResultCode();

            CLKernel kernel = CL.CreateKernel(CreateProgram(path),name,out err);
            if (err != CLResultCode.Success) {
                throw new Exception(err.ToString());
            }

            return kernel;
        }

        public unsafe void Upload<T>(CLBuffer buffer,T[] data) where T:unmanaged{

            int sizes = Marshal.SizeOf(typeof(T))*data.Count();
            UIntPtr size = new UIntPtr((uint)sizes);

            //CL.EnqueueWriteBuffer(commandQueue,buffer,true,UIntPtr.Zero,size,new IntPtr(&data),null,out event0);

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

        public unsafe void SetKernelArg(CLKernel kernel,uint index,CLBuffer buffer){
            
            //MemoryObjectType.Buffer; //4336
            //MemoryObjectInfo.Size; //4354

            byte[] result;
            CL.GetMemObjectInfo(buffer,MemoryObjectInfo.Size,out result);
            UInt32 size = BitConverter.ToUInt32(result);
            UInt32 size2 = (UInt32)Marshal.SizeOf(typeof(CLBuffer));
            UInt32 size3 = (UInt32)Marshal.SizeOf(typeof(IntPtr));

            CLResultCode err = CL.SetKernelArg(kernel, index,(UIntPtr)Marshal.SizeOf(typeof(CLBuffer)), new IntPtr(&buffer));

            if (err != CLResultCode.Success) {
                throw new Exception(err.ToString());
            }
        }

        private int GetMAXGROUPESIZE(){
            byte[] MAX_WORK;
            CL.GetDeviceInfo(Gpu_1,DeviceInfo.MaximumWorkGroupSize,out MAX_WORK);
            return BitConverter.ToInt32(MAX_WORK);
        }
        public unsafe void Execute(CLKernel kernel,uint dim,int count){
            UIntPtr[] global = new UIntPtr[dim];
            global[0] = new UIntPtr((uint)count);

            UIntPtr[] local = new UIntPtr[1];
            int local_work = count;

            local[0] = new UIntPtr((uint)local_work);            
            
            CLResultCode err;
            err = CL.EnqueueNDRangeKernel(
                commandQueue,
                kernel,
                dim,
                null,
                global,
                null,
                0,
                null,
                out event0
            );

            if (err != CLResultCode.Success) {
                throw new Exception(err.ToString());
            }

            CL.Finish(commandQueue);
        }

        public unsafe void Download<T>(CLBuffer buffer,T[] output) where T:unmanaged{
            
            fixed (T* pointer = output)
                CL.EnqueueReadBuffer(commandQueue, buffer, true, UIntPtr.Zero, (UIntPtr) (ulong) (output.Length * Marshal.SizeOf<T>()), (IntPtr) (void*) pointer, 0U, null, out _);
            
        }
        
        public void End(){

        }

    }
}