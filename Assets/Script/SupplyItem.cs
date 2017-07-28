using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyItem : MonoBehaviour {
    public enum TYPE { HP,SHIELD, ALLINONE};
    
    public TYPE type;

    [SerializeField]
    private float vestPer;//쉴드 회복수치 << 체력에 쓰기엔 이게임이 너무 빡쌔다고생각함

    public Sprite[] image;

    // Use this for initialization
    void EnableItem()
    {
        var player = GameManager.inst.player;

        switch (type)
        {
            case TYPE.HP:
                {
                    player.hp = player.maxHp;//최대체력으로 회복
                }
                break;
            case TYPE.SHIELD:
                //vestPer만큼만 증가
                player.barrier += (int)(player.maxHp * vestPer / 100);
                if (player.barrier > player.maxHp)
                    player.barrier = player.maxHp;
                break;
            case TYPE.ALLINONE:
                player.maxHp += 10;
                player.hp = player.maxHp;
                player.barrier = player.maxHp;
                break;
        }
        Destroy(gameObject);//보급 됐으니 소멸
        //보급사용 스위치를 통해서 아이템을 사용
        //스타트에서도 스위치를 통해서 해당 스프라이트를 돌려쓸것
    }
    void Start () {
        Random.InitState((int)System.DateTime.Now.Ticks);
        //타입 hp~ AllinOne중 1개를 잡음
        type = (TYPE)Random.Range((int)TYPE.HP, (int)TYPE.ALLINONE +1);

        SpriteRenderer boxImage =  transform.Find("Image").GetComponent<SpriteRenderer>();
        boxImage.sprite = image[(int)type];
	}
	
	// Update is called once per frame
	void Update () {
	}
    private void OnTriggerEnter(Collider other)
    {
        //총으로 치던 어떻게하던 플레이어면 ok
        if(other.tag == "Player")
        {
            EnableItem();
        }
    }
}
