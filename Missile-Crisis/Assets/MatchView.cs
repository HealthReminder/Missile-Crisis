using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchView : MonoBehaviour
{
    [Header("Map view")]
    public Transform map_container;
    public GameObject cell_prefab;
    public BoardCell[,] cell_map;
    public Gradient TESTPLAYERCOLORS;

    public bool is_drawing = false;
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
                if(map[o,i].owner_id == -1)
                    cell_map[o,i].static_appearance.color = Color.blue;
                else
                    cell_map[o,i].static_appearance.color = TESTPLAYERCOLORS.Evaluate(((float)map[o,i].owner_id)*0.25f);
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
                    new_obj.transform.position = new Vector3(o*1,i*1,0);
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
    public IEnumerator DrawDynamicMap(DynamicMap[,] dynamic_map) {
        while(is_drawing)
            yield return null;
        is_drawing = true;

        int size_x = dynamic_map.GetLength(0);
        int size_y = dynamic_map.GetLength(1);
        //List<Color> colors = new List<Color>();
        for (int i = 0; i < size_y; i++){
            for (int o = 0; o < size_x; o++){
                cell_map[o,i].owner_id = dynamic_map[o,i].owner_id;
                if(dynamic_map[o,i].owner_id == -1)
                    cell_map[o,i].static_appearance.color = Color.blue;
                else
                    if(dynamic_map[o,i].is_capital)
                        cell_map[o,i].static_appearance.color = TESTPLAYERCOLORS.Evaluate(((float)dynamic_map[o,i].owner_id)*0.25f)/1.2f;
                    else
                        cell_map[o,i].static_appearance.color = TESTPLAYERCOLORS.Evaluate(((float)dynamic_map[o,i].owner_id)*0.25f);
            }
            yield return null;
        }        
        is_drawing = false;
        yield break;
    }
    public IEnumerator DrawStaticMap(StaticMap[,] static_map) {
        while(is_drawing)
            yield return null;
        is_drawing = true;

        Clear();
        int size_x = static_map.GetLength(0);
        int size_y = static_map.GetLength(1);
        cell_map = new BoardCell[size_x,size_y];
        for (int i = 0; i < size_y; i++){
            for (int o = 0; o < size_x; o++){
                //obj
                GameObject new_obj = Instantiate(cell_prefab,transform.position,Quaternion.identity);
                new_obj.transform.position = new Vector3(o*1,i*1,0);
                new_obj.transform.parent = map_container;
                //cell
                BoardCell new_cell = new_obj.GetComponent<BoardCell>();
                new_cell.coordinates = new Vector2(o,i);
                cell_map[o,i] = new_cell;
                //appearance
                if(static_map[o,i].type == 1)
                    cell_map[o,i].static_appearance.color = Color.green;
                else
                    cell_map[o,i].static_appearance.color = Color.blue;
                


                new_obj.SetActive(true);
            }
            yield return null;
        }        
        is_drawing = false;
        yield break;
    }
    void Clear () {
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
