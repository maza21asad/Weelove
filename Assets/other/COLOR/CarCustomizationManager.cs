using UnityEngine;

public class CarCustomizationManager : MonoBehaviour
{
    [Header("Car Material")]
    public Material carMaterial;

    [Header("Colors")]
    public Color lightBlueColor = new Color(0.4f, 0.8f, 1f); // editable
    public Color pinkColor = new Color(1f, 0.901f, 0.984f);  // FFE6FB

    private const string COLOR_KEY = "CarColor";

    void Start()
    {
        ApplySavedColor();
    }

    // 🔵 BUTTON: Light Blue
    public void SelectLightBlue()
    {
        carMaterial.color = lightBlueColor;
        PlayerPrefs.SetInt(COLOR_KEY, 0);
        PlayerPrefs.Save();
    }

    // 🌸 BUTTON: Pink
    public void SelectPink()
    {
        carMaterial.color = pinkColor;
        PlayerPrefs.SetInt(COLOR_KEY, 1);
        PlayerPrefs.Save();
    }

    void ApplySavedColor()
    {
        int savedColor = PlayerPrefs.GetInt(COLOR_KEY, 0);

        if (savedColor == 0)
            carMaterial.color = lightBlueColor;
        else
            carMaterial.color = pinkColor;
    }
}
