﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CerealSquad.InputManager;
using SFML.Graphics;

namespace CerealSquad.Menus
{
    namespace Players
    {
        public enum Type
        {
            Undefined = -1,
            Keyboard = 0,
            Controller
        }

        class Player : Drawable
        {
            public Type Type { get; protected set; }
            public uint ControllerId { get; protected set; }
            public uint KeyboardId { get; protected set; }
            public uint Selection { get; protected set; }
            public bool LockedChoice { get; protected set; }

            private uint _Id;
            private Renderer _Renderer;

            private void init(Type type, uint id)
            {
                Type = type;
                ControllerId = 0;
                KeyboardId = 0;

                _Id = id;

                _PlayerText = new Text("Player " + (id + 1), Factories.FontFactory.FontFactory.Instance.getFont(Factories.FontFactory.FontFactory.Font.XirodRegular));
                _PlayerText.Position = new SFML.System.Vector2f(50 + 400 * _Id, 5);

                _JoinText.Position = new SFML.System.Vector2f(120 + 400 * _Id, 500);
                _JoinText.Color = Color.Green;

                _SelectionText[0] = new Text("Mike", Factories.FontFactory.FontFactory.Instance.getFont(Factories.FontFactory.FontFactory.Font.XirodRegular));
                _SelectionText[0].Position = new SFML.System.Vector2f(95 + 400 * _Id, 40);
                _SelectionText[1] = new Text("Jack", Factories.FontFactory.FontFactory.Instance.getFont(Factories.FontFactory.FontFactory.Font.XirodRegular));
                _SelectionText[1].Position = new SFML.System.Vector2f(90 + 400 * _Id, 40);
                _SelectionText[2] = new Text("Orange Hina", Factories.FontFactory.FontFactory.Instance.getFont(Factories.FontFactory.FontFactory.Font.XirodRegular));
                _SelectionText[2].Position = new SFML.System.Vector2f(5 + 400 * _Id, 40);
                _SelectionText[3] = new Text("Tchong", Factories.FontFactory.FontFactory.Instance.getFont(Factories.FontFactory.FontFactory.Font.XirodRegular));
                _SelectionText[3].Position = new SFML.System.Vector2f(60 + 400 * _Id, 40);
            }

            public Player(uint id, Renderer renderer)
            {
                _Renderer = renderer;

                init(Type.Undefined, id);
            }
            protected Player(Type type, uint id, Renderer renderer, List<uint> LockedList)
            {
                _Renderer = renderer;

                Selection = 0;
                if (LockedList.Contains(Selection))
                    SelectNext(LockedList);

                init(type, id);
            }

            public void SelectNext(List<uint> lockedList)
            {
                if (!LockedChoice)
                {
                    bool prevent_continue = false;
                    uint checking_counter = 0;
                    while (!prevent_continue)
                    {
                        if (checking_counter > CharacterSelectMenu.SELECTION_COUNT)
                            throw new NotSupportedException("No character left for selection");
                        Selection++;
                        Selection = Selection % CharacterSelectMenu.SELECTION_COUNT;
                        if (!lockedList.Contains(Selection))
                            prevent_continue = true;
                        checking_counter++;
                    }
                }
            }

            public void SelectPrevious(List<uint> lockedList)
            {
                if (!LockedChoice)
                {
                    bool prevent_continue = false;
                    uint checking_counter = 0;
                    while (!prevent_continue)
                    {
                        if (checking_counter > CharacterSelectMenu.SELECTION_COUNT)
                            throw new NotSupportedException("No character left for selection");
                        if (Selection == 0)
                            Selection = CharacterSelectMenu.SELECTION_COUNT;
                        Selection--;
                        if (!lockedList.Contains(Selection))
                            prevent_continue = true;
                        checking_counter++;
                    }
                }
            }

            public void LockSelection(bool _lock)
            {
                LockedChoice = _lock;
                if (_lock)
                    _SelectionText[Selection].Color = new Color(250, 65, 65, 175);
                else
                    _SelectionText[Selection].Color = Color.White;
            }

            private Text _PlayerText;
            private Text _JoinText = new Text("Join", Factories.FontFactory.FontFactory.Instance.getFont(Factories.FontFactory.FontFactory.Font.XirodRegular));
            private Text[] _SelectionText = new Text[CharacterSelectMenu.SELECTION_COUNT];
            public void Draw(RenderTarget target, RenderStates states)
            {
                target.Draw(_PlayerText);
                if (Type == Type.Undefined)
                    target.Draw(_JoinText, states);
                else
                    target.Draw(_SelectionText[Selection], states);
            }
        }

        class ControllerPlayer : Player
        {
            public ControllerPlayer(uint controllerId, uint id, Renderer renderer, List<uint> LockedList) : base(Type.Controller, id, renderer, LockedList)
            {
                ControllerId = controllerId;
            }
        }

        class KeyboardPlayer : Player
        {
            public KeyboardPlayer(uint keyboardId, uint id, Renderer renderer, List<uint> LockedList) : base(Type.Keyboard, id, renderer, LockedList)
            {
                if (keyboardId > 2 || keyboardId < 1)
                    throw new ArgumentOutOfRangeException("Keyboard Id can only be 1 or 2");
                KeyboardId = keyboardId;
            }
        }
    }

    class CharacterSelectMenu : Menu, Drawable
    {
        public static uint SELECTION_COUNT = 4;

        public Players.Player[] Players { get; private set; }

        private Renderer _Renderer;

        public CharacterSelectMenu(Renderer renderer, InputManager.InputManager inputManager) : base(inputManager)
        {
            _Renderer = renderer;

            Players = new Players.Player[SELECTION_COUNT];
            uint i = 0;
            while (i < SELECTION_COUNT)
            {
                Players[i] = new Players.Player(i, _Renderer);
                i++;
            }

            Buttons.IButton returnButton = new Buttons.BackButton("", Factories.FontFactory.FontFactory.Instance.getFont(Factories.FontFactory.FontFactory.Font.ReenieBeanie), 0, this);
            MenuItem back_Button = new MenuItem(returnButton, MenuItem.ItemType.KeyBinded, InputManager.Keyboard.Key.Escape);
            _menuList.Add(back_Button);

            _InputManager.JoystickButtonPressed += _InputManager_JoystickButtonPressed;
            _InputManager.JoystickMoved += _InputManager_JoystickMoved;

            _InputManager.KeyboardKeyPressed += _InputManager_KeyboardKeyPressed;
        }

        public override void Show()
        {
            uint i = 0;
            while (i < SELECTION_COUNT)
            {
                if (Players[i].Type != Menus.Players.Type.Undefined)
                    Players[i] = new Players.Player(i, _Renderer);
                i++;
            }
            base.Show();
        }

        private void _InputManager_KeyboardKeyPressed(object source, InputManager.Keyboard.KeyEventArgs e)
        {
            if (Displayed)
            {
                List<uint> LockedList = GetLockedList();

                if (e.KeyCode == InputManager.Keyboard.Key.Space || e.KeyCode == InputManager.Keyboard.Key.Z)
                {
                    if (Players.FirstOrDefault(x => x.Type == Menus.Players.Type.Keyboard && x.KeyboardId == 1) == null) // Joining
                        JoinKeyboardPlayer(source, e, 1, LockedList);
                    else
                    {
                        Players.Player currentPlayer = Players.First(x => x.Type == Menus.Players.Type.Keyboard && x.KeyboardId == 1);
                        if (!currentPlayer.LockedChoice)
                            ValidatePlayer(currentPlayer, LockedList);
                        else
                            ReturnPlayer(currentPlayer);
                    }
                }
                else if (e.KeyCode == InputManager.Keyboard.Key.BackSpace || e.KeyCode == InputManager.Keyboard.Key.S)
                    ReturnPlayer(Players.FirstOrDefault(x => x.Type == Menus.Players.Type.Keyboard && x.KeyboardId == 1));
                else if (e.KeyCode == InputManager.Keyboard.Key.Q)
                    SelectPreviousPlayer(Players.FirstOrDefault(x => x.Type == Menus.Players.Type.Keyboard && x.KeyboardId == 1), LockedList);
                else if (e.KeyCode == InputManager.Keyboard.Key.D)
                    SelectNextPlayer(Players.FirstOrDefault(x => x.Type == Menus.Players.Type.Keyboard && x.KeyboardId == 1), LockedList);
            }
        }

        private void _InputManager_JoystickMoved(object source, InputManager.Joystick.MoveEventArgs e)
        {
            List<uint> LockedList = GetLockedList();

            if (Displayed && (e.Axis == InputManager.Joystick.Axis.X || e.Axis == InputManager.Joystick.Axis.PovX || e.Axis == InputManager.Joystick.Axis.U))
            {
                if (e.Position > 99)
                    SelectNextPlayer(Players.FirstOrDefault(x => x.Type == Menus.Players.Type.Controller && x.ControllerId == e.JoystickId), LockedList);
                else if (e.Position < -99)
                    SelectPreviousPlayer(Players.FirstOrDefault(x => x.Type == Menus.Players.Type.Controller && x.ControllerId == e.JoystickId), LockedList);
            }
        }

        private List<uint> GetLockedList()
        {
            List<uint> LockedList = new List<uint>();
            uint locked_i = 0;
            while (locked_i < SELECTION_COUNT)
            {
                if (Players[locked_i].Type != Menus.Players.Type.Undefined && Players[locked_i].LockedChoice)
                    LockedList.Add(Players[locked_i].Selection);
                locked_i++;
            }

            return LockedList;
        }

        private void _InputManager_JoystickButtonPressed(object source, InputManager.Joystick.ButtonEventArgs e)
        {
            if (Displayed)
            {
                List<uint> LockedList = GetLockedList();

                if (e.Button == 0) // A Button
                {
                    if (Players.FirstOrDefault(x => x.Type == Menus.Players.Type.Controller && x.ControllerId == e.JoystickId) == null) // Joining
                        JoinControllerPlayer(source, e, LockedList);
                    else
                        ValidatePlayer(Players.First(x => x.Type == Menus.Players.Type.Controller && x.ControllerId == e.JoystickId), LockedList);
                }
                else if (e.Button == 1)
                    ReturnPlayer(Players.FirstOrDefault(x => x.Type == Menus.Players.Type.Controller && x.ControllerId == e.JoystickId));
                else if (e.Button == 4)
                    SelectPreviousPlayer(Players.FirstOrDefault(x => x.Type == Menus.Players.Type.Controller && x.ControllerId == e.JoystickId), LockedList);
                else if (e.Button == 5)
                    SelectNextPlayer(Players.FirstOrDefault(x => x.Type == Menus.Players.Type.Controller && x.ControllerId == e.JoystickId), LockedList);
            }
        }

        private void JoinControllerPlayer(object source, InputManager.Joystick.ButtonEventArgs e, List<uint> LockedList)
        {
            uint i = 0;
            while (i < 4 && Players[i] != null && Players[i].Type != Menus.Players.Type.Undefined)
                i++;
            if (i < 4) // Team is not full
            {
                System.Diagnostics.Debug.WriteLine("NEW PLAYER JOINED");
                Players[i] = new Players.ControllerPlayer(e.JoystickId, i, _Renderer, LockedList);
            }
        }
        private void JoinKeyboardPlayer(object source, InputManager.Keyboard.KeyEventArgs e, uint keyboardId, List<uint> LockedList)
        {
            uint i = 0;
            while (i < 4 && Players[i] != null && Players[i].Type != Menus.Players.Type.Undefined)
                i++;
            if (i < 4) // Team is not full
            {
                System.Diagnostics.Debug.WriteLine("NEW PLAYER JOINED");
                Players[i] = new Players.KeyboardPlayer(keyboardId, i, _Renderer, LockedList);
            }
        }

        private void SelectPreviousPlayer(Players.Player currentPlayer, List<uint> LockedList)
        {
            if (currentPlayer != null)
                currentPlayer.SelectPrevious(LockedList);
        }
        private void SelectNextPlayer(Players.Player currentPlayer, List<uint> LockedList)
        {
            if (currentPlayer != null)
                currentPlayer.SelectNext(LockedList);
        }

        private void ReturnPlayer(Players.Player currentPlayer)
        {
            int id = Array.IndexOf(Players, currentPlayer);
            if (id != -1 && Players[id].Type != Menus.Players.Type.Undefined)
            {
                if (Players[id].LockedChoice == true)
                    Players[id].LockSelection(false);
                else
                    Players[id] = new Players.Player((uint)id, _Renderer);
            }
        }
        private void ValidatePlayer(Players.Player currentPlayer, List<uint> LockedList)
        {
            currentPlayer.LockSelection(true);
            LockedList.Add(currentPlayer.Selection);
            uint i = 0;
            while (i < SELECTION_COUNT)
            {
                if (Players[i].Type != Menus.Players.Type.Undefined && Players[i].LockedChoice == false)
                {
                    while (LockedList.Contains(Players[i].Selection))
                        Players[i].SelectNext(LockedList);
                }
                i++;
            }
        }

        public new void Draw(RenderTarget target, RenderStates states)
        {
            uint i = 0;
            while (i < SELECTION_COUNT)
            {
                target.Draw(Players[i]);
                i++;
            }
            base.Draw(target, states);
        }
    }
}
