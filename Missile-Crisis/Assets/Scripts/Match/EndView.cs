using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndView : MonoBehaviour
{
    public bool is_on = false;
    bool is_toggling = false;
    public AnimationCurve bouncy_curve;

    public Transform top_bar,bot_bar,player_bg,player_nuke, text_top,text_bot, player_image;
    public Text name_text;
    public static EndView instance;
    private void Awake() {
        instance = this;
    }
    private void Update() {
        if(is_on)
            if(Input.anyKeyDown)    {
                is_on = false;
                StartCoroutine(GUIOut());
            }
    }

    public IEnumerator GUIOut(){
        while(is_toggling)
            yield return null;
        is_toggling = true;


        float scale_prog = 0;
        while(scale_prog <= 1){
            player_image.localScale = new Vector3(bouncy_curve.Evaluate(scale_prog),bouncy_curve.Evaluate(scale_prog),bouncy_curve.Evaluate(scale_prog));
            player_bg.localScale = player_image.localScale;
            player_image.Rotate(new Vector3(0,0,bouncy_curve.Evaluate(scale_prog)*5));
            
            //bot_bar.position += new Vector3(scale_prog*50,0,0);
            //text_bot.position = bot_bar.position;
            //top_bar.position += new Vector3(-scale_prog*50,0,0);
            //text_top.position = top_bar.position;
            scale_prog+=Time.deltaTime;
            yield return null;
        }

        text_top.localScale = text_bot.localScale = bot_bar.localScale = top_bar.localScale = Vector3.zero;        

        is_toggling = false;
        yield break;
    }

    public IEnumerator GUIIn(string player_name) {
        while(is_toggling)
            yield return null;
        is_toggling = true;

        top_bar.localScale = Vector3.zero;
        bot_bar.localScale = Vector3.zero;
        player_nuke.localScale = Vector3.zero;
        player_image.localScale = Vector3.zero;
        player_bg.localScale = Vector3.zero;
        text_top.localScale = Vector3.zero;
        text_bot.localScale = Vector3.zero;
        player_image.rotation = Quaternion.Euler(0,0,0);

        top_bar.gameObject.SetActive(true);
        bot_bar.gameObject.SetActive(true);
        player_nuke.gameObject.SetActive(true);
        player_image.gameObject.SetActive(true);
        player_bg.gameObject.SetActive(true);
        text_top.gameObject.SetActive(true);
        text_bot.gameObject.SetActive(true);
        

        float scale_prog = 0;
        while(scale_prog <= 1){
            player_nuke.localScale = new Vector3(bouncy_curve.Evaluate(1-scale_prog),bouncy_curve.Evaluate(1-scale_prog),bouncy_curve.Evaluate(1-scale_prog));
            player_bg.localScale = player_nuke.localScale;
            player_nuke.Rotate(new Vector3(0,0,bouncy_curve.Evaluate(1-scale_prog)*1));
            scale_prog+=Time.deltaTime;
            yield return null;
        }
        scale_prog = 0;
        while(scale_prog <= 1){
            player_nuke.Rotate(new Vector3(0,0,-bouncy_curve.Evaluate(1-scale_prog)*2));
            scale_prog+=Time.deltaTime*2;
            yield return null;
        }

        scale_prog = 0;
        while(scale_prog <= 1){
            player_nuke.localScale = new Vector3(bouncy_curve.Evaluate(scale_prog),bouncy_curve.Evaluate(scale_prog),bouncy_curve.Evaluate(scale_prog));
            player_bg.localScale = player_nuke.localScale;
            player_nuke.Rotate(new Vector3(0,0,-4));
            scale_prog+=Time.deltaTime;
            yield return null;
        }

        name_text.text = player_name;
        text_top.localScale = text_bot.localScale = bot_bar.localScale = top_bar.localScale = Vector3.one;

        scale_prog = 0;
        while(scale_prog <= 1){
            player_image.localScale = new Vector3(bouncy_curve.Evaluate(1-scale_prog),bouncy_curve.Evaluate(1-scale_prog),bouncy_curve.Evaluate(1-scale_prog));
            player_bg.localScale = player_image.localScale;
            scale_prog+=Time.deltaTime*2;
            yield return null;
        }
        is_on = true;
        is_toggling = false;
        yield break;
    }

}
