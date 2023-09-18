﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;


public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void ChangeSceneButton()
    {
        SceneManager.LoadScene(_sceneName);
    }
    
    public void RestartGame()
    {
        ResetPlayerData();
        ChangeSceneButton();
    }

    private void ResetPlayerData()
    {
        YandexGame.savesData.MoneyScore = 0;
        YandexGame.savesData.ScoreMultiplayer = 1;
        YandexGame.savesData.BgNum = -1;
        YandexGame.savesData.ObjectImageNum = -1;
        YandexGame.savesData.ObjectImageSecNum = -1;
        YandexGame.savesData.IsAnimal = false;
        UpdateLeaderboard();
        YandexGame.SaveProgress();
        Debug.Log("ResetPlayerData");
    }
    
    private void UpdateLeaderboard()
    {
        try
        {
            YandexGame.NewLeaderboardScores("BestLevelPlayerEggClicker", 0);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
    }
}