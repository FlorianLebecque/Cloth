namespace Univers;

using Raylib_cs;
using static Raylib_cs.Raymath;

using Gravity.utils;

public class SphereGenerator{
    public static Vector3 WORLD_UP = new Vector3(0,1,0);
    public static Random rnd = new Random(2);
    public static void CreateSphere(UniversSettings univers,List<Particule> entities,List<Color> colors,SoftPlanet softPlanet){

        if(rnd == null){
            rnd = new Random(2);
        }

        IcoSphere icoSphere = new IcoSphere(softPlanet.radius,softPlanet.subdivision);

        Vector3[] icosahedron = icoSphere.GetVertices();

        //generate a particule for each vertex of the icosahedron
        foreach(Vector3 pos in icosahedron){
            float mass = softPlanet.mass/icosahedron.Length;
            float radius = softPlanet.particule_radius;
            Particule p = new Particule(pos + softPlanet.position,new Vector3(0),mass,radius,softPlanet.bounciness,softPlanet.roughness);

            
            entities.Add(p);
            colors.Add(new Color(rnd.Next(255),rnd.Next(255),rnd.Next(255),255));
        }

        
    }




}