using System;
using System.Collections;
using System.Collections.Generic;
using OdinSerializer;
using Photon.Pun;
using UnityEngine;

[System.Serializable]   public class GameData {
    public bool is_match_started = false;
    public int players_in_room;
    public int votes_to_start = 0;
}
[System.Serializable] public class GameManager : MonoBehaviour {

    [SerializeField]    public GameData data;
    public PlayerManager[] listOfPlayersPlaying;
    public PhotonView photon_view;
    //Game state
    public bool is_synchronizing_players = false;
    public bool is_migrating_host = false;
    
    public static GameManager instance;
    [SerializeField]    public DoomsdayClockView match_start_view;
    void Awake () {
        instance = this;
        //Setup
        data = new GameData();
        data.is_match_started = false;
        data.players_in_room = 1;
        photon_view = GetComponent<PhotonView> ();
    }
    private void Update () {
        //This will ensure that every player is in the game before starting the game loop
        //This is fed by the room controller
        

        if(listOfPlayersPlaying != null)
            if(listOfPlayersPlaying.Length <= 0f){
                match_start_view.TogglePlayerCounter(false);
                return;
                //Not enough players to start the game
            } else {
                if(!data.is_match_started){
                    match_start_view.TogglePlayerCounter(true);
                    int votes_needed = 1;
                    //int votes_needed = listOfPlayersPlaying.Length/2 + 1;
                    //Check if there are enough votes to start the game
                    if(CheckIfMatchCanStart(votes_needed)){
                        //Start game
                        match_start_view.ToggleClock(false);
                        data.is_match_started = true;
                        if(!PhotonNetwork.IsMasterClient)
                            return;
                        if(match_start_view.is_working_gui)
                            return;
                        StartCoroutine(CreateMapAndEnableSharedLoop(listOfPlayersPlaying.Length));  
                    } else {
                        //Wait
                    }
                }
            }

        if (!PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < listOfPlayersPlaying.Length; i++)
            if (listOfPlayersPlaying[i] == null)
                OnPlayerLeftRoom ();
    }
    
#region Loop
    IEnumerator CreateMapAndEnableSharedLoop(int player_quantity) {
        yield return MatchManager.instance.CreateAndDistributeMap(player_quantity);  
        photon_view.RPC("RPC_EnableSharedLoop",RpcTarget.All);
    }
    [PunRPC] void RPC_EnableSharedLoop() {
        StartCoroutine(MatchManager.instance.SharedLoop(listOfPlayersPlaying.Length));
    }

#endregion
#region Start
    public void CastStartVote() {
        photon_view.RPC("RPC_CastStartVote",RpcTarget.All);
    }
    [PunRPC] void RPC_CastStartVote() {
        data.votes_to_start +=1;
        UpdateWaitingClock();
    }
    public void UpdateWaitingClock() {
        match_start_view.UpdateClock(GameManager.instance.listOfPlayersPlaying.Length, GameManager.instance.data.votes_to_start);    
    }
    bool CheckIfMatchCanStart(int votes_needed) {
        if(data.votes_to_start >= votes_needed)
            return(true);
        else
            return(false);
    }
#endregion
#region Host migration
    //SET MASTER CLIENT
    public void SetMasterClient(string user_id){
        photon_view.RPC("RPC_SetMasterClient",RpcTarget.AllViaServer,System.Text.Encoding.UTF8.GetBytes(user_id));
    }
    [PunRPC]void RPC_SetMasterClient(byte[] user_id_bytes) {
        is_migrating_host = true;
        string received_user =  System.Text.Encoding.UTF8.GetString(user_id_bytes);
        var p = PhotonNetwork.MasterClient;
        //CommunicationManager.instance.PostNotification("Received host: "+received_user);
        //CommunicationManager.instance.PostNotification("Current host: "+p.UserId);
        foreach (var i in PhotonNetwork.PlayerList)
            if(i.UserId == received_user)
                p = i;
        //CommunicationManager.instance.PostNotification("New host: "+p.UserId);
        PhotonNetwork.SetMasterClient(p);
        //CommunicationManager.instance.PostNotification("Set host: "+p.UserId);
        is_migrating_host = false;
        ChatManager.instance.AddEntry(GameManager.instance.listOfPlayersPlaying[int.Parse(PhotonNetwork.MasterClient.NickName)-1].data.player_name, " is leading the way!","#7d3c98","#884ea0", false);
    }
#endregion
#region Synchronization
    public void OnPlayerLeftRoom () {
        Debug.Log ("A player left the room.");
        if (!PhotonNetwork.IsMasterClient)
            return;

        List<PlayerManager> p = new List<PlayerManager>();

        for (int i = 0; i < listOfPlayersPlaying.Length; i++)
            p.Add(listOfPlayersPlaying[i]);

        for (int o = p.Count - 1; o >= 0; o--)
            if (p[o] == null){
                p.RemoveAt (o);
            }

        listOfPlayersPlaying = new PlayerManager[p.Count];
        for (int u = 0; u < p.Count; u++)
            listOfPlayersPlaying[u] = p[u];

        ChatManager.instance.AddEntry("Someone"," abandoned the adventure.","#154360","#1b4f72",false);
        data.players_in_room = listOfPlayersPlaying.Length;

        SynchronizeAllPlayers();
    }
    [PunRPC] public void RPC_AddPlayer (byte[] viewBytes,byte[] name_bytes) {
        //This function is responsible for adding the player to the listOfPlayersPlaying 
        //So the update function can start the match when all the players have loaded the room properly
        int newPlayerIndex = -1;
        if (listOfPlayersPlaying == null || listOfPlayersPlaying.Length <= 0) {
            listOfPlayersPlaying = new PlayerManager[1];
            newPlayerIndex = 0;
        } else {
            PlayerManager[] newList = new PlayerManager[listOfPlayersPlaying.Length + 1];
            newPlayerIndex = listOfPlayersPlaying.Length;

            for (int i = 0; i < listOfPlayersPlaying.Length; i++) {
                newList[i] = listOfPlayersPlaying[i];
            }
            listOfPlayersPlaying = newList;
            //Debug.Log ("Created new array");
        }

        //Deserialize information to get the viewID so the player PhotonView can be found in the network
        //And added to the player list and also be setup
        int receivedPhotonViewID = BitConverter.ToInt32 (viewBytes, 0);
        string received_name = System.Text.Encoding.UTF8.GetString(name_bytes);

        //Debug.Log ("Player "+received_name+" with view ID of" + receivedPhotonViewID + " joined the room with ID of " + newPlayerIndex);
        ChatManager.instance.AddEntry(received_name, " joined the adventure!","#2471a3","#2e86c1",false);

        PhotonView playerView = PhotonNetwork.GetPhotonView (receivedPhotonViewID);
        Debug.Log ("Adding new player with index of " + newPlayerIndex + " to the list of size " + listOfPlayersPlaying.Length);
        PlayerManager received_manager = playerView.GetComponent<PlayerManager>();
        Debug.Log(received_manager.photon_viewID);
        received_manager.data = new PlayerData();
        received_manager.data.Reset(received_name,receivedPhotonViewID,newPlayerIndex);

        float color_distribution = 2f;
        float[] color_values = new float[3];
        for (int i = 0; i < color_values.Length; i++){
            float new_value;
            if(color_distribution >=1){
                new_value = UnityEngine.Random.Range(0f,1f);
                
            } else 
                new_value = UnityEngine.Random.Range(0f,color_distribution);
            color_distribution-= new_value;
            color_values[i] = new_value;
        }
        
        received_manager.data.player_color = new Color(color_values[0],color_values[1],color_values[2],1);

        listOfPlayersPlaying[newPlayerIndex] = received_manager;        
        data.players_in_room = listOfPlayersPlaying.Length;
        UpdateWaitingClock();
        if (!PhotonNetwork.IsMasterClient)
            return;
        SynchronizeAllPlayers();
    }
    public void SynchronizeAllPlayers(){
        for (int i = 0; i < listOfPlayersPlaying.Length; i++){
            photon_view.RPC ("RPC_SynchronizePlayer", RpcTarget.All,
                Serialization.SerializeGameData(data),
                //EnemyData
                //Serialization.instance.SerializeEnemyData(enemy_manager.data, enemy_manager.available_enemies),
                //PlayerData
                Serialization.SerializePlayerData(listOfPlayersPlaying[i].data),
                Serialization.SerializeMatchData(MatchManager.instance.data)
            ); 
        }
    }

    [PunRPC] public void RPC_SynchronizePlayer (byte[] game_data_bytes, byte[] player_data_bytes, byte[] match_data_bytes) {
        Debug.Log ("RPC_SynchronizePlayer");
        is_synchronizing_players = true;
        
        //GameData
        GameData received_game_data = Serialization.DeserializeGameData(game_data_bytes);
        data = received_game_data;

        //PlayerData
        PlayerData received_player_data = Serialization.DeserializePlayerData(player_data_bytes);
        PhotonView received_photon_view = PhotonNetwork.GetPhotonView (received_player_data.photon_view_id);
        PlayerManager received_player_manager = received_photon_view.GetComponent<PlayerManager> ();
        received_player_manager.data = received_player_data;    

        //MatchData
        MapCellData[,] map = null;
        if(MatchManager.instance.data.map != null)
            map = MatchManager.instance.data.map;
        MatchManager.instance.data = Serialization.DeserializeMatchData(match_data_bytes);
        if(map != null)
            MatchManager.instance.data.map = map;

        //If there is no list create it
        if (listOfPlayersPlaying == null || listOfPlayersPlaying.Length != data.players_in_room)
            listOfPlayersPlaying = new PlayerManager[data.players_in_room];

        if(received_player_manager)
            if(received_player_data.player_id < listOfPlayersPlaying.Length)
            listOfPlayersPlaying[received_player_data.player_id] = received_player_manager;

        is_synchronizing_players = false;
        Debug.Log ("Finished synchronizing player data");
    }
#endregion
}