﻿using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

    float zLength = 100;

    Bounds bounds;

	// Use this for initialization
	void Start () {

        CalculateLengthOfStage();
	}
	
	
	public void GetLength (out float lengthOfStage)
    {

        lengthOfStage = zLength;

	}

    void CalculateLengthOfStage()
    {

        MeshFilter[] allChildren = GetComponentsInChildren<MeshFilter>();

        bounds = new Bounds(Vector3.zero, Vector3.zero);

        foreach (MeshFilter mf in allChildren)
        {
            Vector3 pos = mf.transform.localPosition;
            Bounds child_bounds = mf.sharedMesh.bounds;
            child_bounds.center += pos;
            bounds.Encapsulate(child_bounds);
        }

        zLength = bounds.size.z + bounds.min.z;

    }

}