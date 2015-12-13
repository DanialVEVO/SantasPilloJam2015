using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

    [SerializeField]
    float maxCameraMov = 8;

    //Transform target;

    LevelManager levelMgr;

    Vector3 origPos;

    float initHeightAtDist;

    bool dzEnabled = false;

    float origSpeed = 1;

    float newSpeed = 1;


       float FrustumHeightAtDistance(float distance)
    {
        return 2.0f * distance * Mathf.Tan(GetComponent<Camera>().fieldOfView * 0.5f * Mathf.Deg2Rad);
    }


    // Calculate the FOV needed to get a given frustum height at a given distance.
    float FOVForHeightAndDistance(float height, float distance)
    {
        return 2 * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
    }


    void Start()
    {
        //target = GameObject.Find("Player").transform;

        levelMgr = GameObject.Find("LevelManagerObject").GetComponent<LevelManager>();

        origPos = transform.position;
    }

    // Start the dolly zoom effect.
    void StartDZ()
    {
        var distance = Vector3.Distance(transform.position, Vector3.up * 4);
        initHeightAtDist = FrustumHeightAtDistance(distance);
        dzEnabled = true;
    }


    // Turn dolly zoom off.
    void StopDZ()
    {
        dzEnabled = false;
    }


    void Update()
    {
        

        levelMgr.GetSpeedMult(out newSpeed);

        //Debug.Log(newSpeed + " " + origSpeed);

        if (origSpeed != newSpeed && !dzEnabled)
            StartDZ();

        if (dzEnabled)
        {
            Debug.Log("i update");

            // Measure the new distance and readjust the FOV accordingly.
            var currDistance = Vector3.Distance(transform.position, Vector3.up*4);
            GetComponent<Camera>().fieldOfView = FOVForHeightAndDistance(initHeightAtDist, currDistance);

            transform.position = Vector3.Slerp(transform.position, origPos + Vector3.forward * (newSpeed -1) * maxCameraMov, Time.deltaTime) ;

            if (origSpeed == newSpeed &&  origPos.z >= transform.position.z)
                dzEnabled = false;
            
        }
                
    }
}
