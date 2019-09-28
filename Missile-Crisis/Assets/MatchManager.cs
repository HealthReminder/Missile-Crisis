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
        int[] map_seed = new int[]{20*player_quantity, player_quantity, 75, Random.Range(0,999999),Random.Range(0,999999)};
        static_map = MapGenerator.GenerateValidStaticMap(map_seed[0],map_seed[1], map_seed[2],map_seed[3]);
        dynamic_map = MapGenerator.GenerateValidDynamicMap(static_map,map_seed[1],map_seed[4]);
        data.map = MapGenerator.GetMapData(static_map,dynamic_map);
        Debug.Log("Finished creating and dividing map.");
        photon_view.RPC("RPC_ReceiveMap",RpcTarget.All,Serialization.SerializeMapSeed(map_seed));
        Debug.Log("Sent map through network.");
        //StartCoroutine(match_view.DrawMap(data.map));
        yield break;
    }
    [PunRPC] void RPC_ReceiveMap(byte[] bytes){
        int[] map_seed = Serialization.DeserializeMapSeed(bytes);
        static_map = MapGenerator.GenerateValidStaticMap(map_seed[0],map_seed[1], map_seed[2],map_seed[3]);
        dynamic_map = MapGenerator.GenerateValidDynamicMap(static_map,map_seed[1],map_seed[4]);
        data.map = MapGenerator.GetMapData(static_map,dynamic_map);
        Debug.Log(static_map[0,0].type + " " + static_map.GetLength(0));
        Debug.Log(dynamic_map[0,0].owner_id + " " + static_map.GetLength(0));
        Debug.Log(data.map[0,0].owner_id + " " + data.map.GetLength(0));
        Debug.Log("Received a map!");
        StartCoroutine(match_view.DrawMap(data.map));
    }
}
