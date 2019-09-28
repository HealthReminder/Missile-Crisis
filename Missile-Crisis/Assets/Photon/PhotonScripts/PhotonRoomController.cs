using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class PhotonRoomController : MonoBehaviourPunCallbacks, IInRoomCallbacks {
    //Room info
    public static PhotonRoomController instance;
    private PhotonView PV;

    public bool is_scene_loaded;
    public int currentScene;

    //Player info
    Player[] photonPlayers;
    public int playersInRoom;
    public int IDInRoom;
    public int playerInGame;


    //Singleton pattern
    private void Awake() {
        if(PhotonRoomController.instance == null) 
            PhotonRoomController.instance = this;
        else
            if(PhotonRoomController.instance != this){
                //Destroy(PhotonRoomController.instance.gameObject);
                PhotonRoomController.instance = this;
            }
        DontDestroyOnLoad(this.gameObject);
        PV = GetComponent<PhotonView>();
    }


    public override void OnJoinedRoom(){
        base.OnJoinedRoom();
        Debug.Log("Entered room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        IDInRoom = playersInRoom;
        PhotonNetwork.NickName = IDInRoom.ToString();

        StartCoroutine(StartGame());
        
    }

    public override void OnLeftRoom()
    {
        playersInRoom--;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has joined the room");
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;

    }

    IEnumerator StartGame() {
        Debug.Log("Loading game");
        is_scene_loaded = true;
        if(!PhotonNetwork.IsMasterClient)
            yield break;

        //Debug.Log("Here is when you can setup some data like procedurally generated maps, configurations etc and send the initial data to the GameManager");
       
        //Debug.Log("Finished setting up everythin. Loading level for all.");
        PhotonNetwork.LoadLevel(MultiplayerSettings.instance.multiplayerScene);

        //WE CANNOT SERIALIZE GAME DATA
        //SO IT WILL BE SHARED ACROSS THE USERS 
        

        // Debug.Log("Data serialized");  
        //PV.RPC("RPC_SetupBoard",RpcTarget.AllBuffered,
        // byteGameData);
        //if(PhotonNetwork.IsMasterClient)
        //    PV.RequestOwnership();
        //Debug.Log("Setting up boards");
        //PV.RPC("RPC_SetupBoard",RpcTarget.AllBuffered);
        //Debug.Log("Updating boards");
        //PV.RPC("RPC_UpdateBoard",RpcTarget.AllBuffered);
        
        //PV.RPC("RPC_SetupBoard",RpcTarget.AllBuffered,
        //GameData.instance.staticMap,GameData.instance.playersInGame);
        //PV.RPC("RPC_UpdateBoard",RpcTarget.AllBuffered,
        //GameData.instance.dynamicMap,GameData.instance.playersInGame);

        
        yield break;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) {
        Debug.Log("Scene finished loading");
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSettings.instance.multiplayerScene){
            is_scene_loaded = true;
            
            CreatePlayer();
            
        }
    }
    public int lastView;
    private void CreatePlayer () {
        Debug.Log("Creating player");
        GameObject newPlayer = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","PhotonNetworkPlayer"),
        transform.position, Quaternion.identity, 0);
        int newPlayerView = newPlayer.GetComponent<PhotonView>().ViewID;
        byte[] viewByte = BitConverter.GetBytes(newPlayerView);
        //Debug.Log("Sending player array of "+viewByte.Length+" bytes");
        GameManager.instance.photon_view.RPC("RPC_AddPlayer",RpcTarget.AllBuffered,viewByte,System.Text.Encoding.UTF8.GetBytes(PlayerPrefs.GetString("Player_Name","Rogue")));

    }


    //This function is responsible for setting up an event listener
    //Whenever we load a new scene call the OnSceneLoaded function
    public override void OnEnable() {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        Debug.Log("Adding callback to OnSceneFinishedLoading");
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

     public override void OnDisable() {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded  -= OnSceneFinishedLoading;
    }
    public override void OnRoomListUpdate (List< RoomInfo > roomList) {
        Debug.Log("Room list updated");
    }
}
