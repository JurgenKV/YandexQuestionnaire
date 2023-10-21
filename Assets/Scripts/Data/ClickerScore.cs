﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using YG;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;


public class ClickerScore : MonoBehaviour
{
    [SerializeField] private GameController _gameController;
    [SerializeField] private ProgressUI _progressUI;
    
    [Header("Upgrade Click")]
    [SerializeField] private TMP_Text upgradeClickCostUI;
        
    [SerializeField] private double constX;
    [SerializeField] private double degreeY;
    
    [Space(1)]
    [SerializeField] private Button _buttonX2;
    [SerializeField] private Button _buttonAuto;
    [SerializeField] private Button _buttonUpdate;
    
    [Header("Upgrade Exp Multiplayer")]
    [SerializeField] private TMP_Text upgradeExpCostUI;
    [SerializeField] private Button _buttonUpgaradeExp;

    [SerializeField] private ChangeImage _changeImage;
    
    [SerializeField] private List<AudioSource> _sources = new List<AudioSource>();

    public bool IsAutoClickActive = false;
    
    private long _clickCount = 0;
    public long ClicksCount
    {
        get
        {
            return _clickCount ; // возвращаем значение свойства
        }
        set
        {
            YandexGame.savesData.MoneyAmount = value;
            YandexGame.SaveProgress();
            _clickCount = value; // устанавливаем новое значение свойства
        }
    }
    
    private bool _coroutineX2CLicks = false;
    public int ClickMultiplayer = 1;
    
    [SerializeField]public int level = 1;
    [SerializeField]public long experienceMultiplayer = 1;
    [SerializeField]public long experience;
    [SerializeField]public long experienceToNextLevel;

    [SerializeField] public List<GameObject> ParticleSystems;
    
    //[SerializeField] private List<Animator> _animators;
    private void Start()
    {
        LoadData();
        
        
        SetLevel(level);
       
        UpdateUpgradeClickUI();
        _progressUI.RefreshAllUI();
        
        InvokeRepeating(nameof(AutoExp), 1,1);
    }

    // private void AutoClick()
    // {
    //     if(IsAutoClickActive)
    //         Click();
    // }

    private void LoadData()
    {
        ClicksCount = YandexGame.savesData.MoneyAmount;
        ClickMultiplayer = YandexGame.savesData.ClickMultiplayer;
        level = YandexGame.savesData.Level;
        experience = YandexGame.savesData.Experience;
        experienceToNextLevel = YandexGame.savesData.ExperienceToNextLevel;
        experienceMultiplayer = YandexGame.savesData.ExperienceMultiplayer;
    }

    public void Click()
    {
        if(!gameObject.activeSelf)
            return;

        if (_coroutineX2CLicks)
        {
            ClicksCount += 1 * ClickMultiplayer * 2;
            //AddExperience(1 * ClickMultiplayer * 2);
        }
        else
        {
            ClicksCount += 1 * ClickMultiplayer;
            //AddExperience(1 * ClickMultiplayer);
        }
        Debug.Log("Click");
        YandexGame.savesData.MoneyAmount = ClicksCount;
        YandexGame.SaveProgress();
        _progressUI.RefreshAllUI();
    }

    private void AutoExp()
    {
        AddExperience(experienceMultiplayer);
        _progressUI.RefreshAllUI();
    }

    public void ConvertMoneyToExp(int cost)
    {
        if(ClicksCount < cost)
            return;
        ClicksCount -= cost;

        AddExperience((long) (ClickMultiplayer * 1.5f) + 5);
        
        if (YandexGame.savesData.IsSoundEnabled)
        {
            if (_sources.All(i => !i.isPlaying))
            {
                _sources[Random.Range(0, _sources.Count)].Play();
            }
        }
        _progressUI.RefreshAllUI();
        YandexGame.savesData.MoneyAmount = ClicksCount;
        YandexGame.SaveProgress();
    }

    public void VideoClickX2()
    {
        if (!_coroutineX2CLicks)
        {
            //YandexGame.RewVideoShow(1);
            YGRewardedVideoManager.OpenRewardAd(1);
            //StartCoroutine(TimerX2Coroutine());
        }
           
    }

    public void EndRewardStartTimerX2Coroutine()
    {
        StartCoroutine(TimerX2Coroutine());
    }
    
    public void EndRewardAutoClickCoroutine()
    {
        StartCoroutine(TimerAutoClickCoroutine());
    }

    IEnumerator TimerX2Coroutine()
    {
        _coroutineX2CLicks = true;
        _buttonX2.interactable = false;
        yield return new WaitForSeconds(60);
        _buttonX2.interactable = true;
        _coroutineX2CLicks = false;
    }
    
    IEnumerator TimerAutoClickCoroutine()
    {
        IsAutoClickActive = true;
        _buttonAuto.interactable = false;
        yield return new WaitForSeconds(90);
        _buttonAuto.interactable = true;
        IsAutoClickActive = false;
    }

    public void UpgradeClick()
    {
         if (ClicksCount < GetUpgradeCostClick())
             return;
        
         ClicksCount -= GetUpgradeCostClick();
        
        ClickMultiplayer++;
        UpdateUpgradeClickUI();
        YandexGame.savesData.ClickMultiplayer = ClickMultiplayer;
        YandexGame.SaveProgress();
    }
    
    public void UpgradeExpMultiplayer()
    {
        if (ClicksCount < GetUpgradeCostExp())
            return;
        
        ClicksCount -= GetUpgradeCostExp();
        
        experienceMultiplayer++;
        UpdateUpgradeExpUI();
        YandexGame.savesData.ExperienceMultiplayer = experienceMultiplayer;
        YandexGame.SaveProgress();
    }

    private void UpdateUpgradeClickUI()
    {
        if(upgradeClickCostUI != null)
            upgradeClickCostUI.text = GetUpgradeCostClick().ToString() + "$";
    }
    
    private void UpdateUpgradeExpUI()
    {
        if(upgradeExpCostUI != null)
            upgradeExpCostUI.text = GetUpgradeCostExp().ToString() + "$";
    }

    private long GetUpgradeCostClick()
    {
        //x * ((level+1) ^ y) - (x * level):
        double cost = ((constX * Math.Pow((ClickMultiplayer+1), degreeY)) - (constX * (ClickMultiplayer+1)));
        Debug.Log("cost = " + cost);
        
        
        return (long)cost;
    }
    
    private long GetUpgradeCostExp()
    {
        //x * ((level+1) ^ y) - (x * level):
        double cost = ((constX * Math.Pow((ClickMultiplayer+2), degreeY)) - (constX * (ClickMultiplayer+1)));
        Debug.Log("cost = " + cost);
        
        
        return (long)cost;
    }

    public void ADSUpgradeClick()
    {
        // try
        // {
        //     YandexGame.RewVideoShow(2);
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        // }
        YGRewardedVideoManager.OpenRewardAd(2);
        
        // ClickMultiplayer++;
        // UpdateUpgradeClickUI();
        // StartCoroutine(TimerUpdateCoroutine());
        // YandexGame.savesData.ClickMultiplayer = ClickMultiplayer;
        // YandexGame.SaveProgress();
    }
    
    public void ADSUpgradeExp()
    {
        YGRewardedVideoManager.OpenRewardAd(6);
    }
    
    public void ADSAutoClick()
    {
        YGRewardedVideoManager.OpenRewardAd(5);
    }

    public void EndRewardUpgradeClick()
    {
        ClickMultiplayer++;
        UpdateUpgradeClickUI();
        StartCoroutine(TimerUpdateCoroutine());
        YandexGame.savesData.ClickMultiplayer = ClickMultiplayer;
        YandexGame.SaveProgress();
    }
    
    public void EndRewardUpgradeMultiplayer()
    {
        experienceMultiplayer++;
        UpdateUpgradeExpUI();
        StartCoroutine(TimerUpdateExpCoroutine());
        YandexGame.savesData.ExperienceMultiplayer = experienceMultiplayer;
        YandexGame.SaveProgress();
    }
    
    
    IEnumerator TimerUpdateCoroutine()
    {
        _buttonUpdate.interactable = false;
        yield return new WaitForSeconds(60);
        _buttonUpdate.interactable = true;
    }
    
    IEnumerator TimerUpdateExpCoroutine()
    {
        _buttonUpgaradeExp.interactable = false;
        yield return new WaitForSeconds(60);
        _buttonUpgaradeExp.interactable = true;
    }

    public void AddExperience(long experienceToAdd)
    {
        experience += experienceToAdd;
        
        if (experience >= experienceToNextLevel)
        {
            experience = 0;
            SetLevel(level + 1);
        }
        YandexGame.savesData.Experience = experience;
        YandexGame.SaveProgress();
    }

    private void SetLevel(int value)
    {
        level = value;
        experienceToNextLevel = (int)(500 * (Mathf.Pow(level + 1, 2) - (5 * (level + 1)) + 8));
        UpdateVisual();
        
        YandexGame.savesData.Level = level;
        YandexGame.savesData.ExperienceToNextLevel = experienceToNextLevel;
        GameController.SetLeaderboard(level);
        YandexGame.SaveProgress();
        
        _progressUI.RefreshAllUI();
    }

    public void UpdateVisual()
    {
        Debug.Log(level.ToString("0") + "\nto next lvl: " + experienceToNextLevel + "\ncurrent exp: " + experience);
        //lvlUI.text = _level.ToString();
        _changeImage.LoadImage(YandexGame.savesData.catType);
    }
}