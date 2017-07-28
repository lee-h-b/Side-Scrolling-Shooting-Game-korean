using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Gun {
    //우선 발사구현부터, 머즐은 총3개임
    //1번째는 0~20도,2번째는~20~20도, 3번째는 -20도 이렇게 각 3개씩 총9개를 랜덤배치할것
    //이걸 코루틴으로는 할필요 없을거같음 걍 케이스 돌리거나 그럴것
    // Use this for initialization
    public int count;
    public int maxBullet;
    [SerializeField]
    private float minAngle = -20f;
    [SerializeField]
    private float maxAngle = 20f;
    private float tum;
    //샷건은 머신건 같은 애들과 다르게 총알이 확산함
    public override void Shoot()
    {
        Vector3 newrot = muzzle.rotation.eulerAngles;
        float addAngle = minAngle;
        if (Time.time > nextShootTime)
        {
            if(transform.parent.tag == "Player")
            {
                CunsumeBullet();
            }
            for(int i = 0; i< maxBullet; i++)
            {
                int muzzlePos = i / 3;// i/ 3번째 int형 자리에 총알 생성

                Projectile temp = Instantiate(projectile, muzzle.GetChild(muzzlePos).position, muzzle.rotation) as Projectile;
                temp.transform.eulerAngles = new Vector3(temp.transform.eulerAngles.x, temp.transform.eulerAngles.y, temp.transform.eulerAngles.z + addAngle);
                temp.tag = transform.parent.tag;//태그 달아서 팀킬방지
                temp.SetSpeed(muzzleVelocity);
                addAngle += tum;
                if (addAngle > maxAngle) addAngle = maxAngle;
            }
        }

    }
    void Start () {
        //최소 -20 최대 20 총 40도
        count = transform.Find("Muzzles").childCount;

        maxBullet = count * 3;// 1머즐당 3알씩나오도록함
        tum = (Mathf.Abs(minAngle) + Mathf.Abs(maxAngle)) / maxBullet;

    }

    // Update is called once per frame
    void Update () {
		
	}
}
