using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class PlanetSelector : MonoBehaviour
{
    public Transform m_camera;
    public Vector3 camOffset;
    Vector3 originalPosition;
    public Transform planets;
    Transform selectedPlanetTransform;
    PlanetSO selectedPlanetSO;

    bool planetSelected;
    bool camFollowPlanets;

    // UI
    public GameObject planetMenu;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image whiteFade;
    public GameObject backBtn;
    public GameObject goBtn;
    public GameObject closeBtn;

    private void Start()
    {
        originalPosition = m_camera.position;
        planets.DORotate(new Vector3(0, 360, 0), 350, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    public void ZoomToPlanet(Transform planetTransform, PlanetSO planetSO)
    {
        selectedPlanetTransform = planetTransform;
        selectedPlanetSO = planetSO;

        m_camera.DOMove(selectedPlanetTransform.position + camOffset, 0.4f).OnComplete(() => camFollowPlanets = true);

        titleText.text = planetSO.title;
        descriptionText.text = planetSO.description;

        if (planetSO.unlocked)
        {
            goBtn.SetActive(true);
            closeBtn.SetActive(false);
        }
        else
        {
            goBtn.SetActive(false);
            closeBtn.SetActive(true);
        }

        OpenMenu();

        planetSelected = true;
    }

    public void GoToOriginalPos()
    {
        camFollowPlanets = false;

        CloseMenu();

        planetSelected = false;
    }

    private void Update()
    {
        if (camFollowPlanets)
        {
            Vector3 targetPosition = selectedPlanetTransform.position + camOffset;
            float transitionSpeed = 0.5f;
            m_camera.position = Vector3.Lerp(m_camera.position, targetPosition, transitionSpeed * Time.deltaTime);
        }
    }

    public void GoToPlanet()
    {
        planets.DOKill();
        camFollowPlanets = false;
        CloseMenu();
        backBtn.SetActive(false);

        Vector3 zoomOutOffset = new Vector3(2, 4, -7);
        m_camera.DOMove(selectedPlanetTransform.position + zoomOutOffset, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            m_camera.DOMove(selectedPlanetTransform.position, 2).SetEase(Ease.InQuart);
            whiteFade.DOFade(1, 0.8f).SetDelay(1f).SetEase(Ease.InQuart).OnComplete(() => SceneManager.LoadScene(selectedPlanetSO.sceneBuildIndex));
        });
    }

    void OpenMenu()
    {
        planetMenu.transform.localScale = new Vector2(1, 0);
        planetMenu.transform.DOScaleY(1, 0.2f);
    }

    public void CloseMenu()
    {
        m_camera.DOMove(originalPosition, 0.5f);
        planetMenu.transform.DOScaleY(0, 0.2f);
    }

    public bool PlanetSelected()
    {
        return planetSelected;
    }
}
