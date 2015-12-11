using UnityEngine;
using System.Collections;

public class TrapHit : MonoBehaviour {

	public int points;

	void Start () {
		print("start");
	}
	

	void Update () {
	
	}

	void OnCollisionEnter (Collision collision) {
			print("wehweh");
	}

	void SubtractPoints() {

	}
}
