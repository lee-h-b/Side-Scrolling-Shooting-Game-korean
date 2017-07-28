using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//체력바때문에참조

/*소환되자마자 대기상태를하다가
 * sight 범위안에 들어왔다면 그쪽으로 사격을함
 */
 //hp바 만들기 현재 canvas를 가지고있고 적(자신)의 체력바를 거기에 보냄
 //이렇게 보낼때 canvas안에 체력바가 들어가도록함
public class Enemy : Character {
    GameObject player;//플레이어, 바로찾아서 바로잡을꺼임

    SphereCollider range;//공격범위 들어오자마자(체크하는방법은 이거 반지름 안에들어올경우
                         //플레이어 좌표에 바로 총을쏘기시작함
                         // Use this for initialization
    float lastAttack = 0;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    GameObject healthBarPrefab;
    GameObject healthBar;
    Slider healthSlider;
    
    public float AttackDelay = 0;//공격의들레이
    public bool find;//플레이어가 공격범위안에 한번이라도 들어왔었으면 이게참이되고 참이되면 쫓아감
                     //ㄴ 적에따라 참으로 시작할지도?
                     //점프높이는 2정도
    float jumpHeight = 7;//원래는 ray같은걸로 여기가 막혔는지 체크하는게 눈에띄게 하고싶음
    public float speed =3f;
    public bool jumping = false;
    public GunHandle gunHand;
    public int dropMoney;
    
    //체력바를 canvas에 뿌려주는 역활
    void MakeHealthBar()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        healthBar = Instantiate(healthBarPrefab);
        healthBar.transform.SetParent(canvas.transform,false);

        healthSlider = healthBar.GetComponent<Slider>();
    }
    void UpdateHealthBar()
    {
        healthSlider.value = (float)hp / maxHp;
        Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        healthBar.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
    }
    void Fire()
    {
        if (Time.time > lastAttack)
            {
            gunHand.GetComponent<GunHandle>().Shoot();
            lastAttack = Time.time + AttackDelay;
        }
    }
    public void aiming(Vector3 targetPoint)
    {
                float angle = Mathf.Atan2(transform.position.x - targetPoint.x,
                    transform.position.y - targetPoint.y) * Mathf.Rad2Deg;
                gunHand.transform.localRotation = Quaternion.Euler(0,  0, -(angle + 90f));
        //총조준 플레이어 컨트롤에 있어서 걍새로만듬
        //혹시 더만지게되면 이걸 합치도록할거임
    }
    void MoveMent()
    {
        //이동하기
        Vector3 view;
        if (player.transform.position.x > this.transform.position.x)//플레이어x가더큼
            view = Vector3.right;
        else//아냐 플레이어 x가 더작아!
            view = Vector3.left;

            RaycastHit hit;
        if (Physics.Raycast(transform.position, view, out hit,4f))//보는쪽으로 4앞
        {
            if(hit.transform.tag != "Player" && jumping == false)
            {
                jumping = true;
                GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpHeight),ForceMode.Impulse);
            }
        }//레이캐스트에 닿은게있으면 점프를하면서 이동,아니면 그냥이동
        //rb.AddForce(Vector3.right * speed, ForceMode.Impulse);
            transform.Translate(view * speed * Time.deltaTime);
    }
    protected override void Die()
    {
        GameManager.inst.money += dropMoney;
        AchievementManager.instance.RecordValueAdder(ACHIEVEMENTTYPE.ENEMY);//죽으면이거추가
        DestroyObject(healthBar);//체력바의 소멸
        base.Die();
    }
    void Start () {
        base.Start();
        MakeHealthBar();
        range = transform.Find("Range").GetComponent<SphereCollider>();
        if (player == null) 
        player = GameObject.Find("Player");
        Random.InitState((int)System.DateTime.Now.Ticks);
        //시작총 랜덤할당
        //0부터 웨폰매니저의 최대길이까지로함, 아마 0~length -1의 길이일껀데 맨뒤는 수류탄이니 상관없다고봄
        //Equip는 안됨 아마 이거 스타트하고 건핸드의 스타트를 실행해서 그럼
        gunHand.startingGun = GameManager.inst.GetComponent<WeaponManager>().weapons[Random.Range(0,GameManager.inst.GetComponent<WeaponManager>().weapons.Count-1)].gun;

        //샷건이 아니라면 미발견 상태로둠
        print(gunHand.startingGun.name);
        if (gunHand.startingGun.name == "ShotGun")
            find = true;
        else
            find = false;

        //사정거리의 반지름을 설정 <이사정거리 안에 들어오면 공격 시작
        range.radius = gunHand.startingGun.MuzzleVelocity * gunHand.startingGun.Projectile.LifeTime;

        //공격딜레이 정립 <기존딜레이에 추가로 총쏘는 간격
        AttackDelay += (gunHand.startingGun.BetweenFire*0.001f);
    }

    // Update is called once per frame
    void Update () {
        //플레이어가 없다면 null
        if (player == null)
            return;
        UpdateHealthBar();
        aiming(player.transform.position);

        //나와 플레이어의 거리정도
        float EtoP = Vector2.Distance(player.transform.position, transform.position);
        
        if (find == true && EtoP > range.radius)
        {
            Fire();
            MoveMent();
        }
        //적과 플레이어의 거리가 사정거리 안에 들어올경우
        else if (EtoP <= range.radius)
        {
            find = true;
            // 총쏘기시작함
            Fire();
    //        StartCoroutine(Fire());
        }
		
	}
    private void OnCollisionEnter(Collision collision)
    {
        //만약 물체에 닿아 있고 벨로시티가 0일경우만 점핑 << 벨로시티체크안하면 높이높이날아버림
        if(collision.transform.position.y < transform.position.y && GetComponent<Rigidbody>().velocity == Vector3.zero)
        jumping = false;
    }

}
