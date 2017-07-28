using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CapsuleCollider))]//이캡슐이 위의 라인(레이저)의 충돌판정냄
public class AdhesionProjectile : Projectile {
    //부착타입은 완전히 다르게돌아갈것, 하지만 발사하기위해 projectile로 선정함
    //ㄴ이부착타입을 복제해서 쏘는 타입으로 하면?
    //해당좌표를 총에 전달해주고 그래야하는 번거로움이 생겨서 무리
    //총자체는 비활성된 상태로 있었던걸로

    [SerializeField]
    float ionTime;//레이저 쏘아지는 타임정도
    float ionFireTime;//이온발사된시간쯤
    [SerializeField]
    private float fireHeight;//이온의 발사시작 높이
    private float ionLength;//이온의 길이 << -fireHeight까지갈거임
    private float damageTick = 0.5f;
    private float[] lastHit = new float[0];//레이저안에 들어온 애들의 마지막 받은 데미지를 관리할듯 
    private List<GameObject> hitObjects = new List<GameObject>();
    private List<int> objectDamages = new List<int>();//각각의 오브젝트에게 줄 데미지들
    private void IonFire(CapsuleCollider capsule)
    {
        //이온파이어 발사한시간과 이온의 지속시간이 time보다 크면 항상진행
        if(ionFireTime + ionTime > Time.time)
        {
            //이온 길이늘림
            ionLength++;
            //중심을 새로잡아준다 1만큼 늘어나니깐 0.5만큼( 길이가 0.5 0.5씩 오르니)내림
            capsule.center -= new Vector3(0, 0.5f);
            capsule.height = Mathf.Abs(ionLength);//중심을 잡았으면 길이를 동기화해야하니깐

            //그리고 라인렌더(레이저)의 1번째(목표지점)을 높이에서 이온길이를 뺀값으로 낮춤
            GetComponent<LineRenderer>().SetPosition(1, new Vector3(transform.position.x, fireHeight - ionLength, 0));

        }
        else
        {//만약 빔이 다컸을경우
            Destroy(gameObject);
        }
        //빔이 자라난다는걸 암시ㅎ
    }
     //너무 코루틴을 남발하는건 안좋을듯
     void IonSet()//이온을 발사하기위한 사전작업
    {
        var ion = GetComponent<LineRenderer>();//라인렌더를 이온으로할거니깐 이렇게잡음
        var capsule = GetComponent<CapsuleCollider>();
        ion.SetPosition(0,new Vector3(transform.position.x,fireHeight,0) );//0이 시작위치
        capsule.center = new Vector3(0,fireHeight,0);//이후 이 값은 ionLength/2값으로줄어
        GetComponent<SphereCollider>().enabled = false;//구체콜라이더는 비활성화시켜서 데미지를못가하도록
        capsule.enabled = true;//캡슐 활성화
        GetComponent<MeshRenderer>().enabled = false;//메쉬 비활성
      }
    // Use this for initialization
    private bool OverlapCheck()
    {
        Collider[] Overlap = Physics.OverlapSphere(transform.position, 0.1f);
        if(Overlap[0].GetComponent<Character>() != null)
        {
            return true;
        }
        return false;
    }
    void Start () {
        Destroy(gameObject, lifeTime);//이거 일안하는거같아서
	}
    //업데이트를 오버라이드 하지말고 Hit을 오버라이드함
    //ㄴ 그래도 혹시모르니 이대로냅둠
    protected override void Hit(RaycastHit hit)
    {
        IActor hitObject = hit.collider.GetComponent<IActor>();
        if (hitObject != null && hit.transform.tag != tag)
        {
            speed = 0;//속도는 0으로하고
            ionFireTime = Time.time;
            IonSet();//이온세트 

        }
        //부착식은 짱짱맨이라 소멸안함 << 레이저가 파괴가능 물체고뭐고 다붙어버리면
        //난감할것
    }
    // Update is called once per frame
    protected override void Update () {
        float distance = speed * Time.deltaTime;
        //물체가 갔을곳을 미리 충돌체크? 하는느낌
        CheckCollisions(distance);//모든 총기는 이걸함
        transform.Translate(Vector3.right * distance);//이름이 이상하지만 스피드* 델타타임이니 ㄱㅊ

        //속도가 0이아닌데 오버랩되있다면(즉 시작하자마자 나온상태)
        if (speed != 0 && OverlapCheck() == true) speed = 0;

        if (speed == 0)//스피드가 0이면 정지상태니깐 거기에 레이저를쏨
        {
            IonFire(GetComponent<CapsuleCollider>());
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        //들어온 캐릭터들을 리스트로 담아낸다
        if (other.GetComponent<Character>() != null)
        {
            //count를 늘려서 갯수 증진
            int count = lastHit.Length + 1;
            //새로 담을 Temp
            float[] temp = new float[count];
            //반복을 통해 lastHit을 Temp로 일단 옮겨옴
            for (int i = 0; i < lastHit.Length; i++)
            {
                temp[i] = lastHit[i];
            }
            //라스트힛을 temp로 이사함ㅎ
            lastHit = temp;
            //여기 까지가 시간(라스트힛)에 대한 확장이고
            //이제 해당오브젝트를 담아감
            hitObjects.Add(other.gameObject);
            objectDamages.Add(damage);//받을 데미지 넣어줌
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //속도가 0일경우에만 활동함
        if (speed == 0 && other.GetComponent<Character>() != null && this.tag != other.tag)
        {
            //현재 충돌한 오브젝트중 other를 찾음 < other가많아지면 느려지겠지만 
            //금방사라지고하니 Exit처리는 안해씀
            int pos = hitObjects.IndexOf(other.gameObject);
            //마지막 데미지받은 시간에서 damageTick만큼의 시간이 지날경우
            if (lastHit[pos] + damageTick < Time.time)
            {
                //lastHit의 길이는 hitObjects와 같고 위치또한 같으니 그대로감
                //딕셔너리했으면 빨랐을지도
                
                //마지막 데미지 받은 시간갱신
                lastHit[pos] = Time.time;
                objectDamages[pos] += damage;

                other.GetComponent<Character>().TakeDamage(objectDamages[pos]);//데미지받음
            }
        }
    }
}
