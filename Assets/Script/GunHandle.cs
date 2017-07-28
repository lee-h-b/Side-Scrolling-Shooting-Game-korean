using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHandle : MonoBehaviour {
    //    public Transform weaponHold
    public Gun startingGun;
    Gun equippedGun;
    public void UnEquip()//끼고 있는 무기를 벗고 기본무기 낄꺼임 이게다 수류탄탓
    {
        equippedGun = null;
        Equip(startingGun);
    }
    public void Equip(Gun newGun)
    {
        if(equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }
        Transform hand = this.transform.Find("Hand");
        equippedGun = Instantiate(newGun);
        equippedGun.transform.SetParent(hand,false);
    }
    public void Shoot()
    {//리로드 가 여기에 있는이유는 이렇게 하는게 맞는거같아서ㅇ
        if(equippedGun != null)
        {
            WeaponInfo gunInfo = GameManager.inst.GetComponent<WeaponManager>().GetWeaponInfo(equippedGun.id);
            //쏘는데 자기가 플레이어고 현재 총알이x
            if(gunInfo.curMagazine <= 0 && transform.tag == "Player")
            {
                    equippedGun.Reload();
            }
            else
            equippedGun.Shoot();
        }
    }
	// Use this for initialization
	void Start () {
		if(startingGun != null)
        {
            Equip(startingGun);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
