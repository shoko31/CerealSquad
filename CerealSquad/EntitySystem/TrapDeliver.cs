﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CerealSquad.AEntity;
using CerealSquad.Graphics;
using SFML.System;
using SFML.Graphics;

namespace CerealSquad.EntitySystem
{
    class TrapDeliver : Transformable, Drawable
    {
        public enum EStep
        {
            NOTHING             = 0,
            START_SELECTING     = 1,
            SELECTING           = 2,
            END_SELECTING       = 3,
            DELIVER             = 4
        }

        public EStep Step { get; private set; }
        public EMovement Target { get; private set; }
        public bool IsTargetValid { get; private set; }


        private AnimatedSprite Sprite;
        private APlayer Player;
        private Timer Timer = new Timer(Time.FromSeconds(1));

        public Time Cooldown { get { return Timer.Time; } set { Timer.Time = value; } }

        public TrapDeliver(APlayer player)
        {
            Player = player;
            Step = EStep.NOTHING;
            Target = EMovement.Up;
            Factories.TextureFactory.Instance.load("Cursor", "Assets/Effects/Cursor.png");
            Sprite = new AnimatedSprite(64, 64);
            Sprite.addAnimation(EStateEntity.IDLE, "Cursor", new List<uint> { 0, 1, 2, 3 }, new Vector2u(64, 64));
            Sprite.SetColor(Color.Green);
            Sprite.Speed = Time.FromMilliseconds(100);
            IsTargetValid = true;
        }

        public bool IsNotDelivering()
        {
            return Step.Equals(EStep.NOTHING);
        }

        public void Update(SFML.System.Time DeltaTime, GameWorld.AWorld World, List<EMovement> Input, bool TrapPressed)
        {
            Processing(Input, World, TrapPressed);
            Sprite.Update(DeltaTime);
        }

        private void Processing(List<EMovement> Input, GameWorld.AWorld World, bool TrapPressed)
        {
            // Player have nothing to put on map.
            if (Player.TrapInventory.Equals(e_TrapType.NONE))
                return;

            // Player can't put on map because of cooldown.
            if (!Timer.IsTimerOver())
                return;

            if (TrapPressed && Step == EStep.NOTHING)
                Step = EStep.START_SELECTING;
            else if (!TrapPressed && EStep.START_SELECTING == Step)
                Step = EStep.SELECTING;

            if (Step == EStep.START_SELECTING || Step == EStep.SELECTING)
            {
               Target = (Input.Count > 0) ? Input.ElementAt(Input.Count - 1) : EMovement.None;
                
                Vector2f pos = new Vector2f();
                if (Target.Equals(EMovement.Down))
                    pos = new Vector2f(Player.ressourcesEntity.Position.X, Player.ressourcesEntity.Position.Y + Player.ressourcesEntity.sprite.Size.Y);
                else if (Target.Equals(EMovement.Up))
                    pos = new Vector2f(Player.ressourcesEntity.Position.X, Player.ressourcesEntity.Position.Y - Player.ressourcesEntity.sprite.Size.Y);
                else if (Target.Equals(EMovement.Right))
                    pos = new Vector2f(Player.ressourcesEntity.Position.X + Player.ressourcesEntity.sprite.Size.X, Player.ressourcesEntity.Position.Y);
                else if (Target.Equals(EMovement.Left))
                    pos = new Vector2f(Player.ressourcesEntity.Position.X - Player.ressourcesEntity.sprite.Size.X, Player.ressourcesEntity.Position.Y);
                else
                    pos = new Vector2f(Player.ressourcesEntity.Position.X, Player.ressourcesEntity.Position.Y);

                Position = pos;
                //Position = new Vector2f(pos.X * 64, pos.Y * 64);
               // if (World.getPosition(pos.X, pos.Y) == GameWorld.RoomParser.e_CellType.Wall)
                //    IsTargetValid = false;
                //else
                    IsTargetValid = true;
                if (EMovement.None == Target)
                    IsTargetValid = false;
                
                Sprite.SetColor((IsTargetValid) ? Color.Green : Color.Red);
            }

            if (TrapPressed && Step == EStep.SELECTING)
            {
                if (Target != EMovement.None && IsTargetValid)
                {
                    //TODO check position of futur Trap

                    ATrap trap = Factories.TrapFactory.createTrap(Player, Player.TrapInventory);
                    trap.setPosition(Target);
                    Player.addChild(trap);
                }
                Step = EStep.END_SELECTING;
            }
            else if (!TrapPressed && Step == EStep.END_SELECTING)
            {
                Step = EStep.NOTHING;
                // Restart timer to launch cooldown
                Timer.Start();
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (Step > EStep.NOTHING)
            {
                states.Transform *= Transform;
                ((Drawable)Sprite).Draw(target, states);
            }
        }
    }
}