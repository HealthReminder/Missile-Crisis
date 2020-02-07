using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class PlayerController : MonoBehaviour {
    public List<Silo> silos;
    public void InsertMissile(int qtd, PlayerLocalData data) {
        data.left_missiles += qtd;
    }

    public void ShootMissile (int left_missiles, Vector2 init_coords, Vector2 end_coord, MapView map_view, PlayerLocalData data, PhotonView photon_view) {
        byte[] qtd_bytes = BitConverter.GetBytes (left_missiles);
        byte[] id_bytes = BitConverter.GetBytes (data.player_id);
        Vector3 launch_pos = map_view.cell_map[(int) init_coords.x, (int) init_coords.y].transform.position;
        Vector3 impact_pos = map_view.cell_map[(int) end_coord.x, (int) end_coord.y].transform.position;
        byte[] i_bytes = Serialization.SerializeVector3 (launch_pos);
        byte[] e_bytes = Serialization.SerializeVector3 (impact_pos);

        photon_view.RPC ("RPC_ShootMissile", RpcTarget.All, qtd_bytes, id_bytes, i_bytes, e_bytes);

        
        AudioController.instance.PlaySound ("Bomb_Launched", Vector3.zero);

    }
    #region Input
    public Silo GetClosestUnukedSilo (Vector3 point) {
        Silo closest_silo = new Silo ();
        if (silos.Count > 0)
            closest_silo = silos[0];
        float closest_distance = 9999;
        for (int k = 0; k < silos.Count; k++) {
            if (!GetSiloCell (silos[k].coords).is_nuked) {
                float current_distance = Vector3.Distance (silos[k].transform.position, point);
                if (current_distance <= closest_distance) {
                    closest_silo = silos[k];
                    closest_distance = current_distance;
                }
                Debug.Log(closest_distance);
            }
        }
        return closest_silo;
    }
    public MapCellData GetSiloCell (Vector2 silo_coords) {
        if (MatchManager.instance)
            return (MatchManager.instance.data.map[(int) silo_coords.x, (int) silo_coords.y]);
        return (null);
    }
    #endregion
}