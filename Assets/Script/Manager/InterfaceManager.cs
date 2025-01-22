using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterfaceManager : MonoBehaviour
{
    public static InterfaceManager instance;

    [SerializeField] private CanvasGroup startWindow, gameWindow, victoryWindow, endWindow;
    [SerializeField] private GameObject effect;
    [SerializeField] private TextMeshProUGUI[] levelTextArray;  

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetTextLevel(int id)
    {
        foreach (TextMeshProUGUI tmp in levelTextArray)
        {
            tmp.text = "Level " + (id + 1);
        }
    }

    public void TriggerGameScreen()
    {
        effect.SetActive(false);
        GameManager.instance.IsLevelFinished = false;
        gameWindow.gameObject.SetActive(true);
        startWindow.DOFade(0, 0.5f).OnComplete(() => startWindow.gameObject.SetActive(false));
        gameWindow.DOFade(1, 0.5f);
    }

    public void TriggerVictoryScreen()
    {
        effect.SetActive(true);
        victoryWindow.gameObject.SetActive(true);
        gameWindow.DOFade(0, 0.5f).OnComplete(() => gameWindow.gameObject.SetActive(false));
        victoryWindow.DOFade(1, 0.5f);
    }

    public void TriggerStartScreen()
    {
        effect.SetActive(false);
        AudioManager.instance.Stop(SoundState.Victory);
        startWindow.gameObject.SetActive(true);
        victoryWindow.gameObject.SetActive(false);
        startWindow.DOFade(1, 0.5f);
    }

    public void TriggerEndWindow()
    {
        effect.SetActive(true);
        gameWindow.DOFade(0, 0.5f).OnComplete(() => gameWindow.gameObject.SetActive(false));
        
        endWindow.gameObject.SetActive(true);
        endWindow.DOFade(1, 0.5f);
    }
}
