﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace CerealSquad.Menus.Buttons
{
    class NewGameButton : IButton
    {
        private GameWorld.GameManager _GameManager;
        t_pos _pos = new t_pos(300, 300);
        private bool selected;
        public bool Selected { get { return selected; } set { selected = value; selectionChanged(); } }

        protected void selectionChanged()
        {
            if (Selected)
            {
                Text = new Text(">" + _Text, Factories.FontFactory.FontFactory.Instance.getFont(Factories.FontFactory.FontFactory.Font.ReenieBeanie), 64);
                Text.Color = Color.Green;
                Text.Style = Text.Styles.Bold;
                Text.Position = new SFML.System.Vector2f(_pos.X, _pos.Y + _Offsety);
            }
            else
            {
                Text = new Text(" " + _Text, Factories.FontFactory.FontFactory.Instance.getFont(Factories.FontFactory.FontFactory.Font.ReenieBeanie), 64);
                Text.Color = Color.White;
                Text.Style = Text.Styles.Bold;
                Text.Position = new SFML.System.Vector2f(_pos.X, _pos.Y + _Offsety);
            }
        }

        protected Text Text;
        protected string _Text;
        protected int _Offsety;

        private NewGameButton() { }
        public NewGameButton(string text, Font font, int offsety, GameWorld.GameManager gameManager)
        {
            _GameManager = gameManager;
            _Text = text;
            Text = new Text(" " + text, font, 64);
            Text.Style = Text.Styles.Bold;
            Text.Position = new SFML.System.Vector2f(_pos.X, _pos.Y + offsety);
            _Offsety = offsety;
        }

        public void Trigger(object source, InputManager.Keyboard.KeyEventArgs e, bool up = true)
        {
            if (e.KeyCode == InputManager.Keyboard.Key.Return && up == true)
            {
                selectionChanged();
                Menus.MenuManager.Instance.Clear();
                _GameManager.newGame();
            }
            else if (e.KeyCode == InputManager.Keyboard.Key.Return && up == false)
            {
                Text.Color = Color.Red;
            }
        }
        public void Trigger(object source, InputManager.Joystick.ButtonEventArgs e, bool up = true) { }
        public void Trigger(object source, InputManager.Joystick.MoveEventArgs e) { }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Text, states);
        }
    }
}
