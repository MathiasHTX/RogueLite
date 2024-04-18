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
    public PlanetSO[] planetSOs;
    public Transform[] planetTransforms;
    private int planetIndex = -1;
    private int currentPlanetIndex = -1; // Starts with -1 to indicate no planet is selected initially

    bool planetSelected;
    bool camFollowPlanets;
    bool goingToPlanet = false;
    bool isUnknownPlanet;

    [SerializeField] GameObject unlockParSys;

    [Header("UI")]
    public GameObject planetMenu;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI powerLevelText;
    public Image whiteFade;
    public GameObject backBtn;
    public GameObject goBtn;
    public GameObject closeBtn;

    public GameObject openTransition;
    public Image whiteTop;
    public Image whiteBottom;

    [Header("Audio")]
    public AudioSource audioSrc;
    public AudioClip hoverSound;
    public AudioClip goToPlanetSound;
    public AudioClip clickSound;
    public AudioClip unlockSound;

    private void Start()
    {
        originalPosition = m_camera.position;
        planets.DORotate(new Vector3(0, 360, 0), 350, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

        openTransition.SetActive(true);
        whiteTop.rectTransform.DOScaleY(0, 0.2f);
        whiteBottom.rectTransform.DOScaleY(0, 0.2f);

        backBtn.SetActive(false);

        DontDestroyOnLoad(this.gameObject);
    }

    public void ZoomToPlanet(Transform planetTransform, PlanetSO planetSO)
    {
        audioSrc.PlayOneShot(clickSound);

        selectedPlanetTransform = planetTransform;
        selectedPlanetSO = planetSO;

        m_camera.DOMove(selectedPlanetTransform.position + camOffset, 0.4f).OnComplete(() => camFollowPlanets = true);

        if(PlayerPrefs.GetInt(planetSO.planetNumber + "LockedState") == 2)
        {
            titleText.text = planetSO.title;
            descriptionText.text = planetSO.description;

            goBtn.SetActive(true);
            closeBtn.SetActive(false);

            isUnknownPlanet = false;
        }
        else
        {
            titleText.text = "UNKNOWN";
            descriptionText.text = "Unlock this planet by crafting a weapon of at least power level " + planetSO.powerLevel + ".";

            goBtn.SetActive(false);
            closeBtn.SetActive(true);

            isUnknownPlanet = true;
        }

        powerLevelText.text = "Power level " + planetSO.powerLevel;

        OpenMenu();

        planetSelected = true;

        backBtn.SetActive(true);
    }

    public void GoToOriginalPos()
    {
        camFollowPlanets = false;

        CloseMenu();

        planetSelected = false;

        backBtn.SetActive(false);
    }

    private void Update()
    {
        if (camFollowPlanets)
        {
            Vector3 targetPosition = selectedPlanetTransform.position + camOffset;
            float transitionSpeed = 0.5f;
            m_camera.position = Vector3.Lerp(m_camera.position, targetPosition, transitionSpeed * Time.deltaTime);
        } 

        if (planetSelected && Input.GetKeyDown(KeyCode.Return) && !isUnknownPlanet) 
        {
            GoToPlanet();
        }

        if (planetSelected && Input.GetKeyDown(KeyCode.Escape) && !goingToPlanet)
        {
            BackButton();
            planetIndex = -1;
        }

        // Use numbers on the keyboard to zoom to planets
        for (int i = 0; i < planetTransforms.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && !goingToPlanet)
            {
                if (i != currentPlanetIndex)
                {
                    currentPlanetIndex = i;  // Update current planet index
                    planetIndex = currentPlanetIndex;
                    ZoomToPlanet(planetTransforms[i], planetSOs[i]);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && !goingToPlanet)
        {
            if (planetIndex < planetTransforms.Length - 1)
            {
                planetIndex += 1;
                currentPlanetIndex = planetIndex;
                ZoomToPlanet(planetTransforms[planetIndex], planetSOs[planetIndex]);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && !goingToPlanet)
        {
            if (planetIndex > 0)
            {
                planetIndex -= 1;
                currentPlanetIndex = planetIndex;
                ZoomToPlanet(planetTransforms[planetIndex], planetSOs[planetIndex]);
            }
        }
    }

    public void BackButton()
    {
        GoToOriginalPos();
        audioSrc.PlayOneShot(clickSound);
    }

    public void GoToPlanet()
    {
        planets.DOKill();
        camFollowPlanets = false;
        CloseMenu();
        backBtn.SetActive(false);
        goingToPlanet = true;
        m_camera.DOKill();

        Vector3 zoomOutOffset = new Vector3(2, 5.5f, -7);
        m_camera.DOMove(selectedPlanetTransform.position + zoomOutOffset, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            m_camera.DOMove(selectedPlanetTransform.position, 2).SetEase(Ease.InQuart);
            whiteFade.DOFade(1, 0.8f).SetDelay(1f).SetEase(Ease.InQuart).OnComplete(() => SceneManager.LoadScene(selectedPlanetSO.sceneBuildIndex));
        });

        audioSrc.PlayOneShot(goToPlanetSound);
    }

    public void UnlockPlanet(Transform planetTransform, PlanetSO planetSO)
    {
        audioSrc.PlayOneShot(unlockSound);

        goingToPlanet = true;
        Vector3 zoomOutOffset = new Vector3(0, 1, -2);
        m_camera.DOMove(originalPosition + zoomOutOffset, 1).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            Instantiate(unlockParSys, planetTransform);

            whiteFade.DOFade(1, 0.1f).OnComplete(() =>
            {
                m_camera.position = originalPosition;
                whiteFade.DOFade(0, 1);
                m_camera.DOShakePosition(2, 0.7f, 20);

                PlayerPrefs.SetInt(planetSO.planetNumber + "LockedState", 2);
                planetTransform.gameObject.GetComponent<Planet>().CheckLockedState();

                goingToPlanet = false;
            });

        });
    }

    void OpenMenu()
    {
        if (selectedPlanetSO.planetNumber == 0)
        {
            powerLevelText.gameObject.SetActive(false);
            descriptionText.rectTransform.anchoredPosition = new Vector2(0, -170);
        }
        else
        {
            powerLevelText.gameObject.SetActive(true);
            descriptionText.rectTransform.anchoredPosition = new Vector2(0, -195);
        }


        planetMenu.transform.localScale = new Vector2(1, 0);
        planetMenu.transform.DOScaleY(1, 0.2f);
    }

    public void CloseMenu()
    {
        m_camera.DOKill();
        m_camera.DOMove(originalPosition, 0.5f);
        planetMenu.transform.DOScaleY(0, 0.2f);
    }

    public bool PlanetSelected()
    {
        return planetSelected;
    }

    public void PlayHoverSound()
    {
        audioSrc.PlayOneShot(hoverSound);
    }

    public bool GoingToPlanet()
    {
        return goingToPlanet;
    }
}
