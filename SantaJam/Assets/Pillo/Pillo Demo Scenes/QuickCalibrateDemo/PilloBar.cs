using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Pillo;

public class PilloBar : MonoBehaviour {
	public int m_pilloIndex;
	public Image gaugeFill, gaugeFillRaw;
	public Text calibratedText;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		gaugeFill.rectTransform.sizeDelta = new Vector2 (250 * PilloController.GetSensor ((PilloID)m_pilloIndex, true), 24); //use the calibrated value to scale to green bar
		gaugeFillRaw.rectTransform.sizeDelta = new Vector2 (250 * PilloController.GetSensor ((PilloID)m_pilloIndex, false), 10);// use the raw value to scale the blue bar
		if (PilloController.IsPilloCalibrated ((PilloID)m_pilloIndex))
		{
			calibratedText.text = "IS CALIBRATED";
			calibratedText.color = Color.green;
		}
		else
		{
			calibratedText.text = "IS NOT CALIBRATED";
			calibratedText.color = Color.red;
		}
	}
}
