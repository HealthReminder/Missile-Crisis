using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
public class MatchData {
    public bool has_placed_silos;
    public MapCellData[,] map; //Should be synchronized before match
}

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;
    private void Awake() {
        instance = this;
        data = new MatchData();
        data.map = null;
        data.has_placed_silos = false;
    }
    public PhotonView photon_view;
    public MatchData data;
    StaticMap[,] static_map;
    DynamicMap[,] dynamic_map;
    public IEnumerator SharedLoop(int player_quantity) {
        RPC_ToggleSiloPlacement(BitConverter.GetBytes(true), BitConverter.GetBytes(5));
        yield return new WaitForSeconds(10);
        RPC_ToggleSiloPlacement(BitConverter.GetBytes(false), BitConverter.GetBytes(0));
        data.has_placed_silos = true;
        while(data.has_placed_silos) {
            yield return new WaitForSeconds(5);
            AddMissileAll(5);
            yield return null;
        }

        Debug.Log("Enable war!!");
        yield break;
    }
#region Missile Gain
    public void AddMissileAll(int quantity) {
        PlayerManager[] players = GameManager.instance.listOfPlayersPlaying;
        for (int i = 0; i < players.Length; i++)
            if(players[i] != null)
                players[i].InsertMissile(quantity);
    }
#endregion
#region Silo Placement
    public void PlaceSilo(Vector2 coords, int owner_id) {
        photon_view.RPC("RPC_PlaceSilo",RpcTarget.All,BitConverter.GetBytes(owner_id),BitConverter.GetBytes(coords.x),BitConverter.GetBytes(coords.y));
    }
    
    [PunRPC] void RPC_PlaceSilo(byte[] id_bytes, byte[] x_bytes, byte[] y_bytes) {
        int received_id = BitConverter.ToInt32(id_bytes,0);
        Vector2 received_coords = new Vector2(BitConverter.ToSingle(x_bytes,0),BitConverter.ToSingle(y_bytes,0));
        Debug.Log("Placing silo on "+received_coords+" for player "+received_id);
        MapCellData cell = data.map[(int)received_coords.x,(int)received_coords.y];
        if(cell.owner_id != received_id)
            return;
        if(cell.has_silo)
            return;
        cell.has_silo = true;
        UpdatePlayerMap();
    }
    void ToggleSiloPlacement(bool is_on, int qtd) {
        photon_view.RPC("RPC_ToggleSiloPlacement",RpcTarget.All, BitConverter.GetBytes(is_on), BitConverter.GetBytes(qtd));
    }

    [PunRPC]  void RPC_ToggleSiloPlacement(byte[] on_bytes, byte[] qtd_bytes) {
        bool is_on = BitConverter.ToBoolean(on_bytes,0);
        int qtd = BitConverter.ToInt32(qtd_bytes,0);
        PlayerManager[] players = GameManager.instance.listOfPlayersPlaying;
        for (int i = 0; i < players.Length; i++)
            if(players[i] != null)
                players[i].TogglePlacement(is_on, qtd);
    }
#endregion
#region  Map Display
    void UpdatePlayerMap() {
        PlayerManager[] p = GameManager.instance.listOfPlayersPlaying;
        Debug.Log("Updating map of "+p.Length+ " players!");
        for (int i = 0; i < p.Length; i++)
        {
            if(p[i].photon_view.IsMine)
                p[i].map_view.UpdateMap(data.map,p[i].data.player_id);
            else
                p[i].map_view.Clear();
        }
    }
    public IEnumerator CreateAndDistributeMap (int player_quantity) {
        int[] map_seed = new int[]{20*player_quantity, player_quantity, 75, UnityEngine.Random.Range(0,999999),UnityEngine.Random.Range(0,999999)};
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
        UpdatePlayerMap();
    }
    #endregion
}
