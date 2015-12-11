using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

    [SerializeField]
    float LevelStartZ = 100;

    [SerializeField]
    float levelEndZ = -20;

    [SerializeField]
    float levelSpeed = 20;

    [SerializeField]
    GameObject[] levels;

    ArrayList movingGameObjects = new ArrayList();


	// Use this for initialization
	void Start () {
	
	}

    void LoadNewLevel()
    {
        GameObject newLevel = Instantiate(levels[Random.Range(0, levels.Length)], new Vector3(0,0,LevelStartZ), Quaternion.identity) as GameObject;
        movingGameObjects.Add(newLevel);
    }

	// Update is called once per frame
	void FixedUpdate () {

        foreach (GameObject level in movingGameObjects)
        {
            level.transform.position -= Vector3.forward * levelSpeed * Time.deltaTime;

            if (level.transform.position.z < levelEndZ)
            {
                Destroy(level);
                LoadNewLevel();
            }
        }

	}
}
