using System.Net;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

public class MachineGunFirePacked : BasePacket
{
    public int netID;
    public float timeStamp;

    public Vector3 bulletPosition;
    public Quaternion bulletRotation;

    public MachineGunFirePacked() { }

    public MachineGunFirePacked(int netID, Vector3 bulletPosition, Quaternion bulletRotation)
    {

        this.netID = netID;
        this.bulletPosition = bulletPosition;
        this.bulletRotation = bulletRotation;

    }

    public override DataStreamWriter Write()
    {

        DataStreamWriter writer = new DataStreamWriter(40, Allocator.Temp);

        writer.Write((int)packetTypes.MachineGunFire);

        writer.Write(netID);
        writer.Write(timeStamp);

        writer.Write(bulletPosition.x);
        writer.Write(bulletPosition.y);
        writer.Write(bulletPosition.z);

        writer.Write(bulletRotation.x);
        writer.Write(bulletRotation.y);
        writer.Write(bulletRotation.z);
        writer.Write(bulletRotation.w);

        return writer;

    }

    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        netID = reader.ReadInt(ref context);
        timeStamp = reader.ReadFloat(ref context);
        bulletPosition = new Vector3(reader.ReadFloat(ref context), reader.ReadFloat(ref context), reader.ReadFloat(ref context));
        bulletRotation = new Quaternion(reader.ReadFloat(ref context), reader.ReadFloat(ref context), reader.ReadFloat(ref context), reader.ReadFloat(ref context));

    }
}
