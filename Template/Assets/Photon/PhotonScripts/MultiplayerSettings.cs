using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSettings : MonoBehaviour
{
    public static MultiplayerSettings instance;
    public int maxPlayers;
    public int menuScene;
    public int multiplayerScene;

    //Singleton pattern
    private void Awake() {
        if(MultiplayerSettings.instance == null) 
            MultiplayerSettings.instance = this;
        else
            if(MultiplayerSettings.instance != this)
                Destroy(this);
        DontDestroyOnLoad(this.gameObject);
    }
}
