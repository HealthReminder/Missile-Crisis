using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

//public class RoomOptions : Photon.Realtime.RoomOptions {
    
//}
public class PhotonLobbyController : MonoBehaviourPunCallbacks
{

    public static PhotonLobbyController lobby;
   


    public Dictionary<string, RoomInfo> cachedRoomList;
    [SerializeField] LobbyView view;
  
    
    public static PhotonLobbyController instance;
    private void Awake() {
        if(instance == null)
            instance = this;
        else 
            Destroy(this.gameObject);

        lobby = this;
        cachedRoomList = new Dictionary<string, RoomInfo>();
    }


    private void Start() {
        //PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        Debug.Log("The player has connected to the main server");
        //This line of code makes if the master client loads a scene, every player does it too
        PhotonNetwork.AutomaticallySyncScene = true;
        view.OnConnectedToMaster();
        
    }

    public void OnConnectButtonClicked() {
        PhotonNetwork.ConnectUsingSettings();
        view.OnConnectClicked();
        //PhotonNetwork.JoinRandomRoom();
    }


    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    
    public void JoinRoom(string roomName){
        Debug.Log("You joined the room: "+roomName);
        PhotonNetwork.JoinRoom(roomName,null);
    }
    public void JoinRandomRoom(){
        Debug.Log("You joined a random room");
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinRandomFailed(short returnCode, string message){
        Debug.Log("The player could not connect to a random room");
        CreateRoom();
    }

    public void CreateRoom() {
        Debug.Log("Creating a new room");
        int randomRoomID = Random.Range(0,10000);
        RoomOptions roomOptions = new RoomOptions() {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)MultiplayerSettings.instance.maxPlayers,
            PublishUserId = true
        };
        PhotonNetwork.CreateRoom("Room"+randomRoomID, roomOptions); 
    }

    public override void OnJoinedRoom(){
        Debug.Log("You joined a room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Room creation failed");
        CreateRoom();
    }
    
    //public int roomCount;
    public void Update() {
        //roomCount = PhotonNetwork.CountOfRooms;
        if(PhotonNetwork.IsConnectedAndReady)
            if (!PhotonNetwork.InLobby)
                {
                    PhotonNetwork.JoinLobby();
                }
                
            
            
    }
    public void RefreshLobby() {
        if (PhotonNetwork.InLobby) {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
        //Clean room list GUI
        UpdateCachedRoomList(roomList);
        //Update room list
        RoomInfo[] r = roomList.ToArray();
        string[] roomNames = new string[r.Length];
        for (int i = 0; i < r.Length; i++)
        {
            roomNames[i] = r[i].Name;
        }
        view.UpdateRooms(roomList.Count,roomNames);


    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
           // if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            //{
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                //continue;
            //}

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
        Debug.Log("Found "+cachedRoomList.Count+ " rooms available.");
    }



}