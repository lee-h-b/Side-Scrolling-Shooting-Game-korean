using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyAndDistance : MonoBehaviour {
    public Text distance;
    public Text money;
	// Use this for initialization
	void Start () {
        distance = transform.Find("Distance").GetComponent<Text>();
        money = transform.Find("Money").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {

        distance.text = (int)GameManager.inst.dist + "M";
        money.text = GameManager.inst.money + "원";
	}
}
