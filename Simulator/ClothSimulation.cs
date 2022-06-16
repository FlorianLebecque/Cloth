using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ClothSimulator;

using OPENCL;
using OpenTK.Compute.OpenCL;



namespace Simulator {

    public class ClothSimulation {
        GPU computeGPU;

        CLBuffer bSprings;
        CLBuffer bSpringsForce;
        CLBuffer bClothSettings;

        CLKernel kComputeSpringForce;
        CLKernel kComputeSpring;

        ICloth cloth;
        int springCount;
        int particuleCount;
        public ClothSimulation(GPU cGPU,ICloth c,CLBuffer readParticuleBuffer){
            computeGPU = cGPU;

            bSprings       = computeGPU.CreateBuffer<Spring>(MemoryFlags.ReadWrite,c.springs.ToArray());
            bSpringsForce  = computeGPU.CreateBuffer<Spring_force>(MemoryFlags.ReadWrite,c.spring_forces);
            bClothSettings = computeGPU.CreateBuffer<Cloth_settings>(MemoryFlags.ReadWrite,new Cloth_settings[1]{c.settings});

            computeGPU.Upload<Spring>(bSprings,c.springs);
            computeGPU.Upload<Cloth_settings>(bClothSettings,new Cloth_settings[1]{c.settings});

            kComputeSpringForce    = computeGPU.CreateKernel("resources/kernels/kComputeSpringForce.cl","ComputeSpringForce");
            kComputeSpring         = computeGPU.CreateKernel("resources/kernels/kComputeSpring.cl","ComputeSpring");

            computeGPU.SetKernelArg(kComputeSpringForce,0,readParticuleBuffer);
            computeGPU.SetKernelArg(kComputeSpringForce,1,bSprings);
            computeGPU.SetKernelArg(kComputeSpringForce,2,bSpringsForce);
            
            computeGPU.SetKernelArg(kComputeSpring,0,readParticuleBuffer);
            computeGPU.SetKernelArg(kComputeSpring,1,bSpringsForce);
            computeGPU.SetKernelArg(kComputeSpring,2,bClothSettings);

            springCount = c.springs.Count();
            particuleCount = c.settings.count;
            cloth = c;
        }

        public void Simulate(){

            computeGPU.Execute(kComputeSpringForce,1,springCount);
            computeGPU.Execute(kComputeSpring,1,particuleCount);
        }

        public void Update(){
            computeGPU.Download<Spring>(bSprings,cloth.springs);
        }

        public Spring[] GetSprings(){
            return cloth.springs;
        }
        public void Clear(){
            computeGPU.ClearBuffer(bSprings);
            computeGPU.ClearBuffer(bSpringsForce);
            computeGPU.ClearBuffer(bClothSettings);


            //CL.ReleaseKernel(kComputeSpringForce);
            //CL.ReleaseKernel(kComputeSpring);

        }

    }
}   