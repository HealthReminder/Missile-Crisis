using System;
using System.Collections;
using System.Collections.Generic;
using Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable] public class PlayerLocalData {
    public string player_name;
    public string character;
    public Color player_color;
    public int photon_view_id;
    public int player_id;
    public bool is_playing = false;
    public bool is_selecting = false;
    public bool is_placing = false;
    public int left_silos;
    public int left_missiles;
    public void Reset (string name, int photon_id, int player_room_id) {
        player_name = name;
        photon_view_id = photon_id;
        player_id = player_room_id;
        is_playing = false;
        is_placing = false;
        left_silos = 0;
        left_missiles = 0;
        player_color = Color.black;
        character = "rat";
    }

}

[System.Serializable] public class PlayerManager : MonoBehaviourPun, IPunObservable {
    //Data
    [SerializeField] public PlayerLocalData data;

    //Accountability
    public bool is_setup = false;
    public int photon_viewID;
    public PhotonView photon_view;
    public Camera player_camera;
    public GameObject bomb_prefab;
    public GameObject silo_prefab;
    public PlayerController player_controller;

    //Exclusive
    public CameraBehaviour cam_behaviour;
    public ScreenShakeBehaviour shake_behaviour;
    public MapView map_view;
    public InventoryView inventory_view;
    public CharacterSelectionView selection_view;
    private void Start () {
        if (!photonView.IsMine)
            return;
        StartCoroutine (WaitSetup (1));
        StartCoroutine (PlayerLoop ());
    }
    //This makes the silos grow larger every 3 seconds
    IEnumerator PlayerLoop () {
        while (true) {
            if (data.is_playing) {
                for (int i = 0; i < player_controller.silos.Count; i++)
                    player_controller.silos[i].range_transform.localScale += new Vector3 (0.4f, 0.4f, 0.4f);
            }
            yield return new WaitForSeconds (3);
        }
    }
    Silo closest_silo;
    Vector3 m_pos;
    private void Update () {
        if (!is_setup)
            return;

        if (!photon_view.IsMine)
            return;

        //m_pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        //m_pos.z = 20;
        //closest_silo = player_controller.GetClosestUnukedSilo (m_pos);
        //if (closest_silo != null)
        //    closest_silo.RotateTowardsMouse ();

        //Move camera
        if (Input.GetMouseButtonDown (1)) {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, 1000))
                cam_behaviour.MoveFocus (hit.transform.position);
        }
        if (Input.GetAxis ("Mouse ScrollWheel") != 0f) {
            cam_behaviour.Zoom (Input.GetAxis ("Mouse ScrollWheel"));
        }

        if (!data.is_playing)
            return;

        if (Input.GetMouseButtonDown (0)) {
            if (data.is_placing && data.left_silos > 0) {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit, 1000))
                    if (hit.transform.GetComponent<BoardCell> ()) {
                        BoardCell selected_cell = hit.transform.GetComponent<BoardCell> ();
                        if (selected_cell.owner_id == data.player_id && !selected_cell.has_silo) {
                            data.left_silos--;

                            selected_cell.has_silo = true;
                            MatchManager.instance.PlaceSilo (selected_cell.coordinates, data.player_id);

                            Silo new_silo = Instantiate (silo_prefab, selected_cell.transform.position + new Vector3 (0, 0.55f, 0), Quaternion.identity).GetComponent<Silo> ();
                            new_silo.Setup (data.player_color);
                            new_silo.range_transform = new_silo.transform.GetChild (0).GetChild (0);
                            new_silo.transform.gameObject.SetActive (true);
                            new_silo.transform.parent = selected_cell.transform;
                            new_silo.coords = selected_cell.coordinates;

                            player_controller.silos.Add (new_silo);
                        }
                    }
            } else if (MatchManager.instance.data.is_war_on && data.left_missiles > 0) {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit, 1000))
                    if (CheckBounds (hit.point))
                        if (hit.transform.GetComponent<BoardCell> ()) {
                            BoardCell selected_cell = hit.transform.GetComponent<BoardCell> ();
                            if (selected_cell.owner_id != data.player_id) {
                                //Debug.Log(data.player_id + " is trying to shoot a missile on "+selected_cell.coordinates+" having "+data.left_missiles+" on stock.");
                                data.left_missiles--;
                                player_controller.ShootMissile (data.left_missiles, player_controller.GetClosestUnukedSilo (hit.point).coords, selected_cell.coordinates, map_view, data, photon_view);
                                if (inventory_view.gameObject.activeSelf)
                                    inventory_view.RemoveBomb (0);
                            }
                        }
            }
        }
    }
    #region Input
    bool CheckBounds (Vector3 point) {
        for (int k = 0; k < player_controller.silos.Count; k++)
            if (player_controller.silos[k].range_transform.GetComponent<SphereCollider> ().bounds.Contains (point))
                return (true);
        return (false);
    }
    #endregion
    #region Camera
    public void FocusCoordinates (int x, int y) {
        if (map_view.cell_map != null)
            if (map_view.cell_map[x, y] != null)
                cam_behaviour.MoveFocus (map_view.cell_map[x, y].transform.position);
            else
                cam_behaviour.MoveFocus (new Vector3 (x * 10, y * 10));
        else
            cam_behaviour.MoveFocus (new Vector3 (x * 10, y * 10));
    }
    #endregion
    #region Loop
    [PunRPC] void RPC_ShootMissile (byte[] qtd_bytes, byte[] id_bytes, byte[] i_bytes, byte[] e_bytes) {
        int left_missiles = BitConverter.ToInt32 (qtd_bytes, 0);
        int player_id = BitConverter.ToInt32 (id_bytes, 0);
        Vector3 init_pos = Serialization.DeserializeVector3 (i_bytes);
        Vector3 end_pos = Serialization.DeserializeVector3 (e_bytes);
        BombView bomb = Instantiate (bomb_prefab, init_pos, Quaternion.identity).GetComponent<BombView> ();
        bomb.Setup (data.player_color);
        //int random_size = UnityEngine.Random.Range(1,4);
        StartCoroutine (bomb.LaunchMissile (init_pos, end_pos, 1));
        data.left_missiles = left_missiles;
        if (!photon_view.IsMine)
            return;
        map_view.UpdateMap (MatchManager.instance.data.map, data.player_id);
        //Debug.Log(player_id + " shot a missile on "+x+"/"+y+"and has "+left_missiles+" on stock.");
    }
    //Insert missile with sounds and GUI
    public void OnGainMissile (int qtd) {
        player_controller.InsertMissile (qtd, data);
        if (inventory_view.gameObject.activeSelf)
            inventory_view.AddBomb (0);
        AudioController.instance.PlaySound ("Bomb_Received", Vector3.zero);
    }
    #endregion
    #region Start
    //Allow the placement of silos and gives an initial quantity
    public void TogglePlacement (bool is_on, int target_silos) {
        if (!photon_view.IsMine)
            return;

        data.is_placing = is_on;
        data.left_silos = target_silos;
    }
    public void ToggleCharacterSelection (bool is_on) {
        if (!photon_view.IsMine)
            return;

        data.is_selecting = is_on;
        selection_view.player_id = data.player_id;
        selection_view.ToggleGUI(is_on);
    }
    #endregion
    #region Setup
    //SETUP - BEFORE THE ADVENTURE GET STARTED
    IEnumerator WaitSetup (float seconds) {
        inventory_view.gameObject.SetActive (false);
        player_controller.silos = new List<Silo> ();
        //OVERLAY
        yield return new WaitForSeconds (seconds);
        Setup ();
        yield break;
    }

    void Setup () {
        if (!photon_view.IsMine)
            return;
        inventory_view.gameObject.SetActive (true);
        selection_view.player_id = data.player_id;
        photon_view.RPC ("RPC_PlayerSetup", RpcTarget.All);
        PlayerManager[] p_list = GameManager.instance.listOfPlayersPlaying;
        foreach (PlayerManager p in p_list)
            p.RPC_PlayerSetup ();
        cam_behaviour.can_zoom = true;
    }

    [PunRPC] public void RPC_PlayerSetup () {
        if (is_setup)
            return;

        //Here is where you setup the player
        GameManager gm = GameManager.instance;
        if (!photon_view.IsMine) {
            //PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues((UnityEngine.Random.Range(99,99999)).ToString());
            player_camera.gameObject.SetActive (false);
        } else {
            player_camera.gameObject.SetActive (true);
            if (gm.data.is_match_started) {
                //data.is_playing = false;    
                //Player is spectator   
                //Turn off the waiting players canvas 
                gm.match_start_view.ToggleClock (false);
                gm.UpdateWaitingClock ();
            } else {
                //Player can still join
                //Turn on the waiting players canvas

                gm.match_start_view.ToggleClock (true);
                if (gm.listOfPlayersPlaying.Length > 1) {
                    gm.match_start_view.TogglePlayerCounter (true);
                    gm.UpdateWaitingClock ();
                }
            }
        }
        is_setup = true;
        return;
    }
    #endregion
    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) { }
}