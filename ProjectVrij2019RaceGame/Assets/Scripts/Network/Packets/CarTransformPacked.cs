using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

public class CarTransformPacked : BasePacket
{   
    public int netID;
    public float timeStamp;
    public Vector3 postition;
    public Quaternion rotation;

    public CarTransformPacked(){
       
    }

    public CarTransformPacked(int carID, float timeStamp, Vector3 postition, Quaternion rotation){

        netID = carID;
        this.timeStamp = timeStamp;
        this.postition = postition;
        this.rotation = rotation; 

    }

    public override DataStreamWriter Write(){

        DataStreamWriter writer = new DataStreamWriter(40, Allocator.Temp);

        writer.Write((int)packetTypes.UpdatePlayer);

        writer.Write(netID);
        writer.Write(timeStamp);

        writer.Write(postition.x);
        writer.Write(postition.y);
        writer.Write(postition.z);

        writer.Write(rotation.x);
        writer.Write(rotation.y);
        writer.Write(rotation.z);
        writer.Write(rotation.w);

        return writer;

    }

    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context){

        netID = reader.ReadInt(ref context);
        timeStamp = reader.ReadFloat(ref context);
        postition = new Vector3(reader.ReadFloat(ref context),reader.ReadFloat(ref context),reader.ReadFloat(ref context));
        rotation = new Quaternion(reader.ReadFloat(ref context),reader.ReadFloat(ref context),reader.ReadFloat(ref context),reader.ReadFloat(ref context));

    }

}
