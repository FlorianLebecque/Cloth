using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Raylib_cs;
using static Raylib_cs.Raylib;

using Simulator;
using Univers;

namespace Renderer
{
    public class Menu{

        List<string> option;

        int height;
        int x;
        int y;

        int selected;
        public bool valide;

        RaylibRenderer renderer;
        UniverSimulation univer_simulation;
        public Menu(){

            selected = 0;

            option = new();

            height = 50;

            x = GetScreenWidth() / 16;
            y = GetScreenHeight() / 2 - ((height+10));

            Camera3D camera = new Camera3D();
            camera.position = new Vector3(2.5f, 400f, 3.0f);    // Camera position
            camera.target = new Vector3(0.0f, 0.0f, 0.7f);      // Camera looking at point
            camera.up = new Vector3(0,1,0);    
            camera.fovy = 90.0f;                                // Camera field-of-view Y
            camera.projection = CameraProjection.CAMERA_PERSPECTIVE;

            UniversCreator.CreateMenuUniver(new Vector3(0,1,0));
            renderer = new RaylibRenderer(camera,UniversCreator.colors);
            univer_simulation = new UniverSimulation(UniversCreator.univers,UniversCreator.entities,UniversCreator.clothList);

        }

        public void Add(string option_name){
            this.option.Add(option_name);
        }

        public void reRun(){
            valide = false;
        }
        private void Draw(){

            
            renderer.Run(univer_simulation);
            renderer.current_view = selected;

            BeginDrawing();

                //ClearBackground(Color.DARKGRAY);
            int count = 0;
            foreach(string v in option){
                int yp = y + count*(height+10);

                Color c = Color.BLANK;

                if(selected == count)
                    c = Color.BEIGE;

                DrawRectangle(x ,yp,500,height,c);
                DrawText(v,x+15,yp + (height/4),25,Color.WHITE);
                count++;
            }

            EndDrawing();

        }

        private void Control(){
            if(IsKeyPressed(KeyboardKey.KEY_UP)){
                selected --;
                if(selected<0)
                    selected = 0;
            }
            if(IsKeyPressed(KeyboardKey.KEY_DOWN)){
                selected ++;
                if(selected==option.Count())
                    selected = option.Count()-1;
            }

            if((IsKeyPressed(KeyboardKey.KEY_ENTER))||(IsKeyPressed(KeyboardKey.KEY_SPACE))){
                valide = true;
            }

        }

        public int Run(){
            if(valide)
                return selected;

            Draw();
            Control();

            return -1;
        }
        
    }
}