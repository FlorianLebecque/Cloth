using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClothSimulator
{
    public interface ICloth
    {
        public Spring[] springs { get; }
        public Spring_force[] spring_forces { get; }
        public Cloth_settings settings { get; }
        public Raylib_cs.Color color { get; }

    }
}