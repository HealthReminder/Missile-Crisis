﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    bool is_focusing = false;
    bool can_zoom = true;
    public Transform focusing_transform;
    public Camera camera;
    public AnimationCurve smooth_curve;
    bool is_positive = true;
    public void Zoom(float rate) {
        if(!can_zoom)
            return;

        float max_size = 120;
        float min_size = 10;
        float movement_velocity = 250;
        float current_distance = camera.orthographicSize;
        float progress = Mathf.InverseLerp(min_size,max_size,current_distance);
        if(rate > 0) {
            if(!is_positive) {
                is_positive = true;
            }
            if(current_distance > min_size) {
                camera.orthographicSize += smooth_curve.Evaluate(progress)*-movement_velocity*Time.deltaTime*10;
            }
        } else {
            if(is_positive) {
                is_positive = false;
            }
            if(current_distance < max_size) {
                camera.orthographicSize += smooth_curve.Evaluate(1-progress)*movement_velocity*Time.deltaTime*5;
            }
        }
    }
    public void MoveFocus(Vector3 new_pos){
        Debug.Log("Moving camera");
        focusing_transform.position = new_pos;
        StartCoroutine(FocusTransform());
    }
    IEnumerator FocusTransform() {
        is_focusing = false;
        yield return null;
        is_focusing = true;

        float progress = 0;
        while(is_focusing) {
            Vector3 mid_pos = Vector3.Lerp(transform.position,focusing_transform.transform.position,progress*smooth_curve.Evaluate(progress));
            transform.position = new Vector3(mid_pos.x,mid_pos.y,-200);
            progress+= Time.deltaTime*5;
            if(progress > 1){
                transform.position = new Vector3(focusing_transform.transform.position.x,focusing_transform.transform.position.y,-200);
                is_focusing = false;
            }
            yield return null;
        }
        can_zoom = true;
        yield break;
    }
}