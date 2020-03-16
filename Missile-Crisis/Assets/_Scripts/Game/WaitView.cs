using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]   public class WaitView: MonoBehaviour
{
    public GameObject view_container;
    public Text situation_text;
    public Image clock_bg;
    public Transform button_transform;
    public Button button_button;
    public bool is_initialized = false;
    public AnimationCurve fade_curve;
    public Gradient color_gradient;
    private void Update() {
        //if(Input.GetKeyDown(KeyCode.A))
            //UpdateClock(10,Random.Range(0,8));
        
        //if(Input.GetKeyDown(KeyCode.S))
            //TogglePlayerCounter(true);
        
        //if(Input.GetKeyDown(KeyCode.D))
            //ToggleClock(false);
        
    }
    public void UpdateClock(int player_quantity, int current_votes) {
        int votes_needed = player_quantity/2+1;
        float percentage = 0;
        if(current_votes < votes_needed){
            //percentage = fade_curve.Evaluate(Mathf.InverseLerp(0,votes_needed,current_votes));
            percentage = Mathf.InverseLerp(0,votes_needed,current_votes);
            situation_text.text = "NATIONS IN WORLD: "+player_quantity+"\n"+"NATIONS AT WAR: "+current_votes+"/"+votes_needed;
        }   else {
            percentage = 1;
            situation_text.text = "WAR WILL START SOON";            
        }
        StartCoroutine(WorkGUI(percentage));    
    }
    public bool is_working_gui = false;
    IEnumerator WorkGUI (float target_percentage) {
        while(is_working_gui) {
            yield return null;
        }
        
        Debug.Log("Working GUI with "+target_percentage);
        float current_step = 0;
        is_working_gui = true;
        while(is_working_gui) {
            clock_bg.color = Color.Lerp(clock_bg.color, color_gradient.Evaluate(target_percentage),current_step);
            current_step += 0.01f;

            if(current_step >= 1)
                is_working_gui = false;
            yield return null;
        }
        Debug.Log("FINISHED "+target_percentage);
        is_working_gui = false;
    }
    public void TogglePlayerCounter(bool is_on) {
        is_initialized = is_on;
        if(is_initialized) {
            
            button_button.interactable = true;
        } else {
            button_button.interactable = false;
            situation_text.text = "WAITING FOR\nMAJOR POWERS";
            clock_bg.color = color_gradient.Evaluate(0);
        }
    }
    public void ToggleClock(bool is_on) {
        if(is_on) {
            view_container.SetActive(true);
        } else {
            StartCoroutine(TurnOffRoutine());
        }
    }
    IEnumerator TurnOffRoutine(){

        float prog = 0;
        float timeout = 1000;
        while(prog <= 1) {
            float inverse_curve = fade_curve.Evaluate(1-prog);

            button_transform.localScale = new Vector3(inverse_curve,inverse_curve,inverse_curve);

            clock_bg.transform.Rotate(new Vector3(0,0,-Time.deltaTime*100*inverse_curve));

            Color new_color = clock_bg.color;
            new_color.a = inverse_curve;
            clock_bg.color = new_color;

            Color new_color2 = situation_text.color;
            new_color2.a = inverse_curve;
            situation_text.color = new_color2;

            prog += Time.deltaTime/2;
            yield return null;
            timeout -= Time.deltaTime;
            if (timeout <= 0)
                break;
        }
        yield return null;
        view_container.SetActive(false);

        yield break;
    }

}
