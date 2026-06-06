using UnityEngine;
using UnityEngine.UI;
using System;

public class GasBar : MonoBehaviour
{
    public Image gasBarImage;
    public Text gasText;

    public float maxGas = 1f;
    public float currentGas = 1f;
    public float gasConsumptionRate = 0.01f;

    [Header("Respawn Settings")]
    public Transform startPoint;
    private Car car;

    public float gasFillAmount;

    private QuestionManager questionManager;


    void Start()
    {
        car = FindObjectOfType<Car>();

        questionManager = FindObjectOfType<QuestionManager>();

        gasBarImage.fillAmount = currentGas;
        UpdateGasText();
    }

    void Update()
    {
        if (car != null && car.IsGasPressed())
        {
            currentGas -= gasConsumptionRate * Time.deltaTime;
            currentGas = Mathf.Clamp01(currentGas);
            gasBarImage.fillAmount = currentGas;
            gasFillAmount = currentGas;
            UpdateGasText();

            if (currentGas <= 0f)
            {
                //RespawnCar();
                questionManager.StartCoroutine(questionManager.LoadMainMenuAfterDelay());
            }
        }
    }

    public void AddGas(float amount)
    {
        currentGas = Mathf.Clamp01(currentGas + amount);
        gasBarImage.fillAmount = currentGas;
        UpdateGasText();
    }

    private void RespawnCar()
    {
        if (startPoint != null && car != null)
        {
            car.transform.position = startPoint.position;
            car.transform.rotation = startPoint.rotation;

            Rigidbody rb = car.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            currentGas = maxGas;
            gasBarImage.fillAmount = currentGas;
            UpdateGasText();

            car.RespawnAtStart();
        }
    }

    private void UpdateGasText()
    {
        if (gasText != null)
        {
            int percentage = Mathf.RoundToInt(currentGas * 100f);
            gasText.text = percentage + "%";
        }
    }
}