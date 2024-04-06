using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] CanvasGroup startMenu;
    [SerializeField] CanvasGroup buttons;
    [SerializeField] Image whiteFade;

    bool hasClicked;

    void Update()
    {
        if (Input.GetMouseButton(0) && !hasClicked)
        {
            OpenButtons();
        }
    }

    void OpenButtons()
    {
        hasClicked = true;
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
        whiteFade.gameObject.SetActive(true);
        whiteFade.DOFade(1, 1).OnComplete(() => SceneManager.LoadScene("Home"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
