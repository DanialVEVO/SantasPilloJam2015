using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float laneSwitchTime = 1.0f;
	[SerializeField] float[] lanePosX = { -5.0f, 0.0f, 5.0f };

	float timeAccumulated;
	int laneNow, laneMovingTo;

	// Use this for initialization
	void Start()
	{
		timeAccumulated = 0.0f;
		laneNow = (lanePosX.GetLength(0) - 1) / 2;
		laneMovingTo = laneNow;
	}

	void FixedUpdate()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (laneNow == laneMovingTo)
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow))
			{
				laneMovingTo--;
				timeAccumulated = 0.0f;
			}

			else if (Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow))
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
