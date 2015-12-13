using UnityEngine;
using System.Collections;

public class ParticleScript : MonoBehaviour {

	void Start () {
		ParticleDestroy();
	}

	void Update () {
	
	}

	void ParticleDestroy(){
		Destroy(gameObject, GetComponent<ParticleSystem>().duration);
	}

}
