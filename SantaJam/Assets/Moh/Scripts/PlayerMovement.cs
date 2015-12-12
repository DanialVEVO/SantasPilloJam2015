using UnityEngine;
using System.Collections;
using Pillo;

public class PlayerMovement : MonoBehaviour
{
	// Horizontal Movement
	[SerializeField] bool		laneShow				= true;
	[SerializeField] float[]	lanePosX				= { -5.0f, 0.0f, 5.0f };
	[SerializeField] float		laneSwitchTime			= 0.3f;

	// Vertical Movement
	[SerializeField] float		posYMin					= 0.0f;
	[SerializeField] float		posYMax					= 4.0f;

	[SerializeField] float		MaxJumpSpeed			= 1.00f;
	[SerializeField] float		MaxFallSpeed			= 1.00f;

	[SerializeField] float		jumpAccelerate			= 0.10f;
	[SerializeField] float		jumpGravity				= 0.20f;
	
	[SerializeField] float		pilloThreshold			= 0.60f;
	[SerializeField] int		pilloCrashThreshold		= 90;

	[SerializeField] float		btnHoldTimeThreshold1	= 0.02f;
	[SerializeField] float		btnHoldTimeThreshold2	= 0.10f;
	[SerializeField] float		btnHoldTimeThreshold3	= 0.12f;
	
	float pilloLeftLastValue, pilloRightLastValue;
	int pilloLeftCrashCounter, pilloRightCrashCounter; 

	float btnLeftTime, btnRightTime;
	float jumpVelocity, movementVelocity, timeAccumulated;
	int laneNow, laneMovingTo;
	bool jumping;

	// Use this for initialization
	void Start()
	{
		// Horizontal Movement
		timeAccumulated = 0.0f;
		laneNow = (lanePosX.GetLength(0) - 1) / 2;
		laneMovingTo = laneNow;
		
		pilloLeftLastValue = 0.0f;
		pilloRightLastValue = 0.0f;
		pilloLeftCrashCounter = 0;
		pilloRightCrashCounter = 0;
	
		// Vertical Movement
		jumpVelocity = 0.0f;
		jumping = false;

		PilloController.ConfigureSensorRange((int)(pilloThreshold * 0xFF), 0xFF);
	}

	void FixedUpdate()
	{

	}

	// Update is called once per frame
	void Update()
	{
		Vector3 pos = transform.position;

		float deltaTime = Time.deltaTime;

		// Keyboard Buttons checking
		float pilloLeft = PilloController.GetSensor(PilloID.Pillo1, false);
		float pilloRight = PilloController.GetSensor(PilloID.Pillo2, false);


		// Pillo Left Crash: if same value for x frames, assume it's 0
		if (pilloLeft == pilloLeftLastValue)
		{
			if (++pilloLeftCrashCounter > pilloCrashThreshold)
			{
				pilloLeft = 0.0f;
				pilloLeftCrashCounter = pilloCrashThreshold;
			}
		}
		else
		{
			pilloLeftCrashCounter = 0;
			pilloLeftLastValue = pilloLeft;
		}

		// Pillo Right Crash: if same value for x frames, assume it's 0
		if (pilloRight == pilloRightLastValue)
		{
			if (++pilloRightCrashCounter > pilloCrashThreshold)
			{
				pilloRight = 0.0f;
				pilloRightCrashCounter = pilloCrashThreshold;
			}
		}
		else
		{
			pilloRightCrashCounter = 0;
			pilloRightLastValue = pilloRight;
		}

		print("pilloLeft: " + pilloLeft + " pilloRight: " + pilloRight);

		if (Input.GetKey(KeyCode.LeftArrow) || pilloLeft > 0.01f)
			btnLeftTime += deltaTime;
		else
			btnLeftTime -= deltaTime * 2;

		if (Input.GetKey(KeyCode.RightArrow) || pilloRight > 0.01f)
			btnRightTime += deltaTime;
		else
			btnRightTime -= deltaTime * 2;

		btnLeftTime = Mathf.Clamp(btnLeftTime, 0, btnHoldTimeThreshold3);
		btnRightTime = Mathf.Clamp(btnRightTime, 0, btnHoldTimeThreshold3);

		if (btnLeftTime > btnHoldTimeThreshold2 && btnRightTime > btnHoldTimeThreshold2)
			jumping = true;
		if (btnLeftTime < btnHoldTimeThreshold1 || btnRightTime < btnHoldTimeThreshold1)
			jumping = false;

		// if player is not moving horizontally
		if (laneNow == laneMovingTo) 
		{
			if (btnLeftTime > btnHoldTimeThreshold2 && btnRightTime < btnHoldTimeThreshold1)
			{
				laneMovingTo--;
				timeAccumulated = 0.0f;
			}

			else if (btnRightTime > btnHoldTimeThreshold2 && btnLeftTime < btnHoldTimeThreshold1)
			{
				laneMovingTo++;
				timeAccumulated = 0.0f;
			}
		}
		
		if (jumping)
			jumpVelocity += jumpAccelerate;
		else
			jumpVelocity -= jumpGravity;

		jumpVelocity = Mathf.Clamp(jumpVelocity, -MaxFallSpeed, MaxJumpSpeed);
		laneMovingTo = Mathf.Clamp(laneMovingTo, 0, lanePosX.GetLength(0) - 1);

		// Apply Vertical Movement
		pos.y = Mathf.Clamp(pos.y + jumpVelocity, posYMin, posYMax);
		
		// Stand on Floor
		if (pos.y == posYMin && jumpVelocity < 0.0f)
		{
			jumpVelocity = 0.0f;
		}

		// Horizontal Movement
		if (laneNow != laneMovingTo)
		{
			timeAccumulated = Mathf.Clamp(timeAccumulated + deltaTime / laneSwitchTime, 0, 1);

			float distanceDiv2 = (lanePosX[laneMovingTo] - lanePosX[laneNow]) / 2.0f;

			pos.x = Mathf.Clamp(lanePosX[laneNow] + distanceDiv2 + distanceDiv2 * Mathf.Sin(Mathf.PI * timeAccumulated - (Mathf.PI * 0.5f)),
								 Mathf.Min(lanePosX[laneNow], lanePosX[laneMovingTo]),
								 Mathf.Max(lanePosX[laneNow], lanePosX[laneMovingTo]));

			if (Mathf.Abs(pos.x - lanePosX[laneMovingTo]) < 0.001f)
			{
				laneNow = laneMovingTo;
				pos.x = lanePosX[laneMovingTo];
			}
		}

		transform.position = pos;
	}
}
