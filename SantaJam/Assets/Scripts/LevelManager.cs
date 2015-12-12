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

   

	// Use this for initialization
	void Start () {

        LoadNewLevel();
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

    



    // Update is called once per frame
    void FixedUpdate () {

        
       foreach (GameObject level in movingGameObjects)
        {
            level.transform.position -= Vector3.forward * levelSpeed * Time.deltaTime;

            float stageLength;
            level.GetComponent<Level>().GetLength(out stageLength);

            
            if (level.transform.position.z + stageLength < levelEndZ)
                StartCoroutine(RemoveAndDestroy(level));
        }

	}
}
