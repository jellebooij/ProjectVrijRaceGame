using System.Net;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.Networking.Transport;
using Unity.Networking.Transport.LowLevel.Unsafe;
using Unity.Networking.Transport.Utilities;


using NetworkConnection = Unity.Networking.Transport.NetworkConnection; 


public class ClientBehaviour : MonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public float time;
    public bool m_Done;
    public GameObject playerPrefab;
    private NetworkPipeline relieablePipeline;
    private NetworkPipeline unrelieablePipeline;
    private PacketHandler packetHandler;
    public Dictionary<int, Transform> transforms = new Dictionary<int, Transform>();

    TransformList packets;

    public Transform player;
    public float msIntervall;
    int networkId;

    bool connected;

    float t = 0;

    
    void Start ()
	{   

        Screen.fullScreen = false;

        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);
        m_Connection = default(NetworkConnection);
        
        //relieablePipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        //unrelieablePipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));

        var endpoint = NetworkEndPoint.Parse("192.168.178.42", 9000);
        m_Connection = m_Driver.Connect(endpoint);
        
        packetHandler = new PacketHandler();

        packetHandler.RegisterHandler(packetTypes.UpdatePlayer, UpdatePlayer);
        packetHandler.RegisterHandler(packetTypes.PlayerConnected, PlayerConnected);
        packetHandler.RegisterHandler(packetTypes.SetupConnection, SetupConnection);
        packetHandler.RegisterHandler(packetTypes.ServerTime, ReceiveTime);

        packets = new TransformList();

    }
    
    public void OnDestroy()
    {
        m_Driver.Dispose();
    }
    
    void Update()
    {

        time += Time.deltaTime;

        if(connected)
            UpdateWorldState();

        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!m_Done)
                Debug.Log("Something went wrong during connect");
            return;
        }
        
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        
        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != 
               NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");

                var readerCtx = default(DataStreamReader.Context);
                connected = true;

            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                packetHandler.ProcessPacket(stream);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                m_Connection = default(NetworkConnection);
            }
        }


        t += Time.deltaTime * 1000;
        if(connected && t > msIntervall){
            t = 0;
            SendPosition();
            RequestTime();
        }
    }

    void RequestTime(){

        RequestTimePacket timePacket = new RequestTimePacket(networkId,time);
        var writer = timePacket.Write();
        m_Driver.Send(NetworkPipeline.Null, m_Connection, writer);
        
    }

    void SendPosition(){
        
        Vector3 position = player.position;
        Quaternion rotation = player.rotation;

        CarTransformPacked packed = new CarTransformPacked(networkId, time, position, rotation);

        var writer = packed.Write();

        m_Driver.Send(NetworkPipeline.Null, m_Connection, writer);

    }

    void ReceiveTime(DataStreamReader reader, ref DataStreamReader.Context context){

        ServerTimePacket packet = new ServerTimePacket();
        packet.Read(reader, ref context);

        float t = packet.serverTime + (time - packet.localTime) / 2;

        if(Mathf.Abs(time - t) > 0.5f){
            time = t;
        }

    }

    void PlayerConnected(DataStreamReader reader, ref DataStreamReader.Context context){

        int playerID = reader.ReadInt(ref context);

        Transform p = Instantiate(playerPrefab,Vector3.zero,Quaternion.identity).transform;
        transforms.Add(playerID, p);

        //Debug.Log(playerID + "  " + networkId);

    }

    void SetupConnection(DataStreamReader reader, ref DataStreamReader.Context context){

        networkId = reader.ReadInt(ref context);

        for(int i = 0; i < networkId; i++){
            Transform p = Instantiate(playerPrefab,Vector3.zero,Quaternion.identity).transform;
            transforms.Add(i, p);
        }

    }

    void UpdatePlayer(DataStreamReader reader, ref DataStreamReader.Context context){

        BasePacket packet = new CarTransformPacked();
        packet.Read(reader, ref context);
        CarTransformPacked p = packet as CarTransformPacked;

        packets.Add(p.netID, p);
        
    }

    void UpdateWorldState(){

        float currentTime = time - 0.4f;

        for(int i = 0; i < 10; i++){
            
            TransformPair pair = packets.GetPair(i,currentTime);

            if(pair == null)
                continue;   

            if(pair.before == null || pair.after == null)   
                continue;   
                
            if(!transforms.ContainsKey(pair.after.netID))
                continue;
                
            float lerpValue = (currentTime - pair.before.timeStamp) / (pair.after.timeStamp - pair.before.timeStamp);
            transforms[pair.after.netID].position = Vector3.Lerp(pair.before.postition, pair.after.postition, lerpValue);
            transforms[pair.after.netID].rotation = Quaternion.Slerp(pair.before.rotation, pair.after.rotation, lerpValue);
            

        }

    }

    

}
