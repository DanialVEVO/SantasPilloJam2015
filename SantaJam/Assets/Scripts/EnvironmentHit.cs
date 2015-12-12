using UnityEngine;
using System.Collections;

public class EnvironmentHit : MonoBehaviour {

	public int points;
	public float rotateSpeed=25f;

	void Start () {

	}
	

	void Update () {
		ObjRotate();
	}

	void OnTriggerEnter (Collider player) {
		player.GetComponent<Score>().CalcScore(points);
	}

	void ObjRotate (){
		if(points > 0){
			transform.RotateAround(transform.position, transform.up, rotateSpeed * Time.deltaTime);
		}	
	}
}
