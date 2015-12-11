using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuickCalibration : MonoBehaviour {

	public float timer;
	public QuickCalibrationState m_state;
	int selectedPillo;

	public Text infoText;

	public enum QuickCalibrationState
	{
		WaitingForSelection,
		CountdownToCalibration,
		CalibratingMaximum,
		CalibratingMinimum,
		CalibrationComplete,
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		switch(m_state)
		{
		case QuickCalibrationState.WaitingForSelection:
			CheckForSelection();
			break;
		case QuickCalibrationState.CountdownToCalibration:
			DoCountdown();
			break;
		case QuickCalibrationState.CalibratingMaximum:
			DoMaximumCalibration();
			break;
		case QuickCalibrationState.CalibratingMinimum:
			DoMinimumCalibration();
			break;
		case QuickCalibrationState.CalibrationComplete:
			DoCalibrationComplete();
			break;
		}
	}

	void CheckForSelection()
	{
		if(PilloController.GetSensor(Pillo.PilloID.Pillo1,false) > PilloController.GetCalibratedMinimum(Pillo.PilloID.Pillo1))
		{
			selectedPillo = 0;
			timer += Time.deltaTime;
		}
		else
			if(PilloController.GetSensor(Pillo.PilloID.Pillo2,false) > PilloController.GetCalibratedMinimum(Pillo.PilloID.Pillo2))
		{
			selectedPillo = 1;
			timer += Time.deltaTime;
		}
		else
		{
			timer = 0.0f;
		}
		if(timer >= 2.0f)
		{
			StartCountdown();
		}
	}

	void StartCountdown()
	{
		m_state = QuickCalibrationState.CountdownToCalibration;
		timer = 0.0f;
	}

	void DoCountdown()
	{
		timer += Time.deltaTime;
		infoText.text = "Starting calibration for Pillo " + (selectedPillo + 1).ToString () + " in " + Mathf.CeilToInt (3.0f-timer).ToString () + "\n Hold Pillo " +(selectedPillo + 1).ToString () + " as tight as possible!" ;
		if (timer > 3.0f)
			SwitchToMaximumCalibration ();
	}

	void SwitchToMaximumCalibration()
	{
		m_state = QuickCalibrationState.CalibratingMaximum;
		timer = 0.0f;
		infoText.text = "Keep holding Pillo " + (selectedPillo + 1).ToString () + " as tight as possible!";
	}

	void DoMaximumCalibration()
	{
		timer += Time.deltaTime;
		if (timer > 2.0f)
		{
			PilloController.SetCalibratedMaximum (PilloController.GetSensor ((Pillo.PilloID)selectedPillo, false), (Pillo.PilloID)selectedPillo);
			SwitchToMinimumCalibration();
		}
	}

	void SwitchToMinimumCalibration()
	{
		m_state = QuickCalibrationState.CalibratingMinimum;
		timer = 0.0f;
		infoText.text = "Let go of Pillo " + (selectedPillo + 1).ToString () + " now!";
	}

	void DoMinimumCalibration ()
	{
		timer += Time.deltaTime;
		if (timer > 3.0f)
		{
			PilloController.SetCalibratedMinimum (PilloController.GetSensor ((Pillo.PilloID)selectedPillo, false), (Pillo.PilloID)selectedPillo);
			SwitchToCalibrationComplete();
		}
	}

	void SwitchToCalibrationComplete()
	{
		m_state = QuickCalibrationState.CalibrationComplete;
		timer = 0.0f;
		infoText.text = "Pillo " + (selectedPillo + 1).ToString () + " is calibrated, well done!";
	}

	void DoCalibrationComplete()
	{
		timer += Time.deltaTime;
		if (timer > 3.0f)
		{
			SwitchToWaitingForSelection();
		}
	}

	void SwitchToWaitingForSelection()
	{
		m_state = QuickCalibrationState.WaitingForSelection;
		PilloController.SaveCalibrationValues ();
		timer = 0.0f;
		infoText.text = "Press the Pillo you want to calibrate";
	}
}
