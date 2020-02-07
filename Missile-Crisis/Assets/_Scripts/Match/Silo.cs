using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silo : MonoBehaviour {
    public Transform range_transform;
    public Vector2 coords;
    public bool is_destroyed = false;

    public Renderer[] renderers;

    public void Setup (Color player_color) {
        MaterialPropertyBlock _propBlock = new MaterialPropertyBlock ();
        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].GetPropertyBlock (_propBlock);
            _propBlock.SetColor ("_Color", player_color + new Color (0.2f, 0.2f, 0.2f, 1));
            renderers[i].SetPropertyBlock (_propBlock);
        }
    }

    public void RotateTowardsMouse () {

        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint (transform.position);

        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2) Camera.main.ScreenToViewportPoint (Input.mousePosition);

        //Get the angle between the points
        float angle = AngleBetweenTwoPoints (positionOnScreen, mouseOnScreen);

        //Ta Daaa
        transform.rotation = Quaternion.Euler (new Vector3 (0f, -angle, 0f));
    }
    float AngleBetweenTwoPoints (Vector3 a, Vector3 b) {
        return Mathf.Atan2 (a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

}