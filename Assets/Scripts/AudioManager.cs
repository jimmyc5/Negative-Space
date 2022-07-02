using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    // Start is called before the first frame update
    public static AudioManager instance;
    public bool muted = false;
    private bool wait = false;
    public bool doDynamicMusic = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if(s.name == "ThemeBlack")
            {
                s.source.mute = true;
            }
        }
    }
    void Start()
    {
        if (doDynamicMusic)
        {
            Play("ThemeWhite");
            Play("ThemeBlack");
        }
        else
        {
            Play("Theme");
        }
    }
    private void Update()
    {
        if (doDynamicMusic)
        {
            if (Input.GetKeyDown(KeyCode.M) && !wait)
            {
                wait = true;
                if (!muted)
                {
                    muted = true;
                    Stop("ThemeWhite");
                    Stop("ThemeBlack");
                }
                else
                {
                    muted = false;
                    Play("ThemeWhite");
                    Play("ThemeBlack");
                }
            }
            if (Input.GetKeyUp(KeyCode.M))
            {
                wait = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.M) && !wait)
            {
                wait = true;
                if (!muted)
                {
                    muted = true;
                    Stop("Theme");
                }
                else
                {
                    muted = false;
                    Play("Theme");
                }
            }
            if (Input.GetKeyUp(KeyCode.M))
            {
                wait = false;
            }
        }
    }

    // Update is called once per frame
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("sound: " + name + "not found.");
            return;
        }
        s.source.Play();
    }

    public void SetTheme(bool setThemeToWhite)
    {
        if (setThemeToWhite)
        {
            Sound s = Array.Find(sounds, sound => "ThemeBlack" == sound.name);
            if (s == null)
            {
                Debug.Log("black theme not found.");
                return;
            }
            s.source.mute = true;
            Sound s2 = Array.Find(sounds, sound => "ThemeWhite" == sound.name);
            if (s2 == null)
            {
                Debug.Log("white theme not found.");
                return;
            }
            s2.source.mute = false;
        }
        else
        {
            Sound s = Array.Find(sounds, sound => "ThemeBlack" == sound.name);
            if (s == null)
            {
                Debug.Log("black theme not found.");
                return;
            }
            s.source.mute = false;
            Sound s2 = Array.Find(sounds, sound => "ThemeWhite" == sound.name);
            if (s2 == null)
            {
                Debug.Log("white theme not found.");
                return;
            }
            s2.source.mute = true;
        }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("sound: " + name + "not found.");
            return;
        }
        s.source.Stop();
    }
}
