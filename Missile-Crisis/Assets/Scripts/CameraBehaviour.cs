using System.Collections;
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

        float movement_velocity = 1;
        float min_y = 10;
        float max_y = 100;
        float current_y = transform.position.y;
        if(rate > 0) {
            if(!is_positive) {
                is_positive = true;
            }
            if(current_y > min_y) {
                camera.transform.position += camera.transform.forward*movement_velocity;
            }
        } else {
            if(is_positive) {
                is_positive = false;
            }
            if(current_y < max_y) {
                camera.transform.position -= camera.transform.forward*movement_velocity;
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
            transform.position = new Vector3(mid_pos.x,60,mid_pos.z);
            progress+= Time.deltaTime*5;
            if(progress > 1){
                transform.position = new Vector3(mid_pos.x,60,mid_pos.z);
                is_focusing = false;
            }
            yield return null;
        }
        can_zoom = true;
        yield break;
    }
}
