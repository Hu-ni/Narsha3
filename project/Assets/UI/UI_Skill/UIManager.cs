using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public void Awake()
    {
        UIManager.instance = this;
    }

    // Skill Type
    public const int SKILL_P = 0;
    public const int SKILL_Q = 1;
    public const int SKILL_W = 2;
    public const int SKILL_E = 3;

    // Skill UI
    private GameObject[] panelSkill = new GameObject[4];
    private Image[] imageBack = new Image[4]; // 쿨타임일 때 뒷 배경 검게
    private Image[] imageCooldown = new Image[4]; // 쿨타임 표시 UI (시계 방향)
    private Text[] textTime = new Text[4]; // 쿨타임 표시
    private int[] currentCooldown = { 0, 0,0,0};

    void Start()
    {
        panelSkill[0] = GameObject.Find("SkillP").gameObject;
        panelSkill[1] = GameObject.Find("SkillQ").gameObject;
        panelSkill[2] = GameObject.Find("SkillW").gameObject;
        panelSkill[3] = GameObject.Find("SkillE").gameObject;

        for(int i=0; i<panelSkill.Length; i++)
        {
            GameObject back = panelSkill[i].transform.Find("Image_Back").gameObject;
            back.SetActive(true);
            GameObject cooldown = panelSkill[i].transform.Find("Image_Cooldown").gameObject;
            cooldown.SetActive(true);
            GameObject time = panelSkill[i].transform.Find("Text_Time").gameObject;
            time.SetActive(true);

            imageBack[i] = back.GetComponent<Image>();
            imageCooldown[i] = cooldown.GetComponent<Image>();
            textTime[i] = time.GetComponent<Text>();

            imageBack[i].enabled = false;
            imageCooldown[i].enabled = false;
            textTime[i].enabled = false;
        }
    }

    public void SkillUI_Cooldown(int skillType, int cooldown)
    {
        currentCooldown[skillType] = cooldown;
        textTime[skillType].text = "" + cooldown;

        StartCoroutine(Cooldown(skillType, cooldown));
        StartCoroutine(CooldownCounter(skillType));
    }


    IEnumerator Cooldown(int skillType, int cooldown)
    {
        imageBack[skillType].enabled = true;
        imageCooldown[skillType].enabled = true;

        imageCooldown[skillType].fillAmount = 1;
        
        while (currentCooldown[skillType] > 0)
        {
            imageCooldown[skillType].fillAmount -= Time.smoothDeltaTime / cooldown;

            yield return null;
        }

        imageBack[skillType].enabled = false;
        imageCooldown[skillType].enabled = false;

        yield break;
    }

    // 남은 시간 계산
    IEnumerator CooldownCounter(int skillType)
    {
        textTime[skillType].enabled = true;

        while (currentCooldown[skillType] > 0)
        {
            yield return new WaitForSeconds(1.0f);

            currentCooldown[skillType] -= 1;
            textTime[skillType].text = "" + currentCooldown[skillType];
        }
        textTime[skillType].enabled = false;

        yield break;
    }

}
