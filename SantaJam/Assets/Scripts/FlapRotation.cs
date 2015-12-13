using UnityEngine;
using System.Collections;

public class FlapRotation : MonoBehaviour {

	Vector3	rotateAmount = new Vector3(-2,0,0);
	Transform rightFlap;
	Transform leftFlap;

	void Start () {
        rightFlap = transform.FindChild("RightFlapControl");
        leftFlap = transform.FindChild("LeftFlapControl");

        Debug.Log(leftFlap.name + " " + rightFlap.name);
    }

	void Update () {

		FlapRotate ();
	}

	void FlapRotate () {		
		if(Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow) && (leftFlap.localRotation.eulerAngles.x < 30 || leftFlap.localRotation.eulerAngles.x > 330)){
			leftFlap.Rotate(-rotateAmount);
			rightFlap.Rotate(-rotateAmount);
		}
		else if(Input.GetKey(KeyCode.LeftArrow)){
			if(leftFlap.localRotation.eulerAngles.x < 30 || leftFlap.localRotation.eulerAngles.x > 330){
					leftFlap.Rotate(-rotateAmount);
					rightFlap.Rotate(rotateAmount);
			}
		} 
		else if(Input.GetKey(KeyCode.RightArrow)){
			if(rightFlap.localRotation.eulerAngles.x < 30 || rightFlap.localRotation.eulerAngles.x > 330){
				leftFlap.Rotate(rotateAmount);
				rightFlap.Rotate(-rotateAmount);
			}
		}
		else{
			leftFlap.localRotation = Quaternion.Slerp(leftFlap.localRotation, Quaternion.Euler(Vector3.zero), 5f*Time.deltaTime);
			rightFlap.localRotation = Quaternion.Slerp(rightFlap.localRotation, Quaternion.Euler(Vector3.zero), 5f*Time.deltaTime);
			
		}	
	}
}
