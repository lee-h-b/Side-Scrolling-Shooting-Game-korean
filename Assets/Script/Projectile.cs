using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    //이것또한 총의 타입별로 다른 프로젝타일이 날라가야하니 건과 유사하게될듯
    [SerializeField]
    protected int damage = 5;
    protected float speed = 9.5f;
    [SerializeField]
    protected float lifeTime = 1f;//총알수명
    

    [SerializeField]
    private LayerMask collisionMask;
    public float LifeTime
    {
        get { return lifeTime; }
    }
	// Use this for initialization
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    protected void CheckCollisions(float distance)
    {
        Ray ray = new Ray(transform.position, transform.right);
        //안될지도모름 3d에선 코앞이라 포워드인데 이건 오른쪽이라 라이트로 했거든

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, distance,collisionMask, QueryTriggerInteraction.Collide))
        {
            Hit(hit);
        }
    }
    protected virtual void Hit(RaycastHit hit)
    {
        IActor hitObject = hit.collider.GetComponent<IActor>();
        if(hitObject != null && hit.transform.tag != tag)
        {
            hitObject.TakeDamage(damage);

            GameObject.Destroy(gameObject);//만약 데미지를 입힌걸 표현한다면 어쩔까?
            //얘가 파티클호출할껏
        }
        else if(hit.transform.tag == "Breakable")
        {
            Destroy(hit.transform.gameObject);
            Destroy(gameObject);
        }
    }
	void Start () {
        Destroy(gameObject, lifeTime);//시한부

        Collider[] OvelapCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if(OvelapCollisions.Length > 0)
        {
            //자신태그와 똑같은 애면 리턴
            if (OvelapCollisions[0].tag == tag) return;

            else if (OvelapCollisions[0].GetComponent<Character>() != null)
                OvelapCollisions[0].GetComponent<Character>().TakeDamage(damage);
            else
                Destroy(OvelapCollisions[0].gameObject);
            Destroy(gameObject);//자기자신을 파괴
        }
	}

    // Update is called once per frame
    protected virtual void Update () {
        float distance = speed * Time.deltaTime;
        //물체가 갔을곳을 미리 충돌체크? 하는느낌
        CheckCollisions(distance);//모든 총기는 이걸함
        transform.Translate(Vector3.right * distance);//이름이 이상하지만 스피드* 델타타임이니 ㄱㅊ

            
	}

}
