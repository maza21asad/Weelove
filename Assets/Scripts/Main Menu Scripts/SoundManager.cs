using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    // ← Add this static flag
    public static bool returnedFromGameOver = false;
    [Header("Audio Data")]
    public TestMusicData testMusicData;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("UI")]
    public Slider volumeSlider;
    public Image musicOnIcon, musicOffIcon;
    public Image sfxOnIcon, sfxOffIcon;
    public TMP_Text musicButtonText;
    public TMP_Text sfxButtonText;

    [Header("Scene Music")]
    public SceneMusicDatabase sceneMusicDatabase;

    private bool musicMuted = false;
    private bool sfxMuted = false;

    private List<AudioClip> currentPlaylist;
    private int currentTrackIndex;

    private Coroutine playlistCoroutine;        // 3-3-26

    [Header("Music Fade Settings")]
    public float fadeDuration = 0.5f;

    private float originalMusicVolume = 1f;
    private Coroutine fadeCoroutine;

    



    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Load settings
        musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        sfxMuted = PlayerPrefs.GetInt("SfxMuted", 0) == 1;

        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        musicSource.mute = musicMuted;
        sfxSource.mute = sfxMuted;

        UpdateMusicUI();
        UpdateSfxUI();

        //PlayMainMenuMusic();
        //PlayMusic("MainMenu");
        PlaySceneMusic();
    }

    public void PlayMusicPlaylist(List<AudioClip> playlist)
    {
        if (playlistCoroutine != null)
            StopCoroutine(playlistCoroutine);

        playlistCoroutine = StartCoroutine(PlayPlaylistRoutine(playlist));
    }

    private void PlayCurrentTrack()
    {
        if (currentPlaylist == null || currentPlaylist.Count == 0)
            return;

        musicSource.clip = currentPlaylist[currentTrackIndex];
        musicSource.Play();

        Invoke(nameof(PlayNextTrack), musicSource.clip.length);
    }

    private void PlayNextTrack()
    {
        currentTrackIndex++;

        if (currentTrackIndex >= currentPlaylist.Count)
            currentTrackIndex = 0;

        PlayCurrentTrack();
    }

    //private IEnumerator PlayPlaylistRoutine(List<AudioClip> playlist)
    //{
    //    currentPlaylist = playlist;
    //    currentTrackIndex = 0;

    //    while (true)
    //    {
    //        if (currentPlaylist == null || currentPlaylist.Count == 0)
    //            yield break;

    //        musicSource.clip = currentPlaylist[currentTrackIndex];
    //        musicSource.loop = false;
    //        musicSource.Play();

    //        yield return new WaitForSeconds(musicSource.clip.length);

    //        currentTrackIndex++;
    //        if (currentTrackIndex >= currentPlaylist.Count)
    //            currentTrackIndex = 0;
    //    }
    //}
    private IEnumerator PlayPlaylistRoutine(List<AudioClip> playlist, int startIndex = 0)
    {
        currentPlaylist = playlist;
        currentTrackIndex = startIndex; // ← starts from given index

        while (true)
        {
            if (currentPlaylist == null || currentPlaylist.Count == 0)
                yield break;

            musicSource.clip = currentPlaylist[currentTrackIndex];
            musicSource.loop = false;
            musicSource.Play();

            yield return new WaitForSeconds(musicSource.clip.length);

            currentTrackIndex++;
            if (currentTrackIndex >= currentPlaylist.Count)
                currentTrackIndex = 0;
        }
    }

    // 3-3-26
    //public void PlaySceneMusic()
    //{
    //    if (sceneMusicDatabase == null)
    //        return;

    //    string currentScene = UnityEngine.SceneManagement.SceneManager
    //        .GetActiveScene().name;

    //    SceneMusicData sceneData = sceneMusicDatabase.scenes
    //        .Find(x => x.sceneName == currentScene);

    //    if (sceneData != null && sceneData.playlist.Count > 0)
    //    {
    //        PlayMusicPlaylist(sceneData.playlist);
    //    }
    //}
    public void PlaySceneMusic()
    {
        if (sceneMusicDatabase == null) return;

        string currentScene = SceneManager.GetActiveScene().name;

        SceneMusicData sceneData = sceneMusicDatabase.scenes
            .Find(x => x.sceneName == currentScene);

        if (sceneData != null && sceneData.playlist.Count > 0)
        {
            // ← If returned from game over AND there's a second track, start from index 1
            if (returnedFromGameOver && currentScene == "MainMenu" && sceneData.playlist.Count > 1)
            {
                returnedFromGameOver = false; // reset flag
                PlayMusicPlaylistFromIndex(sceneData.playlist, 1); // ← play element 1
            }
            else
            {
                PlayMusicPlaylist(sceneData.playlist); // ← play normally from element 0
            }
        }
    }
    public void PlayMusicPlaylistFromIndex(List<AudioClip> playlist, int startIndex)
    {
        if (playlistCoroutine != null)
            StopCoroutine(playlistCoroutine);

        playlistCoroutine = StartCoroutine(PlayPlaylistRoutine(playlist, startIndex));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneMusic();
    }

    public void SetVolume(float value)
    {
        musicSource.volume = value;
        sfxSource.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void ToggleMusic()
    {
        musicMuted = !musicMuted;
        musicSource.mute = musicMuted;
        PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);
        UpdateMusicUI();
    }

    public void ToggleSfx()
    {
        sfxMuted = !sfxMuted;
        sfxSource.mute = sfxMuted;
        PlayerPrefs.SetInt("SfxMuted", sfxMuted ? 1 : 0);
        UpdateSfxUI();
    }

    void UpdateMusicUI()
    {
        musicOnIcon.enabled = !musicMuted;
        musicOffIcon.enabled = musicMuted;
        musicButtonText.text = musicMuted ? "OFF" : "ON";
    }

    void UpdateSfxUI()
    {
        sfxOnIcon.enabled = !sfxMuted;
        sfxOffIcon.enabled = sfxMuted;
        sfxButtonText.text = sfxMuted ? "OFF" : "ON";
    }

    public void PlayMusic(string musicName)
    {
        if (musicMuted || testMusicData == null) return;

        AudioEntry entry = testMusicData.allMusic
            .Find(x => x.audioName == musicName);

        if (entry != null && entry.clip != null)
        {
            // Prevent restarting same music
            if (musicSource.clip == entry.clip && musicSource.isPlaying)
                return;

            musicSource.Stop();
            musicSource.clip = entry.clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music not found: " + musicName);
        }
    }


    // DATA-DRIVEN SFX SYSTEM
    public void PlaySFX(string sfxName)
    {
        if (sfxMuted || testMusicData == null) return;

        AudioEntry entry = testMusicData.soundEffects
            .Find(x => x.audioName == sfxName);

        if (entry != null && entry.clip != null)
        {
            sfxSource.PlayOneShot(entry.clip);
        }
        else
        {
            Debug.LogWarning("SFX not found: " + sfxName);
        }
    }

    // For UI button click
    public void ButtonClickSFX()
    {
        PlaySFX("ButtonClick");
    }


    public void StopSFX()
    {
        sfxSource.Stop();
    }


    public void FadeMusicOut()
    {
        originalMusicVolume = musicSource.volume;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeMusic(musicSource.volume, 0f, fadeDuration));
    }

    public void FadeMusicIn()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeMusic(musicSource.volume, originalMusicVolume, fadeDuration));
    }

    private IEnumerator FadeMusic(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        musicSource.volume = to;
    }


}
