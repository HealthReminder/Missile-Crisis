using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NameSetBehaviour : MonoBehaviour
{
    [SerializeField] InputField input_field;
    public void SetNameTo() {
        PlayerPrefs.SetString("Player_Name",input_field.text);
    }
    public void TurnOff(){
        gameObject.SetActive(false);
    }
}
