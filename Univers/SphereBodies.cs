namespace Univers;

using Raylib_cs;
using static Raylib_cs.Raymath;

using Gravity.utils;
using ClothSimulator;

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


public class IcoSphereCloth : ICloth{

    public static Random rnd = new Random(2);


    public Spring[] springs { get; }
    public Spring_force[] spring_forces { get; }
    public Cloth_settings settings { get; }
    public Raylib_cs.Color color { get; }

    public IcoSphereCloth(SoftPlanet softPlanet,List<Particule> entities,List<Raylib_cs.Color> colors,Raylib_cs.Color c_){
        if(rnd == null){
            rnd = new Random(2);
        }
        
        IcoSphere icoSphere = new IcoSphere(softPlanet.radius,softPlanet.subdivision);


        Vector3[] icosahedron = icoSphere.GetVertices();

        int offset = entities.Count();
        int count = icosahedron.Count()+1;

        float mass = softPlanet.mass/(icosahedron.Length+1);
        float radius = softPlanet.particule_radius;

        //center particule of the icosphere
        Particule center = new Particule(softPlanet.position,new Vector3(0),mass,radius,softPlanet.bounciness,softPlanet.roughness);
        entities.Add(center);
        colors.Add(new Color(rnd.Next(255),rnd.Next(255),rnd.Next(255),0));

        Dictionary<int,int[]> triangles = icoSphere.GetTriangles();

        List<Spring> spring_list = new List<Spring>();

        //generate a particule for each vertex of the icosahedron
        for(int i = 0; i < icosahedron.Length; i++){
     
            Particule p = new Particule(icosahedron[i] + softPlanet.position,new Vector3(0),mass,radius,softPlanet.bounciness,softPlanet.roughness);

            //distance between the center and the particule
            float distance = Vector3.Distance(center.position,p.position);


            //create a spring between the center and the particule
            
            Spring spring = new Spring(distance,7*distance,softPlanet.pressure,offset,(offset+1+i),2);
            spring_list.Add(spring);


            //create a spring between the current vertex and the connected one
            foreach(int t in triangles[i]){
                float d = Vector3.Distance(icosahedron[t], icosahedron[i]);
                Spring spring_ = new Spring(d,7*d,softPlanet.pressure,offset+i,offset+t,2);
                spring_list.Add(spring_);
            }

            
            entities.Add(p);
            colors.Add(new Color(rnd.Next(255),rnd.Next(255),rnd.Next(255),255));
        }

        settings = new Cloth_settings(offset,count,spring_list.Count());
        springs = spring_list.ToArray();
        spring_forces = new Spring_force[spring_list.Count()];

    }

}