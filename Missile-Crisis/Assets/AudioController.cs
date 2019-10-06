using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable] public struct Sound {
    public string name;
    [Range (0.1f, 1)]
    public float defaultVolume;
    [Range (0.5f, 4)]
    public float defaultPitch;
    [Range (1, 5)]
    public int minDistance;
    public AudioClip audioClip;
}

public class AudioController : MonoBehaviour {
    public bool isInMenu = false;
    public int isOn = 0;
    [SerializeField] int sourceQuantity;
    [SerializeField] float maxListenerDistance;

    [SerializeField] Sound[] availableSounds;

    List<AudioSource> audioSources;
    int currentSource;
    public AudioListener playerListener;
    public static AudioController instance;
    void Awake () {
        //Make it the only one
        if (instance != null) {
            Destroy (gameObject);
        } else {
            instance = this;
        }

        Setup (playerListener);

    }

    public int FindSound (string soundName) {
        for (int i = 0; i < availableSounds.Length; i++) {
            if (availableSounds[i].name == soundName)
                return (i);
        }
        return (-1);
    }

    public void PlaySound (string soundName, Vector3 soundPos) {
        int soundIndex = FindSound(soundName);
        //Checa se a posição do som é perto o suficiente do player
        if(!isInMenu)
            if (Vector3.Distance (soundPos, playerListener.transform.position) > maxListenerDistance) {
                Debug.Log ("Sound is too far.");
                return;
            }
        if (soundIndex >= availableSounds.Length || soundIndex < 0) {
            Debug.Log ("Can't find sound");
            return;
        }

        audioSources[currentSource].transform.position = soundPos;
        if(isInMenu)
            audioSources[currentSource].spatialBlend = 0;
        audioSources[currentSource].Stop ();
        audioSources[currentSource].clip = availableSounds[soundIndex].audioClip;
        audioSources[currentSource].minDistance = availableSounds[soundIndex].minDistance;
        audioSources[currentSource].volume = availableSounds[soundIndex].defaultVolume;
        audioSources[currentSource].pitch = availableSounds[soundIndex].defaultPitch;
        audioSources[currentSource].spatialBlend = 1;
        audioSources[currentSource].Play ();

        currentSource++;
        if (currentSource >= audioSources.Count) {
            currentSource = 0;
        }

        Debug.Log ("Sound played.");
    }

    public int GetSoundByName (string soundName) {
        for (int i = 0; i < availableSounds.Length; i++) {
            if (availableSounds[i].name == soundName)
                return (i);
        }
        Debug.Log ("Could not find sound.");
        return (-1);
    }

    public void Setup (AudioListener playerL) {
        if (audioSources == null) {
            //There are no sources yet. Create them.
            audioSources = new List<AudioSource> ();
            for (int i = 0; i < sourceQuantity; i++) {
                GameObject newSourceObj = new GameObject ("Audio Source " + i);
                audioSources.Add (newSourceObj.AddComponent<AudioSource> ());
                newSourceObj.transform.parent = transform;

            }
        }
        //Reset the sources
        /* for (int i = 0; i < sourceQuantity; i++)
        {
            audioSources[i].transform.position = new Vector3(999,999,999);
            audioSources[i].clip = null;
            audioSources[i].Stop();
            audioSources[i].spatialBlend = 1;
        }*/

        playerListener = playerL;

        currentSource = 0;

        isOn = 1;
    }

}