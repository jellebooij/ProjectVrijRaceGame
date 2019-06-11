using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

public class AssignPositionPacked : BasePacket
{

    public Vector3 postition;
    public Quaternion rotation;

    public AssignPositionPacked(){}

    public AssignPositionPacked(Vector3 postition, Quaternion rotation)
    {

        this.postition = postition;
        this.rotation = rotation;

    }

    public override DataStreamWriter Write()
    {

        DataStreamWriter writer = new DataStreamWriter(32, Allocator.Temp);

        writer.Write((int)packetTypes.AssignPostion);

        writer.Write(postition.x);
        writer.Write(postition.y);
        writer.Write(postition.z);

        writer.Write(rotation.x);
        writer.Write(rotation.y);
        writer.Write(rotation.z);
        writer.Write(rotation.w);

        return writer;

    }

    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        postition = new Vector3(reader.ReadFloat(ref context), reader.ReadFloat(ref context), reader.ReadFloat(ref context));
        rotation = new Quaternion(reader.ReadFloat(ref context), reader.ReadFloat(ref context), reader.ReadFloat(ref context), reader.ReadFloat(ref context));

    }

}
