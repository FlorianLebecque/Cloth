namespace Univers;

using Raylib_cs;
using static Raylib_cs.Raymath;
public class RingGenerator{

    public static Vector3 WORLD_UP = new Vector3(0,1,0);
    public static Random rnd = new Random(2);
    public static void CreateRing(UniversSettings univers,List<Particule> entities,List<Color> colors,Ring ring){

        if(rnd == null){
            rnd = new Random(2);
        }

        Vector3 rotationAxis = Vector3.Normalize(Vector3.Cross(WORLD_UP,ring.normal));

        if(WORLD_UP == ring.normal){
            rotationAxis = WORLD_UP;
        }

        float angle = -(float)Math.Acos(Vector3.Dot(WORLD_UP,ring.normal));
        Matrix4x4 r = MatrixRotate(rotationAxis,angle);

            //generation of a ring of particule arround the first sun
        for(int i = 0; i < ring.nbr_particul; i++){

            float xz_dist = rnd.Next((int)ring.min_distance,(int)ring.max_distance);
            float theta = rnd.Next();

            float x =  xz_dist * (float)Math.Cos(theta);
            float z = -xz_dist * (float)Math.Sin(theta);
            float y = rnd.Next(-(int)entities[ring.target].radius/2,(int)entities[ring.target].radius/2);

            Vector3 pos =  new Vector3(x,y,z);
    
            pos = entities[ring.target].position + Vector3.Transform(pos,r);


            float mass = ring.min_mass +  (float)rnd.NextDouble() * ring.max_mass;
            Particule p = new Particule(
                pos,
                Vector3.One,
                mass,
                mass*ring.radius_factor,
                ring.bounciness,
                ring.roughness
            );
            p.velocity = Particule.GetOrbitalSpeed(entities[ring.target],p,ring.normal,univers);

            entities.Add(p);
            colors.Add(new Raylib_cs.Color(rnd.Next(200,255),rnd.Next(200,255),rnd.Next(200,255),255));
        }
    }

}