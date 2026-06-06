//using UnityEngine;
//using UnityEngine.UI;

//public class AnswerManager : MonoBehaviour
//{
//    public QuestionData questionData; // Reference to your QuestionData scriptable object
//    public ToggleGroup optionsToggleGroup; // Reference to the ToggleGroup containing answer options
//    public Toggle Toggle;

//    private AICarController car;
//    private GasBar gasBar;

//    public int wrongAnswerCount = 0; // Track wrong attempts for current question

//    private void Start()
//    {
//        car = FindObjectOfType<AICarController>();
//        gasBar = FindObjectOfType<GasBar>();
//    }

//    // Call this from your "Correct" button
//    public void OnCorrectAnswer()
//    {
//        wrongAnswerCount = 0; // reset for next question

//        if (gasBar != null)
//            gasBar.AddGas(0.1f); // reward fuel

//        if (car != null)
//        {
//            car.DismissCollectible();  // remove collectible only if correct
//            car.ResumeDriving();       // close question panel & continue
//        }
//    }


//    // Call this from your "Wrong" button
//    public void OnWrongAnswer()
//    {
//        wrongAnswerCount++;

//        if (car == null) return;

//        if (wrongAnswerCount == 1)
//        {
//            car.MoveBackWaypoints(1);
//        }
//        else if (wrongAnswerCount == 2)
//        {
//            car.MoveBackWaypoints(2);
//        }
//        else if (wrongAnswerCount >= 3)
//        {
//            car.RespawnAtStart();
//            wrongAnswerCount = 0;
//        }
//    }



//    public void OnConfirmAnswer()
//    {
//        // 1. Get selected toggle
//        Toggle selectedToggle = GetSelectedToggle();
//        if (selectedToggle == null) return; // no option picked

//        string chosenAnswer = selectedToggle.GetComponentInChildren<Text>().text;

//        // 2. Compare with correct answer
//        string correctAnswer = questionData.questionAnswers[0].answers; // assuming Q #0 for now

//        if (chosenAnswer == correctAnswer)
//        {
//            OnCorrectAnswer();
//        }
//        else
//        {
//            OnWrongAnswer();
//        }
//    }

//    private Toggle GetSelectedToggle()
//    {
//        foreach (var toggle in optionsToggleGroup.GetComponentsInChildren<Toggle>())
//        {
//            if (toggle.isOn)
//                return toggle;
//        }
//        return null;
//    }



//}
