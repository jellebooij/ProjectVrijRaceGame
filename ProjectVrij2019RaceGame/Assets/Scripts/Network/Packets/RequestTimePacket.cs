using System.Net;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

public class RequestTimePacket : BasePacket
{   
    public int netID;
    public float localTime;

    public RequestTimePacket(){}

    public RequestTimePacket(int netID, float localTime){

        this.localTime = localTime;
        this.netID = netID;

    }
    public override DataStreamWriter Write(){

        DataStreamWriter writer = new DataStreamWriter(12, Allocator.Temp);

        writer.Write((int)packetTypes.RequestTime);

        writer.Write(netID);
        writer.Write(localTime);
        

        return writer;

    }
    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context){

        netID = reader.ReadInt(ref context);
        localTime = reader.ReadFloat(ref context);

    }
}
