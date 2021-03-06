﻿using System;
using Infusion.Packets.Server;

namespace Infusion.Packets
{
    public struct Packet
    {
        public byte[] Payload { get; }

        public int Id { get; }

        public int Length => Payload.Length;

        public Packet(int id, byte[] payload)
        {
            Payload = payload;
            Id = id;
        }

        public Packet(byte[] payload)
        {
            Payload = payload;
            Id = payload[0];
        }

        public Packet Clone()
        {
            var clonedPayload = new Byte[Length];
            Payload.CopyTo(clonedPayload, 0);

            return new Packet(Id, clonedPayload);
        }
    }
}