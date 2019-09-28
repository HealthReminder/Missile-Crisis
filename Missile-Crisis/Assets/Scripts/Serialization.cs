﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Serialization
{    
    //[SerializeField] public EnemyData serialize_event;
    //[SerializeField] public EnemyData deserialized_event
    #region GameManager
    public static byte[] SerializeGameData(GameData g_data) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[3][];
        arrays[0] = BitConverter.GetBytes(g_data.is_match_started);
        arrays[1] = BitConverter.GetBytes(g_data.players_in_room);
        arrays[2] = BitConverter.GetBytes(g_data.votes_to_start);
        //Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        return(ArrayConcatenation.MergeArrays(arrays));
    }
    public static GameData DeserializeGameData(byte[] bytes) {
        GameData result_data = new GameData();
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        //Debug.Log("Deserialized "+data_array.GetLength(0) + " arrays.");
        result_data.is_match_started = BitConverter.ToBoolean(data_array[0],0);
        result_data.players_in_room = BitConverter.ToInt32(data_array[1],0);
        result_data.votes_to_start = BitConverter.ToInt32(data_array[2],0);
        return(result_data);
    }
    #endregion
    #region MatchManager
    public static byte[] SerializeMapSeed(int[] seed) {
        List<byte[]> bytes = new List<byte[]>();
        for (int i = 0; i < seed.Length; i++)
            bytes.Add(BitConverter.GetBytes(seed[i]));
        byte[] final_cell_bytes = ArrayConcatenation.MergeArrays(bytes.ToArray());
        return(final_cell_bytes);
    }
    public static int[] DeserializeMapSeed(byte[] map_seed_bytes) {
        Debug.Log("Deserializing Map Data!");
        byte[][] bytes = ArrayConcatenation.UnmergeArrays(map_seed_bytes);
        List<int> seed = new List<int>();
        for (int i = 0; i < bytes.Length; i++)
            seed.Add(BitConverter.ToInt32(bytes[i],0));
        Debug.Log("Finished deserializing map!");
        return(seed.ToArray());
    }
    static MapCellData DeserializeMapCell(byte[] bytes) {
        //Create an array of the arrays you wanna serialize together
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        //Debug.Log("Deserialized "+data_array.GetLength(0) + " arrays.");
        MapCellData cell = new MapCellData();
        cell.has_silo = BitConverter.ToBoolean(data_array[0],0);
        cell.is_nuked = BitConverter.ToBoolean(data_array[1],0);
        cell.owner_id = BitConverter.ToInt32(data_array[2],0);
        //cell.type = BitConverter.ToInt32(data_array[3],0);

        Debug.Log(data_array.Length);
        Debug.Log(data_array[4].Length);
        byte[][] land_bytes = ArrayConcatenation.UnmergeArrays(data_array[4]);
        List<Vector2> deserialized_coords = new List<Vector2>();
       // for (int i = 0; i < land_bytes.Length; i++)
       // {
         //   byte[][] coord_bytes = ArrayConcatenation.UnmergeArrays(land_bytes[i]);
          //  if(coord_bytes[0][0] == 0){
//
           // } else {
          //      Vector2 new_coords = new Vector2(BitConverter.ToSingle(coord_bytes[0],0),BitConverter.ToSingle(coord_bytes[1],0));
          //      deserialized_coords.Add(new_coords);
          //  }
        //}
        //cell.adjacent_land = deserialized_coords.ToArray();
        return(cell);
    }
    static byte[] SerializeMapCell(MapCellData d) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[5][];
        arrays[0] = BitConverter.GetBytes(d.has_silo);
        arrays[1] = BitConverter.GetBytes(d.is_nuked);
        arrays[2] = BitConverter.GetBytes(d.owner_id);
        //arrays[3] = BitConverter.GetBytes(d.type);
        //get bytes for neighbours
        List<byte[]> neighbour_bytes = new List<byte[]>();
        //if(d.adjacent_land != null)
            //for (int i = 0; i < d.adjacent_land.Length; i++)
            //{
                //byte[][] coordinates_byte = new byte[2][];
                //coordinates_byte[0] = BitConverter.GetBytes(d.adjacent_land[i].x);
                //coordinates_byte[1] = BitConverter.GetBytes(d.adjacent_land[i].y);
                //neighbour_bytes.Add(ArrayConcatenation.MergeArrays(coordinates_byte));
            //}
        //else {
            byte[] no_neighbours = new byte[1];
            no_neighbours[0] = (byte)0;
            neighbour_bytes.Add(no_neighbours);
        //}
        byte[] neighbour_array = ArrayConcatenation.MergeArrays(neighbour_bytes.ToArray());
        arrays[4] = neighbour_array;
        //Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        byte[] bytes = ArrayConcatenation.MergeArrays(arrays);
        Debug.Log(arrays.Length + " array length + bytes length"+ bytes.Length);
        return(bytes);
    }
    #endregion
    #region PlayerData
    public static byte[] SerializePlayerData(PlayerData p_data) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[4][];
        arrays[0] = System.Text.Encoding.UTF8.GetBytes(p_data.player_name);
        arrays[1] = BitConverter.GetBytes(p_data.photon_view_id);
        arrays[2] = BitConverter.GetBytes(p_data.player_id);
        arrays[3] = BitConverter.GetBytes(p_data.is_playing);
        //Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        return(ArrayConcatenation.MergeArrays(arrays));
    }
    public static PlayerData DeserializePlayerData(byte[] bytes) {
        PlayerData result_data = new PlayerData();
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        //Debug.Log("Deserialized "+data_array.GetLength(0) + " arrays.");
        result_data.player_name = System.Text.Encoding.UTF8.GetString(data_array[0]);
        result_data.photon_view_id = BitConverter.ToInt32(data_array[1],0);
        result_data.player_id = BitConverter.ToInt32(data_array[2],0);
        result_data.is_playing = BitConverter.ToBoolean(data_array[3],0);
        return(result_data);
    }
    #endregion 
}
