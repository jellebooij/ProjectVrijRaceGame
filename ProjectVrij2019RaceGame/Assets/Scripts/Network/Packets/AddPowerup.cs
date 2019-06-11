﻿using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

public class AddPowerup : BasePacket
{
    public int powerupID;
    public Vector3 postition;

    public AddPowerup()
    {

    }

    public AddPowerup(int powerupID, Vector3 postition)
    {

        this.powerupID = powerupID;
        this.postition = postition;

    }

    public override DataStreamWriter Write()
    {

        DataStreamWriter writer = new DataStreamWriter(20, Allocator.Temp);

        writer.Write((int)packetTypes.AddPowerup);

        writer.Write(powerupID);

        writer.Write(postition.x);
        writer.Write(postition.y);
        writer.Write(postition.z);

        return writer;

    }

    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        powerupID = reader.ReadInt(ref context);
        postition = new Vector3(reader.ReadFloat(ref context), reader.ReadFloat(ref context), reader.ReadFloat(ref context));

    }

}
