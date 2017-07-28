using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [SerializeField]
    GameObject gameOverText;
    [SerializeField]
    GameObject anyKeyText;
    public int money;
    public float dist;

    public Player player;
    public static GameManager inst;
    IEnumerator LoseEvent()
    {
        yield return new WaitForSeconds(0.5f);
        if(anyKeyText.activeSelf == false)
        {            
            anyKeyText.SetActive(true);
        }
        else if(anyKeyText.activeSelf == true)
        {
            if (Input.anyKey)
            {
                SceneManager.LoadScene("StartMenu");
                SceneManager.UnloadSceneAsync("InGame");
            }

        }
    }
//이름은 드로우 UI지만 체력,마력만 Draw할것
    public void DrawUI()
    {
        var image = GameObject.Find("Canvas").transform.Find("HpBar").Find("Cur").GetComponent<Image>();
        image.fillAmount = (float)player.hp / player.maxHp;
        var text = image.transform.parent.Find("Text").GetComponent<Text>();
        text.text = "체력 : " + player.hp + " / " + player.maxHp;
        //이미지,텍스트 변수를 재활용해서썼음
        image = GameObject.Find("Canvas").transform.Find("BarrierBar").Find("Cur").GetComponent<Image>();
        image.fillAmount = (float)player.barrier / player.maxHp;
        text = image.transform.parent.Find("Text").GetComponent<Text>();
        text.text = "쉴드 : " + ((float)player.barrier / player.maxHp  *100).ToString("N0")+ " %";
    }
	// Use this for initialization
	void Awake () {
        
        if (inst == null) inst = this;
        AchievementManager.instance.RecordValueAdder();

    }

    // Update is called once per frame
    void Update () {
        DrawUI();
        if (player == null)//플레이어가 사라짐
        {
            gameOverText.gameObject.SetActive(true);
            StartCoroutine(LoseEvent());
        }
        //플레이어생존
        else
        {
            float temp = GameManager.inst.player.transform.position.x;
            if (Mathf.Max(temp, dist) == temp)
            {
                if ((int)temp - (int)dist > 0)
                    AchievementManager.instance.RecordValueAdder(ACHIEVEMENTTYPE.MOVE);

                dist = temp;


            }

            //        if (dist > AchievementManager.instance.record.distance)
            //      AchievementManager.instance.RecordValueAdder(ACHIEVEMENTTYPE.MOVE, (int)dist - AchievementManager.instance.record.distance);
            if (transform.Find("BLOCK") != null)//플레이어 뒤를 막아두는 역활을함
                transform.Find("BLOCK").transform.position = new Vector3(dist - 10, 2);
        }
        
        var input = Input.inputString;
        switch(input)
        {
            case "1": GetComponent<WeaponManager>().weapons[0].gun.Equip(); break;
            case "2": GetComponent<WeaponManager>().weapons[1].gun.Equip(); break;
            case "3": GetComponent<WeaponManager>().weapons[2].gun.Equip(); break;
            case "4": GetComponent<WeaponManager>().weapons[3].gun.Equip(); break;
            case "5": GetComponent<WeaponManager>().weapons[4].gun.Equip(); break;
        }

    }
}
