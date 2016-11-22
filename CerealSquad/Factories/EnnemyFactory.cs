﻿using CerealSquad.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CerealSquad.Factories
{
    static class EnnemyFactory
    {
        public static IEntity CreateEnnemy(IEntity owner, s_Pos<int> pos, GameWorld.ARoom room, e_EnnemyType type)
        {
            IEntity result = null;
            switch (type)
            {
                case e_EnnemyType.Egg:
                    result = new EntitySystem.EggEnemy(owner, new s_position(pos.X, pos.Y), room);
                    break;
                case e_EnnemyType.Ghost:
                    result = new EntitySystem.GhostEnemy(owner, new s_position(pos.X, pos.Y), room);
                    break;
                case e_EnnemyType.HalfEgg:
                    result = new EntitySystem.HalfEggEnemy(owner, new s_position(pos.X, pos.Y), room);
                    break;
                case e_EnnemyType.RiceBowl:
                    result = new EntitySystem.RiceBowlEnemy(owner, new s_position(pos.X, pos.Y), room);
                    break;
                default:
                    throw new ArgumentException("Invalid Trap entity requested");
            }

            return result;
        }
    }
}
