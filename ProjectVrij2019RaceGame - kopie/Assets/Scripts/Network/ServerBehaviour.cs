using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

using Unity.Collections;
using System.Collections.Generic;

using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public enum packetTypes { PlayerConnected, SetupConnection, UpdatePlayer, Test }

public class ServerBehaviour : MonoBehaviour
{   
    public UdpNetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;
    private NetworkPipeline relieablePipeline;
    private NetworkPipeline unrelieablePipeline;

    public Dictionary<int, PlayerTransform> players = new Dictionary<int, PlayerTransform>();
    

    void Start () {

        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);

        if (m_Driver.Bind(NetworkEndPoint.Parse("10.3.27.18", 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
            m_Driver.Listen();

        //relieablePipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        //unrelieablePipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }
    
    void OnDestroy() {

        m_Driver.Dispose();
        m_Connections.Dispose();

    }
    

    void Update () {

        m_Driver.ScheduleUpdate().Complete();
        
        // CleanUpConnections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }
        // AcceptNewConnections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            
            Debug.Log("Accepted a connection");

            m_Connections.Add(c);
        
            for(int i = 0; i < m_Connections.Length; i++){

                using (DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp))
                {
                    if(i == m_Connections.Length - 1)
                        continue;

                    writer.Write((int)packetTypes.PlayerConnected);
                    writer.Write(m_Connections.Length - 1);
                    m_Driver.Send(NetworkPipeline.Null, m_Connections[i], writer);
                      
                }

            }
            
            DataStreamWriter otherWriter = new DataStreamWriter(8, Allocator.Temp);
            otherWriter.Write((int)packetTypes.SetupConnection);
            otherWriter.Write(m_Connections.Length - 1);
            m_Driver.Send(relieablePipeline,c,otherWriter);
   
        }
        
        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
                Debug.Log("Unity is kut");
            
            NetworkEvent.Type cmd;
            
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) !=
                   NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    var readerCtx = default(DataStreamReader.Context);
                    packetTypes packetType = (packetTypes)stream.ReadInt(ref readerCtx);

                    switch(packetType){

                        case packetTypes.UpdatePlayer:

                            int id = stream.ReadInt(ref readerCtx);
                            Debug.Log(id);
                            float x = stream.ReadFloat(ref readerCtx);
                            float y = stream.ReadFloat(ref readerCtx);
                            float z = stream.ReadFloat(ref readerCtx);

                            float xr = stream.ReadFloat(ref readerCtx);
                            float yr = stream.ReadFloat(ref readerCtx);
                            float zr = stream.ReadFloat(ref readerCtx);
                            float wr = stream.ReadFloat(ref readerCtx);


                            SendPosition(id,x,y,z,xr ,yr ,zr,wr);
                                
                        break; 

                        case packetTypes.Test:
                            //Debug.Log("received from client" + "  " + stream.ReadInt(ref readerCtx));
                        break;

                    }
                    
                    //Debug.Log("Got " + number + " from the Client adding + 2 to it.");
                    //number +=2;
//
                    //using (var writer = new DataStreamWriter(4, Allocator.Temp))
                    //{
                    //    writer.Write(number);
                    //    m_Driver.Send(NetworkPipeline.Null, m_Connections[i], writer);
                    //}
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }



            for(int i = 0; i < m_Connections.Length; i++){

                using (DataStreamWriter w = new DataStreamWriter(8, Allocator.Temp))
                {
                    w.Write((int)packetTypes.Test);
                    m_Driver.Send(NetworkPipeline.Null, m_Connections[i], w);
                      
                }

            }
    }

    void SendPosition(int id, float x, float y, float z, float xr, float yr, float zr, float wr){

        var writer = new DataStreamWriter(36, Allocator.Temp);

        writer.Write((int)packetTypes.UpdatePlayer);

        writer.Write(id);
        writer.Write(x);
        writer.Write(y);
        writer.Write(z);

        writer.Write(xr);
        writer.Write(yr);
        writer.Write(zr);
        writer.Write(wr);

        for(int j = 0; j < m_Connections.Length; j++){
            m_Driver.Send(relieablePipeline, m_Connections[j], writer);
        }

    }
}
