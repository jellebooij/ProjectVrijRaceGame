using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

using Unity.Collections;
using System.Collections.Generic;

using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public enum packetTypes { PlayerConnected, SetupConnection, UpdatePlayer, RequestTime, ServerTime, Last }

public class ServerBehaviour : MonoBehaviour
{   
    public UdpNetworkDriver m_Driver;
    public float time;
    private NativeList<NetworkConnection> m_Connections;
    private NetworkPipeline relieablePipeline;
    private NetworkPipeline unrelieablePipeline;
    private PacketHandler packetHandler;

    void Start () {

        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);

        if (m_Driver.Bind(NetworkEndPoint.Parse("192.168.178.42", 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
            m_Driver.Listen();

        //relieablePipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        //unrelieablePipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        packetHandler = new PacketHandler();
        packetHandler.RegisterHandler(packetTypes.UpdatePlayer, UpdatePlayer);
        packetHandler.RegisterHandler(packetTypes.RequestTime,RespondTime);

    }
    
    void OnDestroy() {

        m_Driver.Dispose();
        m_Connections.Dispose();

    }
    

    void Update () {

        time += Time.deltaTime;

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
                    packetHandler.ProcessPacket(stream);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }

    }

    void RespondTime(DataStreamReader stream, ref DataStreamReader.Context context){
        
        RequestTimePacket packet = new RequestTimePacket();
        packet.Read(stream, ref context);

        ServerTimePacket returnPacket = new ServerTimePacket(time + Random.Range(-0.1f,0.1f), packet.localTime);
        var writer = returnPacket.Write();

        m_Driver.Send(unrelieablePipeline,m_Connections[packet.netID], writer);

    }

    void UpdatePlayer(DataStreamReader stream, ref DataStreamReader.Context context){

        BasePacket packet = new CarTransformPacked();
        packet.Read(stream, ref context);
        SendPosition(packet as CarTransformPacked);
                                
    }

    void SendPosition(CarTransformPacked packed){

        var writer = packed.Write();
        
        for(int j = 0; j < m_Connections.Length; j++){
            m_Driver.Send(unrelieablePipeline, m_Connections[j], writer);
        }

    }
}
