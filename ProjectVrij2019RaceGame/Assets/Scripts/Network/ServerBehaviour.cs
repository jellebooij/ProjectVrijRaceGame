using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;

using Unity.Collections;
using System.Collections.Generic;

using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public enum packetTypes { PlayerConnected, SetupConnection, UpdatePlayer, RequestTime, ServerTime,
    PlayerDisconected, MachineGunFire, Damage, PlayerDied, ActivateShield, AssignPostion, AddPowerup, RemovePowerup, Last }

public class ServerBehaviour : MonoBehaviour
{   
    public UdpNetworkDriver m_Driver;
    public float time;
    private Dictionary<int, NetworkConnection> m_Connections = new Dictionary<int, NetworkConnection>();
    private NetworkPipeline relieablePipeline;
    private NetworkPipeline unrelieablePipeline;
    private PacketHandler packetHandler;
    public Transform[] spawns;


    public Transform[] powerupSpawns;
    List<float> powerupCountdown = new List<float>();
    private List<int> alivePlayersID = new List<int>();



    public int connectedPlaters;
    public int alivePlayers;

    int powerupID;

    float t = 0;

    int spawnIndex = 0;

    private List<int> IDs = new List<int>();
    private Dictionary<int, float> timeSinceLastPacked = new Dictionary<int, float>();

    void Start () {

        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);

        if (m_Driver.Bind(NetworkEndPoint.Parse("10.3.21.103", 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
            m_Driver.Listen();

        //relieablePipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        //unrelieablePipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));

        packetHandler = new PacketHandler();
        packetHandler.RegisterHandler(packetTypes.UpdatePlayer, UpdatePlayer);
        packetHandler.RegisterHandler(packetTypes.RequestTime,RespondTime);
        packetHandler.RegisterHandler(packetTypes.MachineGunFire, MachineGunFire);
        packetHandler.RegisterHandler(packetTypes.Damage, Damage);
        packetHandler.RegisterHandler(packetTypes.PlayerDied, PlayerDied);
        packetHandler.RegisterHandler(packetTypes.ActivateShield, ActivateShield);
        packetHandler.RegisterHandler(packetTypes.RemovePowerup, RemovePowerup);

        for(int i = 0; i < powerupSpawns.Length; i++)
        {
            powerupCountdown.Add(0);
        }

    }
    
    void OnDestroy() {

        m_Driver.Dispose();

    }
    

    void Update () {

        time += Time.deltaTime;
        CheckDisconnect();
        m_Driver.ScheduleUpdate().Complete();
        UpdatePowerup();

        if (connectedPlaters >= 2 && alivePlayersID.Count < 2)
        {
            StartGame();
        }
        
        // AcceptNewConnections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            
            Debug.Log("Accepted a connection");

            int id = GetNewID();
            

            foreach (KeyValuePair<int,NetworkConnection> pair in m_Connections){

                using (DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp))
                {

                    writer.Write((int)packetTypes.PlayerConnected);
                    writer.Write(id);
                    m_Driver.Send(NetworkPipeline.Null, pair.Value, writer);
                    Debug.Log("player" + id + "connectedToServer");
                      
                }

            }

            m_Connections.Add(id,c);
            connectedPlaters++;
            SetupConnection conn = new SetupConnection(id, IDs.Count, IDs.ToArray());
            m_Driver.Send(relieablePipeline,c,conn.Write());
   
        }
        
        DataStreamReader stream;
        foreach (KeyValuePair<int,NetworkConnection> value in m_Connections)
        {
            if (!value.Value.IsCreated)
                Debug.Log("Unity is kut");
            
            NetworkEvent.Type cmd;

            
            
            while ((cmd = m_Driver.PopEventForConnection(value.Value, out stream)) !=
                   NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    packetHandler.ProcessPacket(stream);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connectedPlaters--;

                    int disconnectedId = value.Key;
                    bool found = false;

                    if (alivePlayersID.Contains(disconnectedId) && found)
                    {
                        alivePlayersID.Remove(disconnectedId);
                    }

                    m_Connections.Remove(disconnectedId);
                    IDs.Remove(disconnectedId);
                    timeSinceLastPacked.Remove(disconnectedId);

                }
            }
        }

    }

    void StartGame()
    {

        alivePlayers = connectedPlaters;
        alivePlayersID.Clear();

        foreach (KeyValuePair<int,NetworkConnection> value in m_Connections)
        {

            AssignPosition(value.Key, spawns[spawnIndex]);
            alivePlayersID.Add(value.Key);

            spawnIndex++;

            if (spawnIndex == spawns.Length)
                spawnIndex = 0;

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
                    m_Connections.Remove(disId);

                    foreach (KeyValuePair<int,NetworkConnection> value in m_Connections)
                    {

                        if (value.Key == disId)
                            continue;

                        using (DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp))
                        {

                            writer.Write((int)packetTypes.PlayerDisconected);
                            writer.Write(disId);
                            m_Driver.Send(relieablePipeline, value.Value, writer);

                        }

                    }

                    connectedPlaters--;
                    alivePlayers--;
                    IDs.Remove(disId);

                    if (alivePlayersID.Contains(disId))
                    {
                        alivePlayersID.Remove(disId);
                    }

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

        m_Driver.Send(unrelieablePipeline, m_Connections[packet.netID], writer);

    }

    void AssignPosition(int carID, Transform position)
    {

        if (m_Connections.ContainsKey(carID))
        {
            AssignPositionPacked packed = new AssignPositionPacked(position.position, position.rotation);
            m_Driver.Send(relieablePipeline, m_Connections[carID], packed.Write() );
        }
    }

    void ActivateShield(DataStreamReader stream, ref DataStreamReader.Context context)
    {

        ActivateShieldPackage packed = new ActivateShieldPackage();
        packed.Read(stream, ref context);

        foreach (KeyValuePair<int, NetworkConnection> value in m_Connections)
        {
            if (value.Key == packed.netID)
                continue;

            m_Driver.Send(relieablePipeline, value.Value, packed.Write());
        }

    }

    void RemovePowerup(DataStreamReader stream, ref DataStreamReader.Context context)
    {

        RemovePowerup packed = new RemovePowerup();
        packed.Read(stream, ref context);

        foreach (KeyValuePair<int, NetworkConnection> value in m_Connections)
        {
            m_Driver.Send(relieablePipeline, value.Value, packed.Write());
        }

    }



    void MachineGunFire(DataStreamReader stream, ref DataStreamReader.Context context)
    {

        MachineGunFirePacked packed = new MachineGunFirePacked();
        packed.Read(stream, ref context);

        foreach (KeyValuePair<int, NetworkConnection> value in m_Connections)
        {
            if (value.Key == packed.netID)
                continue;

            m_Driver.Send(relieablePipeline, value.Value, packed.Write());
         }

    }

    void PlayerDied(DataStreamReader stream, ref DataStreamReader.Context context)
    {
        PlayerDiedPackage packed = new PlayerDiedPackage();
        packed.Read(stream, ref context);
        alivePlayers--;

        if (alivePlayersID.Contains(packed.netID))
        {
            alivePlayersID.Remove(packed.netID);
        }

        foreach (KeyValuePair<int, NetworkConnection> value in m_Connections)
        {

            if (value.Key == packed.netID)
                continue;

            m_Driver.Send(relieablePipeline, value.Value, packed.Write());

        }
    }

    void Damage(DataStreamReader stream, ref DataStreamReader.Context context)
    {

        TakeDamage packed = new TakeDamage();
        packed.Read(stream, ref context);

        foreach (KeyValuePair<int, NetworkConnection> value in m_Connections)
        {
            m_Driver.Send(relieablePipeline, value.Value, packed.Write());
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
        
        foreach(KeyValuePair<int,NetworkConnection> value in m_Connections){
            m_Driver.Send(unrelieablePipeline, value.Value, writer);
        }

    }

    void SpawnPowerup(Vector3 position)
    {

        AddPowerup package = new AddPowerup(powerupID, position);
        powerupID++;

        foreach (KeyValuePair<int, NetworkConnection> value in m_Connections)
        {
            m_Driver.Send(relieablePipeline, value.Value, package.Write());
        }

    }

    void UpdatePowerup()
    {

        for(int i = 0; i < powerupCountdown.Count; i++)
        {
            powerupCountdown[i] -= Time.deltaTime;

            if(powerupCountdown[i] < 0)
            {
                powerupCountdown[i] = 15;
                SpawnPowerup(powerupSpawns[i].position);
            }

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