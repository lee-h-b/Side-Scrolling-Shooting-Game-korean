using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//수류탄을 던지기 인터페이스로 잡아야할듯함
//ㄴ기각 그냥 슛만 오버라이드 하면 얼추 해결될것으로 예상
public class Grenade : Gun {
    private float lifeTime = 3f;//3초후 터짐
    //아래는 private로 바뀌어야함
    public Vector3 destination;
    public Vector3 startPosition;
    public float height;
    public float distance;
    //여기까지
    private float middlePoint;// 두 x의 중간지점 << 적어도 중간에서는 내려가도록하기위함
    private float startTime;//이 수류탄 던지기 시작한 시간
    [SerializeField]
    private int damage;//폭팔데미지  폭팔구성은 시간이 다됐다면 온트리거 엔터를돌림
    //그러기 위해서 스피어 콜라이더를 이스트리거로 하나챙김

        //폭팔하고 일정시간 대기하고 삭제되도록
    IEnumerator Explosion()
    {
        var part = GetComponent<ParticleSystem>();
        if (part.isPlaying == false)
        {
            GetComponent<SphereCollider>().enabled = true;
            part.Play();
            
            yield return new WaitForSeconds(part.main.startLifetimeMultiplier);

                Destroy(gameObject);
        }
        else
            yield return null;
    }
    public override void CunsumeBullet()
    {
        myMagazine = GameManager.inst.GetComponent<WeaponManager>().GetWeaponInfo(id);

        myMagazine.curMagazine -= 1;
        if (myMagazine.curMagazine < 0) myMagazine.curMagazine = 0;
        if (myMagazine.maxMagazine > 0)
        {//이거 하나때문에 가상화시킴ㅇ
            myMagazine.maxMagazine -= 1;
            myMagazine.curMagazine += 1;
        }
        GameManager.inst.GetComponent<WeaponManager>().SetGun(myMagazine);
        nextShootTime = Time.time + BetweenFire / 1000;
    }
    //마우스 클릭 놓은 순간 던지는게 시작되서 목표 좌표를 잡고 그럼
    public override void Shoot()
    {
        if (transform.parent.tag == "Player")
        {
            //기존과는 달리 이거 자체를 던지는것 따라서 프로젝타일복제 쪽은 최대한 피함
            CunsumeBullet();
        }

        GetComponent<Rigidbody>().isKinematic = false;
        startPosition = this.transform.position;
        destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);//마우스의 현위치를 변환
        //우선 자신의 부모에서 탈출+ 소멸 + 기본무기장착을함 이3개를 다해야 스위칭할때소멸x
        transform.parent.transform.parent.GetComponent<GunHandle>().UnEquip();
        transform.parent = null;
        middlePoint = Mathf.Abs(destination.x - transform.position.x) / 2;
        
        RaycastHit hitInfo;
        if(Physics.Raycast(destination, Vector3.down,out hitInfo,50f) )
        {//아래로 50쏴서 닿는게 있을경우
            destination = hitInfo.transform.position + Vector3.up * 3f;//도착지재정의
            
        }
        destination.z = 0f;

        if (destination.y > startPosition.y)
            height = Mathf.Abs(destination.y - startPosition.y + 7f);
        else height = 7f;

        distance = Mathf.Abs(destination.x - startPosition.x);

        startTime = Time.time;//시간설정
    }
    // Use this for initialization
    private void Throwing(float per)
    {

            //상향
            if (per < middlePoint && height > transform.position.y)
            {//AddForce와는 달리 이건 상수식으로 속력을줌 왜 상수냐면 물체를 던지는데 그게 점점 빨라지면 이상하니까
                if (startPosition.x > destination.x)
                    GetComponent<Rigidbody>().velocity = new Vector3(-muzzleVelocity, muzzleVelocity / 2f);
                else
                    GetComponent<Rigidbody>().velocity = new Vector3(muzzleVelocity, muzzleVelocity / 2f);
            }
            //하향
            else if (per < distance)
            {
                height = 0;//높이를 0으로해서 위의 조건을 다신 못부르게 만듬
                //중간에 힘이 다해서 내려가듯이, 벨로시티로 하면 삼각형되버림
                if (startPosition.x > destination.x)
                    GetComponent<Rigidbody>().AddForce(-muzzleVelocity * Time.deltaTime, -muzzleVelocity / 2 * Time.deltaTime, 0, ForceMode.VelocityChange);

                GetComponent<Rigidbody>().AddForce(muzzleVelocity * Time.deltaTime, -muzzleVelocity / 2 * Time.deltaTime, 0, ForceMode.VelocityChange);
                //                GetComponent<Rigidbody>().velocity = new Vector3(muzzleVelocity , -muzzleVelocity / 2 , 0);
                GetComponent<Rigidbody>().useGravity = true;//중력을줘서 좀더 리얼하게 내려가게함
                GetComponent<CapsuleCollider>().enabled = true;//특정물체 위에 얹어지는 느낌을 주게함
            }
}

    // Update is called once per frame
    void Update()
    {
        //원래 Throwing에 두려 했으나 그렇게되면 함수가 스택에 함들어가야하니 무리가 갈듯
        float per = Mathf.Abs(startPosition.x - transform.position.x);

        if (transform.parent == null && per < distance)//해당거리만큼 이동다할때까지
        {        
            Throwing(per);
        }

        if (transform.parent == null && startTime + lifeTime < Time.time)
        {
            //Explosion();
            StartCoroutine(Explosion());
        }
    }
    
    private void OnTriggerEnter(Collider other)//키자마자 하면 전부 온트리거 되겠지?
    {
        //캐릭터 전부에게 데미지를 가함
        if(other.GetComponent<Character>() != null)
        {
            other.GetComponent<Character>().TakeDamage(damage);
        }
    }
}
//본래 중간에 멈추는 속도를 제어시키려 했으나 필요없는듯