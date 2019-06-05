using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

using Unity.Collections;
using System.Collections.Generic;

using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public enum packetTypes { PlayerConnected, SetupConnection, UpdatePlayer, RequestTime, ServerTime, PlayerDisconected, MachineGunFire, Damage, Last }

public class ServerBehaviour : MonoBehaviour
{   
    public UdpNetworkDriver m_Driver;
    public float time;
    private List<NetworkConnection> m_Connections;
    private Dictionary<int, int> idMap = new Dictionary<int, int>();
    private NetworkPipeline relieablePipeline;
    private NetworkPipeline unrelieablePipeline;
    private PacketHandler packetHandler;

    private List<int> IDs = new List<int>();
    private Dictionary<int, float> timeSinceLastPacked = new Dictionary<int, float>();

    void Start () {

        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);

        if (m_Driver.Bind(NetworkEndPoint.Parse("10.3.21.132", 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
            m_Driver.Listen();

        //relieablePipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        //unrelieablePipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));

        m_Connections = new List<NetworkConnection>();

        packetHandler = new PacketHandler();
        packetHandler.RegisterHandler(packetTypes.UpdatePlayer, UpdatePlayer);
        packetHandler.RegisterHandler(packetTypes.RequestTime,RespondTime);
        packetHandler.RegisterHandler(packetTypes.MachineGunFire, MachineGunFire);
        packetHandler.RegisterHandler(packetTypes.Damage, Damage);

    }
    
    void OnDestroy() {

        m_Driver.Dispose();

    }
    

    void Update () {

        time += Time.deltaTime;
        CheckDisconnect();
        m_Driver.ScheduleUpdate().Complete();
        
        // CleanUpConnections
        for (int i = 0; i < m_Connections.Count; i++)
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

            Debug.Log(id);
            idMap.Add(id, m_Connections.Count - 1);


            for (int i = 0; i < m_Connections.Count; i++){

                using (DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp))
                {
                    if(i == m_Connections.Count - 1)
                        continue;

                    writer.Write((int)packetTypes.PlayerConnected);
                    writer.Write(id);
                    m_Driver.Send(NetworkPipeline.Null, m_Connections[i], writer);
                    Debug.Log("player" + id + "connectedToServer");
                      
                }

            }
            
            SetupConnection conn = new SetupConnection(id, IDs.Count, IDs.ToArray());
            m_Driver.Send(relieablePipeline,c,conn.Write());
   
        }
        
        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Count; i++)
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
                   


                }
            }
        }

    }

    void CheckDisconnect()
    {
        for(int i = 0; i < IDs.Count; i++)
        {
            if (timeSinceLastPacked.ContainsKey(IDs[i]))
            {

                timeSinceLastPacked[IDs[i]] += Time.deltaTime;

                if (timeSinceLastPacked[IDs[i]] > 2)
                {

                    int disId = IDs[i];
                    m_Connections[idMap[disId]] = default(NetworkConnection);

                    for (int j = 0; j < m_Connections.Count; j++)
                    {

                        if (j == idMap[disId])
                            continue;

                        using (DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp))
                        {

                            writer.Write((int)packetTypes.PlayerDisconected);
                            writer.Write(disId);
                            m_Driver.Send(relieablePipeline, m_Connections[j], writer);

                        }

                    }

                    IDs.Remove(disId);
                    idMap.Remove(disId);
                    timeSinceLastPacked.Remove(disId);
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

    void MachineGunFire(DataStreamReader stream, ref DataStreamReader.Context context)
    {

        MachineGunFirePacked packed = new MachineGunFirePacked();
        packed.Read(stream, ref context);

         for(int j =0; j<m_Connections.Count; j++)
         {
            m_Driver.Send(unrelieablePipeline, m_Connections[j], packed.Write());
         }

    }

    void Damage(DataStreamReader stream, ref DataStreamReader.Context context)
    {

        TakeDamage packed = new TakeDamage();
        packed.Read(stream, ref context);

        for (int j = 0; j < m_Connections.Count; j++)
        {
            m_Driver.Send(unrelieablePipeline, m_Connections[j], packed.Write());
        }

    }


    void UpdatePlayer(DataStreamReader stream, ref DataStreamReader.Context context){

        BasePacket packet = new CarTransformPacked();
        packet.Read(stream, ref context);
        
        if(!timeSinceLastPacked.ContainsKey((packet as CarTransformPacked).netID)){
            timeSinceLastPacked.Add((packet as CarTransformPacked).netID,0);
        }

        timeSinceLastPacked[(packet as CarTransformPacked).netID] = 0;

        SendPosition(packet as CarTransformPacked);
                                
    }

    void SendPosition(CarTransformPacked packed){

        var writer = packed.Write();
        
        for(int j = 0; j < m_Connections.Count; j++){
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

        id = IDs.Count;
        IDs.Add(id);
        return id;
    }
}