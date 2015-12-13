using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

    float zLength = 100;

    Bounds bounds;

    float startZ = 0;

    float lengthBeforeNext = 20;

    LevelManager levelMgr;

    bool once = false;

	// Use this for initialization
	void Start () {

        CalculateLengthOfStage();

        startZ = transform.position.z;
	}

    public void setLengthBeforeNextAndLvlMgr(float newLength, LevelManager lvlMgr)
    {
        lengthBeforeNext = newLength;

        levelMgr = lvlMgr;

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

        zLength =  bounds.max.z;

    }

    void Update()
    {

        if (once)
            return;

        if (transform.position.z + zLength + lengthBeforeNext < startZ)
        {
            levelMgr.LoadNewLevel();
            once = true;
        }
    }

    void OnDestroy()
    {
        if (!once)
            levelMgr.LoadNewLevel();
    }

}
