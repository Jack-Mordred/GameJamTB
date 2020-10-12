using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour {

    public GameObject waypointsHolder;

    List<Transform> waypoints = new List<Transform>();

    public float speed = 10f;
    public float waitTime = 1f;
    public LayerMask layertoCheck; // sensé etre la couche du joueur
    //spot
    private Light spotLight;

    Color originalSpotColor;
    GameObject player;

    // Start is called before the first frame update
    void Start() {

        player = GameObject.FindGameObjectWithTag("Player");
        spotLight = transform.GetChild(0).GetComponent<Light>();
        originalSpotColor = spotLight.color;
        foreach (Transform item in waypointsHolder.transform) {
            waypoints.Add(item);
        }

        if (waypoints != null && waypoints.Count > 0) {
            StartCoroutine(Patrol(waypoints));
        }

    }

    // Update is called once per frame
    void Update() {

        if (ISEEYOU()) {
            spotLight.color = Color.red;
        } else {
            //print("nein");
            spotLight.color = originalSpotColor;
        }
    }

    // return true if the player is in range, is visible and isInAngle
    bool ISEEYOU() {
        Vector3 dirToPlyer = (player.transform.position - transform.position).normalized;
        //print("Range : " + inRange(player.transform.position, spotLight.range));
        //print("Angle : " + isBetweenAngle(transform.forward, dirToPlyer, spotLight.spotAngle));
        //print("Visible : " + isVisible(transform.position, player.transform.position));
        if (inRange(player.transform.position, spotLight.range) && isBetweenAngle(transform.forward, dirToPlyer, spotLight.spotAngle) && isVisible(transform.position, player.transform.position)) {
            return true;
        }

        return false;
    }

    bool isBetweenAngle(Vector3 from, Vector3 to, float angleTocheck) {
        if (Vector3.Angle(from, to) < angleTocheck) {
            return true;
        }
        return false;
    }


    bool inRange(Vector3 check, float distanceToCheck) {
        if (Vector3.Distance(transform.position, check) < distanceToCheck) {
            return true;
        }
        return false;
    }


    bool isVisible(Vector3 start, Vector3 end) {


        if (!Physics.Linecast(start, end, ~layertoCheck)) {

            return true;
        }
        return false;
    }

    IEnumerator Patrol(List<Transform> waypoints) {
        transform.position = waypoints[0].position;

        int index = 0;

        while (true) {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[index].position, speed * Time.deltaTime);
            if (transform.position == waypoints[index].position) {

                index = (index + 1) % waypoints.Count;

                yield return new WaitForSeconds(waitTime);
            }
            yield return null;
        }


    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        if (player) {
            //Gizmos.DrawLine(transform.position, player.transform.position);
            drawAnlge(spotLight.spotAngle, spotLight.range);
        }

        Gizmos.color = Color.black;
        if (waypoints.Count > 0) {
            foreach (Transform item in waypoints) {
                Gizmos.DrawSphere(item.position, .2f);
            }
        }



    }

    void drawAnlge(float fov, float range) {
        float totalFOV = fov;
        float rayRange = range;
        float halfFOV = totalFOV / 2.0f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position, leftRayDirection * rayRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * rayRange);
    }
}
