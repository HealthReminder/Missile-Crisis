using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;
using System;
using UnityEngine.UI;
[System.Serializable] public class PlayerData {
    public string player_name;
    public Color player_color;
    public int photon_view_id;
    public int player_id;    
    public bool is_playing = false;
    public bool is_placing = false;
    public int left_silos;
    public int left_missiles;
    public void Reset(string name,int photon_id,int player_room_id){
        player_name = name;
        photon_view_id = photon_id;
        player_id = player_room_id;
        is_playing = false;
        is_placing = false;
        left_silos = 0;
        left_missiles = 0;
        player_color = Color.black;
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
    public CameraBehaviour cam_behaviour;
    public MapView map_view;
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

        //Move camera
        if(Input.GetMouseButtonDown(1)){
            RaycastHit2D hit = Physics2D.Raycast(player_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null)
            {
                //Debug.Log ("Target Position: " + hit.collider.gameObject.transform.position);
                cam_behaviour.MoveFocus(hit.transform.position);
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0f ) {
            cam_behaviour.Zoom(Input.GetAxis("Mouse ScrollWheel"));
            
        }
            
        if(!data.is_playing)
            return;

        if(Input.GetMouseButtonDown(0)){
            if(data.is_placing && data.left_silos > 0) { 
                RaycastHit2D hit = Physics2D.Raycast(player_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if(hit)
                if(hit.transform.GetComponent<BoardCell>()){
                    BoardCell selected_cell = hit.transform.GetComponent<BoardCell>();
                    if(selected_cell.owner_id == data.player_id && !selected_cell.has_silo) {
                        data.left_silos--;
                        selected_cell.has_silo = true;
                        MatchManager.instance.PlaceSilo(selected_cell.coordinates, data.player_id);
                    }
                }
            } else if(MatchManager.instance.data.is_war_on && data.left_missiles > 0) {
                RaycastHit2D hit = Physics2D.Raycast(player_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if(hit)
                if(hit.transform.GetComponent<BoardCell>()){
                    BoardCell selected_cell = hit.transform.GetComponent<BoardCell>();
                    if(selected_cell.owner_id != data.player_id) {
                        //Debug.Log(data.player_id + " is trying to shoot a missile on "+selected_cell.coordinates+" having "+data.left_missiles+" on stock.");
                        data.left_missiles--;
                        ShootMissile(data.left_missiles,selected_cell.coordinates);
                    }
                }
                
            }
        }
    }
    
    #region Loop
    void ShootMissile(int left_missiles, Vector2 coords) {
        byte[] qtd_bytes = BitConverter.GetBytes(left_missiles);
        byte[] id_bytes = BitConverter.GetBytes(data.player_id);
        byte[] x_bytes = BitConverter.GetBytes((int)coords.x);
        byte[] y_bytes = BitConverter.GetBytes((int)coords.y);
        photon_view.RPC("RPC_ShootMissile",RpcTarget.All,qtd_bytes,id_bytes,x_bytes,y_bytes);

    }
    [PunRPC] void RPC_ShootMissile(byte[] qtd_bytes, byte[] id_bytes, byte[] x_bytes, byte[] y_bytes) {
        int left_missiles = BitConverter.ToInt32(qtd_bytes,0);
        int player_id = BitConverter.ToInt32(id_bytes,0);
        int x = BitConverter.ToInt32(x_bytes,0);
        int y = BitConverter.ToInt32(y_bytes,0);
        MatchManager.instance.data.map[x,y].is_nuked = true;
        data.left_missiles = left_missiles;
        if(!photon_view.IsMine)
            return;

        map_view.UpdateMap(MatchManager.instance.data.map,data.player_id);
        //Debug.Log(player_id + " shot a missile on "+x+"/"+y+"and has "+left_missiles+" on stock.");
    }
    public void InsertMissile(int qtd) {
        data.left_missiles += qtd;
    }
    #endregion
    #region Start
    public void TogglePlacement(bool is_on, int target_silos) {
        if(!photon_view.IsMine)
            return;
        
        data.is_placing = is_on;
        data.left_silos = target_silos;
    }
    #endregion
    #region Setup
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
    #endregion
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){}   
}
