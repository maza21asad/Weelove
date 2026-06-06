using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneMusicData", menuName = "Audio/Scene Music Data")]
public class SceneMusicData : ScriptableObject
{
    public string sceneName;              // Must match Scene name
    public List<AudioClip> playlist = new List<AudioClip>();
}