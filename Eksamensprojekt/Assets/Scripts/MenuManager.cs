using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] CanvasGroup startMenu;
    [SerializeField] CanvasGroup buttons;
    [SerializeField] CanvasGroup settings;
    [SerializeField] Image whiteFade;

    [SerializeField] AudioSource audioSrc;
    [SerializeField] AudioSource transitionSound;
    [SerializeField] AudioSource MenuMusic;
    [SerializeField] AudioSource MenuMusicWithBeats;
    [SerializeField] AudioClip clickSound;

    [SerializeField] AudioMixer musicMixer;
    [SerializeField] AudioMixer soundEffectsMixer;

    bool hasClicked;

    private void Awake()
    {
        DontDestroyOnLoad(MenuMusicWithBeats);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !hasClicked)
        {
            transitionSound.Play();
            MenuMusic.DOFade(0, 1);
            MenuMusicWithBeats.DOFade(0.7f, 1);

            OpenButtons();
        }
    }

    void OpenButtons()
    {
        hasClicked = true;
        audioSrc.PlayOneShot(clickSound);

        startMenu.transform.DOScale(0.9f, 0.5f);
        startMenu.DOFade(0, 0.5f).OnComplete(() => startMenu.gameObject.SetActive(true));

        buttons.transform.localScale = Vector3.one * 0.9f;
        buttons.alpha = 0;
        buttons.gameObject.SetActive(true);
        buttons.transform.DOScale(1, 0.5f).SetDelay(1);
        buttons.DOFade(1, 0.5f).SetDelay(1);
    }

    public void ContinueGame()
    {
        MenuMusicWithBeats.DOFade(0, 4);
        whiteFade.gameObject.SetActive(true);
        whiteFade.DOFade(1, 1).OnComplete(() => SceneManager.LoadScene("Home"));
    }

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        ContinueGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        buttons.transform.DOScale(0.9f, 0.5f);
        buttons.DOFade(0, 0.5f).OnComplete(()=> buttons.gameObject.SetActive(false));

        settings.alpha = 0;
        settings.gameObject.SetActive(true);
        settings.transform.DOScale(1, 0.5f).SetDelay(1);
        settings.DOFade(1, 0.5f).SetDelay(1);
    }

    public void CloseSettings()
    {
        settings.transform.DOScale(0.9f, 0.5f);
        settings.DOFade(0, 0.5f).OnComplete(()=> settings.gameObject.SetActive(false));

        buttons.alpha = 0;
        buttons.gameObject.SetActive(true);
        buttons.transform.DOScale(1, 0.5f).SetDelay(1);
        buttons.DOFade(1, 0.5f).SetDelay(1);
    }

    public void SetMusicVolume(float sliderValue)
    {
        musicMixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }

    public void SetSoundEffectsVolume(float sliderValue)
    {
        soundEffectsMixer.SetFloat("SoundEffectsVolume", Mathf.Log10(sliderValue) * 20);
    }
}
