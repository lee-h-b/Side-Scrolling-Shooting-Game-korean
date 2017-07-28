using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//진짜어쩌면 타일돌리는거? 그건 필요없을지도몰라

//해당블록의 x가 -21로가면 다시 21로돌아감
//ㄴ 이상태에서 추가옵션이 발생해서 장식,조형물이 장착될수있음
//구성은? 필드에붙일까 게임매니저가 가지고있을까?
//필드에 붙여두면 오브젝트를 생성할때나 그럴때 편할거임
//예상중인건 플레이어가 a,d로 움직이면 이게 대신 움직이는식임
/// <summary>
/// 그러면 필드를 일괄적으로 움직이게해야하는데 이걸 자식으로 두고 자식둘을 움직이게 할것인가?
/// 아니면 개별적으로 움직이게될것인가? 두개가 있겠음
/// </summary>
public class FieldManager : MonoBehaviour {
    [SerializeField]//private지만 외부에선 보이게
                    //    private float pinX = 21f;//리셋될 X란뜻임ㅎ
    private GameObject adderField;
    [SerializeField]
    private Transform map;
    [SerializeField]
    public float distance;//진행한거리
    [SerializeField]
    private List<GameObject> buildPack = new List<GameObject>();//특정위치를 잡고 알아서 추출하고 별짓 하게해보려했으나 잘알지도못하고,그것을 했다가 위치또바꿔먹고 그러면 곤란할것으로 예상
    [SerializeField]
    private float increaseSupplyDistance = 50f;

    public SupplyItem box;
    private float nextSupplyDistance = 50f;
    
    private float period;//도착지점, 여기에 도착하면 맵이 생기는구조
    [SerializeField]
    Queue<GameObject> fields = new Queue<GameObject>(2);//추가될필드들 항상2~3개를유지< 2개는 버그위험때문? 
    //ㄴ 아직 모자란지 이거 사이즈를 초기화 못하겠음

   // [SerializeField]
   // private float speed = 5f;//지형의 이동, 플레이어가 실제로 움직이는게 아닌 지형이 이동해서 플레이어가 움직이는듯하게 보이게함
    //ㄴ 기본이동속도, 아마 안바뀔것

    //플레이어관련은 아마 싱글게임형태로 될테니 게임매니저가 갖는 형태로 해봄
    //ㄴ이단일듯ㅎ

       
	// Use this for initialization
    void AddMap(float setX = 0)
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        var temp = Instantiate(adderField, map);
        temp.transform.position = new Vector3(setX, -6f);
        if (fields.Count != 0)
        {
            int num = Random.Range(0, buildPack.Count);
            Instantiate(buildPack[num], temp.transform);
        }

        fields.Enqueue(temp);
    }
	void Start () {
       
		if ( fields.Count == 0)//간단하게 0번째만체크
        {
            AddMap();
            AddMap(adderField.transform.localScale.x);
        }
        period = adderField.transform.localScale.x -1;//혹시모를 공백방지용
	}
	
	// Update is called once per frame
	void Update () {
        //필드르 최적화(?)하기위해 지나간자리는 지움
        if (distance >= period -2)
        {
            Destroy(fields.Dequeue() ) ;//맨앞에꺼 지우기
            AddMap(distance + adderField.transform.localScale.x );
            
            period += adderField.transform.localScale.x;//x만큼 더늘림
        }
        if(distance > nextSupplyDistance)
        {//50배수마다 물건드랍
            nextSupplyDistance += increaseSupplyDistance;
            Instantiate(box, new Vector3(distance + 8f, 10f), Quaternion.identity);
        }
	}
    
}
