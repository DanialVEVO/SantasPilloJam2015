using UnityEngine;
using System.Collections;

public class ParticleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ParticleDestroy();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ParticleDestroy(){
		Destroy(gameObject, GetComponent<ParticleSystem>().duration);
	}

}
