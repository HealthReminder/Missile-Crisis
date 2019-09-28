using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderBehaviour : MonoBehaviour
{
    public SpriteRenderer sprt;
    public Transform followingTransform;
    public int offset = 0;

    private void Update() {
        if(sprt){
            if(!followingTransform){
                sprt.sortingOrder = (int)(-transform.position.y*100)+offset;
            } else {
                sprt.sortingOrder = (int)(-followingTransform.position.y*100)+offset;
            }
        }
        
    }
}
