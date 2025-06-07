using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Image Loadingbar;
    public GameObject LoadingScreen;

    private void Start()
    {
        Loadingbar.DOFillAmount(1f, 2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            LoadingScreen.SetActive(false);
        }
 );
        ;
    }
    void Update()
    {

    }
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
