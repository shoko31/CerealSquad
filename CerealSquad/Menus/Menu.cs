﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace CerealSquad.Menus
{
    public class MenuItem : Drawable
    {
        public enum ItemType
        {
            Normal,
            Disabled,
            KeyBinded
        }

        private MenuItem() { }
        public MenuItem(Buttons.IButton button, ItemType type = ItemType.Normal, InputManager.Keyboard.Key keyboardKey = InputManager.Keyboard.Key.Unknown, uint joystickKey = 0)
        {
            Type = type;
            Button = button;
            KeyboardKey = keyboardKey;
            JoystickKey = joystickKey;
        }

        public InputManager.Keyboard.Key KeyboardKey { get; private set; }
        public uint JoystickKey { get; private set; }
        public ItemType Type { get; set; }
        public Buttons.IButton Button { get; private set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Button.Draw(target, states);
        }
    }

    public class Menu : Drawable
    {
        protected InputManager.InputManager _InputManager;

        private bool _displayed = false;
        public bool Displayed { get { return _displayed; } }

        protected List<MenuItem> _menuList = new List<MenuItem>();

        public Menu(InputManager.InputManager inputManager)
        {
            _InputManager = inputManager;

            _InputManager.KeyboardKeyPressed += _InputManager_KeyboardKeyPressed;
            _InputManager.KeyboardKeyReleased += _InputManager_KeyboardKeyReleased;
        }

        private void nextMenu()
        {
            List<MenuItem> validItems = _menuList.FindAll(x => x.Type == MenuItem.ItemType.Normal);

            MenuItem _current = validItems.FirstOrDefault<MenuItem>(x => x.Button.Selected == true);
            if (_current == null && validItems.Count > 0)
            {
                validItems[0].Button.Selected = true;
            }
            else if (validItems.Count > 0)
            {
                int index = validItems.IndexOf(_current) + 1;
                MenuItem _next = validItems.ElementAt(index > validItems.Count - 1 ? 0 : index);

                _current.Button.Selected = false;
                _next.Button.Selected = true;
            }
        }

        private void previousMenu()
        {
            List<MenuItem> validItems = _menuList.FindAll(x => x.Type == MenuItem.ItemType.Normal);

            MenuItem _current = validItems.FirstOrDefault<MenuItem>(x => x.Button.Selected == true);
            if (_current == null && validItems.Count > 0)
            {
                validItems[0].Button.Selected = true;
            }
            else if (validItems.Count > 0)
            {
                int index = validItems.IndexOf(_current) - 1;
                MenuItem _prev = validItems.ElementAt(index < 0 ? validItems.Count - 1 : index);

                _current.Button.Selected = false;
                _prev.Button.Selected = true;
            }
        }

        #region Menus_KeyBinding

        private MenuItem _lastItemPressed = null;
        /// <summary>
        /// Manage key bindings for menus
        /// </summary>
        private void _InputManager_KeyboardKeyPressed(object source, InputManager.Keyboard.KeyEventArgs e)
        {
            _lastItemPressed = null;
            if (Displayed)
            {
                MenuItem _keyBinded = _menuList.Find(x => x.Type == MenuItem.ItemType.KeyBinded && x.KeyboardKey == e.KeyCode);
                if (_keyBinded != null)
                    _keyBinded.Button.Trigger(source, e, false);
                else if (e.KeyCode.Equals(InputManager.Keyboard.Key.Down))
                    nextMenu();
                else if (e.KeyCode.Equals(InputManager.Keyboard.Key.Up))
                    previousMenu();
                else
                {
                    MenuItem _current = _menuList.Find(x => x.Button.Selected == true);
                    if (_current != null)
                    {
                        _current.Button.Trigger(source, e, false);
                        _lastItemPressed = _current;
                    }
                }
            }
        }

        private void _InputManager_KeyboardKeyReleased(object source, InputManager.Keyboard.KeyEventArgs e)
        {
            if (Displayed)
            {
                MenuItem _keyBinded = _menuList.Find(x => x.Type == MenuItem.ItemType.KeyBinded && x.KeyboardKey == e.KeyCode);
                if (_keyBinded != null)
                    _keyBinded.Button.Trigger(source, e, true);
                else if (_lastItemPressed != null)
                {
                    MenuItem _current = _menuList.Find(x => x.Button.Selected == true);
                    if (_current != null && _lastItemPressed == _current)
                        _current.Button.Trigger(source, e, true);
                }
            }
        }
        #endregion

        public virtual void Update(SFML.System.Time DeltaTime) { }

        public virtual void Show()
        {
            System.Diagnostics.Debug.WriteLine("SHOWED MENU");
            if (!Displayed)
            {
                _displayed = true;
            }
        }

        public virtual void Hide()
        {
            if (Displayed)
            {
                _displayed = false;
            }
        }

        public void Toggle()
        {
            if (Displayed)
                Hide();
            else
                Show();
        }

        public void Exit()
        {
            MenuManager.Instance.RemoveMenu(this);
        }

        public void AddItem(MenuItem item)
        {
            _menuList.Add(item);
        }

        public void ClearItems()
        {
            _menuList.Clear();
        }

        public void Initialize()
        {
            _lastItemPressed = null;
            MenuItem firstValid = _menuList.FirstOrDefault<MenuItem>(x => x.Type != MenuItem.ItemType.Disabled && x.Type != MenuItem.ItemType.KeyBinded);
            if (firstValid != null)
                firstValid.Button.Selected = true;
        }
            
        public virtual void Draw(RenderTarget target, RenderStates states)
        {
            if (Displayed)
                _menuList.ForEach(x => x.Draw(target, states));
        }
    }
}
