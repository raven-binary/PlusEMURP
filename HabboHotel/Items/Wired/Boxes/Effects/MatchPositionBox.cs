﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using Plus.Communication.Packets.Incoming;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Core;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Wired.Boxes.Effects
{
    internal class MatchPositionBox : IWiredItem, IWiredCycle
    {
        private int _delay;
        public Room Instance { get; set; }

        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.EffectMatchPosition;

        public ConcurrentDictionary<int, Item> SetItems { get; set; }

        public string StringData { get; set; }

        public bool BoolData { get; set; }

        public int Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                TickCount = value + 1;
            }
        }

        public int TickCount { get; set; }

        private bool _requested;
        public string ItemsData { get; set; }

        public MatchPositionBox(Room instance, Item item)
        {
            Instance = instance;
            Item = item;
            SetItems = new ConcurrentDictionary<int, Item>();
            TickCount = Delay;
            _requested = false;
        }

        public void HandleSave(ClientPacket packet)
        {
            if (SetItems.Count > 0)
                SetItems.Clear();

            int unknown = packet.PopInt();
            int state = packet.PopInt();
            int direction = packet.PopInt();
            int placement = packet.PopInt();
            string unknown2 = packet.PopString();

            int furniCount = packet.PopInt();
            for (int i = 0; i < furniCount; i++)
            {
                Item selectedItem = Instance.GetRoomItemHandler().GetItem(packet.PopInt());
                if (selectedItem != null)
                    SetItems.TryAdd(selectedItem.Id, selectedItem);
            }

            StringData = state + ";" + direction + ";" + placement;

            int delay = packet.PopInt();
            Delay = delay;
        }

        public bool Execute(params object[] @params)
        {
            if (!_requested)
            {
                TickCount = Delay;
                _requested = true;
            }

            return true;
        }

        public bool OnCycle()
        {
            if (!_requested || string.IsNullOrEmpty(StringData) || StringData == "0;0;0" || SetItems.Count == 0)
                return false;

            foreach (Item item in SetItems.Values.ToList())
            {
                if (Instance.GetRoomItemHandler().GetFloor == null && !Instance.GetRoomItemHandler().GetFloor.Contains(item))
                    continue;

                foreach (string I in ItemsData.Split(';'))
                {
                    if (string.IsNullOrEmpty(I))
                        continue;

                    int itemId = Convert.ToInt32(I.Split(':')[0]);
                    Item ii = Instance.GetRoomItemHandler().GetItem(Convert.ToInt32(itemId));
                    if (ii == null)
                        continue;

                    string[] partsString = I.Split(':');
                    try
                    {
                        if (string.IsNullOrEmpty(partsString[0]) || string.IsNullOrEmpty(partsString[1]))
                            continue;
                    }
                    catch
                    {
                        continue;
                    }

                    string[] part = partsString[1].Split(',');

                    try
                    {
                        if (int.Parse(StringData.Split(';')[0]) == 1) //State
                        {
                            if (part.Count() >= 4)
                                SetState(ii, part[4]);
                            else
                                SetState(ii, "1");
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.LogWiredException(e);
                    }

                    try
                    {
                        if (int.Parse(StringData.Split(';')[1]) == 1) //Direction
                            SetRotation(ii, Convert.ToInt32(part[3]));
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.LogWiredException(e);
                    }

                    try
                    {
                        if (int.Parse(StringData.Split(';')[2]) == 1) //Position
                            SetPosition(ii, Convert.ToInt32(part[0]), Convert.ToInt32(part[1]), Convert.ToDouble(part[2]));
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.LogWiredException(e);
                    }
                }
            }

            _requested = false;
            return true;
        }

        private void SetState(Item item, string extradata)
        {
            if (item.ExtraData == extradata)
                return;

            if (item.GetBaseItem().InteractionType == InteractionType.Dice)
                return;

            item.ExtraData = extradata;
            item.UpdateState(false, true);
        }

        private void SetRotation(Item item, int rotation)
        {
            if (item.Rotation == rotation)
                return;

            item.Rotation = rotation;
            item.UpdateState(false, true);
        }

        private void SetPosition(Item item, int coordX, int coordY, double coordZ)
        {
            Instance.SendPacket(new SlideObjectBundleComposer(item.GetX, item.GetY, item.GetZ, coordX, coordY, coordZ, 0, 0, item.Id));

            Instance.GetRoomItemHandler().SetFloorItem(item, coordX, coordY, coordZ);
            //Instance.GetGameMap().GenerateMaps();
        }
    }
}