﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour {

    public GameObject waypointsHolder;

    List<Transform> waypoints = new List<Transform>();



    public float range = 2f;
    public float waitTime = 1f;
    public LayerMask layertoCheck; // sensé etre la couche du joueur
    //spot
    private Light spotLight;

    Color originalSpotColor;
    GameObject player;
    NavMeshAgent agent;
    Animator animator;

    static readonly string WALK = "WALK";
    static readonly string ISEEU = "ISEEU";



    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start() {

        player = GameObject.FindGameObjectWithTag("Player");
        spotLight = transform.GetChild(0).GetComponent<Light>();
        originalSpotColor = spotLight.color;
        foreach (Transform item in waypointsHolder.transform) {
            waypoints.Add(item);
        }

        if (waypoints != null && waypoints.Count > 0) {
            agent.Warp(RandomPoint(waypoints[0].position, range));
            StartCoroutine(Patrol(waypoints));
        }

    }

    // Update is called once per frame
    void Update() {

        if (ISEEYOU()) {
            animator.SetTrigger(ISEEU);
            // end GAME
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
        if (inRange(player.transform.position, spotLight.range)
            && isBetweenAngle(transform.forward, dirToPlyer, spotLight.spotAngle)
            && isVisible(transform.position + Vector3.up, dirToPlyer)) {
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
        RaycastHit hit;
        if (Physics.Raycast(start,end,out hit,Mathf.Infinity,layertoCheck)) {
            if (hit.transform.CompareTag("Player")) {
                return true;
            }
        }

        return false;
    }

    bool destinationReached {
        get {
            // Check if we've reached the destination
            if (!agent.pathPending) {
                if (agent.remainingDistance <= agent.stoppingDistance) {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    Vector3 destination;
    public float rotationSpeed = 10f;
    private float turnSmoothVelocity;
    private float turnSmoothTime = 0.1f;

    IEnumerator Patrol(List<Transform> waypoints) {
        transform.position = waypoints[0].position;

        int index = 0;

        while (true) {
            //transform.LookAt(waypoints[index].position);
           
            //transform.position = Vector3.MoveTowards(transform.position, waypoints[index].position, speed * Time.deltaTime);
            destination = RandomPoint(waypoints[index].position, range);
            if (destination != Vector3.zero) {
                agent.SetDestination(destination);
                animator.SetBool(WALK, true);

                //for (int i = 0; i < agent.path.corners.Length; i++) {
                //    Vector3 dir = agent.path.corners[i] - transform.position ;
                //    float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                //    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

                //    transform.rotation = Quaternion.Euler(0, angle, 0);
                //}

                //transform.position == waypoints[index].position
                if (destinationReached) {
                    animator.SetBool(WALK, false);
                    index = (index + 1) % waypoints.Count;

                    yield return new WaitForSeconds(waitTime);
                }
                yield return null;
            }
           
           
        }


    }

    Vector3 RandomPoint(Vector3 center, float range) {
        Vector3 result = Vector3.zero;
        for (int i = 0; i < 30; i++) {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, agent.areaMask)) {
                result = hit.position;
            }
        }
        return result;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        if (player) {
            Gizmos.DrawLine(transform.position + Vector3.up, (player.transform.position - transform.position));
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
