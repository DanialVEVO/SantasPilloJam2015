using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    float levelStartZ = 100;

    [SerializeField]
    float levelEndZ = -20;
    
    [SerializeField]
    float levelSpeed = 20;

    [SerializeField]
    GameObject[] levels;

    ArrayList movingGameObjects = new ArrayList();

    GameObject[] childrenAsBackground;

    Bounds[] childBounds;

    int currentLastBG = 0;

	float levelSpeedMultiplied;
   

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
                Debug.Log(mf.sharedMesh.bounds.size);

                Vector3 pos = mf.transform.localPosition;
                Bounds child_bounds = mf.sharedMesh.bounds;
                child_bounds.center += pos;
                child_bounds.size *= mf.transform.localScale.z;
                theseBounds.Encapsulate(child_bounds);
            }

            childBounds[i] = theseBounds;

            Debug.Log(childBounds[i].size);
        }
	}

	
	public void setLevelSpeedMultiplier(float multiplier)
	{
		levelSpeedMultiplied = levelSpeed * multiplier;
	}


   void LoadNewLevel()
    {
        GameObject newLevel = Instantiate(levels[Random.Range(0, levels.Length)], new Vector3(0,0,levelStartZ), Quaternion.identity) as GameObject;
        movingGameObjects.Add(newLevel);
    }


    IEnumerator RemoveAndDestroy(GameObject thisLevel)
    {          

        yield return new WaitForEndOfFrame();
               
        
        movingGameObjects.Remove(thisLevel);
        Destroy(thisLevel);

        LoadNewLevel();

        StopCoroutine(RemoveAndDestroy(null));
    }


    void LevelMovement()
    {
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
        for (int i=0; i < childrenAsBackground.Length; i++)
        {
			childrenAsBackground[i].transform.position -= Vector3.forward * levelSpeedMultiplied * Time.deltaTime;

            if (childrenAsBackground[i].transform.position.z + childBounds[i].max.z < levelEndZ)
            {

                float movAdjust = 0;

                if (i == 0)
					movAdjust = levelSpeedMultiplied * Time.deltaTime;

                childrenAsBackground[i].transform.position = new Vector3(childrenAsBackground[i].transform.position.x,
                                                                         childrenAsBackground[i].transform.position.y,
                                                                         childrenAsBackground[currentLastBG].transform.position.z + childBounds[currentLastBG].max.z - movAdjust);

                currentLastBG = i;
            }
        }
    }


    // Update is called once per frame
    void FixedUpdate ()
    {

        LevelMovement();

        BackgroundMovement();
      

	}
}
