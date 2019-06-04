using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

using Unity.Collections;
using System.Collections.Generic;

using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public enum packetTypes { PlayerConnected, SetupConnection, UpdatePlayer, RequestTime, ServerTime, PlayerDisconected ,Last }

public class ServerBehaviour : MonoBehaviour
{   
    public UdpNetworkDriver m_Driver;
    public float time;
    private NativeList<NetworkConnection> m_Connections;
    private Dictionary<NetworkConnection, int> idMap = new Dictionary<NetworkConnection, int>();
    private NetworkPipeline relieablePipeline;
    private NetworkPipeline unrelieablePipeline;
    private PacketHandler packetHandler;

    private List<int> IDs = new List<int>();

    void Start () {

        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);

        if (m_Driver.Bind(NetworkEndPoint.Parse("10.3.21.132", 9000)) != 0)
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

            int id = GetNewID();
            idMap.Add(c, id);


            for (int i = 0; i < m_Connections.Length; i++){

                using (DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp))
                {
                    if(i == m_Connections.Length - 1)
                        continue;

                    writer.Write((int)packetTypes.PlayerConnected);
                    writer.Write(id);
                    m_Driver.Send(NetworkPipeline.Null, m_Connections[i], writer);
                      
                }

            }
            
            SetupConnection conn = new SetupConnection(id, IDs.Count, IDs.ToArray());
            m_Driver.Send(relieablePipeline,c,conn.Write());
   
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
                    int disId = idMap[m_Connections[i]];
                    idMap.Remove(m_Connections[i]);
                    m_Connections[i] = default(NetworkConnection);

                    for (int j = 0; j < m_Connections.Length; j++)
                    {

                        using (DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp))
                        {
                            if (i == m_Connections.Length - 1)
                                continue;

                            writer.Write((int)packetTypes.PlayerConnected);
                            writer.Write(disId);
                            m_Driver.Send(NetworkPipeline.Null, m_Connections[i], writer);

                        }

                    }

                    IDs.Remove(disId);


                }
            }
        }

    }

    void RespondTime(DataStreamReader stream, ref DataStreamReader.Context context){
        
        RequestTimePacket packet = new RequestTimePacket();
        packet.Read(stream, ref context);

        ServerTimePacket returnPacket = new ServerTimePacket(time, packet.localTime);
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

    int GetNewID()
    {
        int id = 0;

        for(int i = 0; i < IDs.Count; i++)
        {
            if (IDs[i] != i)
            {
                id = i;
                IDs.Add(i);
                return id;
            }
        }

        return IDs.Count;
    }
}