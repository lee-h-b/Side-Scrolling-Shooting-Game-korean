using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    Vector3 nextPosition;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        nextPosition = new Vector3(GameManager.inst.dist, 4, -10f);

        transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * 10f);
	}
}
