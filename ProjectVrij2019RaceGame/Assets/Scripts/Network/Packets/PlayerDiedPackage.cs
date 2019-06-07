
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class PlayerDiedPackage : BasePacket
{
    public int netID;

    public PlayerDiedPackage() { }

    public PlayerDiedPackage(int netID)
    {

        this.netID = netID;

    }

    public override DataStreamWriter Write()
    {

        DataStreamWriter writer = new DataStreamWriter(40, Allocator.Temp);

        writer.Write((int)packetTypes.PlayerDied);

        writer.Write(netID);

        return writer;

    }

    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        netID = reader.ReadInt(ref context);

    }
}
