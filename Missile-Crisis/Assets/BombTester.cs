using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTester : MonoBehaviour
{
    public GameObject bomb_prefab;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000)){
                int random_size = Random.Range(2,5);
                Instantiate(bomb_prefab,hit.point,Quaternion.identity).GetComponent<NuclearBombView>().Explode(random_size*random_size);
                Debug.Log("Exploding bomb of power "+ random_size);
            }
        }
    }
}
