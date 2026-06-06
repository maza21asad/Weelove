using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RegionData
{
    public string regionName;
    public List<string> languages;
}

[System.Serializable]
public class ModeData
{
    public string modeName;
    public bool isUnlocked;
}

[CreateAssetMenu(fileName = "GameSettingsData", menuName = "Game/GameSettingsData")]
public class GameSettingsData : ScriptableObject
{
    public List<RegionData> regions;
    //public List<string> modes;
    public List<ModeData> modes;
}
