//  PilloController.cs
//  PDK 3
//
//  Created by Salvatore CASTELLANO on 27-03-15.
// 	Edited and extended by Ruud Geldhof on 12-08-15
//  Copyright (c) 2015 PILLO Games. All rights reserved.
//
using UnityEngine;
using Pillo;

public class PilloController : MonoBehaviour 
{
	public static bool Debugging_Enabled = true; // use this to enable/disable debug messages. Be sure to set to false in the final build

	public static bool Use_Saved_Calibration_Values = false; // to determine whether this controller should use calibration values saved in the player prefs
	private static Vector2[] calibration_PressureValue; //array to contain values needed for calibration. x = bottom value, y = top value

	private static Vector3[] calibration_AcceleroValue;

	protected static PilloReceiver m_receiver = null;
	// Do not change this code
	void Start()
	{
		if (m_receiver == null)
			m_receiver = new PilloReceiver();
		m_receiver.Connect();

		InitializeCalibrationValues ();

	}
	void OnDestroy()
	{
		// Never forget to Dispose the PilloReader!!!!
		if (m_receiver != null)
			m_receiver.Dispose();
		m_receiver = null;
	}
	// Below are the API calls that can be used

	///Set the max and mininimal value of the sensor
	///</summary>
	public static void ConfigureSensorRange(int min, int max)
	{
		PilloSender.SensorMin = min;
		PilloSender.SensorMax = max;
	}

	///returns the current version of the Pillo
	///</summary>
	public static int GetVersion(PilloID pillo)
	{

		return m_receiver.GetVersion(pillo);
	}

	///returns the name of the specified Pillo Id
	///</summary>
	public static string GetName(PilloID pillo)
	{
		return m_receiver.GetName(pillo);
	}

	///Sets the specified ID with the specified name
	///</summary>
	public static void SetName(PilloID pillo, string name)
	{
		m_receiver.SetName(pillo, name);
	}

	///Get the ID of the specified Pillo name
	///</summary>
	public static PilloID GetPilloByName(string name)
	{
		return m_receiver.GetPilloByName(name);
	}
	
	///checks if the specified Pillo's battery is low
	///</summary>
	public static bool isBatteryLow(PilloID pillo)
	{
		return m_receiver.GetBatteryLow(pillo);
	}

	///returns true if externally powered, false if it's not
	///</summary>
	public static bool isExternallyPowered(PilloID pillo)
	{
		return m_receiver.GetExternalPower(pillo);
	}

	///returns the id of the specified Pillo
	///</summary>
	public static string getUniqueID(PilloID pillo)
	{
		return m_receiver.GetUniqueID(pillo);
	}


	///Get all the pressure sensor values wrapped in a Vector4 of the specified Pillo
	///</summary>
	public static Vector4 GetSensors(PilloID pillo)
	{

		Vector4 vector = new Vector4();
		vector.x = m_receiver.GetSensor1(pillo);
		vector.y = m_receiver.GetSensor2(pillo);
		vector.z = m_receiver.GetSensor3(pillo);
		vector.w = m_receiver.GetSensor4(pillo);
		return vector;
	}

	///Get the pressure applied to the specified Pillo.
	///</summary>
	public static float GetSensor(PilloID pillo)
	{
		return m_receiver.GetSensor(pillo, PilloSensor.Sensor1);
	}

	///Get the pressure applied to the specified Pillo.
	///</summary>
	public static float GetSensor(PilloID pillo, PilloSensor sensor)
	{
		return m_receiver.GetSensor(pillo, sensor);
	}

	///Get the pressure applied to the specified Pillo. Set last param to true to use calibrated values;
	///</summary>
	public static float GetSensor(PilloID pillo, bool useCalibrated)
	{
		if (useCalibrated) 
		{
			if(!IsPilloCalibrated(pillo) && Debugging_Enabled)
				Debug.LogWarning("Pillo "+ ((int)pillo).ToString() + " is not calibrated, but calibrated values are requested");
			return Mathf.Clamp ((m_receiver.GetSensor(pillo, PilloSensor.Sensor1)-calibration_PressureValue[(int)pillo].x) / (calibration_PressureValue[(int)pillo].y - calibration_PressureValue[(int)pillo].x ),0.0f,1.0f);
		}
		else
		{
			return m_receiver.GetSensor(pillo, PilloSensor.Sensor1);
		}
	}


	/// <summary>
	/// Wraps all the accelerometer values in an vector3 of the specified Pillo
	/// </summary>
	/// <returns>The accelero.</returns>
	/// <param name="pillo">Pillo.</param>
	public static Vector3 GetAccelero(PilloID pillo)
	{
		Vector3 vector = new Vector3();
		vector.x = m_receiver.GetAcceleroX(pillo);
		vector.y = m_receiver.GetAcceleroY(pillo);
		vector.z = m_receiver.GetAcceleroZ(pillo);
		return vector;
	}



	/// <summary>
	/// Get's the X value of the accelerometer. (of the specified Pillo)
	/// </summary>
	/// <returns>The accelero x.</returns>
	/// <param name="pillo">Pillo.</param>
	public static float GetAcceleroX(PilloID pillo)
	{
		return m_receiver.GetAcceleroX(pillo);
	}

	/// <summary>
	/// Get's the Y value of the accelerometer. (of the specified Pillo)	/// </summary>
	/// <returns>The accelero y.</returns>
	/// <param name="pillo">Pillo.</param>
	public static float GetAcceleroY(PilloID pillo)
	{
		return m_receiver.GetAcceleroY(pillo);
	}

	/// <summary>
	/// Get's the Z value of the accelerometer. (of the specified Pillo)
	/// </summary>
	/// <returns>The accelero z.</returns>
	/// <param name="pillo">Pillo.</param>
	public static float GetAcceleroZ(PilloID pillo)
	{
		return m_receiver.GetAcceleroZ(pillo);
	}

	/// <summary>
	/// Loads the calibration values saved in the PlayerPrefs. When none are saved default values 1 and 0 will be used as bottom and top values.
	/// </summary>
	public static void LoadSavedCalibrationValues()
	{
		calibration_PressureValue = new Vector2[4];
		int i = 0;
		while (i<4) {
			if (!PlayerPrefs.HasKey ("PilloCalibration_Min" + i.ToString ()) && Debugging_Enabled)
				Debug.LogWarning ("no saved calibration values were found for pillo " + i.ToString ());
			calibration_PressureValue [i] = new Vector2 (PlayerPrefs.GetFloat ("PilloCalibration_Min" + i.ToString (), 0), PlayerPrefs.GetFloat ("PilloCalibration_Max" + i.ToString (), 1));
			i++;
		}
	}

	/// <summary>
	/// Saves the calibration values to the PlayerPrefs. Run this after using the SetCalibratedMinimum/-Maximum functions.
	/// </summary>
	public static void SaveCalibrationValues()
	{
		int i = 0;
		while (i<4) {
			PlayerPrefs.SetFloat("PilloCalibration_Min" + i.ToString (),calibration_PressureValue[i].x);
			PlayerPrefs.SetFloat("PilloCalibration_Max" + i.ToString (),calibration_PressureValue[i].y);
			i++;
		}
	}
	/// <summary>
	/// Deletes the calibration values saved in the PlayerPrefs.
	/// </summary>
	public static void DeleteSavedCalibrationValues()
	{
		int i = 0;
		while (i<4) {
			PlayerPrefs.DeleteKey("PilloCalibration_Min" + i.ToString ());
			PlayerPrefs.DeleteKey("PilloCalibration_Max" + i.ToString ());
			i++;
		}
	}


	/// <summary>
	/// Determines if the specified pillo is calibrated. will return true unless calibration values are both zero, or if the minimum is higher than the maximum.
	/// </summary>
	/// <returns><c>true</c> if is pillo calibrated the specified pillo; otherwise, <c>false</c>.</returns>
	/// <param name="pillo">Pillo.</param>
	public static bool IsPilloCalibrated (PilloID pillo)
	{
		if (calibration_PressureValue [(int)pillo] == Vector2.zero || calibration_PressureValue [(int)pillo].x > calibration_PressureValue [(int)pillo].y)
			return false;
		else
			return true;
	}
	/// <summary>
	/// Initializes the calibration values.
	/// </summary>
	public static void InitializeCalibrationValues()
	{
		if (Use_Saved_Calibration_Values) {
			LoadSavedCalibrationValues ();
		} else {
			calibration_PressureValue = new Vector2[4];
			calibration_AcceleroValue = new Vector3[4];
		}
	}

	/// <summary>
	/// Sets the calibrated minimum for the specified Pillo.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="pilloIndex">Pillo index.</param>
	public static void SetCalibratedMinimum(float value, int pilloIndex)
	{
		calibration_PressureValue [pilloIndex] = new Vector2 (value, calibration_PressureValue [pilloIndex].y);
	}

		/// <summary>
		/// Sets the calibrated minimum for the specified Pillo.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="pillo">Pillo.</param>
	public static void SetCalibratedMinimum(float value, PilloID pillo)
	{
		calibration_PressureValue [(int) pillo] = new Vector2 (value, calibration_PressureValue [(int) pillo].y);
	}
	/// <summary>
	/// Gets the calibrated minimum for the specified Pillo.
	/// </summary>
	/// <returns>The calibrated minimum.</returns>
	/// <param name="pilloIndex">Pillo index.</param>
	public static float GetCalibratedMinimum(int pilloIndex)
	{
		return calibration_PressureValue [pilloIndex].x;
	}
	/// <summary>
	/// Gets the calibrated minimum for the specified Pillo.
	/// </summary>
	/// <returns>The calibrated minimum.</returns>
	/// <param name="pillo">Pillo.</param>
	public static float GetCalibratedMinimum(PilloID pillo)
	{
		return calibration_PressureValue [(int)pillo].x;
	}

	/// <summary>
	/// Sets the calibrated maximum for the specified Pillo.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="pilloIndex">Pillo index.</param>
	public static void SetCalibratedMaximum(float value, int pilloIndex)
	{
		calibration_PressureValue [pilloIndex] = new Vector2 (calibration_PressureValue [pilloIndex].x, value);
	}

	/// <summary>
	/// Sets the calibrated maximum for the specified Pillo.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="pillo">Pillo.</param>
	public static void SetCalibratedMaximum(float value, PilloID pillo)
	{
		calibration_PressureValue [(int) pillo] = new Vector2 (calibration_PressureValue [(int) pillo].x,value);
	}
	/// <summary>
	/// Gets the calibrated maximum for the specified Pillo.	
	/// </summary>
	/// <returns>The calibrated maximum.</returns>
	/// <param name="pilloIndex">Pillo index.</param>
	public static float GetCalibratedMaximum(int pilloIndex)
	{
		return calibration_PressureValue [pilloIndex].y;
	}

	/// <summary>
	/// Gets the calibrated maximum for the specified Pillo.
	/// </summary>
	/// <returns>The calibrated maximum.</returns>
	/// <param name="pillo">Pillo.</param>
	public static float GetCalibratedMaximum(PilloID pillo)
	{
		return calibration_PressureValue [(int)pillo].y;
	}

	// Included only for debugging purposes
	protected static int m_state = 0;
	void Update()
	{
		// report some debugging feedback
		if (Debugging_Enabled) {
			switch (m_state) {
			case 0:
				print ("Started Example project...");
				m_state = 1;
				break;
			case 1:
				if (m_receiver.FoundPillo) {
					print ("Found PILLO Game Controller!");
					m_state = 2;
				}
				break;
			case 2:
				if (m_receiver.ConnectedToPillo) {
					print ("Connected to PILLO Game Controller!");
					m_state = 3;
				}
				break;
			case 3:
				if (!m_receiver.ConnectedToPillo) {
					print ("Disconnected from PILLO Game Controller?");
					m_state = 1;
				}
				break;
			default:
				m_state = 1;
				break;		// plain paranoia
			}
		}
	}

	/// <summary>
	/// Sets the accelero calibration value of the specified pillo. use this to determine which way is down for the pillo
	/// </summary>
	/// <param name="pillo">Pillo.</param>
	public static void SetAcceleroCalibrationValue(PilloID pillo)
	{
		calibration_AcceleroValue[(int)pillo] = GetAccelero (pillo);
	}
	/// <summary>
	/// Sets the accelero calibration value of the specified pillo. use this to determine which way is down for the pillo
	/// </summary>
	/// <param name="pillo">Pillo.</param>
	public static void SetAcceleroCalibrationValue(int pilloIndex)
	{
		calibration_AcceleroValue[pilloIndex] = GetAccelero ((PilloID)pilloIndex);
	}




}
