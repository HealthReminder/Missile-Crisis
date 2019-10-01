using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{
    [Header("Map view")]
    public Transform map_container;
    public GameObject cell_prefab;
    public BoardCell[,] cell_map;

    public bool is_drawing = false;
    public void UpdateMap(MapCellData[,] map, int player_id) {
        int size_x = map.GetLength(0);
        int size_y = map.GetLength(1);
        SetupMap(size_x,size_y);
        for (int i = 0; i < size_y; i++){
            for (int o = 0; o < size_x; o++){
                BoardCell cell = cell_map[o,i];
                cell.owner_id = map[o,i].owner_id;
                cell.has_silo = map[o,i].has_silo;
                cell.is_nuked = map[o,i].is_nuked;
                MaterialPropertyBlock _propBlock = new MaterialPropertyBlock();
                cell.block_appearance.GetPropertyBlock(_propBlock);
                if(cell.owner_id == -1){
                    if(!cell.is_nuked)
                        _propBlock.SetColor("_Color",new Color(0,0,0.6f,1));
                    else
                        _propBlock.SetColor("_Color", new Color(0,0,0.2f,1));
                } else {
                    Color player_color = GameManager.instance.listOfPlayersPlaying[cell.owner_id].data.player_color;
                    if(cell.owner_id != player_id){
                        if(!cell.is_nuked)
                            _propBlock.SetColor("_Color",new Color(player_color.r,player_color.g,player_color.b,1));
                        else if(!cell.has_silo && cell.is_nuked)
                            _propBlock.SetColor("_Color",new Color(player_color.r/2,player_color.g/2,player_color.b/2,1));
                        else if(cell.has_silo && cell.is_nuked)
                            _propBlock.SetColor("_Color",new Color(player_color.r/3,player_color.g/3,player_color.b/3,1));
                        
                    }
                    else if(cell.owner_id == player_id){
                        if(cell.has_silo && !cell.is_nuked)
                            _propBlock.SetColor("_Color",new Color(player_color.r,player_color.g,player_color.b,1));
                        else if(!cell.has_silo && !cell.is_nuked)
                            _propBlock.SetColor("_Color",new Color(player_color.r/2,player_color.g/2,player_color.b/2,1));
                        else if(!cell.has_silo && cell.is_nuked)
                            _propBlock.SetColor("_Color",new Color(player_color.r/3,player_color.g/3,player_color.b/3,1));
                        else if(cell.has_silo && cell.is_nuked)
                            _propBlock.SetColor("_Color",new Color(player_color.r/4,player_color.g/4,player_color.b/4,1));
                    }
                }
                cell.block_appearance.SetPropertyBlock(_propBlock);
                //else if(cell.owner_id == player_id) {
                //    if(cell.has_silo)
                //        cell.static_appearance.color = new Color(player_color.r,player_color.g,player_color.b,1);
                //    else 
                //        cell.static_appearance.color = new Color(player_color.r/4,player_color.g/4,player_color.b/4,1);
                //} else 
                //        cell.static_appearance.color = new Color(player_color.r/4,player_color.g/4,player_color.b/4,1);
                
                    
            }
        }   
    }
    public IEnumerator DrawMap(MapCellData[,] map) {
        while(is_drawing)
            yield return null;
        is_drawing = true;
        
        int size_x = map.GetLength(0);
        int size_y = map.GetLength(1);
        SetupMap(size_x,size_y);
        //List<Color> colors = new List<Color>();
        for (int i = 0; i < size_y; i++){
            for (int o = 0; o < size_x; o++){
                cell_map[o,i].owner_id = map[o,i].owner_id;
                cell_map[o,i].has_silo = map[o,i].has_silo;
                MaterialPropertyBlock _propBlock = new MaterialPropertyBlock();
                cell_map[o,i].block_appearance.GetPropertyBlock(_propBlock);
                Vector3 cur_pos = cell_map[o,i].block_appearance.transform.position;
                if(map[o,i].owner_id == -1) {
                    _propBlock.SetColor("_Color",new Color(0,0,0.4f,1));
                    cell_map[o,i].block_appearance.transform.position = new Vector3(cur_pos.x,0,cur_pos.z);
                } else {
                    _propBlock.SetColor("_Color",new Color(0,0,0f,1));
                    cell_map[o,i].block_appearance.transform.position = new Vector3(cur_pos.x,1,cur_pos.z);
                }
                cell_map[o,i].block_appearance.SetPropertyBlock(_propBlock);
            }
            yield return null;
        }        
        is_drawing = false;
        yield break;
    }
    void SetupMap(int xsize, int ysize) {
        if(cell_map == null || cell_map.GetLength(0) != xsize || cell_map.GetLength(1) != ysize) {
            Clear();
            cell_map = new BoardCell[xsize,ysize];
            for (int i = 0; i < ysize; i++){
                for (int o = 0; o < xsize; o++){
                    //obj
                    GameObject new_obj = Instantiate(cell_prefab,transform.position,Quaternion.identity);
                    new_obj.transform.position = new Vector3(o*1,0,i*1);
                    new_obj.transform.parent = map_container;
                    //cell
                    BoardCell new_cell = new_obj.GetComponent<BoardCell>();
                    new_cell.coordinates = new Vector2(o,i);
                    cell_map[o,i] = new_cell;
                    new_obj.SetActive(true);
                }
            }   
        }
    }
    public void Clear () {
        if(cell_map != null){
            float mx = cell_map.GetLength(0);
            float my = cell_map.GetLength(1);
            for (int i = 0; i < my; i++)
                for (int o = 0; o < mx; o++)
                    if(cell_map[o,i] != null)
                        Destroy(cell_map[o,i].gameObject);
        }
        cell_map = null;
    }
}
