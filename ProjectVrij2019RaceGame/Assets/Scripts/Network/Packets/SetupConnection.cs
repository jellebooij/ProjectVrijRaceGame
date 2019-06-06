using System.Net;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

public class SetupConnection : BasePacket
{
    public int netID;
    public int connectedPlayerAmount;
    public int[] IDs;

    public SetupConnection() { }

    public SetupConnection(int netID, int connectedPlayerAmount, int[] IDs)
    {

        this.netID = netID;
        this.connectedPlayerAmount = connectedPlayerAmount;
        this.IDs = IDs;

    }
    public override DataStreamWriter Write()
    {

        DataStreamWriter writer = new DataStreamWriter(12 + connectedPlayerAmount * 4, Allocator.Temp);

        writer.Write((int)packetTypes.SetupConnection);

        writer.Write(netID);
        writer.Write(connectedPlayerAmount);

        for (int i = 0; i < IDs.Length; i++)
        {
            writer.Write(IDs[i]);
        }

        return writer;

    }
    public override void Read(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        netID = reader.ReadInt(ref context);
        connectedPlayerAmount = reader.ReadInt(ref context);
        IDs = new int[connectedPlayerAmount];

        for (int i = 0; i < IDs.Length; i++)
        {
            IDs[i] = reader.ReadInt(ref context);
        }

    }
}
