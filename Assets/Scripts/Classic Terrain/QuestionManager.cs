using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

[System.Serializable]
public class QuestionUISet
{
    public GameObject questionPanel;
    public TMP_Text questionText;
    public List<Toggle> optionToggles;
    public List<TMP_Text> optionLabels;
    public ToggleGroup toggleGroup;
    public TMP_Text timerText;
    public TMP_Text answerText;
    public Text scoreText;
    public Text wrongAnswersText;
    public GameObject gameOver;
    public List<Image> emojisImages;
    public AndroidTTS tts;

    [Header("Fonts")]
    public TMP_FontAsset defaultFont;
    public TMP_FontAsset amharicFont;
    public TMP_FontAsset russianFont;    // ← add this

    public TMP_FontAsset chineseFont;
    public TMP_FontAsset spanishFont; // optional
    public TMP_FontAsset engFont;     // optional (if different)

    public Font chineseLegacyFont;
    public Font spanishLegacyFont;
    public Font engLegacyFont;

    [Header("Legacy Fonts")]
    public Font defaultLegacyFont;          // ← normal legacy font
    public Font amharicLegacyFont;
}

public class QuestionManager : MonoBehaviour
{
    [Header("Data")]
    public QuesData englishQuesData;
    public QuesData hebrewQuesData;
    public QuesData russianQuesData;
    public QuesData arabicQuesData;
    public QuesData amharicQuesData;  // ← add this

    public QuesData engQuesData;
    public QuesData chineseQuesData;
    public QuesData spanishQuesData;


    [Header("UI Sets")]
    public QuestionUISet portraitUI;
    public QuestionUISet landscapeUI;

    [Header("Level Settings")]
    public int levelTestIndex = 0;

    private QuestionUISet ui;

    [Header("Timer")]
    public float questionTime = 15f;

    [Header("Level Timer Settings")]
    public bool useTimer = false;
    public float timedLevelDuration = 15f;

    private int currentQuestionIndex = 0;
    private float currentTime;
    private bool isCountingDown = false;

    private bool isSceneUnloading = false;

    private SoundManager soundManager;
    private Car car;
    private GasBar gasBar;

    [Header("Game Data")]
    public int score = 0;
    public int life = 3000;

    bool lastLandscape;


    private QuesData quesData;

    //Shuffling mechanism 
    private List<QuesAnswer> shuffledQuestions = new List<QuesAnswer>();

    private TriggerZone currentTriggerZone;


    private Coroutine speakCoroutine = null;
    private bool answerSubmitted = false;

    [Header("Wrong Answer Tracking")]
    private int wrongAnswerCount = 0;

    // ============================
    // UNITY EVENTS
    // ============================


    void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void OnSceneUnloaded(Scene scene)
    {
        isSceneUnloading = true;
    }

    void Start()


    {

        DOTween.Init();

        switch (LocalizationManager.currentLanguage)
        {
            case LocalizationManager.Language.ENG:
                quesData = engQuesData != null ? engQuesData : englishQuesData;
                break;

            case LocalizationManager.Language.Hebrew:
                quesData = hebrewQuesData != null ? hebrewQuesData : englishQuesData;
                break;

            case LocalizationManager.Language.Russian:
                quesData = russianQuesData != null ? russianQuesData : englishQuesData;
                break;

            case LocalizationManager.Language.Arabic:
                quesData = arabicQuesData != null ? arabicQuesData : englishQuesData;
                break;

            case LocalizationManager.Language.Amharic:
                quesData = amharicQuesData != null ? amharicQuesData : englishQuesData;
                break;

            case LocalizationManager.Language.Chinese:
                quesData = chineseQuesData != null ? chineseQuesData : englishQuesData;
                break;

            case LocalizationManager.Language.Spanish:
                quesData = spanishQuesData != null ? spanishQuesData : englishQuesData;
                break;

            default:
                quesData = englishQuesData;
                break;
        }

        car = FindObjectOfType<Car>();
        gasBar = FindObjectOfType<GasBar>();
        soundManager = FindObjectOfType<SoundManager>();

        ShuffleQuestions();

        UpdateScoreUI();
        UpdateWrongAnswersUI();
    }

    void ShuffleQuestions()
    {
        if (quesData == null || levelTestIndex >= quesData.tests.Count) return;

        shuffledQuestions = new List<QuesAnswer>(quesData.tests[levelTestIndex].quesAnswers);

        // Fisher-Yates shuffle
        for (int i = shuffledQuestions.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            QuesAnswer temp = shuffledQuestions[i];
            shuffledQuestions[i] = shuffledQuestions[rand];
            shuffledQuestions[rand] = temp;
        }

        Debug.Log($"Questions shuffled: {shuffledQuestions.Count} total");
    }



    void Update()
    {
        UpdateUISet();

        if (!isCountingDown || isSceneUnloading) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isCountingDown = false;
            HandleTimeUp();
        }


        UpdateTimerUI();
    }

    void HandleTimeUp()
    {
        if (!useTimer)
            return;

        if (ui != null)
            ui.answerText.text = "Time's Up!";

        // Move back 3 waypoints
        if (car != null)
            car.MoveBackByWaypoints(3);

        // Deduct score
        score = Mathf.Max(0, score - 1);
        UpdateScoreUI();

        StartCoroutine(HideQuestionPanelAfterDelay());
    }


    void UpdateUISet()
    {
        bool isLandscape = Screen.width > Screen.height;

        if (ui == null || isLandscape != lastLandscape)
        {
            ui = isLandscape ? landscapeUI : portraitUI;
            lastLandscape = isLandscape;
        }
    }

    void UpdateTimerUI()
    {
        if (ui != null && ui.timerText != null)
            ui.timerText.text = Mathf.CeilToInt(currentTime).ToString();
    }

    // ============================
    // QUESTION FLOW
    // ============================

    public void ShowNextQuestion(TriggerZone zone = null)
    {
        currentTriggerZone = zone; // ← store reference

        if (isSceneUnloading) return;
        if (ui == null || ui.questionPanel == null) return;
        if (shuffledQuestions == null || shuffledQuestions.Count == 0) return;

        if (currentQuestionIndex >= shuffledQuestions.Count)
        {
            StartNextTest();
            return;
        }

        ShowQuestion(shuffledQuestions[currentQuestionIndex]);
    }

    void ShowQuestion(QuesAnswer qa)
    {
        if (isSceneUnloading) return;
        if (ui == null || ui.questionPanel == null) return;

        if (car != null)
            car.PauseCar();

        SoundManager.Instance?.FadeMusicOut(); // ← fade music out when question appears

        GameObject panel = ui.questionPanel;
        panel.SetActive(true);

        SwapFontForLanguage();

        RectTransform rt = panel.GetComponent<RectTransform>();
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();

        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        // Kill previous tweens to prevent overlap
        rt.DOKill();
        cg.DOKill();

        // Reset state
        rt.localScale = Vector3.zero;
        cg.alpha = 0f;

        // Smooth professional open animation
        rt.DOScale(Vector3.one, 0.5f)
          .SetEase(Ease.OutBack);

        cg.DOFade(1f, 0.4f);

        SoundManager.Instance?.PlaySFX("QuestionPopup");

        if (useTimer)
        {
            currentTime = timedLevelDuration;
            isCountingDown = true;
            UpdateTimerUI();
        }
        else
        {
            isCountingDown = false;

            if (ui.timerText != null)
                ui.timerText.text = "";
        }


        ui.questionText.text = qa.questions;

        //ui.tts.Speak(qa.questions);     // tts added here
        //AndroidTTS.instance.Speak(qa.questions);
        //StartCoroutine(SpeakQuestionAndOptions(qa));

        answerSubmitted = false;
        //if (speakCoroutine != null) StopCoroutine(speakCoroutine);
        if (speakCoroutine != null)
        {
            StopCoroutine(speakCoroutine);
            //AndroidTTS.instance?.Stop();
        }
        speakCoroutine = StartCoroutine(SpeakQuestionAndOptions(qa));

        for (int i = 0; i < ui.optionToggles.Count; i++)
        {
            if (i < qa.options.Count)
            {
                ui.optionLabels[i].text = qa.options[i];
                ui.optionToggles[i].gameObject.SetActive(true);
                ui.optionToggles[i].isOn = false;
                ui.optionToggles[i].group = ui.toggleGroup;

                int index = i; // capture index for lambda
                ui.optionToggles[i].onValueChanged.RemoveAllListeners();
                ui.optionToggles[i].onValueChanged.AddListener((isOn) =>
                {
                    if (isOn) SoundManager.Instance?.PlaySFX("Tap");
                });
            }
            else
            {
                ui.optionToggles[i].gameObject.SetActive(false);
            }
        }

        ui.toggleGroup.SetAllTogglesOff(true);

        for (int i = 0; i < ui.emojisImages.Count; i++)
        {
            if (qa.emogis != null && i < qa.emogis.Length)
            {
                ui.emojisImages[i].sprite = qa.emogis[i];
                ui.emojisImages[i].gameObject.SetActive(true);
            }
            else
            {
                ui.emojisImages[i].gameObject.SetActive(false);
            }
        }
    }


    void SwapFontForLanguage()
    {
        if (ui == null) return;

        bool isAmharic = LocalizationManager.currentLanguage == LocalizationManager.Language.Amharic;
        bool isRussian = LocalizationManager.currentLanguage == LocalizationManager.Language.Russian;
        bool isChinese = LocalizationManager.currentLanguage == LocalizationManager.Language.Chinese;
        bool isSpanish = LocalizationManager.currentLanguage == LocalizationManager.Language.Spanish;
        bool isENG = LocalizationManager.currentLanguage == LocalizationManager.Language.ENG;

        bool isRTL = LocalizationManager.currentLanguage == LocalizationManager.Language.Arabic ||
                     LocalizationManager.currentLanguage == LocalizationManager.Language.Hebrew;

        TMP_FontAsset targetTMPFont;

        if (isAmharic) targetTMPFont = ui.amharicFont;
        else if (isRussian && ui.russianFont != null) targetTMPFont = ui.russianFont;
        else if (isChinese && ui.chineseFont != null) targetTMPFont = ui.chineseFont;
        else if (isSpanish && ui.spanishFont != null) targetTMPFont = ui.spanishFont;
        else if (isENG && ui.engFont != null) targetTMPFont = ui.engFont;
        else targetTMPFont = ui.defaultFont;


        // this might cause bug in font UTSHO
        Font targetLegacyFont;

        if (isAmharic) targetLegacyFont = ui.amharicLegacyFont;
        else if (isChinese && ui.chineseLegacyFont != null) targetLegacyFont = ui.chineseLegacyFont;
        else if (isSpanish && ui.spanishLegacyFont != null) targetLegacyFont = ui.spanishLegacyFont;
        else if (isENG && ui.engLegacyFont != null) targetLegacyFont = ui.engLegacyFont;
        else targetLegacyFont = ui.defaultLegacyFont;




        // ← TMP texts
        if (targetTMPFont != null)
        {
            if (ui.questionText != null)
            {
                ui.questionText.font = targetTMPFont;
                ui.questionText.isRightToLeftText = isRTL;
                ui.questionText.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
            }

            if (ui.answerText != null)
                ui.answerText.font = targetTMPFont;

            if (ui.timerText != null)
                ui.timerText.font = targetTMPFont;

            foreach (var toggle in ui.optionToggles)
            {
                if (toggle == null) continue;
                TMP_Text tmp = toggle.GetComponentInChildren<TMP_Text>();
                if (tmp != null)
                {
                    tmp.font = targetTMPFont;
                    tmp.isRightToLeftText = isRTL;
                    tmp.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                }
            }
        }

        // ← Legacy texts
        if (targetLegacyFont != null)
        {
            if (ui.scoreText != null)
                ui.scoreText.font = targetLegacyFont;

            if (ui.wrongAnswersText != null)
                ui.wrongAnswersText.font = targetLegacyFont;

            // Replace legacy label loop with this:
            foreach (var label in ui.optionLabels)
            {
                if (label != null)
                {
                    label.font = targetTMPFont;
                    label.isRightToLeftText = isRTL;
                    label.alignment = isRTL ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
                }
            }
        }
    }

    //IEnumerator SpeakQuestionAndOptions(QuesAnswer qa)
    //{
    //    if (AndroidTTS.instance == null)
    //        yield break;

    //    // Speak Question
    //    AndroidTTS.instance.Speak(qa.questions);

    //    // wait for question speech
    //    yield return new WaitForSeconds(2f);

    //    // Speak Options
    //    for (int i = 0; i < qa.options.Count; i++)
    //    {
    //        string optionText = qa.options[i];

    //        AndroidTTS.instance.Speak(optionText);

    //        // wait before speaking next option
    //        yield return new WaitForSeconds(0.7f);
    //    }
    //}
    IEnumerator SpeakQuestionAndOptions(QuesAnswer qa)
    {
        //if (AndroidTTS.instance == null) yield break;
        if (AndroidTTS.instance == null || !AndroidTTS.instance.IsEnabled()) yield break;

        AndroidTTS.instance.Speak(qa.questions);

        float elapsed = 0f;
        float duration = EstimateSpeakDuration(qa.questions);
        while (elapsed < duration)
        {
            if (answerSubmitted) { AndroidTTS.instance?.Stop(); yield break; }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (answerSubmitted) { AndroidTTS.instance?.Stop(); yield break; }
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < qa.options.Count; i++)
        {
            if (answerSubmitted) { AndroidTTS.instance?.Stop(); yield break; }
            AndroidTTS.instance.Speak(qa.options[i]);

            float optElapsed = 0f;
            float optDuration = EstimateSpeakDuration(qa.options[i]);
            while (optElapsed < optDuration)
            {
                if (answerSubmitted) { AndroidTTS.instance?.Stop(); yield break; }
                optElapsed += Time.deltaTime;
                yield return null;
            }

            if (answerSubmitted) { AndroidTTS.instance?.Stop(); yield break; }
            yield return new WaitForSeconds(0.4f);
        }
        speakCoroutine = null;
    }

    // Estimates how long a text will take to speak
    // based on average speaking speed (~2.5 words per second)
    private float EstimateSpeakDuration(string text)
    {
        if (string.IsNullOrEmpty(text)) return 1f;

        int wordCount = text.Trim().Split(' ').Length;
        float duration = wordCount / 2.5f;

        // Minimum 1.5 seconds, maximum 10 seconds
        return Mathf.Clamp(duration, 1.5f, 10f);
    }

    // read current question using TTS (can be called from a button)
    //public void ReadCurrentQuestion()
    //{
    //    if (levelTestIndex >= quesData.tests.Count) return;
    //    if (currentQuestionIndex >= quesData.tests[levelTestIndex].quesAnswers.Count) return;

    //    //AndroidTTS.instance?.Stop();

    //    var qa = quesData.tests[levelTestIndex].quesAnswers[currentQuestionIndex];
    //    AndroidTTS.instance.Speak(qa.questions);
    //}
    public void ReadCurrentQuestion()
    {
        if (shuffledQuestions == null || shuffledQuestions.Count == 0) return;
        if (currentQuestionIndex >= shuffledQuestions.Count) return;

        // Stop any existing speech first
        //AndroidTTS.instance?.Stop();

        var qa = shuffledQuestions[currentQuestionIndex];
        //StartCoroutine(SpeakQuestionAndOptions(qa));
        if (speakCoroutine != null)
        {
            StopCoroutine(speakCoroutine);
            //AndroidTTS.instance?.Stop();
        }

        answerSubmitted = false;
        speakCoroutine = StartCoroutine(SpeakQuestionAndOptions(qa));
    }


    // ============================
    // ANSWER CHECK
    // ============================

    public void CheckAnswer()
    {

        answerSubmitted = true;
        if (speakCoroutine != null) { StopCoroutine(speakCoroutine); speakCoroutine = null; }
        //AndroidTTS.instance?.Stop();


        Debug.Log("=== CheckAnswer CALLED ===");
        Debug.Log($"Language: {LocalizationManager.currentLanguage}");
        Debug.Log($"ui null: {ui == null}");
        Debug.Log($"isSceneUnloading: {isSceneUnloading}");
        Debug.Log($"shuffledQuestions count: {shuffledQuestions?.Count}");
        Debug.Log($"currentQuestionIndex: {currentQuestionIndex}");

        for (int i = 0; i < ui.optionToggles.Count; i++)
        {
            Debug.Log($"Toggle {i} isOn: {ui.optionToggles[i].isOn} | Label: '{ui.optionLabels[i].text}'");
        }



        if (isSceneUnloading) return;

        isCountingDown = false;

        var qa = shuffledQuestions[currentQuestionIndex];

        string selectedOption = "";

        for (int i = 0; i < ui.optionToggles.Count; i++)
        {
            if (ui.optionToggles[i].isOn)
            {
                selectedOption = ui.optionLabels[i].text;
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(selectedOption))
        {
            //ui.answerText.text = "No option selected!";
            ShowAnswerPopup("No option selected!", Color.yellow);

            //if (car != null)
            //{
            //    //car.MoveBackByWaypoints(3);
            //    car.MoveBackByWaypoints(1);
            //    car.ResumeDriving();
            //}

            //StartCoroutine(HideQuestionPanelAfterDelay());
            return;
        }
        bool isCorrect = false;

        if (LocalizationManager.currentLanguage == LocalizationManager.Language.Amharic)
        {
            // ← Check by selected toggle index instead of text
            int selectedIndex = -1;
            for (int i = 0; i < ui.optionToggles.Count; i++)
            {
                if (ui.optionToggles[i].isOn)
                {
                    selectedIndex = i;
                    break;
                }
            }
            Debug.Log($"Selected Index: {selectedIndex} | Correct Index: {qa.correctAnswerIndex}");
            isCorrect = selectedIndex == qa.correctAnswerIndex;
        }
        else
        {
            isCorrect = selectedOption.Trim().ToLower() == qa.answers.Trim().ToLower();
        }

        if (isCorrect)
        {
            SoundManager.Instance?.PlaySFX("CorrectAnswer");

            ShowAnswerPopup("Correct Answer!", Color.green);
            score++;
            UpdateScoreUI();

            if (gasBar != null)
            {
                gasBar.AddGas(0.2f);
                SoundManager.Instance?.PlaySFX("GasRefill");
            }
                


            if (car != null)
                car.ResumeDriving();

            // ← Notify zone that question was answered correctly
            if (currentTriggerZone != null)
            {
                currentTriggerZone.OnQuestionAnsweredCorrectly();
                currentTriggerZone = null;
            }

            currentQuestionIndex++;

            //StartCoroutine(HideQuestionPanelAfterDelay());

        }
        else
        {
            SoundManager.Instance?.PlaySFX("WrongAnswer");

            //ui.answerText.text = "Wrong Answer!";
            ShowAnswerPopup("Wrong Answer!", Color.red);
            life--;
            UpdateWrongAnswersUI();

            if (car != null)
            {
                wrongAnswerCount++;

                if (wrongAnswerCount == 1)
                {
                    car.MoveBackByWaypoints(3);
                }
                else if (wrongAnswerCount == 2)
                {
                    car.MoveBackByWaypoints(5);
                }
                else
                {
                    //car.RespawnAtStart();
                    car.MoveBackByWaypoints(5);
                }

                car.ResumeDriving();
            }

            if (life <= 0)
            {
                life = 3000; // ← reset for next game
                //GameManager.globalLife = 3;
                StartCoroutine(LoadMainMenuAfterDelay());
            }

            //StartCoroutine(HideQuestionPanelAfterDelay());
        }

        StartCoroutine(HideQuestionPanelAfterDelay());
    }

    public IEnumerator LoadMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.returnedFromGameOver = true;
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowAnswerPopup(string message, Color color)
    {
        ui.answerText.text = message;
        ui.answerText.color = color;

        CanvasGroup cg = ui.answerText.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = ui.answerText.gameObject.AddComponent<CanvasGroup>();

        // Reset
        cg.alpha = 0;
        ui.answerText.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(ui.answerText.transform.DOScale(1.2f, 0.25f).SetEase(Ease.OutBack));
        seq.Join(cg.DOFade(1f, 0.2f));

        seq.AppendInterval(1f);

        seq.Append(ui.answerText.transform.DOScale(0.8f, 0.2f));
        seq.Join(cg.DOFade(0f, 0.2f));
    }


    string CleanString(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";

        // Remove all whitespace types including non-breaking spaces
        return input.Trim()
                    .Replace("\u00A0", "")   // non-breaking space
                    .Replace("\u200B", "")   // zero-width space
                    .Replace("\u200C", "")   // zero-width non-joiner
                    .Replace(" ", " ")       // normalize spaces
                    .Trim();
    }



    void StartNextTest()
    {
        if (ui != null && ui.gameOver != null)
            ui.gameOver.SetActive(true);
    }

    //IEnumerator HideQuestionPanelAfterDelay()
    //{
    //    yield return new WaitForSeconds(1.5f); // ← slightly longer for mobile

    //    AndroidTTS.instance?.Stop();

    //    if (isSceneUnloading) yield break; // ← add this check
    //    if (ui == null || ui.questionPanel == null) yield break;

    //    GameObject panel = ui.questionPanel;
    //    if (!panel.activeInHierarchy) yield break; // ← already hidden

    //    RectTransform rt = panel.GetComponent<RectTransform>();
    //    CanvasGroup cg = panel.GetComponent<CanvasGroup>();

    //    if (cg == null)
    //        cg = panel.AddComponent<CanvasGroup>();

    //    rt.DOKill();
    //    cg.DOKill();

    //    rt.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
    //    cg.DOFade(0f, 0.25f);

    //    yield return new WaitForSeconds(0.35f);

    //    if (panel != null)
    //        panel.SetActive(false);

    //    if (car != null)
    //        car.ResumeDriving(); // ← ensure car always resumes
    //}

    IEnumerator HideQuestionPanelAfterDelay()
    {
        // Wait for answer popup animation to finish (1s interval + 0.2s fade)
        yield return new WaitForSeconds(1f);

        //AndroidTTS.instance?.Stop();

        if (isSceneUnloading) yield break;
        if (ui == null || ui.questionPanel == null) yield break;
        if (!ui.questionPanel.activeInHierarchy) yield break;

        GameObject panel = ui.questionPanel;
        RectTransform rt = panel.GetComponent<RectTransform>();
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();

        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        //rt.DOKill();
        //cg.DOKill();

        //rt.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
        //cg.DOFade(0f, 0.25f);

        //yield return new WaitForSeconds(0.35f);

        panel.SetActive(false);
        SoundManager.Instance?.FadeMusicIn(); // ← fade music back in when panel closes
    }





    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        //SoundManager.returnedFromGameOver = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ============================
    // UI
    // ============================

    void UpdateScoreUI()
    {
        if (ui != null && ui.scoreText != null)
            ui.scoreText.text = "Score: " + score;
    }

    void UpdateWrongAnswersUI()
    {
        if (ui != null && ui.wrongAnswersText != null)
            ui.wrongAnswersText.text = "Life: " + Mathf.Max(0, life);
    }

    public void OnSceneReloaded()
    {
        car = FindObjectOfType<Car>();
        gasBar = FindObjectOfType<GasBar>();
        soundManager = FindObjectOfType<SoundManager>();

        currentQuestionIndex = 0;
        wrongAnswerCount = 0;
        life = 3;
        score = 0;

        ShuffleQuestions(); // reshuffle on restart so order is fresh

        UpdateScoreUI();
        UpdateWrongAnswersUI();
    }

    public bool AllQuestionsAnswered()
    {
        return currentQuestionIndex >= shuffledQuestions.Count;
    }

}
