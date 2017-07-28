using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MachineGun : Gun {

    public override void Interaction()
    {//사면서 동시에 장착하게될것
        base.Interaction();
    }
    public override void Shoot()
    {
        
        base.Shoot();//걍총쏘는 것이니 건을 기본으로 닮아 가도됨
       
    }
    // Use this for initialization
    void Start () {
        id = 2;//머신건은 아이디가 1
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
