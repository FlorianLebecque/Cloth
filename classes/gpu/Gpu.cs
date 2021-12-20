using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Cloth.classes {
    public class GPU {
        
        Event event0;
        ErrorCode err;
        Device Gpu_1;
        Context Gpu_context;
        CommandQueue commandQueue;
        

        public GPU(){
            err = new ErrorCode();

            Gpu_1 = GetGpu();
            Gpu_context = GetContext(Gpu_1);
            commandQueue = InitCommandQueue(Gpu_context,Gpu_1);
        }

        private Device GetGpu(){
            ErrorCode errorCode;
            Platform[] platforms = Cl.GetPlatformIDs(out errorCode);
            var Lst_Devices = new List<Device>();

            foreach (Platform platform in platforms)
            {
                String Device_Name = Cl.GetPlatformInfo(platform, PlatformInfo.Name, out errorCode).ToString();

                //GUI : List box du formulaire au besoin
                //Lsb_Devices_Names.Items.Add(Device_Name);

                //GPU uniquement 
                foreach (Device device in Cl.GetDeviceIDs(platform, DeviceType.Gpu, out errorCode))
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

        private Context GetContext(Device Gpu_1){
            OpenCL.Net.ErrorCode errorCode;

            Context Gpu_context = Cl.CreateContext(null, 1, new Device[] { Gpu_1 }, null, IntPtr.Zero, out errorCode);
            if (errorCode != OpenCL.Net.ErrorCode.Success) {
                throw new Exception("Impossible de créer le contexte");
            }

            return Gpu_context;
        }

        private CommandQueue InitCommandQueue(Context Gpu_context,Device Gpu_1){
            OpenCL.Net.ErrorCode errorCode;
            CommandQueue commandQueue = Cl.CreateCommandQueue(Gpu_context, Gpu_1, CommandQueueProperties.OutOfOrderExecModeEnable, out errorCode);
            if (errorCode != OpenCL.Net.ErrorCode.Success) {
                throw new Exception("Impossible de créer la liste de traitement");
            }

            return commandQueue;
        }
    
        public IMem CreateBuffer(MemFlags mFlag, IntPtr size){
            OpenCL.Net.ErrorCode err = new OpenCL.Net.ErrorCode();

            IMem temp = Cl.CreateBuffer(Gpu_context, mFlag, size, out err);

            if (err != ErrorCode.Success) {
                throw new Exception(String.Format("Unable to load memory: {0}", err.ToString()));                
            }

            return temp;
        }
    
        private OpenCL.Net.Program CreateProgram(String path){
            return GetProgram(path,Gpu_context,Gpu_1);
        }

        private Program GetProgram(String path,Context Gpu_context,Device Gpu_1){
            String Program_Source_Code = File.ReadAllText(path);

            OpenCL.Net.ErrorCode err;

            //Création du programme 
            OpenCL.Net.Program program = Cl.CreateProgramWithSource(Gpu_context, 1, new[] { Program_Source_Code }, new[] {(IntPtr)Program_Source_Code.Length}, out err);
            
            Cl.BuildProgram(program, 1, new[] {Gpu_1}, "-cl-std=CL1.2", null, IntPtr.Zero);

            //Récupérations des informations du build
            if (Cl.GetProgramBuildInfo(program, Gpu_1, ProgramBuildInfo.Status, out err).CastTo<BuildStatus>() != BuildStatus.Success) {
                //Affichage si erreurs durant la compilation 
                if (err != OpenCL.Net.ErrorCode.Success) {
                    String Output = "";
                    Output += String.Format("ERROR: " + "Cl.GetProgramBuildInfo" + " (" + err.ToString() + ")");
                    Output += String.Format("Cl.GetProgramBuildInfo != Success");
                    Output += Cl.GetProgramBuildInfo(program, Gpu_1, ProgramBuildInfo.Log, out err);
                }
            }

            return program;
        }

        public Kernel CreateKernel(String path,string name){
            OpenCL.Net.ErrorCode err = new OpenCL.Net.ErrorCode();

            Kernel kernel = Cl.CreateKernel(CreateProgram(path),name,out err);
            if (err != ErrorCode.Success) {
                throw new Exception(err.ToString());
            }

            return kernel;
        }

        public void Upload(IMem buffer,IntPtr size,object data){
            Cl.EnqueueWriteBuffer(
                commandQueue,
                buffer,
                Bool.True,
                IntPtr.Zero,
                size,
                data,
                0,
                null,
                out event0
            );

        }

        public void SetKernelArg(Kernel kernel,uint index,IMem buffer){
            Cl.SetKernelArg(kernel, index, (IntPtr)Marshal.SizeOf(typeof(IntPtr)), buffer);
        }

        public void Execute(Kernel kernel,uint dim,int count){
            IntPtr[] global = new IntPtr[1];
            global[0] = new IntPtr(count);
            
            IntPtr[] local = new IntPtr[1];
            int local_work = count;
            if(local_work == 0){
                local_work = 1;
            }
            local[0] = new IntPtr(local_work);
            

            err = Cl.EnqueueNDRangeKernel(
                commandQueue,
                kernel,
                dim,
                null,
                global,
                local,
                0,
                null,
                out Event _
            );

            if (err != ErrorCode.Success) {
                throw new Exception(err.ToString());
            }

            Cl.Finish(commandQueue);


        }

        public void Download(IMem buffer,IntPtr size,object ouput){
            Cl.EnqueueReadBuffer(commandQueue, (IMem)buffer, Bool.True, IntPtr.Zero, size, ouput, 0, null, out event0);
        }
        
        public void End(){

        }

    }
}