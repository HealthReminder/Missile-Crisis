using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Serialization
{    
    //[SerializeField] public EnemyData serialize_event;
    //[SerializeField] public EnemyData deserialized_event
    #region MatchManager
    public static byte[] SerializeMatchData(MatchData m_data) {
        byte[][] arrays = new byte[3][];
        arrays[0] = BitConverter.GetBytes(m_data.is_war_on);
        arrays[1] = BitConverter.GetBytes(m_data.can_match_end);
        arrays[2] = BitConverter.GetBytes(m_data.is_match_over);
        return(ArrayConcatenation.MergeArrays(arrays));
    }
    public static MatchData DeserializeMatchData(byte[] bytes) {
        MatchData result_data = new MatchData();
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        result_data.is_war_on = BitConverter.ToBoolean(data_array[0],0);
        result_data.can_match_end = BitConverter.ToBoolean(data_array[1],0);
        result_data.is_match_over = BitConverter.ToBoolean(data_array[2],0);
        return(result_data);
    }
    public static byte[] SerializeCoordinates(Vector2[] coordinates) {
        byte[][] bytes = new byte[coordinates.Length*2][];
        //Debug.Log("Serializing "+coordinates.Length+" coordinates into "+bytes.Length);
        for (int i = 0; i < coordinates.Length;i++){
            bytes[i] = BitConverter.GetBytes(coordinates[i].x);
            bytes[coordinates.Length+i] = BitConverter.GetBytes(coordinates[i].y);
        }
        return(ArrayConcatenation.MergeArrays(bytes));
    }
    public static Vector2[] DeserializeCoordinates(byte[] bytes) {
        byte[][] bytes_arrays = ArrayConcatenation.UnmergeArrays(bytes);
        Vector2[] coordinates = new Vector2[bytes_arrays.Length/2];
        //Debug.Log("Deserializing "+coordinates.Length+" coordinates into "+bytes_arrays.Length);
        for (int i = 0; i < coordinates.Length; i++)
            coordinates[i] = new Vector2(-1,-1);
        
        for (int i = 0; i < coordinates.Length;i++){
            coordinates[i].x = BitConverter.ToSingle(bytes_arrays[i],0);
            coordinates[i].y = BitConverter.ToSingle(bytes_arrays[bytes_arrays.Length/2+i],0);
        }

        return(coordinates);
    }
    #endregion
    #region GameManager
    public static byte[] SerializeGameData(GameData g_data) {
        byte[][] arrays = new byte[3][];
        arrays[0] = BitConverter.GetBytes(g_data.is_match_started);
        arrays[1] = BitConverter.GetBytes(g_data.players_in_room);
        arrays[2] = BitConverter.GetBytes(g_data.votes_to_start);
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
    #region MapManager
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
    static byte[] SerializeMapCell(MapCellData d) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[4][];
        arrays[0] = BitConverter.GetBytes(d.has_silo);
        arrays[1] = BitConverter.GetBytes(d.is_nuked);
        arrays[2] = BitConverter.GetBytes(d.owner_id);
        arrays[3] = BitConverter.GetBytes(d.is_capital);
        //Debug.Log("Serialized "+arrays.GetLength(0) + " arrays.");
        //Concatenate the arrays
        byte[] bytes = ArrayConcatenation.MergeArrays(arrays);
        Debug.Log(arrays.Length + " array length + bytes length"+ bytes.Length);
        return(bytes);
    }
    static MapCellData DeserializeMapCell(byte[] bytes) {
        byte[][] data_array = ArrayConcatenation.UnmergeArrays(bytes);
        MapCellData cell = new MapCellData();
        cell.has_silo = BitConverter.ToBoolean(data_array[0],0);
        cell.is_nuked = BitConverter.ToBoolean(data_array[1],0);
        cell.owner_id = BitConverter.ToInt32(data_array[2],0);
        cell.is_capital = BitConverter.ToBoolean(data_array[3],0);

        return(cell);
    }
    #endregion
    #region PlayerData
    public static byte[] SerializePlayerData(PlayerData p_data) {
        //Create an array of the arrays you wanna serialize together
        byte[][] arrays = new byte[8][];
        arrays[0] = System.Text.Encoding.UTF8.GetBytes(p_data.player_name);
        arrays[1] = BitConverter.GetBytes(p_data.photon_view_id);
        arrays[2] = BitConverter.GetBytes(p_data.player_id);
        arrays[3] = BitConverter.GetBytes(p_data.is_playing);
        arrays[4] = BitConverter.GetBytes(p_data.is_placing);
        arrays[5] = BitConverter.GetBytes(p_data.left_silos);

        byte[][] color_bytes = new byte[4][];
        color_bytes[0] = BitConverter.GetBytes(p_data.player_color.r);
        color_bytes[1] = BitConverter.GetBytes(p_data.player_color.g);
        color_bytes[2] = BitConverter.GetBytes(p_data.player_color.b);
        color_bytes[3] = BitConverter.GetBytes(p_data.player_color.a);
        arrays[6] = ArrayConcatenation.MergeArrays(color_bytes);

        arrays[7] = BitConverter.GetBytes(p_data.left_missiles);

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
        result_data.is_placing = BitConverter.ToBoolean(data_array[4],0);
        result_data.left_silos = BitConverter.ToInt32(data_array[5],0);

        float[] color_data = new float[4];
        byte[][] color_bytes = ArrayConcatenation.UnmergeArrays(data_array[6]);
        for (int i = 0; i < color_bytes.Length; i++)
            color_data[i] = BitConverter.ToSingle(color_bytes[i],0);
        result_data.player_color = new Color(color_data[0],color_data[1],color_data[2],color_data[3]);

        result_data.left_missiles = BitConverter.ToInt32(data_array[7],0);
        
        return(result_data);
    }
    #endregion 
}
