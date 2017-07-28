using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {
    float dropHeight = 5f;
    float xMin = -25f;
    float xMax = 0f;

    public Enemy enemy;
    void SpawnEnemy()
    {
        dropHeight += 1f;
        Vector3 pos = new Vector3(Random.Range(xMin+ transform.position.x, xMax + transform.position.x), dropHeight,0f);
        var temp = Instantiate(enemy);
        temp.transform.position = new Vector3(pos.x,pos.y,0f);

        temp.transform.localScale = enemy.transform.lossyScale;
    }
	// Use this for initialization
	void Start () {
        Random.InitState( (int)System.DateTime.Now.Ticks );
        if (GameManager.inst.GetComponent<FieldManager>().distance > 15f)
            for(int i = 0; i< GameManager.inst.GetComponent<FieldManager>().distance/15; i++)
              SpawnEnemy();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
