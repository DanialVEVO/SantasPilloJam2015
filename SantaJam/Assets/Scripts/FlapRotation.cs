using UnityEngine;
using System.Collections;

public class FlapRotation : MonoBehaviour {

	Vector3	rotateAmount = new Vector3(2,0,0);
	public Transform rightFlap;
	public Transform leftFlap;

	void Start () {
	
	}

	void Update () {
		FlapRotate ();
	}

	void FlapRotate () {		
		if(Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow) && (leftFlap.rotation.eulerAngles.x < 30 || leftFlap.rotation.eulerAngles.x > 330)){
			leftFlap.Rotate(rotateAmount);
			rightFlap.Rotate(rotateAmount);
		}
		else if(Input.GetKey(KeyCode.LeftArrow)){
			if(leftFlap.rotation.eulerAngles.x < 30 || leftFlap.rotation.eulerAngles.x > 330){
					leftFlap.Rotate(-rotateAmount);
					rightFlap.Rotate(rotateAmount);
			}
		} 
		else if(Input.GetKey(KeyCode.RightArrow)){
			if(rightFlap.rotation.eulerAngles.x < 30 || rightFlap.rotation.eulerAngles.x > 330){
				leftFlap.Rotate(rotateAmount);
				rightFlap.Rotate(-rotateAmount);
			}
		}
		else{
			leftFlap.rotation = Quaternion.Slerp(leftFlap.rotation, Quaternion.Euler(Vector3.zero), 5f*Time.deltaTime);
			rightFlap.rotation = Quaternion.Slerp(rightFlap.rotation, Quaternion.Euler(Vector3.zero), 5f*Time.deltaTime);
			
		}	
	}
}
