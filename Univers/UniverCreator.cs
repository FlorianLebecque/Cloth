using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Raylib_cs;


namespace Univers {
    public class UniversCreator {
        
        public static UniversSettings univers;
        public static List<Particule> entities = new();
        public static List<Raylib_cs.Color> colors = new();
        public static List<Cloth> clothList = new();

    
        public static void CreateUnivers1(Vector3 WORLD_UP){

            Random rnd = new Random(2);
            

            RingGenerator.rnd = rnd;

            univers = new UniversSettings(20f,0.01f);
            entities    = new();
            colors      = new();
            clothList   = new();

            List<Ring> RingsList = new();

            Particule Sun = new Particule(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,50,0.95f,0.5f);
            Particule Sun2 = new Particule(new Vector3(20, 0f, 4800), new Vector3(0, 0f, -200), 100000,50f,0.95f,0.5f);

            Particule Cloth_Planet   = new Particule(new Vector3(0f, 200, 0f), new Vector3(0f, -300, 0), 500,15,0.5f,0.01f);
            Cloth_Planet.velocity = Particule.GetOrbitalSpeed(Sun,Cloth_Planet,new Vector3(-1,0,0),univers);

            Particule Orbital_planet = new Particule(new Vector3(20, 0, 4500), new Vector3(0, 0f, 0), 500,15,0.6f,0.01f);
            Orbital_planet.velocity  = Particule.GetOrbitalSpeed(Sun2,Orbital_planet,WORLD_UP,univers);

            entities.Add(Sun);
            entities.Add(Sun2);      //secondary sun (far away)
            entities.Add(Cloth_Planet);   
            entities.Add(Orbital_planet);

            colors.Add(new Raylib_cs.Color(237, 217, 200   ,255));
            colors.Add(new Raylib_cs.Color(255  , 150, 30 ,255));
            colors.Add(new Raylib_cs.Color(0  , 230, 207 ,255));
            colors.Add(new Raylib_cs.Color(49 , 224, 0   ,255));

            Cloth drape2 = new Cloth(new Vector3(400,0,0),30,30,4f,1f,entities,colors,Color.SKYBLUE);      //fill the entities array with all the tissue particule
            Cloth drape = new Cloth(new Vector3(0,-200,2),30,30,4f,1f,entities,colors,Color.BROWN);      //fill the entities array with all the tissue particule


            Cloth.SetOrbitalSpeed(drape,entities,0,univers,new Vector3(-1,0,0));
            Cloth.SetOrbitalSpeed(drape2,entities,0,univers,new Vector3(0,-1.5f,-1));

            Ring MainRing = new Ring(entities,0,new Vector3(0,1.5f,1f), 6,9);
            MainRing.radius_factor = 10f;
            MainRing.min_mass = 0.1f;
            MainRing.max_mass = 0.2f;
            Ring SecRing = new Ring(entities,0,new Vector3(0,1,1), 11,12);
            SecRing.nbr_particul = 2000;
            SecRing.min_mass = 0.1f;
            SecRing.max_mass = 0.2f;
            SecRing.radius_factor = 10f;

            Ring OrbitPlanet = new Ring(entities,1,WORLD_UP, 2,3);
            OrbitPlanet.nbr_particul = 200;
            OrbitPlanet.min_mass = 0.1f;
            OrbitPlanet.max_mass = 0.2f;
            OrbitPlanet.radius_factor = 5f;


            RingsList.Add(MainRing);
            RingsList.Add(SecRing);
            RingsList.Add(OrbitPlanet);

            foreach(Ring ring in RingsList){
                RingGenerator.CreateRing(univers,entities,colors,ring);
            }

            
            clothList.Add(drape);
            clothList.Add(drape2);
        }

        public static void CreateUnivers2(Vector3 WORLD_UP){

            Random rnd = new Random(2);
            

            RingGenerator.rnd = rnd;

            univers = new UniversSettings(100f,0.01f);
            entities    = new();
            colors      = new();
            clothList   = new();

            Particule earth = new Particule(
                new Vector3(0,-200,0),Vector3.Zero,
                100000f,100,0,1
            );

            Particule mountain = new Particule(
                new Vector3(0,200,0),Vector3.Zero,
                0,50,0,1
            );

            entities.Add(mountain);

            entities.Add(earth);
            colors.Add(new Color(255,255,255,255));
            colors.Add(new Color(255,255,255,255));

            Cloth c = new Cloth(
                new Vector3(0,400,0),45,45,4,1,entities,colors,new Color(0,0,120,255)
            );

            clothList.Add(c);

        }

        public static void CreateUnivers3(Vector3 WORLD_UP){

            Random rnd = new Random(2);
            

            RingGenerator.rnd = rnd;

            univers = new UniversSettings(20f,0.01f);
            entities    = new();
            colors      = new();
            clothList   = new();

            List<Ring> RingsList = new();

            Particule Sun = new Particule(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,50,0.95f,0.5f);
            Particule Sun2 = new Particule(new Vector3(20, 0f, 4800), new Vector3(0, 0f, -200), 100000,50f,0.95f,0.5f);

            Particule Cloth_Planet   = new Particule(new Vector3(0f, 200, 0f), new Vector3(0f, -300, 0), 500,15,0.5f,0.01f);
            Cloth_Planet.velocity = Particule.GetOrbitalSpeed(Sun,Cloth_Planet,new Vector3(-1,0,0),univers);

            Particule Orbital_planet = new Particule(new Vector3(20, 0, 4500), new Vector3(0, 0f, 0), 500,15,0.6f,0.01f);
            Orbital_planet.velocity  = Particule.GetOrbitalSpeed(Sun2,Orbital_planet,WORLD_UP,univers);

            entities.Add(Sun);
            entities.Add(Sun2);      //secondary sun (far away)
            entities.Add(Cloth_Planet);   
            entities.Add(Orbital_planet);

            colors.Add(new Raylib_cs.Color(237, 217, 200   ,255));
            colors.Add(new Raylib_cs.Color(255  , 150, 30 ,255));
            colors.Add(new Raylib_cs.Color(0  , 230, 207 ,255));
            colors.Add(new Raylib_cs.Color(49 , 224, 0   ,255));


            Ring MainRing = new Ring(entities,0,new Vector3(0,1.5f,1f), 6,9);
            MainRing.nbr_particul = 5000;
            MainRing.radius_factor = 10f;
            MainRing.min_mass = 0.1f;
            MainRing.max_mass = 0.2f;
            Ring SecRing = new Ring(entities,0,new Vector3(0,1,1), 11,12);
            SecRing.nbr_particul = 5000;
            SecRing.min_mass = 0.1f;
            SecRing.max_mass = 0.2f;
            SecRing.radius_factor = 10f;

            Ring OrbitPlanet = new Ring(entities,1,WORLD_UP, 2,3);
            OrbitPlanet.nbr_particul = 1000;
            OrbitPlanet.min_mass = 0.1f;
            OrbitPlanet.max_mass = 0.2f;
            OrbitPlanet.radius_factor = 5f;


            RingsList.Add(MainRing);
            RingsList.Add(SecRing);
            RingsList.Add(OrbitPlanet);

            foreach(Ring ring in RingsList){
                RingGenerator.CreateRing(univers,entities,colors,ring);
            }
        }

    }
}