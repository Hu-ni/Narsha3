using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    private Slider hpSlider;
    private Slider mpSlider;
    private Text levelText;

    void Start()
    {
        this.hpSlider = this.transform.Find("Panel/HPSlider").GetComponent<Slider>();
        this.mpSlider = this.transform.Find("Panel/MPSlider").GetComponent<Slider>();
        this.levelText = this.transform.Find("Panel/LevelUI/Text").GetComponent<Text>();

        hpSlider.minValue = 0;
        hpSlider.maxValue = 100;
        hpSlider.value = hpSlider.maxValue;

        levelText.text = "1";

        mpSlider.minValue = 0;
        mpSlider.maxValue = 100;
        mpSlider.value = mpSlider.maxValue;
    }

    public void setHP(float hp)
    {
        hpSlider.value -= hp;
    }
    public void setMP(float mp)
    {
        mpSlider.value -= mp;
    }

    public void setLevel(int level)
    {
        levelText.text = "" + level;
    }

    public void setMine()
    {
        this.transform.Find("Panel/HPSlider/FillArea/Fill").GetComponent<Image>().color = Color.yellow;
    }
}
