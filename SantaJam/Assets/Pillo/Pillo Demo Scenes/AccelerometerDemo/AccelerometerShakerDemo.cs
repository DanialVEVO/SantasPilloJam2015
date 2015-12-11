using UnityEngine;
using System.Collections;

public class AccelerometerShakerDemo : MonoBehaviour {
	public float shakerAmount;
	public AudioClip shakerSound;
	float soundcooldown;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float additionRaw = PilloController.GetAccelero (Pillo.PilloID.Pillo1).magnitude - 1000; // we subtract 1000, since the magnitude will always be around 1000 when the pillo is not moving
		shakerAmount += additionRaw * 0.002f * Time.deltaTime; // use the raw addition to add up to the color lerp value
		shakerAmount -= Time.deltaTime * 0.2f; // make it decrease over time again
		shakerAmount = Mathf.Clamp (shakerAmount, 0.0f, 1.0f); // clamp the lerp value
		GetComponent<Renderer> ().material.color = Color.Lerp (Color.grey, Color.green, shakerAmount); //apply the color

	
		soundcooldown -= Time.deltaTime;
		if(Mathf.Abs(additionRaw) > 100)
		{
			if(soundcooldown <= 0.0f)
			{
				AudioSource.PlayClipAtPoint(shakerSound,transform.position,additionRaw*0.001f);
				soundcooldown = 0.1f;
			}
		}

		transform.position = PilloController.GetAccelero (Pillo.PilloID.Pillo1) * 0.0001f; // add some movement to the cube for effect
	}


}
