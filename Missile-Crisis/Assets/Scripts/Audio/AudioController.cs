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
    [Range (0, 2)]
    public float pitchModularVariation;
    [Range (0, 1)]
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
        Sound c = availableSounds[soundIndex];
        AudioSource s = audioSources[currentSource];
        s.transform.position = soundPos;
        if(isInMenu)
            s.spatialBlend = 0;
        s.Stop ();
        s.clip = c.audioClip;
        s.minDistance = c.minDistance;
        s.volume = c.defaultVolume;
        s.pitch = c.defaultPitch;
        s.pitch = s.pitch + Random.Range(-c.pitchModularVariation,c.pitchModularVariation);
        s.spatialBlend = 1;
        s.Play ();

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