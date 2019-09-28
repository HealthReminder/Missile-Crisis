using System.Collections;
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
    public static byte[] SerializeMatchData(MatchData g_data) {
        //Create an array of the arrays you wanna serialize together
        //byte[][] arrays = new byte[1][];
        //Serialize map
        int map_x = g_data.map.GetLength(0);
        int map_y = g_data.map.GetLength(1);
        //Get bytes from all the map cells
        List<byte[]> all_cell_bytes = new List<byte[]>();


        for (int y = 0; y < map_y; y++)
            for (int x = 0; x < map_x; x++)
                all_cell_bytes.Add(SerializeMapCell(g_data.map[x,y]));
        //There cannot be more than 255 arrays concatenated together so lets separate them
        int size_limit = 4;
        int current_index = 0;
        List<List<byte[]>> lists_cell_bytes = new List<List<byte[]>>();
        List<byte[]> current_list = new List<byte[]>();
        for (int i = 0; i < all_cell_bytes.Count; i++)
        {
            current_list.Add(all_cell_bytes[i]);
            current_index ++;
            if(current_index >= size_limit){
                current_index = 0;
                lists_cell_bytes.Add(current_list);
                current_list = new List<byte[]>();
                //Debug.Log("Serialized a list!");
            }
        }
        //Debug.Log("Separated arrays! "+lists_cell_bytes.Count);
        //Now we have a list A of lists B
        //List B is a list of byte[]
        //Those byte[] are the data for the map cells
        List<byte[]> concatenated_cell_bytes = new List<byte[]>();
        for (int o = 0; o < lists_cell_bytes.Count; o++){
            concatenated_cell_bytes.Add(ArrayConcatenation.MergeArrays(lists_cell_bytes[o].ToArray()));
            //Debug.Log(lists_cell_bytes[o].Count);
        }
        //Debug.Log("Concatenated bytes! "+concatenated_cell_bytes.Count);

        //Get size of map
        //byte[][] map_size_array = new byte[2][];
        //map_size_array[0] = BitConverter.GetBytes(map_x);
        //map_size_array[1] = BitConverter.GetBytes(map_y);
        //byte[] map_size_bytes = ArrayConcatenation.MergeArrays(map_size_array);
        //concatenated_cell_bytes.Add(map_size_bytes);

        //for (int i = 0; i < concatenated_cell_bytes.Count; i++)
        //{
        //    Debug.Log("L: "+concatenated_cell_bytes[i].Length);
        //}
        //Now we have a list of byte[]
        //These byte[] are the map data concatenated and divided by arrays of length 200
        byte[] final_cell_bytes = ArrayConcatenation.MergeArrays(concatenated_cell_bytes.ToArray());
        Debug.Log("Concatenated the lists of concatenated cell bytes!");
        //arrays[0] = final_cell_bytes;
        //Concatenate the arrays
        return(final_cell_bytes);
    }
    public static MatchData DeserializeMatchData(byte[] map_bytes) {
        Debug.Log("Deserializing Match Data!");
        MatchData result_data = new MatchData();
        //We got a map
        //Maps are a group of byte[]
        byte[][] concatenated_cell_bytes = ArrayConcatenation.UnmergeArrays(map_bytes);
        //byte[][] size_bytes = ArrayConcatenation.UnmergeArrays(concatenated_cell_bytes[concatenated_cell_bytes.Length-1]);
        //int map_x = BitConverter.ToInt32(size_bytes[0],0);
        //int map_y = BitConverter.ToInt32(size_bytes[1],0);
        //Debug.Log(map_x);
        //Debug.Log(map_y);
        

        List<List<byte[]>> l = new List<List<byte[]>>();
        for (int i = 0; i < concatenated_cell_bytes.Length; i++)
        {
            List<byte[]> gathered_cells = new List<byte[]>();
            byte[][] bytes = ArrayConcatenation.UnmergeArrays(concatenated_cell_bytes[i]);
            for (int o = 0; o < bytes.Length; o++)
                gathered_cells.Add(bytes[o]);
            l.Add(gathered_cells);
        }
        //These byte[] are a group of byte[]
        List<byte[]> all_cells = new List<byte[]>();
        foreach (List<byte[]> b in l)
        {
            foreach (byte[] c in b)
            {
                all_cells.Add(c);
            }
        }
        //Get bytes from all the map cells
        //MapCellData[,] result_map = new MapCellData[map_x,map_y];
        //for (int y = 0; y < map_y; y++)
            //for (int x = 0; x < map_x; x++)
                //result_map[x,y] = DeserializeMapCell(all_cells[y*map_x + x]);
        List<MapCellData> cells = new List<MapCellData>();
        for (int i = 0; i < all_cells.Count; i++)
        {
            cells.Add(DeserializeMapCell(all_cells[i]));
        }
        
        //Each byte[] is a map cell
        //Lets get the cells data
        Debug.Log("Finished deserializing map!");
        return(result_data);
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
