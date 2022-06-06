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

            Particule Sun = new Particule(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,50,0.95f,0.9f);
            Particule Sun2 = new Particule(new Vector3(20, 0f, 4800), new Vector3(0, 0f, -200), 100000,50f,0.95f,0.9f);

            Particule Cloth_Planet   = new Particule(new Vector3(0f, 205, 0f), new Vector3(0f, 0, 0), 500,15,0.9f,1f);
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

            Cloth drape2 = new Cloth(new Vector3(400,0,0),30,30,4f,entities,colors,Color.SKYBLUE);      //fill the entities array with all the tissue particule
            Cloth drape = new Cloth(new Vector3(0,-200,2),30,30,4f,entities,colors,Color.BROWN);      //fill the entities array with all the tissue particule


            Cloth.SetOrbitalSpeed(drape,entities,0,univers,new Vector3(-1,0,0));
            Cloth.SetOrbitalSpeed(drape2,entities,0,univers,new Vector3(0,-1.5f,-1));

            Ring MainRing = new Ring(entities,0,new Vector3(0,1.5f,1f), 6,9);
            MainRing.nbr_particul = 2000;
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

            univers = new UniversSettings(10f,0.01f);
            entities    = new();
            colors      = new();
            clothList   = new();

            Particule earth = new Particule(
                new Vector3(0,-1400,0),Vector3.Zero,
                15000000f,1000,0,100
            );

            Particule mountain = new Particule(
                new Vector3(0,0,0),Vector3.Zero,
                0,50,0,5
            );

            Particule block1 = new Particule(
                new Vector3(-20,-400,-20),Vector3.Zero,
                0,1,0,0
            );

            Particule block2 = new Particule(
                new Vector3(-20,-400,20),Vector3.Zero,
                0,1,0,0
            );

            Particule block3 = new Particule(
                new Vector3(20,-400,-20),Vector3.Zero,
                0,1,0,0
            );

            Particule block4 = new Particule(
                new Vector3(20,-400,20),Vector3.Zero,
                0,1,0,0
            );


            entities.Add(mountain);
            entities.Add(earth);

            entities.Add(block1);
            entities.Add(block2);
            entities.Add(block3);
            entities.Add(block4);

            colors.Add(new Color(255,255,255,255));
            colors.Add(new Color(255,255,255,5));
            colors.Add(new Color(255,255,255,5));
            colors.Add(new Color(255,255,255,5));
            colors.Add(new Color(255,255,255,5));
            colors.Add(new Color(255,255,255,5));

            ClothParameter clp = new();
            clp.k1 = 800;
            clp.k2 = 700;
            clp.k3 = 600;
            clp.mass = 1f;
            clp.cd = 4f;
            clp.roughtness = 0.9f;
            

            Cloth.parameter = clp;

            Cloth c = new Cloth(
                new Vector3(0,200,0),60,60,4,entities,colors,new Color(0,0,120,255)
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

        public static void CreateUniversal(Vector3 WORLD_UP){
            univers = new UniversSettings(20f,0.01f);
            entities    = new();
            colors      = new();
            clothList   = new();


            //ParticulText.CreateText("hello",Vector3.Zero,entities,colors);

        }

        public static void CreateMiniUniver(Vector3 WORLD_UP){
            univers = new UniversSettings(8f,0.002f);


            Random rnd = new Random(2);
            entities    = new();
            colors      = new();
            clothList   = new();

            List<Ring> RingsList = new();

            Particule Sun = new Particule(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,10,0.8f,0.9f);

            Particule Sun2 = new Particule(new Vector3(0, 0f, 960), new Vector3(0, 0f, -250), 100000,10,0.8f,0.9f);

            Particule Cloth_Planet   = new Particule(new Vector3(0f, 50, 0f), new Vector3(0f, 0, 0), 500,3,0.5f,0.01f);
            Cloth_Planet.velocity = Particule.GetOrbitalSpeed(Sun,Cloth_Planet,new Vector3(-1,0,0),univers);

            Particule Orbital_planet = new Particule(new Vector3(0, 0,900), new Vector3(0, 0f, 0), 500,3,0.6f,0.01f);
            Orbital_planet.velocity  = Particule.GetOrbitalSpeed(Sun2,Orbital_planet,WORLD_UP,univers);

            entities.Add(Sun);
            entities.Add(Sun2);      //secondary sun (far away)
            entities.Add(Cloth_Planet);   
            entities.Add(Orbital_planet);

            colors.Add(new Raylib_cs.Color(237, 217, 200   ,255));
            colors.Add(new Raylib_cs.Color(255  , 150, 30 ,255));
            colors.Add(new Raylib_cs.Color(0  , 230, 207 ,255));
            colors.Add(new Raylib_cs.Color(49 , 224, 0   ,255));

            Cloth drape2 = new Cloth(new Vector3(80,0,0),30,30,1f,entities,colors,Color.SKYBLUE);      //fill the entities array with all the tissue particule
            Cloth drape = new Cloth(new Vector3(0,-50,0),30,30,1f,entities,colors,Color.BROWN);      //fill the entities array with all the tissue particule


            Cloth.SetOrbitalSpeed(drape,entities,0,univers,new Vector3(-1,0,0));
            Cloth.SetOrbitalSpeed(drape2,entities,0,univers,new Vector3(0,-1.5f,-1));

            clothList.Add(drape2);
            clothList.Add(drape);


            Ring MainRing = new Ring(entities,0,new Vector3(0,1.5f,1f), 6,9);
            MainRing.radius_factor = 10f;
            MainRing.min_mass = 0.02f;
            MainRing.max_mass = 0.04f;
            Ring SecRing = new Ring(entities,0,new Vector3(0,1,1), 11,12);
            SecRing.nbr_particul = 2000;
            SecRing.min_mass = 0.02f;
            SecRing.max_mass = 0.04f;
            SecRing.radius_factor = 10f;

            Ring OrbitPlanet = new Ring(entities,1,WORLD_UP, 2,3);
            OrbitPlanet.nbr_particul = 200;
            OrbitPlanet.min_mass = 0.02f;
            OrbitPlanet.max_mass = 0.04f;
            OrbitPlanet.radius_factor = 5f;


            RingsList.Add(MainRing);
            RingsList.Add(SecRing);
            RingsList.Add(OrbitPlanet);

            foreach(Ring ring in RingsList){
                RingGenerator.CreateRing(univers,entities,colors,ring);
            }


        }

        public static void CreateMenuUniver(Vector3 WORLD_UP){
            Random rnd = new Random(2);
            colors.Add(new Color(255,255,255,255));
            

            RingGenerator.rnd = rnd;

            univers = new UniversSettings(20f,0.01f);
            entities    = new();
            colors      = new();
            clothList   = new();

            List<Ring> RingsList = new();

            Particule Univer1 = new Particule(new Vector3(0, 0, 0), new Vector3(0f, 0f, 0f), 100000f,50,0.95f,0.5f);
            colors.Add(new Raylib_cs.Color(237, 217, 200   ,255));

            Particule mountain = new Particule(
                new Vector3(2000,200,2000),Vector3.Zero,
                0,50,0,5
            );
            colors.Add(new Raylib_cs.Color(237, 217, 200   ,255));


            entities.Add(Univer1);
            entities.Add(mountain);

            Cloth drape2 = new Cloth(new Vector3(400,0,0),30,30,4f,entities,colors,Color.SKYBLUE);      //fill the entities array with all the tissue particule
            Cloth drape = new Cloth(new Vector3(0,-200,2),30,30,4f,entities,colors,Color.BROWN);      //fill the entities array with all the tissue particule

            clothList.Add(drape);
            clothList.Add(drape2);

            Cloth c = new Cloth(
                new Vector3(2000,400,2000),60,60,4,entities,colors,new Color(0,0,120,255)
            );

            clothList.Add(c);

            Ring MainRing = new Ring(entities,0,new Vector3(0,1.5f,1f), 6,9);
            MainRing.radius_factor = 10f;
            MainRing.min_mass = 0.1f;
            MainRing.max_mass = 0.2f;
            Ring SecRing = new Ring(entities,0,new Vector3(0,1,1), 11,12);
            SecRing.nbr_particul = 2000;
            SecRing.min_mass = 0.1f;
            SecRing.max_mass = 0.2f;
            SecRing.radius_factor = 10f;

            RingsList.Add(MainRing);
            RingsList.Add(SecRing);

            foreach(Ring ring in RingsList){
                RingGenerator.CreateRing(univers,entities,colors,ring);
            }

        }

        public static void CreateTestUniver(Vector3 WORLD_UP){

            univers = new UniversSettings(20f,0.01f);
            entities    = new();
            colors      = new();
            clothList   = new();

            Particule Sun = new Particule(new Vector3(100, 0, 0), new Vector3(0f, 0f, 0f), 100f,1,0.95f,0.9f);
            entities.Add(Sun);
            Particule Sun2 = new Particule(new Vector3(-100, 0, 0), new Vector3(0f, 0f, 0f), 1f,0.5f,0.95f,0.9f);
            entities.Add(Sun2);


            colors.Add(new Raylib_cs.Color(237, 217, 200 ,255));
            colors.Add(new Raylib_cs.Color(237, 217, 200 ,255));


            SoftPlanet sp = new SoftPlanet(new Vector3(100,0,0),50f,60f,2f,3);

            SphereGenerator.CreateSphere(univers,entities,colors,sp);

            for(int i = 1;i < entities.Count();i++){

                Particule n = new Particule(entities[i].position,Particule.GetOrbitalSpeed(entities[0], entities[i],Vector3.One,univers),entities[i].mass,entities[i].radius,entities[i].bounciness,entities[i].roughness);

                entities[i] = n;
            }

        }

    }
}