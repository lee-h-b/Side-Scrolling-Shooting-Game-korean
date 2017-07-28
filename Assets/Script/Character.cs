using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IActor {
    public int hp;
    public int maxHp;
    public int barrier;//배리어의 최대값은 maxHP값
    protected bool live = true;
    public event System.Action Death;
    // Use this for initialization
    public void TakeDamage(int damage)
    {
        if (barrier > 0)
            barrier -= damage;
        else
        hp -= damage;
        if (hp <= 0 && live == true) Die();        
    }
    protected virtual void Die()
    {
        live = false;
        if(Death != null)//델리게이트에 뭔가 값이 있다면
        {
            Death();
        }
        Destroy(gameObject);
    }
	protected void Start () {
        hp = maxHp;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
