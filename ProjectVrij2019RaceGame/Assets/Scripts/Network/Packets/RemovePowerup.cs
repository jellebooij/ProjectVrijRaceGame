using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

public class RemovePowerup : BasePacket
{
    public int powerupID;

    public RemovePowerup()
    {

    }

    public RemovePowerup(int powerupID, Vector3 postition)
    {

        this.powerupID = powerupID;

    }

    public override DataStreamWriter Write()
    {

        DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp);

        writer.Write((int)packetTypes.RemovePowerup);

        writer.Write(powerupID);

        return writer;

    }

    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        powerupID = reader.ReadInt(ref context);

    }

}
