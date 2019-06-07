
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class ActivateShieldPackage : BasePacket
{
    public int netID;

    public ActivateShieldPackage() { }

    public ActivateShieldPackage(int netID)
    {

        this.netID = netID;

    }

    public override DataStreamWriter Write()
    {

        DataStreamWriter writer = new DataStreamWriter(40, Allocator.Temp);

        writer.Write((int)packetTypes.ActivateShield);

        writer.Write(netID);

        return writer;

    }

    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        netID = reader.ReadInt(ref context);

    }
}
