using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//총을 인터페이스 화 할까도 싶은데 모든총은 권총의 파생이니 그럴필요가 없다고판단됨
//그대신 모든총은 이총을 기본으로 잡아야겠지만
public class Gun : MonoBehaviour {
    protected float speed;
//    public float reloadTime;//몇몇개는 플레이어를 위해 남겨둠 << 애니클립의 길이로 가게되니 안씀

                            //사실 게임매니저에 있는것들은 이거 접근을 함부로하기 뭐해서 그럼ㅎ..
    public int id= 1;
    [SerializeField]
    [Header("단위는 ms")]
    public float BetweenFire;//발사간격쯤
    [SerializeField]
    protected float muzzleVelocity = 11f;//안쓸지도모름?
                    // Use this for initialization
    protected float nextShootTime;
    [SerializeField]
    protected Projectile projectile;
    [SerializeField]
    protected Transform muzzle;
    protected WeaponInfo myMagazine;//총알정보
    //총알재장전 재장전중에 또하면 또 실행되는경우가 생김 따라서 이건 핸들로 옮겨야할거같음
    //프로퍼티
    public float MuzzleVelocity
    {
        get { return muzzleVelocity; }
    }
    public Projectile Projectile
    {
        get { return projectile; }
    }
    IEnumerator ReloadCoroutine()
    {        
        Animation ani = GetComponent<Animation>();        
        if (myMagazine.maxMagazine == 0)
        {
            //최대총알이 없다면 제거하고, 0번째 총(기본총)을 장착시킨다
            GameManager.inst.GetComponent<WeaponManager>().weapons[0].gun.Equip();
            Destroy(this.gameObject);

            yield return null;
        }
        if (ani.isPlaying == false)
        {
            ani.Play(ani.clip.name);
            //리로드 애니메이션을 수행하고 재장전
            yield return new WaitForSeconds(ani.clip.length);

            if (id == 1) myMagazine.curMagazine = myMagazine.oneMagazine;

            else
            {
                //최대탄약을 1세트-현재만큼의 차이만큼 줄임
                myMagazine.maxMagazine -= myMagazine.oneMagazine - myMagazine.curMagazine;
                myMagazine.curMagazine = myMagazine.oneMagazine;
            }


            GameManager.inst.GetComponent<WeaponManager>().SetGun(myMagazine);

        }
                yield return null;
    }
    //외부에서 스타트코루틴을 못부름
    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }
    public virtual void CunsumeBullet()
    {
        if (transform.parent.tag == "Player")
        {
            //이걸 실행하기도전에도 플레이어였고 이걸하면서 플레이어인지 최종적으로 확인하네
            AchievementManager.instance.RecordValueAdder(ACHIEVEMENTTYPE.BULLET);//총알은 1개씩소모될테니 값x
            //탄피 관련 정보를 받음
            myMagazine = GameManager.inst.GetComponent<WeaponManager>().GetWeaponInfo(id);

            myMagazine.curMagazine -= 1;
            if (myMagazine.curMagazine < 0) myMagazine.curMagazine = 0;

            GameManager.inst.GetComponent<WeaponManager>().SetGun(myMagazine);

            nextShootTime = Time.time + BetweenFire / 1000;

        }
    }
    public virtual void Shoot()
    {
        //총을 쏠시간?
        if(Time.time > nextShootTime)
        {
            //플레이어만 총의 감산이 있도록한다
            if (transform.parent.tag == "Player")
                CunsumeBullet();//if째로 함수로 옮길까 싶었으나 좀비효율? 일거같았음
            Projectile projectileTemp = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            projectileTemp.tag = transform.parent.tag;
            projectileTemp.SetSpeed(muzzleVelocity);
        }
        //얘는 총이무제한일것 따라서 curMagazine을 안씀
        //다른애들은 아마 쓸거같은데
    }

    public virtual void Interaction()
    {
        myMagazine = GameManager.inst.GetComponent<WeaponManager>().GetWeaponInfo(id);
        int pay = myMagazine.pay;
        if (GameManager.inst.money >= pay)
        {
            AchievementManager.instance.RecordValueAdder(ACHIEVEMENTTYPE.MONEY, pay);
            GameManager.inst.money -= pay;
            if (myMagazine.curMagazine == 0)
            {

                myMagazine.curMagazine += myMagazine.oneMagazine;
            }
            else
                myMagazine.maxMagazine += myMagazine.oneMagazine;

            GameManager.inst.GetComponent<WeaponManager>().SetGun(myMagazine);
        }
    }
    public virtual void Equip()
    {
        WeaponInfo myMagazine = GameManager.inst.GetComponent<WeaponManager>().GetWeaponInfo(id);
            //둘다 0일경우
            if (myMagazine.curMagazine == 0 && myMagazine.maxMagazine == 0)
                return;
            else //둘중하나가 0이아님
            {
                Transform handle = GameManager.inst.player.transform.Find("GunHandle");
                handle.GetComponent<GunHandle>().Equip(this);
            }        

    }
    void Start () {
        myMagazine = GameManager.inst.GetComponent<WeaponManager>().GetWeaponInfo(id);

    }

    // Update is called once per frame
    void Update () {
        
		
	}
}
