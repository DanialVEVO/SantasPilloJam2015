using UnityEngine;
using System.Collections;
using Pillo;

public class PlayerMovement : MonoBehaviour
{
	// Pillo Settings
	[SerializeField] [Range(0, 254)] int			pilloSensitivity		= 96;
	//[SerializeField] [Range(0, 120)] int			pilloCrashThreshold		= 40;
	//[SerializeField] bool							pilloCrashProtection	= true;

	// General Controls
	[SerializeField] [Range(0.0f, 0.2f)] float		btnHoldTimeThreshold1	= 0.025f;
	[SerializeField] [Range(0.0f, 0.2f)] float		btnHoldTimeThreshold2	= 0.075f;
	[SerializeField] [Range(0.0f, 0.2f)] float		btnHoldTimeThreshold3	= 0.100f;

	// Horizontal Movement
	[SerializeField] [Range(0.0f, 1.0f)] float		laneSwitchTime			= 0.3f;
	[SerializeField] float[]						lanePosX				= { -6.0f, -3.0f, 0.0f, 3.0f, 6.0f };

	// Vertical Movement
	[SerializeField] float							posYMin					= 0.0f;
	[SerializeField] float							posYMax					= 3.5f;

	[SerializeField] [Range(0.0f, 4.0f)] float		MaxJumpSpeed			= 0.50f;
	[SerializeField] [Range(0.0f, 4.0f)] float		MaxFallSpeed			= 0.50f;

	[SerializeField] [Range(0.0f, 0.5f)] float		jumpAccelerate			= 0.05f;
	[SerializeField] [Range(0.0f, 0.5f)] float		jumpGravity				= 0.05f;
	
	// Boost
	[SerializeField] bool							boostEnabled			= true;
	[SerializeField] [Range(0.0f, 5.0f)] float		boostTimeToGet			= 0.70f;
	[SerializeField] [Range(0.0f, 2.0f)] float		boostAccelerateTime		= 0.30f;
	[SerializeField] [Range(0.0f, 5.0f)] float		boostDurationTime		= 2.00f;
	[SerializeField] [Range(1.0f, 5.0f)] float		boostMultiplier			= 3.00f;

	[SerializeField] [Range(1.0f, 5.0f)] float		swingTime				= 2.00f;
	[SerializeField] [Range(0.0f, 5.0f)] float		swingHeight				= 1.00f;
	[SerializeField] [Range(0.0f, 0.3f)] float		swingRandomness			= 0.10f;
	[SerializeField] [Range(0.0f, 45.0f)] float		rotAngle				= 30.0f;

	LevelManager levelmanager;

	Vector3 pilloLeftAcceleroLastValue, pilloRightAcceleroLastValue;
	float pilloLeftLastValue, pilloRightLastValue;
	int pilloLeftCrashCounter, pilloRightCrashCounter; 

	float btnLeftTime, btnRightTime;
	bool canMoveToLeft, canMoveToRight;

	float jumpVelocity, movementVelocity, timeAccumulated;
	int laneNow, laneMovingTo;
	bool isJumping;

	bool isBooster;
	float boosterTime;
	float rotGoal, swingTimeAccumulated;

	float deltaTime;
	float swingDeltaY;

	//Transform camAccumulatedChanges;


	// Use this for initialization
	void Start()
	{
		// Horizontal Movement
		timeAccumulated = 0.0f;
		laneNow = (lanePosX.GetLength(0) - 1) / 2;
		laneMovingTo = laneNow;

		canMoveToLeft = false;
		canMoveToRight = false;

		pilloLeftAcceleroLastValue = new Vector3();
		pilloRightAcceleroLastValue = new Vector3();
		pilloLeftLastValue = 0.0f;
		pilloRightLastValue = 0.0f;
		pilloLeftCrashCounter = 0;
		pilloRightCrashCounter = 0;
	
		// Vertical Movement
		jumpVelocity = 0.0f;
		isJumping = false;

		boosterTime = 0.0f;
		isBooster = false;

		deltaTime = Time.deltaTime;
		swingTimeAccumulated = 0.0f;
		timeAccumulated = 0.0f;
		rotGoal = 0.0f;
		swingDeltaY = 0.0f;

		levelmanager = GameObject.Find("LevelManagerObject").GetComponent<LevelManager>();

		PilloController.ConfigureSensorRange(pilloSensitivity, pilloSensitivity + 1);
	}


	// Update is called once per frame
	void Update()
	{
		deltaTime = Mathf.Clamp(Time.deltaTime, 0.01f, 0.1f);

		ReadButtons();
		MovementProcess();
		BoosterProcess();
		RotationUpdate();
	}


	void RotationUpdate()
	{
		Vector3 rot = transform.rotation.eulerAngles;
		rot.z = rotGoal;
		transform.rotation = Quaternion.Euler(rot);

	}
	
		
	void MovementProcess()
	{
		Vector3 pos = transform.position;

		pos.y -= swingDeltaY;
		
		swingTimeAccumulated = (((swingTimeAccumulated + deltaTime) ) % swingTime);
		swingDeltaY = swingHeight * Mathf.Sin(Mathf.PI * 2.0f * swingTimeAccumulated / swingTime);
		
		// Jumping
		if (isJumping)
			jumpVelocity += jumpAccelerate;
		else
			jumpVelocity -= jumpGravity;

		jumpVelocity = Mathf.Clamp(jumpVelocity, -MaxFallSpeed, MaxJumpSpeed);
		laneMovingTo = Mathf.Clamp(laneMovingTo, 0, lanePosX.GetLength(0) - 1);

		// Apply Vertical Movement
		pos.y = Mathf.Clamp(pos.y + jumpVelocity, posYMin, posYMax);

		// Stand on Floor
		if (pos.y == posYMin && jumpVelocity < 0.0f)
			jumpVelocity = 0.0f;

		
		// Horizontal Movement
		if (laneNow != laneMovingTo)
		{
			timeAccumulated = Mathf.Clamp(timeAccumulated + deltaTime / laneSwitchTime, 0.0f, 1.0f);

			if (timeAccumulated > 0.7)
				rotGoal = (laneNow - laneMovingTo) * rotAngle * Mathf.Clamp(Mathf.Lerp(1.0f, 0.0f, (timeAccumulated - 0.7f) / 0.3f), 0.0f, 1.0f);
			else
				rotGoal = (laneNow - laneMovingTo) * rotAngle;
					


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

		pos.y += swingDeltaY;

		transform.position = pos;
	}


	void BoosterProcess()
	{
		// Stand on Ceiling: booster
		if (isBooster || transform.position.y >= posYMax - swingHeight)
			boosterTime += deltaTime;
		else if (!isBooster)
			boosterTime = 0;
		
		boosterTime = boosterTime % (boostTimeToGet + boostDurationTime);

		// If booster accelerating
		if (boosterTime >= boostTimeToGet && boosterTime < boostTimeToGet + boostAccelerateTime)
		{
			isBooster = true;
			levelmanager.setLevelSpeedMultiplier(Mathf.Lerp(1, boostMultiplier, (boosterTime - boostTimeToGet) / boostAccelerateTime));
		}
		// If booster decelerating
		else if (boosterTime >= boostTimeToGet + boostAccelerateTime && boosterTime < boostTimeToGet + boostDurationTime)
		{
			levelmanager.setLevelSpeedMultiplier(Mathf.Lerp(boostMultiplier, 1, (boosterTime - boostTimeToGet - boostAccelerateTime) / (boostDurationTime - boostAccelerateTime)));
		}
		else
		{
			levelmanager.setLevelSpeedMultiplier(1);
			isBooster = false;
		}
	}


	void ReadButtons()
	{
		// Keyboard Buttons checking
		float pilloLeft = PilloController.GetSensor(PilloID.Pillo1, false);
		float pilloRight = PilloController.GetSensor(PilloID.Pillo2, false);
		Vector3 pilloLeftAccelero = PilloController.GetAccelero(PilloID.Pillo1);
		Vector3 pilloRightAccelero = PilloController.GetAccelero(PilloID.Pillo1);

		/*print("( " + ((pilloLeft == pilloLeftLastValue) ? (1) : (0)) + ", " + ((pilloLeft < 0.9f) ? (1) : (0)) + ", " + ((pilloLeftAccelero == pilloLeftAcceleroLastValue) ? (1) : (0)) + ") "
				+ pilloLeft + ", " + pilloLeftCrashCounter + ", " + pilloLeftAccelero);

		if (pilloCrashProtection)
		{
			// Pillo Left Crash: if same value for x frames, assume it's 0
			if (pilloLeft == pilloLeftLastValue && pilloLeft < 0.9f * && (Vector3.Distance(pilloLeftAccelero, pilloLeftAcceleroLastValue) > 0)) // Mathf.Abs(pilloLeft - pilloLeftLastValue) < 0.1f
			{
				//if (++pilloLeftCrashCounter > pilloCrashThreshold)
				//{
					pilloLeft = 0.0f;
					pilloLeftLastValue = pilloLeft;
					pilloLeftAcceleroLastValue = pilloLeftAccelero;
					pilloLeftCrashCounter = pilloCrashThreshold;
				//}
			}
			else
			{
				pilloLeftCrashCounter = 0;
				pilloLeftLastValue = pilloLeft;
				pilloLeftAcceleroLastValue = pilloLeftAccelero;
			}

			// Pillo Right Crash: if same value for x frames, assume it's 0
			if (pilloRight == pilloRightLastValue && pilloRight < 0.9f && (Vector3.Distance(pilloRightAccelero, pilloRightAcceleroLastValue) > 0))
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
				pilloRightAcceleroLastValue = pilloRightAccelero;
			}
		}*/

		//print("pilloLeft: " + pilloLeft + " pilloRight: " + pilloRight);

		if (Input.GetKey(KeyCode.LeftArrow) || pilloLeft > 0.9f)
			btnLeftTime += deltaTime;
		else
			btnLeftTime -= deltaTime * 2;

		if (Input.GetKey(KeyCode.RightArrow) || pilloRight > 0.9f)
			btnRightTime += deltaTime;
		else
			btnRightTime -= deltaTime * 2;

		btnLeftTime = Mathf.Clamp(btnLeftTime, 0, btnHoldTimeThreshold3);
		btnRightTime = Mathf.Clamp(btnRightTime, 0, btnHoldTimeThreshold3);

		if (btnLeftTime > btnHoldTimeThreshold2 && btnRightTime > btnHoldTimeThreshold2)
			isJumping = true;

		if (btnLeftTime < btnHoldTimeThreshold1 || btnRightTime < btnHoldTimeThreshold1)
			isJumping = false;


		if (canMoveToLeft && btnLeftTime > btnHoldTimeThreshold2 && btnRightTime < btnHoldTimeThreshold1)
		{
			canMoveToLeft = false;
			canMoveToRight = false;
			laneMovingTo--;
			timeAccumulated = 0.0f;
		}

		if (canMoveToRight && btnRightTime > btnHoldTimeThreshold2 && btnLeftTime < btnHoldTimeThreshold1)
		{
			canMoveToLeft = false;
			canMoveToRight = false;
			laneMovingTo++;
			timeAccumulated = 0.0f;
		}

		if (canMoveToLeft && canMoveToRight)
			rotGoal = rotAngle * Mathf.Clamp((btnLeftTime - btnRightTime) / btnHoldTimeThreshold3, -1.0f, 1.0f);


		if (btnLeftTime < btnHoldTimeThreshold1 && laneNow == laneMovingTo)
		{
			canMoveToLeft = true;
			//canMoveToRight = true;
		}

		if (btnRightTime < btnHoldTimeThreshold1 && laneNow == laneMovingTo)
		{
			//canMoveToLeft = true;
			canMoveToRight = true;
		}
	}


    void OnDrawGizmos()
    {
        for (int i = 0; i < lanePosX.Length; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector3(lanePosX[i], posYMin, transform.position.z), new Vector3(lanePosX[i], posYMin, transform.position.z + 300));
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(lanePosX[i], posYMax, transform.position.z), new Vector3(lanePosX[i], posYMax, transform.position.z + 300));
        }
    }
}