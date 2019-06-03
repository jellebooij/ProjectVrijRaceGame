using System.Net;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

public class ServerTimePacket : BasePacket
{
    public float localTime;
    public float serverTime;

    public ServerTimePacket(){}

    public ServerTimePacket(float serverTime, float localTime){

        this.localTime = localTime;
        this.serverTime = serverTime;

    }
    public override DataStreamWriter Write(){

        DataStreamWriter writer = new DataStreamWriter(12, Allocator.Temp);

        writer.Write((int)packetTypes.ServerTime);

        writer.Write(serverTime);
        writer.Write(localTime);
        

        return writer;

    }
    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context){

        serverTime = reader.ReadFloat(ref context);
        localTime = reader.ReadFloat(ref context);

    }
}
