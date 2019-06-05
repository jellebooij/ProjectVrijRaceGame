using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

public class TakeDamage : BasePacket
{
    public int damagedPlayerID;
    public float damage;


    public TakeDamage()
    {

    }

    public TakeDamage(int damagedPlayerID, float damage)
    {

        this.damagedPlayerID = damagedPlayerID;
        this.damage = damage;

    }

    public override DataStreamWriter Write()
    {

        DataStreamWriter writer = new DataStreamWriter(40, Allocator.Temp);

        writer.Write((int)packetTypes.Damage);

        writer.Write(damagedPlayerID);
        writer.Write(damage);

        return writer;

    }

    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        damagedPlayerID = reader.ReadInt(ref context);
        damage = reader.ReadFloat(ref context);

    }

}
