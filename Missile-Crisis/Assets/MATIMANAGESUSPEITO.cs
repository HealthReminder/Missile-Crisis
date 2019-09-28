using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MATIMANAGESUSPEITO : MonoBehaviour
{
    public bool is_showcasing = false;
    public MatchView match_view;
    public StaticMap[,] current_static_map;
    public DynamicMap[,] current_dynamic_map;

    [Range(0,100)]
    public int land_percentage;
    private void Start() {
        if(is_showcasing)
            StartCoroutine(ShowcaseRoutine());
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Y)){
            current_static_map = MapGenerator.GenerateValidStaticMap(50,3, land_percentage, 1);
            Debug.Log("MAP SEED = "+(current_static_map[0,0].type + current_static_map[1,0].type + current_static_map[1,0].type + current_static_map[1,1].type).ToString());
            StartCoroutine(match_view.DrawStaticMap(current_static_map));
            
        }
        if(Input.GetKeyDown(KeyCode.U)){
            current_dynamic_map = MapGenerator.GenerateValidDynamicMap(current_static_map,3,1);
            //for (int y = 0; y < current_dynamic_map.GetLength(1); y++){
                //string row = "";
                //for (int x = 0; x < current_dynamic_map.GetLength(0); x++)
                //    row += "|"+current_dynamic_map[x,y].owner_id;
               // Debug.Log(row);
            //}
            StartCoroutine(match_view.DrawDynamicMap(current_dynamic_map));
        }
    }

    IEnumerator ShowcaseRoutine() {
        yield return new WaitForSeconds(2);
        while(true) {
            int p_quantity = 3;
            current_static_map = MapGenerator.GenerateValidStaticMap(100,3, land_percentage , 1);
            current_dynamic_map = MapGenerator.GenerateValidDynamicMap(current_static_map,p_quantity,1);
            StartCoroutine(match_view.DrawStaticMap(current_static_map));
            StartCoroutine(match_view.DrawDynamicMap(current_dynamic_map));

            int[] counter = new int[p_quantity];
            for (int i = 0; i < counter.Length; i++)
                counter[i] = 0;
            
            for (int i = 0; i < current_dynamic_map.GetLength(1); i++)
                for (int o = 0; o < current_dynamic_map.GetLength(0); o++)
                    if(current_dynamic_map[o,i].owner_id != -1)
                        counter[current_dynamic_map[o,i].owner_id]++;
            
            for (int i = 0; i < counter.Length; i++)
                Debug.Log(counter[i]);
                
            
            yield return new WaitForSeconds(15);
        }
    }

}
