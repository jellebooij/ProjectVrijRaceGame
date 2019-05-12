using System.Net;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;


using NetworkConnection = Unity.Networking.Transport.NetworkConnection; 


public class ClientBehaviour : MonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool m_Done;
    public GameObject playerPrefab;
    private NetworkPipeline relieablePipeline;
    private NetworkPipeline unrelieablePipeline;
    public Dictionary<int, Transform> transforms = new Dictionary<int, Transform>();
    public Transform player;
    int networkId;

    bool connected;
    
    void Start ()
	{   

        Screen.fullScreen = false;

        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);
        m_Connection = default(NetworkConnection);
        
        //relieablePipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        //unrelieablePipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage));

        var endpoint = NetworkEndPoint.Parse("192.168.178.42", 9000);
        m_Connection = m_Driver.Connect(endpoint);
    }
    
    public void OnDestroy()
    {
        m_Driver.Dispose();
    }
    
    void Update()
    {
        int c = 0;

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
                //var value = 1;
                //using (var writer = new DataStreamWriter(4, Allocator.Temp))
                //{
                //    writer.Write(value);
                //    m_Connection.Send(m_Driver, writer);
                //}
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                packetTypes value = (packetTypes)stream.ReadInt(ref readerCtx);
                

                switch(value){

                    case packetTypes.SetupConnection:
                        networkId = stream.ReadInt(ref readerCtx);
                        
                        for(int i = 0; i < networkId; i++){
                            SpawnPlayer(i);
                        }

                    break;
                    

                    case packetTypes.PlayerConnected:
                        int playerID = stream.ReadInt(ref readerCtx);
                        SpawnPlayer(playerID);
                        //Debug.Log(playerID + "  " + networkId);
                    break;

                    case packetTypes.UpdatePlayer:
                        int id = stream.ReadInt(ref readerCtx);
                        UpdatePlayer(id, stream.ReadFloat(ref readerCtx), stream.ReadFloat(ref readerCtx),stream.ReadFloat(ref readerCtx),
                            stream.ReadFloat(ref readerCtx),stream.ReadFloat(ref readerCtx),stream.ReadFloat(ref readerCtx),stream.ReadFloat(ref readerCtx)
                        );
                    break;

                    case packetTypes.Test:
                        var w = new DataStreamWriter(8, Allocator.Temp);
                        w.Write((int)packetTypes.Test);
                        w.Write(networkId);
                        m_Driver.Send(NetworkPipeline.Null,m_Connection,w);
                    break;
                }
              
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                m_Connection = default(NetworkConnection);
            }
        }

        if(connected){
            SendPosition();
        }
    }

    void SpawnPlayer(int playerID){

        

        Transform p = Instantiate(playerPrefab,Vector3.zero,Quaternion.identity).transform;
        transforms.Add(playerID, p);


    }

    void UpdatePlayer(int playerID, float x, float y, float z, float xr, float yr, float zr, float wr){
       
        if(!transforms.ContainsKey(playerID))
            return;

        Transform p = transforms[playerID];
        p.position = new Vector3(x,y,z);
        p.rotation = new Quaternion(xr,yr,zr,wr);
        
    }

    void SendPosition(){

        var writer = new DataStreamWriter(36, Allocator.Temp);

        writer.Write((int)packetTypes.UpdatePlayer);

        writer.Write(networkId);
        writer.Write(player.position.x);
        writer.Write(player.position.y);
        writer.Write(player.position.z);

        writer.Write(player.rotation.x);
        writer.Write(player.rotation.y);
        writer.Write(player.rotation.z);
        writer.Write(player.rotation.w);

        m_Driver.Send(NetworkPipeline.Null, m_Connection, writer);

    }
}
