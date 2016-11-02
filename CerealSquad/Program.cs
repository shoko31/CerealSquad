﻿using System;
using SFML;
using SFML.Window;
using SFML.Graphics;

namespace CerealSquad
{
    static class Program
    {
        public static Renderer renderer;

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        /// 
        static void Main()
        {
            renderer = new Renderer();
            renderer.Initialization();
            renderer.Resolution = Renderer.EResolution.R800x450;
            renderer.FrameRate = 60;

            InputManager.InputManager manager = new InputManager.InputManager(renderer);
            manager.KeyboardKeyPressed += Manager_KeyboardKeyPressed;

            Menus.MenuManager.Instance.AddMenu(Menus.Prefabs.MainMenu(renderer.Win, manager));

            GameWorld.Game game = new GameWorld.Game(renderer);

            game.GameLoop();
            while (renderer.isOpen())
            {
                renderer.DispatchEvents();
                renderer.Clear(Color.Black);
                if (Menus.MenuManager.Instance.isDisplayed())
                    renderer.Draw(Menus.MenuManager.Instance.CurrentMenu);
                else
                {
                    renderer.Draw(game.CurrentWorld);
                }
                renderer.Display();
            }
        }
        
        private static void Manager_KeyboardKeyPressed(object source, InputManager.Keyboard.KeyEventArgs e)
        {
            if (e.KeyCode.Equals(InputManager.Keyboard.Key.Escape))
                ((Window)source).Close();
            if (e.KeyCode.Equals(InputManager.Keyboard.Key.F))
                renderer.FullScreen = !renderer.FullScreen;
        }
    }
}
