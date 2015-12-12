using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

	// Horizontal Movement
	[SerializeField] bool		laneShow = true;
	[SerializeField] float[]	lanePosX = { -5.0f, 0.0f, 5.0f };
	[SerializeField] float		laneSwitchTime = 1.0f;

	// Vertical Movement
	[SerializeField] float posYMin		= 0.0f;
	[SerializeField] float posYMax		= 4.0f;

	[SerializeField] float MaxJumpSpeed		= 1.00f;
	[SerializeField] float MaxFallSpeed		= 1.00f;

	[SerializeField] float jumpAccelerate	= 0.10f;
	[SerializeField] float jumpGravity		= 0.10f;

	float velocity;

	float timeAccumulated;
	int laneNow, laneMovingTo;

	// Use this for initialization
	void Start()
	{
		// Horizontal Movement
		timeAccumulated = 0.0f;
		laneNow = (lanePosX.GetLength(0) - 1) / 2;
		laneMovingTo = laneNow;

		// Vertical Movement
		velocity = 0.0f;
	}

	void FixedUpdate()
	{

	}

	// Update is called once per frame
	void Update()
	{

		// Vertical Movement
		Vector3 pos = transform.position;

		if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))
		{
			velocity += jumpAccelerate;
		}
		else
		{
			velocity -= jumpGravity;
		}

		velocity = Mathf.Clamp(velocity, -MaxFallSpeed, MaxJumpSpeed);

		pos.y = Mathf.Clamp(pos.y + velocity, posYMin, posYMax);

		if (pos.y == posYMin)
		{
			velocity = 0.0f;
		}

		print("velocity " + velocity);

		transform.position = pos;





		// Horizontal Movement
		if (laneNow == laneMovingTo)
		{
			if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
			{
				laneMovingTo--;
				timeAccumulated = 0.0f;
			}

			else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
			{
				laneMovingTo++;
				timeAccumulated = 0.0f;
			}

			laneMovingTo = Mathf.Clamp(laneMovingTo, 0, lanePosX.GetLength(0) - 1);
		}

		if (laneNow != laneMovingTo)
		{
			timeAccumulated = Mathf.Clamp(timeAccumulated + Time.deltaTime / laneSwitchTime, 0, 1);

			Vector3 pos = transform.position;
			float distanceDiv2 = (lanePosX[laneMovingTo] - lanePosX[laneNow]) / 2.0f;

			pos.x = Mathf.Clamp(lanePosX[laneNow] + distanceDiv2 + distanceDiv2 * Mathf.Sin(Mathf.PI * timeAccumulated - (Mathf.PI * 0.5f)),
								 Mathf.Min(lanePosX[laneNow], lanePosX[laneMovingTo]),
								 Mathf.Max(lanePosX[laneNow], lanePosX[laneMovingTo]));

			if (Mathf.Abs(pos.x - lanePosX[laneMovingTo]) < 0.001f)
			{
				laneNow = laneMovingTo;
				pos.x = lanePosX[laneMovingTo];
			}

			transform.position = pos;
		}
	}
}
