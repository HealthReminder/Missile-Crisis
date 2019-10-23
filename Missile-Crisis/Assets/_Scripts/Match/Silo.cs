using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silo : MonoBehaviour
{
    public Transform range_transform;
    public Vector2 coords;
    public bool is_destroyed = false;

    public Renderer[] renderers;

    public void Setup (Color player_color) {
        MaterialPropertyBlock _propBlock = new MaterialPropertyBlock();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_Color",player_color + new Color(0.2f,0.2f,0.2f,1));
            renderers[i].SetPropertyBlock(_propBlock);
        }
    }

}
