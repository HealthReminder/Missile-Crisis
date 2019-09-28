using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Serialization : MonoBehaviour
{    
    //[SerializeField] public EnemyData serialize_event;
    //[SerializeField] public EnemyData deserialized_event;
    private void Update() {
        //if(Input.GetKeyDown(KeyCode.A))
        //    deserialized_event = DeserializeEnemyData(SerializeEnemyData(serialize_event,EnemyManager.instance.available_enemies),EnemyManager.instance.available_enemies);
    }
    #region GameManager
    public byte[] SerializeGameData(GameData g_data) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[2][];
        arrays[0] = BitConverter.GetBytes(g_data.is_adventure_started);
        arrays[1] = BitConverter.GetBytes(g_data.players_in_room);
        Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        return(ArrayConcatenation.MergeArrays(arrays));
    }
    public GameData DeserializeGameData(byte[] bytes) {
        GameData result_data = new GameData();
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        Debug.Log("Deserialized "+data_array.GetLength(0) + " arrays.");
        result_data.is_adventure_started = BitConverter.ToBoolean(data_array[0],0);
        result_data.players_in_room = BitConverter.ToInt32(data_array[1],0);
        return(result_data);
    }
    #endregion
    #region PlayerData
    public byte[] SerializePlayerData(PlayerData p_data) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[5][];
        arrays[0] = System.Text.Encoding.UTF8.GetBytes(p_data.player_name);
        arrays[1] = BitConverter.GetBytes(p_data.photon_view_id);
        arrays[2] = BitConverter.GetBytes(p_data.player_id);
        arrays[3] = BitConverter.GetBytes(p_data.character_id);
        arrays[4] = BitConverter.GetBytes(p_data.is_playing);
        Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        return(ArrayConcatenation.MergeArrays(arrays));
    }
    public PlayerData DeserializePlayerData(byte[] bytes) {
        PlayerData result_data = new PlayerData();
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        Debug.Log("Deserialized "+data_array.GetLength(0) + " arrays.");
        result_data.player_name = System.Text.Encoding.UTF8.GetString(data_array[0]);
        result_data.photon_view_id = BitConverter.ToInt32(data_array[1],0);
        result_data.player_id = BitConverter.ToInt32(data_array[2],0);
        result_data.character_id = BitConverter.ToInt32(data_array[3],0);
        result_data.is_playing = BitConverter.ToBoolean(data_array[4],0);
        return(result_data);
    }
    #endregion 
    
    public static Serialization instance;
    private void Awake() {
        instance = this;
    }
}
