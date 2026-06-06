using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundSettingsUI : MonoBehaviour
{
    public Slider volumeSlider;
    public Image musicOnIcon, musicOffIcon;
    public Image sfxOnIcon, sfxOffIcon;
    public TMP_Text musicButtonText;
    public TMP_Text sfxButtonText;

    void Start()
    {
        var sm = SoundManager.Instance;

        // Load values
        float volume = PlayerPrefs.GetFloat("Volume", 1f);
        volumeSlider.value = volume;

        volumeSlider.onValueChanged.AddListener(sm.SetVolume);

        UpdateUI();
    }

    public void ToggleMusic()
    {
        SoundManager.Instance.ToggleMusic();
        UpdateUI();
    }

    public void ToggleSFX()
    {
        SoundManager.Instance.ToggleSfx();
        UpdateUI();
    }

    void UpdateUI()
    {
        var sm = SoundManager.Instance;

        bool musicMuted = sm.musicSource.mute;
        bool sfxMuted = sm.sfxSource.mute;

        musicOnIcon.enabled = !musicMuted;
        musicOffIcon.enabled = musicMuted;
        musicButtonText.text = musicMuted ? "OFF" : "ON";

        sfxOnIcon.enabled = !sfxMuted;
        sfxOffIcon.enabled = sfxMuted;
        sfxButtonText.text = sfxMuted ? "OFF" : "ON";
    }
}