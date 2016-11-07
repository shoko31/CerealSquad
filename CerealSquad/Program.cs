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
            // File requirement
            System.Collections.Generic.List<System.Threading.Tasks.Task> tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();
            Downloaders.IDownloader ftpDownloader = new Downloaders.FTPDownloader();
            tasks.Add(ftpDownloader.RequireFile("testAsset", "Assets/Tiles/TestTile.png", new Uri(Downloaders.FTPDownloader.FTP_PATH + "Assets/alts.png"), false));
            tasks.Add(ftpDownloader.RequireFile("jack", "Assets/Character/Jack.png", new Uri(Downloaders.FTPDownloader.FTP_PATH + "Assets/Characters/Jack.png"), false));
            tasks.Add(ftpDownloader.RequireFile("jackHunter", "Assets/Character/JackHunter.png", new Uri(Downloaders.FTPDownloader.FTP_PATH + "Assets/Characters/JackHunter.png"), false));
            tasks.Add(ftpDownloader.RequireFile("orangina", "Assets/Character/Orangina.png", new Uri(Downloaders.FTPDownloader.FTP_PATH + "Assets/Characters/Orangina.png"), false));
            tasks.Add(ftpDownloader.RequireFile("basicEnnemy", "Assets/Character/BasicEnnemy.png", new Uri(Downloaders.FTPDownloader.FTP_PATH + "Assets/Characters/BasicEnnemy.png"), false));
            try
            {
                System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
            } catch (System.AggregateException e)
            {
                System.Diagnostics.Debug.WriteLine("Downloader error ! " + e.InnerException.Message);
                return;
            }

            renderer = new Renderer();
            renderer.Initialization();
            renderer.Resolution = Renderer.EResolution.R1600x900;
            renderer.FrameRate = 60;

            InputManager.InputManager manager = new InputManager.InputManager(renderer);
            manager.KeyboardKeyPressed += Manager_KeyboardKeyPressed;

            Menus.MenuManager.Instance.AddMenu(Menus.Prefabs.MainMenu(renderer.Win, manager));

            GameWorld.Game game = new GameWorld.Game(renderer);

            game.GameLoop(manager);
            FrameClock clock = new FrameClock();
            while (renderer.isOpen())
            {
                renderer.DispatchEvents();
                renderer.Clear(Color.Black);
                if (Menus.MenuManager.Instance.isDisplayed())
                    renderer.Draw(Menus.MenuManager.Instance.CurrentMenu);
                else
                {
                    game.Update(clock.Restart());
                    renderer.Draw(game.CurrentWorld);
                    game.WorldEntity.draw(renderer);
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
