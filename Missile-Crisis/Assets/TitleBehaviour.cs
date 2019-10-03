using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBehaviour : MonoBehaviour
{
    public float delay;
    public GameObject overlay;
    public GameObject top;
    public GameObject bot;
    
    private void Start() {
        StartCoroutine(TitleRoutine());
    }
    IEnumerator TitleRoutine() {
        float next = 0;
        bool is_top = false;
        top.SetActive(true);
        bot.SetActive(false);
        overlay.SetActive(true);
        int change_count = 3;
        SoundtrackController.instance.ChangeSet("Intro");
        yield return new WaitForSeconds(1.5f);
        while(true) {
            if(next <= 0) {
                AudioController.instance.PlaySound(AudioController.instance.GetSoundByName("Menu_Siren"),transform.position);
                if(is_top){
                    is_top = false;
                    top.SetActive(false);
                    bot.SetActive(true);
                } else {
                    is_top = true;
                    top.SetActive(true);
                    bot.SetActive(false);
                }
                
                next = delay;
                change_count++;
                if(change_count >= 1/delay){
                    change_count = 0;
                    delay = delay*4/5;
                }
            } else {
                next -= Time.deltaTime;
            }

            if(Input.GetMouseButtonDown(0) || Input.anyKey)
                break;
            
            if(delay <= 0.15f){
                break;
            }

            yield return null;
        }
        top.SetActive(true);
        bot.SetActive(false);
        while(!Input.GetMouseButtonDown(0) || !Input.anyKey)
            yield return null;
        AudioController.instance.PlaySound(AudioController.instance.GetSoundByName("Menu_Siren"),transform.position);
        overlay.SetActive(false);
        top.SetActive(false);
        bot.SetActive(false);
        yield break;
    }
}
