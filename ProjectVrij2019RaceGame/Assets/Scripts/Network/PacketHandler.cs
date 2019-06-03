using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public delegate void ReadPacketFunction(DataStreamReader reader, ref DataStreamReader.Context context);

public class PacketHandler
{
    private Dictionary<packetTypes,ReadPacketFunction> packetFunctions = new Dictionary<packetTypes,ReadPacketFunction>();

    public void ProcessPacket(DataStreamReader stream){

        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);
        packetTypes value = (packetTypes)stream.ReadInt(ref readerCtx);
        
        if(packetFunctions[value] != null)
            packetFunctions[value](stream, ref readerCtx);

    }

    public void RegisterHandler(packetTypes type, ReadPacketFunction function){
        
        packetFunctions[type] = function;
    }

}
