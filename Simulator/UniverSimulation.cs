using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PhysicObject.classes.tree;
using OPENCL;
using OpenTK.Compute.OpenCL;

using Raylib_cs;

using static Raylib_cs.Color;
using static Raylib_cs.KeyboardKey;
using static Raylib_cs.Raylib;



namespace Simulator {
    public class UniverSimulation {
        
        public PhysicObject.classes.UniversSettings univers;

        public Particule[] output_enties ;

        ClothSimulation[] clothes_sim;
        public Octree UniversTree;
        static GPU computeGPU;

        CLBuffer bUniver;
        CLBuffer B1;
        CLBuffer B2;

        CLBuffer bOctree_data;
        CLBuffer bOctree_regions;
        CLBuffer bOctree_settings;

        static CLKernel kComputeGravity;
        static CLKernel kComputeVel;
        static CLKernel kComputePos;
        static CLKernel kComputeCollision ;

        OctreeSettings[] OCS;
        bool started = false;
        public UniverSimulation(PhysicObject.classes.UniversSettings u, List<Particule> entities,List<Cloth> Clothes){
            univers = u;

            OCS = new OctreeSettings[1];

            output_enties = new Particule[1];
            clothes_sim = new ClothSimulation[1];

            UniversTree = new Octree(10,10);

            if(computeGPU == null)
                computeGPU = new();

            if(kComputeGravity.Handle == IntPtr.Zero)
                kComputeGravity = computeGPU.CreateKernel("resources/kernels/kComputeGravity.cl","ComputeGravity");

            if(kComputeVel.Handle == IntPtr.Zero)
                kComputeVel = computeGPU.CreateKernel("resources/kernels/kComputeVel.cl","ComputeVel");

            if(kComputePos.Handle == IntPtr.Zero)
                kComputePos = computeGPU.CreateKernel("resources/kernels/kComputePos.cl","ComputePos");

            if(kComputeCollision.Handle == IntPtr.Zero)
                kComputeCollision = computeGPU.CreateKernel("resources/kernels/kComputeCollision.cl","ComputeCollision");

            Init(entities,Clothes);
        }

        private void Init(List<Particule> entities,List<Cloth> Clothes){

            output_enties = entities.ToArray();


            UniversTree = new Octree(16,entities.Count()); 
            UniversTree.inserts(entities.ToArray());
            UniversTree.GenParticulesArray();

            B1 = computeGPU.CreateBuffer<Particule>(MemoryFlags.ReadWrite,output_enties);
            B2 = computeGPU.CreateBuffer<Particule>(MemoryFlags.ReadWrite,output_enties);

            bOctree_data       = computeGPU.CreateBuffer<int>(MemoryFlags.ReadOnly,new int[entities.Count()]);
            bOctree_regions    = computeGPU.CreateBuffer<Region>(MemoryFlags.ReadOnly,UniversTree.RegionsArray);
            bOctree_settings   = computeGPU.CreateBuffer<OctreeSettings>(MemoryFlags.ReadOnly,new OctreeSettings[]{UniversTree.settings});

            bUniver = computeGPU.CreateBuffer<PhysicObject.classes.UniversSettings>(MemoryFlags.ReadWrite,new PhysicObject.classes.UniversSettings[1]{ univers });

            clothes_sim = new ClothSimulation[Clothes.Count()];
            for(int i = 0; i < Clothes.Count(); i++){
                clothes_sim[i] = new ClothSimulation(computeGPU,Clothes[i],B2);
            }

            BindKernel();

            computeGPU.Upload<Particule>(B1,output_enties);
            computeGPU.Upload<PhysicObject.classes.UniversSettings>(bUniver, new PhysicObject.classes.UniversSettings[1]{ univers });
            computeGPU.Upload<OctreeSettings>(bOctree_settings,new OctreeSettings[]{UniversTree.settings});
            computeGPU.Upload<Region>(bOctree_regions,UniversTree.RegionsArray);
            computeGPU.Upload<int>(bOctree_data,UniversTree.ParticulesArray);
        }

        private void BindKernel(){
            computeGPU.SetKernelArg(kComputeGravity,0,bUniver);
            computeGPU.SetKernelArg(kComputeGravity,1,B1);
            computeGPU.SetKernelArg(kComputeGravity,2,B2);

            computeGPU.SetKernelArg(kComputeVel,0,bUniver);
            computeGPU.SetKernelArg(kComputeVel,1,B2);
            computeGPU.SetKernelArg(kComputeVel,2,B1);

            computeGPU.SetKernelArg(kComputePos,0,bUniver);
            computeGPU.SetKernelArg(kComputePos,1,B1);
            computeGPU.SetKernelArg(kComputePos,2,B2);

            computeGPU.SetKernelArg(kComputeCollision,0,bUniver);
            computeGPU.SetKernelArg(kComputeCollision,1,B2);
            computeGPU.SetKernelArg(kComputeCollision,2,B1);
            computeGPU.SetKernelArg(kComputeCollision,3,bOctree_settings);
            computeGPU.SetKernelArg(kComputeCollision,4,bOctree_regions);
            computeGPU.SetKernelArg(kComputeCollision,5,bOctree_data);
        }

        public void Simulate(){
            if(!started)
                return;
                
            UniversTree = new Octree(16,output_enties.Count());
            UniversTree.inserts(output_enties);
            UniversTree.GenParticulesArray();

            OCS[0] = UniversTree.settings;

            computeGPU.Upload<OctreeSettings>(bOctree_settings,OCS);
            computeGPU.Upload<Region>(bOctree_regions,UniversTree.RegionsArray);
            computeGPU.Upload<int>(bOctree_data,UniversTree.ParticulesArray);

            computeGPU.Execute(kComputeGravity ,1,output_enties.Count());    

            foreach(ClothSimulation cs in clothes_sim){
                cs.Simulate();
            }

            computeGPU.Execute(kComputeVel,1,output_enties.Count());
            computeGPU.Execute(kComputePos,1,output_enties.Count());
            computeGPU.Execute(kComputeCollision,1,output_enties.Count());

            foreach(ClothSimulation cs in clothes_sim){
                cs.Update();
            }

            computeGPU.Download<Particule>(B1,output_enties);
        }

        public void CheckControl(){
            if(IsKeyDown(KEY_KP_ADD)){
                if(IsKeyDown(KEY_RIGHT_CONTROL)){
                    univers.dt += 0.001f;
                }else{
                    univers.dt += 0.0001f;
                }
                computeGPU.Upload<PhysicObject.classes.UniversSettings>(bUniver, new PhysicObject.classes.UniversSettings[1]{ univers });
            }

            if(IsKeyDown(KEY_KP_SUBTRACT)){
                if(IsKeyDown(KEY_RIGHT_CONTROL)){
                    univers.dt -= 0.001f;
                }else{
                    univers.dt -= 0.0001f;
                }
                if(univers.dt <= 0){
                    univers.dt = 0;
                }
                computeGPU.Upload<PhysicObject.classes.UniversSettings>(bUniver, new PhysicObject.classes.UniversSettings[1]{ univers });
            }

            if(IsKeyPressed(KEY_SPACE)){
                started = !started;
            }
        }

        public void Run(){
            CheckControl();
            Simulate();
        }

        public void Clear(){
            
            computeGPU.Flush();

            computeGPU.ClearBuffer(bUniver);
            computeGPU.ClearBuffer(B1);
            computeGPU.ClearBuffer(B2);
            computeGPU.ClearBuffer(bOctree_data);
            computeGPU.ClearBuffer(bOctree_regions);
            computeGPU.ClearBuffer(bOctree_settings);

            foreach(ClothSimulation c in clothes_sim){
                c.Clear();
            }
            
            //CL.ReleaseKernel(kComputeGravity);
            //CL.ReleaseKernel(kComputeVel);
            //CL.ReleaseKernel(kComputePos);
            //CL.ReleaseKernel(kComputeCollision);
            
            //computeGPU.Clear();
        }

    }
}