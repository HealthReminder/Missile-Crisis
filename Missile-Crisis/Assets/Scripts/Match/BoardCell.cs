using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCell : MonoBehaviour
{
   public Renderer block_appearance;
   public int owner_id;
   public bool has_silo = false;
   public bool is_nuked = false;
   public Vector2 coordinates;
}
