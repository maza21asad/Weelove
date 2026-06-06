using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestMusicData", menuName = "Audio/Test Music Data")]
public class TestMusicData : ScriptableObject
{
    [Header("Music")]
    //public AudioClip mainMenuMusic;
    //public AudioClip gameplayMusic;

    [Header("All Music")]
    public List<AudioEntry> allMusic = new List<AudioEntry>();

    [Header("Sound Effects")]
    public List<AudioEntry> soundEffects = new List<AudioEntry>();
}

[System.Serializable]
public class AudioEntry
{
    public string audioName;   // Example: "ButtonClick"
    public AudioClip clip;
}
