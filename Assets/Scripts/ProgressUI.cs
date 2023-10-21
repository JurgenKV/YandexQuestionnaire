﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class ProgressUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _moneyUI;
    [SerializeField] private ClickerScore _clickerScore;
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _sliderTextUI;
    [SerializeField] private TMP_Text _levelUI;

    public void RefreshAllUI()
    {
        RefreshMoneyUI();
        RefreshLevelUI();
        RefreshScorebarUI();
    }
    
    private void Update()
    {
        RefreshMoneyUI();
    }

    public void RefreshMoneyUI()
    {
        _moneyUI.text = _clickerScore.ClicksCount.ToString() + "$";
    }

    private void RefreshLevelUI()
    {
        _levelUI.text = _clickerScore.level.ToString();
    }
    
    private void RefreshScorebarUI()
    {
        _slider.value =  (GetPercent(_clickerScore.experience, _clickerScore.experienceToNextLevel) + 10);

        if (YandexGame.savesData.language.Equals("ru"))
        {
            _sliderTextUI.text = "Скорость роста " + _clickerScore.experienceMultiplayer.ToString();
        }else if (YandexGame.savesData.language.Equals("en"))
        {
            _sliderTextUI.text = "Growth rate " + _clickerScore.experienceMultiplayer.ToString();
        }
        // _sliderTextUI.text = _clickerScore.experience.ToString() + "/" +
        //                      _clickerScore.experienceToNextLevel.ToString();
    }
    
    public static float GetPercent(long a, long b)
    {
        if (a == 0) 
            return 0;
        
        return ( ((float)a / (float)b) * (float)100);
    }

}