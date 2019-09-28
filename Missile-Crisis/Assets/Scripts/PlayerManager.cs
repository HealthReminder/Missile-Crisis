using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using System;
using UnityEngine.UI;
[System.Serializable] public class PlayerData {
    public string player_name;
    public int photon_view_id;
    public int player_id;    
    public bool is_playing = false;
    public void Reset(string name,int photon_id,int player_room_id){
        player_name = name;
        photon_view_id = photon_id;
        player_id = player_room_id;
        is_playing = false;
    }

}
[System.Serializable]   public class PlayerManager : MonoBehaviourPun,IPunObservable
{
    //Data
    [SerializeField] public PlayerData data;

    //Accountability
    public bool is_setup = false;
    
    public int photon_viewID;
    public PhotonView photon_view;  
    public Camera player_camera;
    private void Start() {
        if(!photonView.IsMine) 
            return;
        StartCoroutine(WaitSetup(1));
    }  

    private void Update() {
        if(!is_setup)
            return;
        
        if(!photon_view.IsMine)
            return;
    }
    

    //SETUP - BEFORE THE ADVENTURE GET STARTED
    IEnumerator WaitSetup(float seconds){
        //OVERLAY
        yield return new WaitForSeconds(seconds);
        Setup();
        yield break;
    }
    
    void Setup() {
        if(!photon_view.IsMine)
            return;    
        photon_view.RPC("RPC_PlayerSetup",RpcTarget.All);
        PlayerManager[] p_list = GameManager.instance.listOfPlayersPlaying;
        foreach (PlayerManager p in p_list)
            p.RPC_PlayerSetup();
    }
    [PunRPC]public void RPC_PlayerSetup() {
        if(is_setup)
            return;    
    
        //Here is where you setup the player
        GameManager gm = GameManager.instance;
        if(!photon_view.IsMine){
            //PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues((UnityEngine.Random.Range(99,99999)).ToString());
            player_camera.gameObject.SetActive(false);
        } else {
            player_camera.gameObject.SetActive(true);
            if(gm.data.is_match_started){
                data.is_playing = false;    
                //Player is spectator   
                //Turn off the waiting players canvas 
                gm.match_start_view.ToggleClock(false);   
                gm.UpdateWaitingClock();
            } else {
                //Player can still join
                //Turn on the waiting players canvas
                
                gm.match_start_view.ToggleClock(true);
                if(gm.listOfPlayersPlaying.Length > 1){
                    gm.match_start_view.TogglePlayerCounter(true);
                    gm.UpdateWaitingClock();
                }
            }
        }
        is_setup = true;
        return;
    } 
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){}   
}
