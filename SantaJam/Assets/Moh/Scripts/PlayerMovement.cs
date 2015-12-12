using UnityEngine;
using System.Collections;
using Pillo;

public class PlayerMovement : MonoBehaviour
{
	// Pillo Settings
	[SerializeField] float		pilloThreshold			= 0.20f;
	[SerializeField] int		pilloCrashThreshold		= 40;
	[SerializeField] bool		pilloCrashProtection	= true;

	// General Controls
	[SerializeField] float		btnHoldTimeThreshold1	= 0.03f;
	[SerializeField] float		btnHoldTimeThreshold2	= 0.07f;
	[SerializeField] float		btnHoldTimeThreshold3	= 0.10f;

	// Horizontal Movement
	[SerializeField] float[]	lanePosX				= { -5.0f, 0.0f, 5.0f };
	[SerializeField] float		laneSwitchTime			= 0.3f;

	// Vertical Movement
	[SerializeField] float		posYMin					= 0.0f;
	[SerializeField] float		posYMax					= 4.0f;

	[SerializeField] float		MaxJumpSpeed			= 1.00f;
	[SerializeField] float		MaxFallSpeed			= 1.00f;

	[SerializeField] float		jumpAccelerate			= 0.10f;
	[SerializeField] float		jumpGravity				= 0.20f;
	
	// Boost
	[SerializeField] bool		boostEnabled			= true;
	[SerializeField] float		boostTimeToGet			= 1.00f;
	[SerializeField] float		boostDurationTime		= 3.00f;
	[SerializeField] float		boostMultiplier			= 2.00f;


	LevelManager levelmanager;

	Vector3 pilloLeftAcceleroLastValue, pilloRightAcceleroLastValue;
	float pilloLeftLastValue, pilloRightLastValue;
	int pilloLeftCrashCounter, pilloRightCrashCounter; 

	float btnLeftTime, btnRightTime;
	bool canMoveToLeft, canMoveToRight;

	float jumpVelocity, movementVelocity, timeAccumulated;
	int laneNow, laneMovingTo;
	bool isJumping;

	float boosterTime;

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

		levelmanager = GameObject.Find("LevelManagerObject").GetComponent<LevelManager>();

		PilloController.ConfigureSensorRange((int)(pilloThreshold * 0xFF), 0xFF);
	}

	void FixedUpdate()
	{

	}

	// Update is called once per frame
	void Update()
	{
		// Read current position & time
		Vector3 pos = transform.position;
		float deltaTime = Time.deltaTime;

		// Keyboard Buttons checking
		float pilloLeft = PilloController.GetSensor(PilloID.Pillo1, false);
		float pilloRight = PilloController.GetSensor(PilloID.Pillo2, false);
		Vector3 pilloLeftAccelero = PilloController.GetAccelero(PilloID.Pillo1);
		Vector3 pilloRightAccelero = PilloController.GetAccelero(PilloID.Pillo1);

		if (pilloCrashProtection)
		{
			// Pillo Left Crash: if same value for x frames, assume it's 0
			if (pilloLeft == pilloLeftLastValue && pilloLeft < 0.8f && pilloLeftAccelero == pilloLeftAcceleroLastValue) // Mathf.Abs(pilloLeft - pilloLeftLastValue) < 0.1f
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
			if (pilloRight == pilloRightLastValue && pilloRight < 0.9f && pilloRightAccelero == pilloRightAcceleroLastValue)
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
		}

		//print("pilloLeft: " + pilloLeft + " pilloRight: " + pilloRight);

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
			isJumping = true;
		if (btnLeftTime < btnHoldTimeThreshold1 || btnRightTime < btnHoldTimeThreshold1)
			isJumping = false;


		if (canMoveToLeft && btnLeftTime > btnHoldTimeThreshold2 && btnRightTime < btnHoldTimeThreshold1)
		{
			canMoveToLeft = false;
			laneMovingTo--;
			timeAccumulated = 0.0f;
		}

		if (canMoveToRight && btnRightTime > btnHoldTimeThreshold2 && btnLeftTime < btnHoldTimeThreshold1)
		{
			canMoveToRight = false;
			laneMovingTo++;
			timeAccumulated = 0.0f;
		}

		if (btnLeftTime < btnHoldTimeThreshold1)
			canMoveToLeft = true;
			
		if (btnRightTime < btnHoldTimeThreshold1)
			canMoveToRight = true;
		
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
		{
			jumpVelocity = 0.0f;
		}
		
		// Stand on Ceiling
		if (pos.y == posYMax)
			boosterTime += deltaTime;
		else
			boosterTime = 0;

		boosterTime = boosterTime % (boostTimeToGet + boostDurationTime);
		
		//Camera.main.fieldOfView

		if (boosterTime >= boostTimeToGet && boosterTime < boostTimeToGet + boostDurationTime)
			levelmanager.setLevelSpeedMultiplier(boostMultiplier - (boostMultiplier - 1) * (boosterTime - boostTimeToGet) / boostDurationTime);
		else
			levelmanager.setLevelSpeedMultiplier(1);
		
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
