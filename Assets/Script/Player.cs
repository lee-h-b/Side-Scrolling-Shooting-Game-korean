using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
[RequireComponent(typeof(PlayerControll))]//둘은 항상 있겠지만 강조해봄

public class Player : Character {
    Camera mainCamera;
    PlayerControll controller;
    GunHandle gun;

    //이게 여기에 있는건 안어울려서 vest를 만들려했으나
    //기본으로 전파되는 애이름이 Gun이고 해서(Item이 더어울릴것)
    //그냥 최대체력의 10%의 값을 빼가는걸로결정 그렇게되면 플레이어 내에서 해도 될것
    public void BarrierCharge()
    {
        int pay = maxHp;
        if (GameManager.inst.money >= pay && barrier < maxHp)
        {
            barrier += maxHp / 10;
            if (barrier > maxHp)
            {                
                barrier = maxHp;
            }
            GameManager.inst.money -= pay;
            AchievementManager.instance.RecordValueAdder(ACHIEVEMENTTYPE.MONEY, pay);
            
        }
    }
	void Start () {
//        base.Start();//능력친 이쪽에서
        controller = GetComponent<PlayerControll>();
        mainCamera = Camera.main;
        gun = transform.Find("GunHandle").GetComponent<GunHandle>();
		
	}
    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero);//법선벡터
        //저대로두면 무한히? 뻗어나가게되서 일종의 커트라인격
        float rayDistance;

        if (plane.Raycast(ray,out rayDistance))
        {
            Vector2 point = ray.GetPoint(rayDistance);

            controller.aiming(point);
        }
        if(Input.GetMouseButton(0) )
        {
            //이벤트시스템으로 현재 게임오브젝트위에 있는게 아니라면 슛을하는구조
            if(EventSystem.current.IsPointerOverGameObject() == false)
            gun.Shoot();
        }
    }
    // Update is called once per frame
    //이것들은 컨트롤할 필요가없으므로~! 는 거짓말이고 게임매니저가 직접건드릴수있는게 아니니깐..
    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<PlayerControll>().isJump = false;
    }

}
