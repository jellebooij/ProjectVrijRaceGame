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
    public GameObject machinegunBullet;
    public Transform parent;

    TransformList packets;

    public Transform player;
    public float msIntervall;
    int networkId;

    bool connected;

    float t = 0;

    public static ClientBehaviour instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }


    void Start ()
	{   

        Screen.fullScreen = false;

        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);
        m_Connection = default(NetworkConnection);
        
        //relieablePipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        //unrelieablePipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));

        var endpoint = NetworkEndPoint.Parse("192.168.1.17", 9000);
        m_Connection = m_Driver.Connect(endpoint);
        
        packetHandler = new PacketHandler();

        packetHandler.RegisterHandler(packetTypes.UpdatePlayer, UpdatePlayer);
        packetHandler.RegisterHandler(packetTypes.PlayerConnected, PlayerConnected);
        packetHandler.RegisterHandler(packetTypes.SetupConnection, SetupConnection);
        packetHandler.RegisterHandler(packetTypes.ServerTime, ReceiveTime);
        packetHandler.RegisterHandler(packetTypes.PlayerDisconected, Disconnect);
        packetHandler.RegisterHandler(packetTypes.MachineGunFire, MachineGunFire);
        packetHandler.RegisterHandler(packetTypes.Damage, Damage);
        packetHandler.RegisterHandler(packetTypes.PlayerDied, PlayerDied);
        packetHandler.RegisterHandler(packetTypes.ActivateShield, PlayerActivateShield);
        packetHandler.RegisterHandler(packetTypes.AssignPostion, AssignPosition);

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

    public void FireMachineGun(Vector3 bulletPosition, Quaternion bulletRotation)
    {
        MachineGunFirePacked packed = new MachineGunFirePacked(networkId, bulletPosition, bulletRotation);
        m_Driver.Send(NetworkPipeline.Null, m_Connection, packed.Write());
    }

    public void Die()
    {
        PlayerDiedPackage package = new PlayerDiedPackage(networkId);
        m_Driver.Send(NetworkPipeline.Null, m_Connection, package.Write());
    }

    public void ActivateShield()
    {
        ActivateShieldPackage package = new ActivateShieldPackage(networkId);
        m_Driver.Send(NetworkPipeline.Null, m_Connection, package.Write());
    }

    void PlayerActivateShield(DataStreamReader reader, ref DataStreamReader.Context context)
    {
        PlayerDiedPackage packed = new PlayerDiedPackage();
        packed.Read(reader, ref context);
        transforms[packed.netID].gameObject.GetComponent<ShieldSwitch>().EnableShield();
    }

    void PlayerDied(DataStreamReader reader, ref DataStreamReader.Context context)
    {
        PlayerDiedPackage packed = new PlayerDiedPackage();
        packed.Read(reader, ref context);
        transforms[packed.netID].gameObject.SetActive(false);
    }

    void PlayerConnected(DataStreamReader reader, ref DataStreamReader.Context context){

        int playerID = reader.ReadInt(ref context);

        Transform p = Instantiate(playerPrefab,Vector3.zero,Quaternion.identity,parent).transform;
        transforms.Add(playerID, p);
        p.GetComponentInChildren<NetworkPlayer>().id = playerID;

        Debug.Log(playerID + " connectedTOClient");

    }

    void Disconnect(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        Debug.Log("playerdisconnectedClient");

        int id = reader.ReadInt(ref context);
        Destroy(transforms[id].gameObject);
        transforms.Remove(id);
    }

    void MachineGunFire(DataStreamReader reader, ref DataStreamReader.Context context)
    {
        MachineGunFirePacked packet = new MachineGunFirePacked();
        packet.Read(reader, ref context);

        GameObject obj = Instantiate(machinegunBullet, transforms[packet.netID].position + transforms[packet.netID].up * 1.2f, packet.bulletRotation);
        obj.GetComponent<MachineGunBullet>().isOwner = false;

    }

    public void TakeDamage(int damagedPlayerId, float damage)
    {
        TakeDamage packed = new TakeDamage(damagedPlayerId, damage);
        m_Driver.Send(NetworkPipeline.Null, m_Connection, packed.Write());

    }

    void SetupConnection(DataStreamReader reader, ref DataStreamReader.Context context){

        SetupConnection conn = new SetupConnection();
        conn.Read(reader, ref context);

        networkId = conn.netID;

        for(int i = 0; i < conn.connectedPlayerAmount; i++){

            if (conn.IDs[i] == networkId) 
                continue;

            Transform p = Instantiate(playerPrefab,Vector3.zero,Quaternion.identity,parent).transform;
            transforms.Add(conn.IDs[i], p);
            p.GetComponentInChildren<NetworkPlayer>().id = conn.IDs[i];
        }

    }

    void UpdatePlayer(DataStreamReader reader, ref DataStreamReader.Context context){

        BasePacket packet = new CarTransformPacked();
        packet.Read(reader, ref context);
        CarTransformPacked p = packet as CarTransformPacked;
        Debug.Log("got position from CLIENT " + (packet as CarTransformPacked).netID);

        packets.Add(p.netID, p);
        
    }


    void Damage(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        BasePacket packet = new TakeDamage();
        packet.Read(reader, ref context);
        TakeDamage p = packet as TakeDamage;

        if(p.damagedPlayerID == networkId)
        {
            player.gameObject.GetComponent<Health>().health -= p.damage;
        }

    }

    void AssignPosition(DataStreamReader reader, ref DataStreamReader.Context context)
    {

        AssignPositionPacked packed = new AssignPositionPacked();
        packed.Read(reader, ref context);

        player.transform.position = packed.postition;
        player.transform.rotation = packed.rotation;

    }



    void UpdateWorldState(){

        float currentTime = time - 0.15f;

        for(int i = 0; i < 20; i++){
            
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
