using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneMusicDatabase", menuName = "Audio/Scene Music Database")]
public class SceneMusicDatabase : ScriptableObject
{
    public List<SceneMusicData> scenes;
}