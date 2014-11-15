using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class MusicPlayer : MonoBehaviour {

    private AudioSource source;
    public AudioClip [] clips;

    void OnEnable()
    {
        EventManager.OnChange += MusicLevelChange;
    }

    void OnDisable()
    {
        EventManager.OnChange -= MusicLevelChange;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        source = GetComponent<AudioSource>();
        clips = Resources.LoadAll<AudioClip>(@"Music") as AudioClip[];
        source.clip = clips[0];
        source.Play();
    }

    private void MusicLevelChange(int levelName)
    {
        //Change music at relavance of level
        AudioClip old = source.clip;
        source.clip = clips[(levelName < clips.Length) ? levelName : 0];
        if (old != source.clip)
        {
            source.Stop();
            source.Play();
        }
    }
}
