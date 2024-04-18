using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    private string currentSceneName;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded scene: " + scene.name);
        // Additional actions can be performed here based on the new scene
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSceneName != SceneManager.GetActiveScene().name)
        {
            AudioSource sound = GetComponent<AudioSource>();
            sound.DOFade(0f, 2f);
            StartCoroutine(DestroyObject(2f));
        }
    }

    IEnumerator DestroyObject(float delayUntilDestory)
    {
        yield return new WaitForSeconds(delayUntilDestory);
        Destroy(gameObject);
    }
}
