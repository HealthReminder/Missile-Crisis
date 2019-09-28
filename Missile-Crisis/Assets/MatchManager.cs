using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MatchData {
    public MapCellData[,] map; //Should be synchronized before match
}

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;
    StaticMap[,] static_map;
    DynamicMap[,] dynamic_map;
    private void Awake() {
        instance = this;
    }
    public MatchData data;
    public PhotonView photon_view;
    
    public MatchView match_view;
    private void Start() {
        data = new MatchData();
    }
    public void MatchStart(int player_quantity) {
        if(!PhotonNetwork.IsMasterClient)
            return;
        StartCoroutine(CreateAndDistributeMap(player_quantity));        
    }
    IEnumerator CreateAndDistributeMap (int player_quantity) {
        static_map = MapGenerator.GenerateValidStaticMap(100*player_quantity,player_quantity, 0.75f,Random.Range(0,999999));
        dynamic_map = MapGenerator.GenerateValidDynamicMap(static_map,player_quantity,Random.Range(0,999999));
        data.map = MapGenerator.GetMapData(static_map,dynamic_map);
        Debug.Log("Finished creating and dividing map.");
        //photon_view.RPC("RPC_ReceiveMap",RpcTarget.All,Serialization.SerializeMatchData(data));
        Debug.Log("Sent map through network.");
        StartCoroutine(match_view.DrawMap(data.map));
        yield break;
    }
    [PunRPC] void RPC_ReceiveMap(byte[] bytes){
        data = Serialization.DeserializeMatchData(bytes);
        Debug.Log("Received a map!");
        match_view.DrawMap(data.map);
    }
}
