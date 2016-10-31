﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using static CerealSquad.APlayer;

namespace CerealSquad
{
    class JackEnnemy : AEnemy
    {
        protected static JackEnnemyScentMap _scentMap;

        protected class JackEnnemyScentMap : scentMap
        {
            public JackEnnemyScentMap(uint x, uint y) : base(x, y)
            {
            }

            public override int getScent(int x, int y)
            {
                if (x > 0 && x < _x && y > 0 && y < _y)
                {
                    int scent = 0;
                    if (_map[x][y][(int)EName.Jack] != -1)
                        scent += _map[x][y][(int)EName.Jack];
                    return (scent);
                }
                return (-1);
            }
        }

        public JackEnnemy(IEntity owner, s_position position) : base(owner, position)
        {
            _speed = 0.1;
            _scentMap = new JackEnnemyScentMap(100, 100);
            _ressources = new SFMLImplementation.EntityResources("jackHunter", 32, 32);
        }

        //
        // TODO check egality in scent
        //
        public override void think()
        {
            int left = _scentMap.getScent(_pos._x - 1, _pos._y);
            int right = _scentMap.getScent(_pos._x + 1, _pos._y);
            int top = _scentMap.getScent(_pos._x, _pos._y - 1);
            int bottom = _scentMap.getScent(_pos._x, _pos._y + 1);
            int maxscent = Math.Max(top, Math.Max(bottom, Math.Max(right, left)));

            if (maxscent == 0)
                _move = EMovement.None;
            else if (maxscent == top)
                _move = EMovement.Up;
            else if (maxscent == bottom)
                _move = EMovement.Down;
            else if (maxscent == right)
                _move = EMovement.Right;
            else
                _move = EMovement.Left;
        }

        public override void update(Time deltaTime)
        {
            _scentMap.update((WorldEntity)_owner);
            think();
            _ressources.update(deltaTime);
            move();
        }
    }
}
