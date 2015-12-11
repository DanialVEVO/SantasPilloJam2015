//  Calibration.cs
//  PDK 3
//
// 	Created by Ruud Geldhof on 12-08-15
//  Copyright (c) 2015 PILLO Games. All rights reserved.
//

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Pillo;

public class Calibration : MonoBehaviour {
	public Text[] minimumText, maximumText, rawText, calibratedText; // these are set in the editor
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		int i = 0;
		while(i<rawText.Length)
		{
			rawText[i].text = "current raw value = " + PilloController.GetSensor((PilloID) i,false);
			i++;
		}

		i = 0;
		while(i<calibratedText.Length)
		{
			calibratedText[i].text = "current calibrated value = " +  PilloController.GetSensor((PilloID) i,true);
			i++;
		}
	}

	/// <summary>
	/// Sets the minimum calibration value of the specified Pillo as the current valu of that pillo
	/// </summary>
	/// <param name="pilloIndex">Pillo index.</param>
	public void SetMinimumPilloValue(int pilloIndex)
	{
		PilloController.SetCalibratedMinimum (PilloController.GetSensor ((PilloID)pilloIndex), pilloIndex);
		UpdateText ();
	}

	/// <summary>
	/// Sets the maximum calibration value of the specified Pillo as the current valu of that pillo
	/// </summary>
	/// <param name="pilloIndex">Pillo index.</param>
	public void SetMaximumPilloValue(int pilloIndex)
	{
		PilloController.SetCalibratedMaximum (PilloController.GetSensor ((PilloID)pilloIndex), pilloIndex);
		UpdateText ();
	}

	/// <summary>
	/// Updates the text that display the calibration values
	/// </summary>
	public void UpdateText()
	{
		int i = 0;
		while(i<minimumText.Length)
		{
			minimumText[i].text = "current minimum = " + PilloController.GetCalibratedMinimum(i);
			i++;
		}

		i = 0;
		while(i<maximumText.Length)
		{
			maximumText[i].text = "current maximum = " + PilloController.GetCalibratedMaximum(i);
			i++;
		}
	}

	/// <summary>
	/// Saves the calibration values.
	/// </summary>
	public void SaveCalibrationValues()
	{
		PilloController.SaveCalibrationValues ();
	}
	/// <summary>
	/// Loads the calibration values.
	/// </summary>
	public void LoadCalibrationValues()
	{
		PilloController.LoadSavedCalibrationValues ();
		UpdateText ();
	}
	/// <summary>
	/// Deletes the saved calibration values.
	/// </summary>
	public void DeleteSavedCalibrationValues()
	{
		PilloController.DeleteSavedCalibrationValues ();
	}
}
