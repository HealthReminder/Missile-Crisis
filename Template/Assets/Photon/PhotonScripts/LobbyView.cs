using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour
{
    
    public Text roomCountText;
    public GameObject templateRoomEntry;
    
    public List<GameObject> roomEntries;

    public GameObject roomsContainer;

    public GameObject connectButton,connectingPanel,menuContainer,roomListContainer, nameContainer;
    //public GameObject optionsContainer;


    
    //public void OnConnectButtonClicked(GameObject entryObj) {
    //    roomsContainer.SetActive(false);

    //}

    private void Start() {
        menuContainer.SetActive(false);
        roomListContainer.SetActive(false);
    }

    public void OnRoomEntryClicked(GameObject entryObj) {
        PhotonLobbyController.instance.JoinRoom(entryObj.name);
    }

    #region MENU
    public void OnBackToMenuClicked(){
        menuContainer.SetActive(true);
        roomListContainer.SetActive(false);
    }
    public void OnRoomListClicked(){
        menuContainer.SetActive(false);
        roomListContainer.SetActive(true);
    }
    #endregion

    #region INITIAL
    public void OnConnectedToMaster(){
        connectButton.SetActive(false);
        connectingPanel.SetActive(false);
        nameContainer.SetActive(true);

    }
    public void OnConnectClicked(){
        connectButton.SetActive(false);
        menuContainer.SetActive(false);
        connectingPanel.SetActive(true);
    }

    #endregion

    public void UpdateRooms(int roomsCount, string[] roomNames) {
        //Display room count
        roomCountText.text = roomsCount.ToString();
        //Clear the currently displayed rooms
        for (int i = roomEntries.Count-1; i >=0; i--)
        {
            Destroy(roomEntries[i]);
            roomEntries.RemoveAt(i);
        }
        roomEntries = new List<GameObject>();
        //Setup new rooms
        Debug.Log("Setting up new rooms");
        for (int i = 0; i <roomNames.Length; i++)
        {
            GameObject newEntry = Instantiate(templateRoomEntry);
            newEntry.transform.parent = roomsContainer.transform;
            roomEntries.Add(newEntry);
            newEntry.transform.GetChild(0).GetComponent<Text>().text = roomNames[i];
            newEntry.name = roomNames[i];
            
        }
    }
}
