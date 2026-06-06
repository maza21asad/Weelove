using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[Serializable]
//public struct QuesAnswer
//{
//    public string questions;
//    //public Image[] image;
//    public Sprite[] emogis;
//    public List<string> options;
//    public string answers;
//}

[Serializable]
public struct QuesAnswer
{
    public string questions;
    public Sprite[] emogis;
    public List<string> options;
    public string answers;
    public int correctAnswerIndex; // ← add this (0/1/2/3)
}



[Serializable]
public class TestData
{
    public string testsName;
    public List<QuesAnswer> quesAnswers;
}

[CreateAssetMenu(fileName = "QuestionData", menuName = "Question/QuesData")]

public class QuesData : ScriptableObject
{
    public List<TestData> tests;
}
