//  Slider.cs
//  PDK 3 - Example use of PILLO sensors and accelerometer
//
//  Created by Salvatore CASTELLANO on 27-03-15.
//  Copyright (c) 2015 PILLO Games. All rights reserved.
//
using UnityEngine;
using System;
using System.Collections;
using Pillo;

public class Slider : MonoBehaviour 
{
	private GameObject[] m_slider = new GameObject[4];

	// Use this for initialization
	void Start() 
	{

		PilloController.ConfigureSensorRange(0x40, 0x60);

		//		setup our sliders for this demo
		m_slider[(int)PilloID.Pillo1] = GameObject.Find("Pillo1");
		m_slider[(int)PilloID.Pillo2] = GameObject.Find("Pillo2");
		m_slider[(int)PilloID.Pillo3] = GameObject.Find("Pillo3");
		m_slider[(int)PilloID.Pillo4] = GameObject.Find("Pillo4");
	}
	
	// Update is called once per frame
	void Update() 
	{
		PilloController.GetSensors (PilloID.Pillo1);
		try
		{
			_update(PilloID.Pillo1);
			_update(PilloID.Pillo2);
			_update(PilloID.Pillo3);
			_update(PilloID.Pillo4);
		} catch (Exception)
		{
			; // failsafe; notify user
		}
	}

	void _update(Pillo.PilloID pillo)
	{

		// set height based on Pillo sensor, and use calibrated values if available
		Vector3 tmp = m_slider[(int)pillo].transform.position;
		tmp.y = 10 * PilloController.GetSensor(pillo,PilloController.IsPilloCalibrated(pillo));
		m_slider[(int)pillo].transform.position = tmp;
		// show Pillo orientation based on accelerometer
		Vector3 acceleration = PilloController.GetAccelero(pillo);
		float angleX = Vector3.Angle (Vector3.right, acceleration) - 90;
		float angleZ = Vector3.Angle (Vector3.up, acceleration) - 90;
		m_slider [(int)pillo].transform.rotation = Quaternion.Euler(angleX, 0, angleZ);
	}
}
