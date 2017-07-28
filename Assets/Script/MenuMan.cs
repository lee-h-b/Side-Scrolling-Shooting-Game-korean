using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuMan : MonoBehaviour {
    [SerializeField]
    private Transform helpCanvas;
    [SerializeField]
    private Transform menu;

    public GameObject achievementViewPanel;
    public RectTransform achievementPanel;

    public void TurnOffAchievePanel()
    {
        transform.Find("AchievementsPanel").gameObject.SetActive(false);
        menu.gameObject.SetActive(true);

    }
    public void ShowAchievements()
    {
        transform.Find("AchievementsPanel").gameObject.SetActive(true);
        menu.gameObject.SetActive(false);//기존(메뉴)는 비활성

        var Ainstance = AchievementManager.instance.Achievements;
        var showCasePanel = achievementViewPanel.transform.Find("Viewport").Find("Content");
        var clearChecker = AchievementManager.instance.clearList;
        //만약 이번에 생성된애의 높이가 넘어서면 높이가 걔가되야함
        float newHeight = 0f;
        //딕셔너리를 키밸류 페어로 해야함
        foreach(KeyValuePair<int,Achievement> cur in AchievementManager.instance.Achievements)
        {
            var temp = Instantiate(achievementPanel, showCasePanel);
            var t_name = temp.Find("Name");
            var t_description = temp.Find("Description");
            var t_progress = temp.Find("Progress");
            t_name.GetComponent<Text>().text = cur.Value.name;
            t_description.GetComponent<Text>().text = cur.Value.Description;
            int value = 0;
            //만약 이게 클리어된 값이라면은 케이스 패널의 색이 바뀌도록하자
            if (AchievementManager.instance.clearList.Contains(cur.Key))
            {
                temp.GetComponent<Image>().color = Color.cyan;
            }

            //스위치,케이스를 통해 cur의 타입에따라 밸류값을 가져오는게 바뀜
            switch (cur.Value.needType)
            {
                case ACHIEVEMENTTYPE.PLAY:
                    value =AchievementManager.instance.record.playCount;
                    break;
                case ACHIEVEMENTTYPE.ENEMY:
                    value = AchievementManager.instance.record.killEnemy;
                    break;
                case ACHIEVEMENTTYPE.MONEY:
                    value = AchievementManager.instance.record.spentMoney;
                    break;
                case ACHIEVEMENTTYPE.BULLET:
                    value = AchievementManager.instance.record.useBullet;
                    break;
                case ACHIEVEMENTTYPE.MOVE:
                    value = AchievementManager.instance.record.distance;
                    break;
            }
            //새 높이값을 사이즈델타의y로함
          newHeight += temp.GetComponent<RectTransform>().sizeDelta.y;
            //만약 현높이보다 새높이가 더크면 새높이로 대입함
            if (showCasePanel.GetComponent<RectTransform>().rect.height < Mathf.Abs(newHeight))
                showCasePanel.GetComponent<RectTransform>().sizeDelta =new Vector2(achievementViewPanel.transform.Find("Viewport").GetComponent<RectTransform>().rect.width, newHeight);
            //만약 밸류값이 더크다면 클리어로표현함
            if (value >= cur.Value.needPoint)
                t_progress.GetComponent<Text>().text = "클리어!";
            else
            t_progress.GetComponent<Text>().text = value.ToString() + " / "+cur.Value.needPoint.ToString();
        }
    }

    public void GameStart()
    {
        SceneManager.LoadScene("InGame");
        SceneManager.UnloadSceneAsync("StartMenu");
        
    }
    public void Help()
    {
        menu.gameObject.SetActive(false);
        helpCanvas.gameObject.SetActive(true);
    }
    
    public void ToMenu()
    {
        menu.gameObject.SetActive(true);
        helpCanvas.gameObject.SetActive(false);
        
    }
    public void Exit()
    {
        Application.Quit();
    }
}
