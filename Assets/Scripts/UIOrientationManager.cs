using UnityEngine;

public class UIOrientationManager : MonoBehaviour
{
    public GameObject portraitLayout;
    public GameObject landscapeLayout;

    void Start()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;

        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }

    void Update()
    {
        bool isLandscape = Screen.width > Screen.height;

        portraitLayout.SetActive(!isLandscape);
        landscapeLayout.SetActive(isLandscape);
    }
}
