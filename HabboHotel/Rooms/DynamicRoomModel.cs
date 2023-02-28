﻿using System;
using System.Text;

namespace Plus.HabboHotel.Rooms
{
    public class DynamicRoomModel
    {
        public bool ClubOnly;
        public int DoorOrientation;
        public int DoorX;
        public int DoorY;
        public double DoorZ;

        public string Heightmap;

        public int MapSizeX;
        public int MapSizeY;
        public short[,] SqFloorHeight;
        public byte[,] SqSeatRot;
        public SquareState[,] SqState;
        private RoomModel _staticModel;

        private readonly string _relativeHeightmap;

        public DynamicRoomModel(RoomModel model)
        {
            _staticModel = model;
            DoorX = _staticModel.DoorX;
            DoorY = _staticModel.DoorY;
            DoorZ = _staticModel.DoorZ;

            DoorOrientation = _staticModel.DoorOrientation;
            Heightmap = _staticModel.Heightmap;

            MapSizeX = _staticModel.MapSizeX;
            MapSizeY = _staticModel.MapSizeY;
            ClubOnly = _staticModel.ClubOnly;

            _relativeHeightmap = string.Empty;

            SqState = new SquareState[MapSizeX, MapSizeY];
            SqFloorHeight = new short[MapSizeX, MapSizeY];
            SqSeatRot = new byte[MapSizeX, MapSizeY];

            for (int y = 0; y < MapSizeY; y++)
            {
                for (int x = 0; x < MapSizeX; x++)
                {
                    if (x > (_staticModel.MapSizeX - 1) || y > (_staticModel.MapSizeY - 1))
                    {
                        SqState[x, y] = SquareState.Blocked;
                    }
                    else
                    {
                        SqState[x, y] = _staticModel.SqState[x, y];
                        SqFloorHeight[x, y] = _staticModel.SqFloorHeight[x, y];
                        SqSeatRot[x, y] = _staticModel.SqSeatRot[x, y];
                    }
                }
            }

            var floorMap = new StringBuilder();

            for (int y = 0; y < MapSizeY; y++)
            {
                for (int x = 0; x < MapSizeX; x++)
                {
                    if (x == DoorX && y == DoorY)
                    {
                        floorMap.Append(DoorZ > 9 ? ((char) (87 + DoorZ)).ToString() : DoorZ.ToString());
                        continue;
                    }

                    if (SqState[x, y] == SquareState.Blocked)
                    {
                        floorMap.Append('x');
                        continue;
                    }

                    double height = SqFloorHeight[x, y];
                    string val = height > 9 ? ((char) (87 + height)).ToString() : height.ToString();
                    floorMap.Append(val);
                }

                floorMap.Append(Convert.ToChar(13));
            }

            _relativeHeightmap = floorMap.ToString();
        }

        public void RefreshArrays()
        {
            var newSqState = new SquareState[MapSizeX + 1, MapSizeY + 1];
            var newSqFloorHeight = new short[MapSizeX + 1, MapSizeY + 1];
            var newSqSeatRot = new byte[MapSizeX + 1, MapSizeY + 1];

            for (int y = 0; y < MapSizeY; y++)
            {
                for (int x = 0; x < MapSizeX; x++)
                {
                    if (x > (_staticModel.MapSizeX - 1) || y > (_staticModel.MapSizeY - 1))
                    {
                        newSqState[x, y] = SquareState.Blocked;
                    }
                    else
                    {
                        newSqState[x, y] = SqState[x, y];
                        newSqFloorHeight[x, y] = SqFloorHeight[x, y];
                        newSqSeatRot[x, y] = SqSeatRot[x, y];
                    }
                }
            }

            SqState = newSqState;
            SqFloorHeight = newSqFloorHeight;
            SqSeatRot = newSqSeatRot;
        }

        public string GetRelativeHeightmap()
        {
            return _relativeHeightmap;
        }

        public void AddX()
        {
            MapSizeX++;
            RefreshArrays();
        }

        public void OpenSquare(int x, int y, double z)
        {
            if (z > 9)
                z = 9;
            if (z < 0)
                z = 0;
            SqFloorHeight[x, y] = (short) z;
            SqState[x, y] = SquareState.Open;
        }

        public void AddY()
        {
            MapSizeY++;
            RefreshArrays();
        }

        public bool DoorIsValid()
        {
            if (DoorX > SqFloorHeight.GetUpperBound(0) || DoorY > SqFloorHeight.GetUpperBound(1))
                return false;
            return true;
        }

        public void SetMapSize(int x, int y)
        {
            MapSizeX = x;
            MapSizeY = y;
            RefreshArrays();
        }

        public void Destroy()
        {
            Array.Clear(SqState, 0, SqState.Length);
            Array.Clear(SqFloorHeight, 0, SqFloorHeight.Length);
            Array.Clear(SqSeatRot, 0, SqSeatRot.Length);

            _staticModel = null;
            Heightmap = null;
            SqState = null;
            SqFloorHeight = null;
            SqSeatRot = null;
        }
    }
}