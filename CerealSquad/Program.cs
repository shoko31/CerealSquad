﻿using System;
using SFML;
using SFML.Window;
using SFML.Graphics;

namespace CerealSquad
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        static void Main()
        {
            /*
            Renderer renderer = new Renderer();

            renderer.initialization();
            renderer.loop();
            */
            RenderWindow win = new RenderWindow(new VideoMode(800, 800), "Cereal Menu");
            InputManager manager = new InputManager(win);
            AMenu mainMenu = new AMenu(win, manager);

            manager.KeyboardKeyPressed += Manager_KeyboardKeyPressed;

            mainMenu.Show();

            while (win.IsOpen)
            {
                win.DispatchEvents();
                win.Clear(Color.Magenta);
                if (mainMenu.Displayed)
                    mainMenu.update();
                else
                {
                    // GameLogic
                }
                win.Display();
            }
        }

        private static void Manager_KeyboardKeyPressed(object source, Keyboard.KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keyboard.Key.Escape))
                ((RenderWindow)source).Close();
        }
    }
}
