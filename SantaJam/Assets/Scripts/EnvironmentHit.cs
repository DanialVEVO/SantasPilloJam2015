using UnityEngine;
using System.Collections;

public class EnvironmentHit : MonoBehaviour {

	public int points;

	void Start () {

	}
	

	void Update () {
	
	}

	void OnTriggerEnter (Collider player) {
		player.GetComponent<Score>().CalcScore(points);
	}

}
