using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public struct WeaponInfo
{
    public int ID;
    public int curMagazine;//현재탄약수
    public int maxMagazine;//최대보유탄약수
    public int oneMagazine;//리로드시 나올,구입마다 증가될 탄약수
    public int pay;
    public Transform Uiview;//아이디 0이면 비활성화
    //이 gun을 불러서 장착함
    public Gun gun;
}


public class WeaponManager : MonoBehaviour {
        public List<WeaponInfo> weapons = new List<WeaponInfo>();
    //0번째는 권총으로 값이없고 ID가 0임, 이러면 무제한으로 쏠수있게함
    //1번째는 기관총으로 가장 보통임
    //2번째는 샷건
    //3번째는 수류탄 2500~3000원쯤? 던진게 땅에닿고 3초후폭발
    //4번째는 공습
    //우선 0번째부터 만들고 어떻게 할지 생각할거임
    // Use this for initialization
    public WeaponInfo GetWeaponInfo(int id)
    {
       int index =  weapons.FindIndex(weapons => weapons.ID == id);
        if (index != -1)
            return weapons[index];
        else return weapons[0];
    }
    public void SetGun(WeaponInfo gun)
    {
        int index = weapons.FindIndex(weapons => weapons.ID == gun.ID);
        //gun과 같은값을 찾아서 넣어주는식
        if (index != -1)
            weapons[index] = gun;
    }
    void WriteUI(int i)
    {
        Transform uiPath = weapons[i].Uiview.Find("Texts");
        Text curText = uiPath.Find("CurMagazine").GetComponent<Text>();
        //weapons[i].curMagazine ;//현재탄약수 << 0이되면 리로드하고 다시 oneMagazine이됨
        Text maxText = uiPath.Find("MaxMagazine").GetComponent<Text>();//최대 보유 탄약수 << 리로드할때마다 oneMagazine에서빼옴
          //위 2개가 0이되면 구매해야함 
        Text payText = uiPath.Find("Price").GetComponent<Text>(); ;//가격,애가 0원이면 기본무기임
        
        curText.text = weapons[i].curMagazine.ToString();
        maxText.text = weapons[i].maxMagazine.ToString();
        payText.text = weapons[i].pay.ToString();
    }
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i< weapons.Count; i++)
        {
            if(weapons[i].ID != 0)
            WriteUI(i);
        }
	}
}
