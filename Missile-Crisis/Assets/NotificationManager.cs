using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;
    private void Awake() {
        instance = this;
    }
    public AnimationCurve descendant_curve;
    public bool is_messaging;
    public RectTransform message_transform;
    public Text message_text;
    public void ShowMessage(string message,int duration) {
        StartCoroutine(MessageRoutine(message,duration));
    }
    IEnumerator MessageRoutine(string message, int duration) {
        while(is_messaging)
            yield return null;
        message_text.text = message;
        float prog;
        is_messaging = true;
        prog = 0;
        while(prog <= 1) {
            message_transform.localPosition = new Vector3(message_transform.localPosition.x, 450 -  descendant_curve.Evaluate(prog)*100, message_transform.localPosition.z);
            prog += Time.deltaTime;
            yield return null;
        }

        int seconds_left = duration; 
        while(seconds_left >= 0) {
            seconds_left --;
            yield return new WaitForSeconds(1);
        }
        
        prog = 0;
        while(prog <= 1) {
            message_transform.localPosition = new Vector3(message_transform.localPosition.x, 450 - descendant_curve.Evaluate(1-prog)*100, message_transform.localPosition.z);
            prog += Time.deltaTime;
            yield return null;
        }

        is_messaging = false;
        Debug.Log("Finished countdown");
        
    }
    public bool is_counting_down;
    public RectTransform countdown_transform;
    public Text countdown_text;

    public void ShowCountdown(int duration) {
        StartCoroutine(CountdownRoutine(duration));
    }
    IEnumerator CountdownRoutine(int duration) {
        countdown_text.text = duration.ToString();
        while(is_counting_down)
            yield return null;
        float prog;
        is_counting_down = true;
        prog = 0;
        while(prog <= 1) {
            countdown_transform.localPosition = new Vector3(countdown_transform.localPosition.x, 450 -  descendant_curve.Evaluate(prog)*100, countdown_transform.localPosition.z);
            prog += Time.deltaTime;
            yield return null;
        }

        int seconds_left = duration; 
        while(seconds_left >= 0) {
            countdown_text.text = seconds_left.ToString();
            seconds_left --;
            yield return new WaitForSeconds(1);
        }
        
        prog = 0;
        while(prog <= 1) {
            countdown_transform.localPosition = new Vector3(countdown_transform.localPosition.x, 450 - descendant_curve.Evaluate(1-prog)*100, countdown_transform.localPosition.z);
            prog += Time.deltaTime;
            yield return null;
        }

        is_counting_down = false;
        Debug.Log("Finished countdown");
        
    }
}
