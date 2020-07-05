using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleBehaviour : MonoBehaviour
{
    public bool is_on = true;
    public GameObject title_obj;
    public Transform nuke_trans;
    public Transform field_trans;
    public Text title_text;
    public AnimationCurve nuke_curve;
    public Image title_overlay;
    
    private void Start() {
        StartCoroutine(TitleRoutine());
    }
    public void TurnTitleOff() {
        StartCoroutine(OffROutine());
    }
    IEnumerator OffROutine() {
        int initial_title_size = title_text.text.Length;
        is_on = false;
        title_obj.SetActive(true);
        for (int i = 0; i < initial_title_size; i++)
        {
            char[] c = title_text.text.ToCharArray();
            string s = "";
            for (int o = 0; o < c.Length-1; o++)
                s += c[o];
            title_text.text = s;
            yield return new WaitForSeconds(0.05f);
        }
        float scale_prog = 0;
        while(scale_prog <= 1){
            nuke_trans.localScale = new Vector3(nuke_curve.Evaluate(scale_prog),nuke_curve.Evaluate(scale_prog),nuke_curve.Evaluate(scale_prog));
            nuke_trans.Rotate(new Vector3(0,0,1));
            field_trans.localScale = nuke_trans.localScale;
            scale_prog+=Time.deltaTime;
            yield return null;
        }
        while(title_overlay.color.a > 0) {
            title_overlay.color += new Color(0,0,0,-0.01f);
            yield return null;
        }
        title_overlay.color = new Color(0, 0, 0, 0);
        yield break;
    }
    float title_delay = 3;
    IEnumerator TitleRoutine() {
        if(SoundtrackController.instance)
            SoundtrackController.instance.ChangeSet("Intro");
        float title_wait = title_delay/2;
        while(is_on) {
            if(title_obj.activeSelf)
                if(title_wait >= title_delay) {
                    title_wait = 0;
                    title_obj.SetActive(false);
                    AudioController.instance.PlaySound("Menu_Siren",Vector3.zero);
                } else
                    title_wait+=Time.deltaTime;
            else 
                if(title_wait >= title_delay/4) {
                    title_wait = 0;
                    title_obj.SetActive(true);
                    
                } else
                    title_wait+=Time.deltaTime;
            nuke_trans.Rotate(new Vector3(0,0,1));
            yield return null;
        }
        yield break;
    }
}
