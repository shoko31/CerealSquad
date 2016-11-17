﻿using CerealSquad.GameWorld;
using CerealSquad.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CerealSquad.APlayer;

namespace CerealSquad
{
    abstract class AEnemy : AEntity
    {
        protected ARoom _room;

        public AEnemy(IEntity owner, s_position position, ARoom room) : base(owner)
        {
            _pos = position;
            _type = e_EntityType.Ennemy;
            _room = room;
        }

        public virtual void attack()
        {
            throw new NotImplementedException();
        }

        public abstract void think();

        public s_position getCoord(s_position pos)
        {
            double x = pos._x;
            double y = pos._y;

            x -= _room.Position.X;
            y -= _room.Position.Y;
            return (new s_position(x, y));
        }

        protected virtual void moveSameTile(WorldEntity world)
        {
            _move = EMovement.None;
            foreach (IEntity entity in world.getChildren())
            {
                if (entity.getEntityType() == e_EntityType.Player)
                {
                    if (entity.Pos._x == Pos._x && entity.Pos._y == Pos._y)
                    {
                        if (Math.Abs(entity.Pos._trueX - Pos._trueX) > 0.1)
                            _move = entity.Pos._trueX - Pos._trueX < 0 ? EMovement.Left : EMovement.Right;
                        else if (Math.Abs(entity.Pos._trueY - Pos._trueY) > 0.1)
                        {
                            _move = entity.Pos._trueY - Pos._trueY < 0 ? EMovement.Up : EMovement.Down;
                        }
                        else
                            _move = EMovement.None;
                    }

                }
            }
        }

        protected class scentMap
        {
            protected int[][][] _map;
            protected uint _x;
            protected uint _y;

            public int[][][] Map
            {
                get
                {
                    return _map;
                }

                set
                {
                    _map = value;
                }
            }

            public scentMap(uint x, uint y)
            {
                _x = x;
                _y = y;
            }

            protected s_position getCoord(s_position pos, s_Pos<int> room)
            {
                double x = pos._x;
                double y = pos._y;

                x -= room.X;
                y -= room.Y;
                return (new s_position(x, y));
            }

            protected void reset(ARoom room)
            {
                _map = new int[_x][][];
                for (uint i = 0; i < _x; i++)
                {
                    _map[i] = new int[_y][];
                    for (uint j = 0; j < _y; j++)
                    {
                        _map[i][j] = new int[4];
                        if (room.getPosition(i, j) == RoomParser.e_CellType.Normal)
                        {
                            _map[i][j][0] = 0;
                            _map[i][j][1] = 0;
                            _map[i][j][2] = 0;
                            _map[i][j][3] = 0;
                        }
                        else
                        {
                            _map[i][j][0] = -1;
                            _map[i][j][1] = -1;
                            _map[i][j][2] = -1;
                            _map[i][j][3] = -1;
                        }
                    }
                }
            }

            public void propagateHeat(int x, int y, int intensity, EName character, int characterWeight)
            {
                if (x >= 0 && x < _x && y >= 0 && y < _y && _map[x][y][(int)character] != -1 && _map[x][y][(int)character] < intensity * characterWeight)
                {
                        _map[x][y][(int)character] = intensity * characterWeight;
                        if (intensity > 1)
                        {
                            propagateHeat(x - 1, y, intensity - 1, character, characterWeight);
                            propagateHeat(x + 1, y, intensity - 1, character, characterWeight);
                            propagateHeat(x, y - 1, intensity - 1, character, characterWeight);
                            propagateHeat(x, y + 1, intensity - 1, character, characterWeight);
                        }
                }
            }

            public void update(WorldEntity world, ARoom room)
            {
                reset(room);
                foreach (IEntity entity in world.getChildren())
                {
                    if (entity.getEntityType() == e_EntityType.Player)
                    {
                        APlayer p = (APlayer)entity;
                        s_position pos = getCoord(p.Pos, room.Position);
                        propagateHeat(pos._x, pos._y, 100, p.getName(), p.Weight);
                    }
                }
            }

            public virtual int getScent(int x, int y)
            {
                if (x >= 0 && x < _x && y >= 0 && y < _y)
                {
                    int scent = 0;
                    if (_map[x][y][0] != -1)
                        scent += _map[x][y][0];
                    if (_map[x][y][1] != -1)
                        scent += _map[x][y][1];
                    if (_map[x][y][2] != -1)
                        scent += _map[x][y][2];
                    if (_map[x][y][3] != -1)
                        scent += _map[x][y][3];
                    return (scent);
                }
                return (-1);
            }

            public void dump()
            {
                for (int y = 0; y < _y; y++)
                {
                    for (int x = 0; x < _x; x++)
                    {
                        if (_map[x][y][0] == 100)
                            System.Console.Out.Write("J");
                        else if (_map[x][y][1] == 100)
                            System.Console.Out.Write("O");
                        else if (_map[x][y][2] == 100)
                            System.Console.Out.Write("M");
                        else if (_map[x][y][3] == 100)
                            System.Console.Out.Write("T");
                        else
                            System.Console.Out.Write(getScent(x, y));
                        System.Console.Out.Write(" ");
                    }
                    Console.Out.Write('\n');
                }
            }
        }
    }
}
