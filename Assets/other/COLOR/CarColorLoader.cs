using UnityEngine;

public class CarColorLoader : MonoBehaviour
{
    public Material carMaterial;

    public Color lightBlueColor = new Color(0.4f, 0.8f, 1f);
    public Color pinkColor = new Color(1f, 0.901f, 0.984f); // FFE6FB

    private const string COLOR_KEY = "CarColor";

    void Start()
    {
        int savedColor = PlayerPrefs.GetInt(COLOR_KEY, 0);

        if (savedColor == 0)
            carMaterial.color = lightBlueColor;
        else
            carMaterial.color = pinkColor;
    }
}
