using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct QuestionAnswer
{
    public string questions;
    public List<string> options;
    public string answers;
}

[CreateAssetMenu(fileName = "QuestionData", menuName = "Question/QuestionData")]

public class QuestionData : ScriptableObject
{
    public List<QuestionAnswer> questionAnswers;
}
