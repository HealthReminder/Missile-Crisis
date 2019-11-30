using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    bool is_focusing = false;
    public bool can_zoom = true;
    public Transform focusing_transform;
    public AnimationCurve smooth_curve;
    public void Zoom(float rate) {
        if(!can_zoom)
            return;
        Debug.Log(rate);
        float movement_velocity = 1;
        float min_z = -10;
        float max_z = 20;
        float current_z = transform.localPosition.z;
        rate = rate*-1;
        if(rate > 0) {
            if(current_z > min_z) {
                transform.localPosition += transform.forward*movement_velocity*-1;
            }
        }else {
            if(current_z < max_z) {
                transform.localPosition += transform.forward*movement_velocity*1;
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
            Vector3 mid_pos = Vector3.Lerp(transform.localPosition,focusing_transform.transform.position,progress*smooth_curve.Evaluate(progress));
            transform.localPosition = new Vector3(mid_pos.x,60,mid_pos.z);
            progress+= Time.deltaTime*5;
            if(progress > 1){
                transform.localPosition = new Vector3(mid_pos.x,60,mid_pos.z);
                is_focusing = false;
            }
            yield return null;
        }
        can_zoom = true;
        yield break;
    }
}
