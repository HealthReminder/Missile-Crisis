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

    public GameObject menuContainer,roomListContainer;
    public Image overlay;
    public void OnRoomEntryClicked(GameObject entryObj) {
        PhotonLobbyController.instance.JoinRoom(entryObj.name);
    }

    #region INITIAL
    public void OnConnectedToMaster(){
        StartCoroutine(FadeOverlayOut());
        roomListContainer.SetActive(true);
    }
    IEnumerator FadeOverlayOut(){
        while(overlay.color.a > 0){
            overlay.color+= new Color(0,0,0,-0.05f);
            yield return null;
        }
        overlay.gameObject.SetActive(false);
        yield break;
    }
    public void OnConnectClicked(){
        overlay.color += new Color(0,0,0,1);
        overlay.gameObject.SetActive(true);
        roomListContainer.SetActive(false);
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
