﻿using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    float levelStartZ = 100;

    [SerializeField]
    float levelEndZ = -20;

    [SerializeField]
    float lengthBetweenLevels = 20.0f;
    
    [SerializeField]
    float levelSpeed = 20;

    [SerializeField]
    GameObject[] levels;

    ArrayList movingGameObjects = new ArrayList();

    GameObject[] childrenAsBackground;

    Bounds[] childBounds;

    int currentLastBG = 0;

	float levelSpeedMultiplied;

    bool win = false;
    
   

	// Use this for initialization
	void Start () {
        
        LoadNewLevel();

        childrenAsBackground = new GameObject[transform.childCount];

        childBounds = new Bounds[transform.childCount];

		levelSpeedMultiplied = levelSpeed;


        for (int i = 0; i < childrenAsBackground.Length; i++)
        {
            childrenAsBackground[i] = transform.GetChild(i).gameObject;

            if (childrenAsBackground[i].transform.position.z > childrenAsBackground[currentLastBG].transform.position.z)
                currentLastBG = i;

            Bounds theseBounds = new Bounds(Vector3.zero, Vector3.zero);

            MeshFilter[] allChildren = childrenAsBackground[i].GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter mf in allChildren)
            {
               // Debug.Log(mf.sharedMesh.bounds.size);

                Vector3 pos = mf.transform.localPosition;
                Bounds child_bounds = mf.sharedMesh.bounds;
                child_bounds.center += pos;
                child_bounds.size *= mf.transform.localScale.z;
                theseBounds.Encapsulate(child_bounds);
            }

            childBounds[i] = theseBounds;

            //Debug.Log(childBounds[i].size);
        }
       
    }


    public void setLevelSpeedMultiplier(float multiplier)
	{
		levelSpeedMultiplied = levelSpeed * multiplier;
	}

    public void GetSpeedMult(out float speedMult)
    {
        speedMult = levelSpeedMultiplied / levelSpeed;
    }


   public void LoadNewLevel()
    {
        if (win)
            return;

        int scale = Random.Range(0, 2);


        GameObject newLevel = Instantiate(levels[Random.Range(0, levels.Length)], new Vector3(0,0,levelStartZ), Quaternion.identity) as GameObject;
        movingGameObjects.Add(newLevel);

        newLevel.transform.localScale -= Vector3.right * scale * 2;
        
        newLevel.GetComponent<Level>().setLengthBeforeNextAndLvlMgr(lengthBetweenLevels, this);
    }

    public void EndLevelSpawn()
    {
        win = true;

        foreach (GameObject level in movingGameObjects)
            level.SetActive(false);
    }


    IEnumerator RemoveAndDestroy(GameObject thisLevel)
    {          

        yield return new WaitForEndOfFrame();
               
        
        movingGameObjects.Remove(thisLevel);
        Destroy(thisLevel);

        //LoadNewLevel();

        StopCoroutine(RemoveAndDestroy(null));
    }


    void LevelMovement()
    {
        if (win)
            return;

        foreach (GameObject level in movingGameObjects)
        {
			level.transform.position -= Vector3.forward * levelSpeedMultiplied * Time.deltaTime;

            float stageLength;
            level.GetComponent<Level>().GetLength(out stageLength);



            if (level.transform.position.z + stageLength < levelEndZ && level.transform.position.z + stageLength > levelEndZ - 200)
            {

                //making sure Coroutine doesn't get called twice
                level.transform.position -= Vector3.forward * 300;

                //wait for end frame to remove level so the list doesn't get changed during foreach loop
                StartCoroutine(RemoveAndDestroy(level));
            }
        }
    }


    void BackgroundMovement()
    {
        //float aa;

        for (int i=0; i < childrenAsBackground.Length; i++)
        {

            //aa = childrenAsBackground[i].transform.position.y;

           // Vector3 temp = childrenAsBackground[i].transform.position;

            //temp.z -= levelSpeedMultiplied * Time.deltaTime;

            //Debug.Log("Difference: " + (childrenAsBackground[i].transform.position.y -  temp.y));

            childrenAsBackground[i].transform.Translate(0,0, -levelSpeedMultiplied * Time.deltaTime);

            

            if (childrenAsBackground[i].transform.position.z + childBounds[i].max.z < levelEndZ)
            {

                float movAdjust = 0;

                if (i == 0)
					movAdjust = levelSpeedMultiplied * Time.deltaTime;

                childrenAsBackground[i].transform.position = new Vector3(0, 0, childrenAsBackground[currentLastBG].transform.position.z + childBounds[currentLastBG].max.z - movAdjust);

                currentLastBG = i;
            }
        }
    }


    // Update is called once per frame
    void Update ()
    {

        LevelMovement();

        BackgroundMovement();
      

	}
}
