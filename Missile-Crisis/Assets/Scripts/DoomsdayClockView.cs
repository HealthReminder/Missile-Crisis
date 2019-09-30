using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]   public class DoomsdayClockView: MonoBehaviour
{
    public GameObject view_container;
    public Text situation_text;
    public Image clock_bg;
    public Transform clock_handle;
    public Button doom_button;
    public bool is_initialized = false;
    public AnimationCurve fade_curve;
    public void UpdateClock(int player_quantity, int current_votes){
        int votes_needed = player_quantity/2+1;
        float percentage = 0;
        if(current_votes >= votes_needed){
            percentage = 1;
            situation_text.text = "WAR WILL START SOON";
        }   else {
            Debug.Log("Percentage " + percentage);
            percentage = fade_curve.Evaluate(Mathf.InverseLerp(0,votes_needed,current_votes));
            Debug.Log("Percentage in curve " + percentage);
            situation_text.text = "NATIONS IN WORLD: "+player_quantity+"\n"+"NATIONS AT WAR: "+current_votes+"/"+votes_needed;
        }
        StartCoroutine(WorkGUI(percentage));    
    }
    public bool is_working_gui = false;
    IEnumerator WorkGUI (float target_percentage) {
        while(is_working_gui)
            yield return null;
        Debug.Log("Working GUI with "+target_percentage);
        is_working_gui = true;
        float step = 0.01f;
        float current_step = step;
        while(is_working_gui) {

            Quaternion target_rotation = Quaternion.Euler(0,0,180 - 360*target_percentage);
            clock_handle.rotation = target_rotation;
            //clock_handle.rotation = Quaternion.Lerp(clock_handle.rotation,target_rotation,current_step);
            //Color target_color = new Color(current_step,current_step,current_step,1);

            Color target_color = new Color(target_percentage,target_percentage,target_percentage,1);
            clock_bg.color += (target_color - clock_bg.color)*current_step;

            current_step+= step;
            if(current_step > 1)
                is_working_gui = false;

            yield return null;
        }
        is_working_gui = false;
    }
    public void TogglePlayerCounter(bool is_on) {
        is_initialized = is_on;
        if(is_initialized) {
            doom_button.interactable = true;
        } else {
            situation_text.text = "WAITING FOR\nMAJOR POWERS";
            clock_bg.color = new Color(0,0,0,1);
            clock_handle.rotation = Quaternion.Euler(0,0,180);
            doom_button.interactable = false;
        }
        is_working_gui = false;
    }
    public void ToggleClock(bool is_on) {
        view_container.SetActive(is_on);
    }

}
