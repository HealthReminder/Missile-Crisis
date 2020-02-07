using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionView : MonoBehaviour {
    public int player_id;
    public GameObject container;
    public void SelectCharacter (string character) {
        MatchManager.instance.SelectCharacter(player_id,character);
        ToggleGUI(false);
    }
    public void ToggleGUI(bool is_on) {
        if(is_on) {
            container.SetActive(true);
        } else {
            container.SetActive(false);
        }
    }
}