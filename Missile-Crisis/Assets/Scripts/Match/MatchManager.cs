using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
[System.Serializable]public class MatchData {
    public bool can_match_end;
    public bool is_war_on;
    public bool is_match_over;
    public MapCellData[,] map; //Should be synchronized before match
    public float match_time;
}

[System.Serializable] public struct PlayerData {
    public int player_id;
    public Color player_color;
    public bool is_dead;
    public List<MapCellData> silos;

}
public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;
    private void Awake() {
        instance = this;
        data = new MatchData();
        data.map = null;
        data.can_match_end = false;
        data.is_war_on = false;
        players_in_match = new List<PlayerData>();
    }
    public PhotonView photon_view;
    [SerializeField] public MatchData data;
    [SerializeField] public List<PlayerData> players_in_match;
    StaticMap[,] static_map;
    DynamicMap[,] dynamic_map;
    public IEnumerator SharedLoop(int player_quantity) {
        //SET PLAYERS PLAYING
        yield return new WaitForSeconds(1);
        Debug.Log("Started shared loop of "+player_quantity+" players.");
        data.is_match_over = false;
        for (int i = 0; i < player_quantity; i++)
            if(GameManager.instance.listOfPlayersPlaying[i] != null){
                PlayerManager p = GameManager.instance.listOfPlayersPlaying[i];
                p.data.is_playing = true;
                GameManager.instance.listOfPlayersPlaying[i] = p;
                PlayerData pm = new PlayerData();
                pm.player_id = i;
                pm.is_dead = false;
                pm.silos = new List<MapCellData>();
                players_in_match.Add(pm);
                
                if(p.photon_view.IsMine)
                    FocusPlayerCapital(i);
            }
        Debug.Log("Started match loop.");
        while(!data.is_match_over) {
            if(!data.is_war_on) {
                //SILO PLACEMENT
                NotificationManager.instance.ShowMessage("PLACE YOUR SILOS",20);
                NotificationManager.instance.ShowCountdown(20);
                RPC_ToggleSiloPlacement(BitConverter.GetBytes(true), BitConverter.GetBytes(5));
                yield return new WaitForSeconds(21);
                RPC_ToggleSiloPlacement(BitConverter.GetBytes(false), BitConverter.GetBytes(0));
                data.is_war_on = true;
                data.can_match_end = true;
                NotificationManager.instance.ShowMessage("NUKE 'EM!!",10);
            }
            //MISSILE GAIN
            int missile_gain = 0;
            
            if(data.is_war_on) {
                yield return new WaitForSeconds(5);
                AddMissileAll(1+missile_gain,1);
                missile_gain = (int)(data.match_time/10);
                if(missile_gain >= 3)
                    missile_gain = 3;
                yield return null;
            }
            yield return null;
            data.match_time += Time.deltaTime;
        }
        Debug.Log("Match loop ended.");
        yield break;
    }
    private void Update() {
        if(data.can_match_end)
            if(CheckEndGame()){
                data.can_match_end = false;
                EndMatch(FindWinner());
            }
    }
#region Camera Control
    public void ShakePlayers() {
        PlayerManager[] players = GameManager.instance.listOfPlayersPlaying;
        for (int i = 0; i < players.Length; i++)
            if(players[i].photon_view.IsMine)
                players[i].shake_behaviour.InduceStress(1);
    }
    void FocusPlayerCapital(int player_id) {
        for (int y = 0; y < data.map.GetLength(1); y++){
            for (int x = 0; x < data.map.GetLength(0); x++){
                if(data.map[x,y].is_capital)
                    if(data.map[x,y].owner_id == player_id)
                        if(GameManager.instance.listOfPlayersPlaying[player_id] != null)
                            GameManager.instance.listOfPlayersPlaying[player_id].FocusCoordinates(x,y);
            }
        }

    }
#endregion
#region End Game
    string FindWinner() {
        if(players_in_match.Count <= 0)
            return("NO ONE");
        PlayerData current_winner = players_in_match[0];
        for (int i = 0; i < players_in_match.Count; i++)  
            if(!players_in_match[i].is_dead)
                current_winner = players_in_match[i];
        return (GameManager.instance.listOfPlayersPlaying[current_winner.player_id].data.player_name);
    }
    bool CheckEndGame (){
        bool may_end = false;
        for (int i = 0; i < players_in_match.Count; i++)  {
            PlayerData p = players_in_match[i];
            //If the player is not out yet
            //Count its silos and compare to which are not destroyed
            //If all is destroyed kill it.
            //Check if all the players but one is dead
            int players_playing = players_in_match.Count;
            int players_dead = 0;
            if(!p.is_dead)  {
                int total_silos = p.silos.Count;
                int destroyed_silos = 0;
                for (int o = 0; o < p.silos.Count; o++)
                    if(p.silos[o].is_nuked)
                        destroyed_silos++;
                
                    
                if(destroyed_silos >= total_silos)
                    p.is_dead = true;
            } else
                players_dead++;
                
            if(players_dead >= players_playing-1)
                may_end = true;
            players_in_match[i] = p;
        }
        return may_end;
    }
    void EndMatch(string player_name) {
        PlayerManager[] players = GameManager.instance.listOfPlayersPlaying;
        for (int i = 0; i < players.Length; i++)
            if(players[i] != null)
                players[i].data.is_playing = false;
        //photon_view.RPC("RPC_EndMatch",RpcTarget.AllViaServer, System.Text.Encoding.UTF8.GetBytes(player_name));
        RPC_EndMatch(System.Text.Encoding.UTF8.GetBytes(player_name));
    }
    [PunRPC] void RPC_EndMatch(byte[] name_bytes) {
        PlayerManager[] players = GameManager.instance.listOfPlayersPlaying;
        for (int i = 0; i < players.Length; i++)
            if(players[i] != null)
                players[i].data.is_playing = false;
        
        string received_name = System.Text.Encoding.UTF8.GetString(name_bytes);
        StartCoroutine(EndView.instance.GUIIn(received_name));
        Debug.Log("The winner is player: "+received_name);
    }
#endregion
#region Missile
    public void ExplodeCells(Vector2[] coordinates){
        //Debug.Log("Sent "+coordinates.Length+" coordinates.");
        photon_view.RPC("RPC_ExplodeCells",RpcTarget.All,Serialization.SerializeCoordinates(coordinates));
    }
    [PunRPC] void RPC_ExplodeCells(byte[] bytes){
        Vector2[] coordinates = Serialization.DeserializeCoordinates(bytes);
        // Debug.Log("Received "+coordinates.Length+" coordinates.");
        foreach (Vector2 v in coordinates)
            data.map[(int)v.x,(int)v.y].is_nuked = true;
        
        UpdatePlayerMap();
    }
    public void AddMissileAll(int quantity, int type) {
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
        players_in_match[received_id].silos.Add(cell);
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
#region Map Display
    void UpdatePlayerMap() {
        PlayerManager[] p = GameManager.instance.listOfPlayersPlaying;
        //Debug.Log("Updating map of "+p.Length+ " players!");
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
    PlayerData GetPlayerInMatch(int player_id){
        foreach (PlayerData p in players_in_match)
            if(p.player_id == player_id)
                return(p);
        return(new PlayerData(){player_id = -1});
    }
}
