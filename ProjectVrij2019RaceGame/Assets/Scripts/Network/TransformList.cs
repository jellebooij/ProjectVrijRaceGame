using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformList : MonoBehaviour
{
    Dictionary<int,List<CarTransformPacked>> packets = new Dictionary<int,List<CarTransformPacked>>();
    float prevTime = 0;


    public void Add(int netID, CarTransformPacked packed){

        if(!packets.ContainsKey(netID))
            packets.Add(netID, new List<CarTransformPacked>());
        

        packets[netID].Add(packed);

    }

    public TransformPair GetPair(int netID, float time){

        if(!packets.ContainsKey(netID))
            return null;

        TransformPair pair = new TransformPair();

        float beforeTime = float.MaxValue;
        float afterTime = float.MaxValue;

        for(int i = 0; i < packets[netID].Count; i++){

            CarTransformPacked currentPacket = packets[netID][i];

            if(time - currentPacket.timeStamp < beforeTime && time - currentPacket.timeStamp > 0){
                beforeTime = time - currentPacket.timeStamp;
                pair.before = currentPacket;
            }

            if(currentPacket.timeStamp - time < afterTime && currentPacket.timeStamp - time > 0){
                afterTime = currentPacket.timeStamp - time;
                pair.after = currentPacket;
            }

        }  
        
        if(pair.before == null || pair.after == null){
                return null;
        }

        List<CarTransformPacked> removables = new List<CarTransformPacked>();

        for(int i = 0; i < packets[netID].Count; i++){
          
            if(packets[netID][i].timeStamp < pair.before.timeStamp){
                removables.Add(packets[netID][i]);
            }

        }

        for(int i = 0; i < removables.Count; i++){
          
            packets[netID].Remove(removables[i]);

        }

        return pair;

    }

}

public class TransformPair{
    
    public CarTransformPacked before;
    public CarTransformPacked after;


}
