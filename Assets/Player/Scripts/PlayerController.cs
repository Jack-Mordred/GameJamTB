using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

    public Transform follow;
    public float movespeed = 2f;
    public float turnSmoothTime = 0.1f;

    float turnSmoothVelocity;
    CharacterController controller;
    NavMeshAgent agent;
    int defaultLayer;

    private static readonly string hiddenLayer = "Dig";

    private void Awake() {
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        defaultLayer = gameObject.layer;
    }

    private void Start() {
        //agent.enabled = true;
    }

    private void Update() {

        Moovement();

        if (Input.GetKey(KeyCode.Mouse0) && IsOn(hiddenLayer)) {
            Debug.Log(" je me cache / animation de creusage tu sais deja");
            Hidding();
        } else if (!Input.GetKey(KeyCode.Mouse0) && IsOn(hiddenLayer)) {
            Debug.Log("JETAIS CACHE");
            NotHidding();
        }


    }


    void Moovement() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertital = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(horizontal, 0f, vertital).normalized;

        if (dir.magnitude >= 0.1f) {

            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            controller.Move(dir * movespeed * Time.deltaTime);
        }
       
    }


    // Is mouse currently on "area"
    public bool IsOn(string area) {
        return getCurrentArea() == (1 << NavMesh.GetAreaFromName(area));
    }

    //return current mouse Area
    public int getCurrentArea() {
        NavMeshHit hit;
        agent.SamplePathPosition(agent.areaMask, 0.01f, out hit);
        return hit.mask;
    }

    void Hidding() {    
        gameObject.layer = (1 << NavMesh.GetAreaFromName(hiddenLayer));
    }
    void NotHidding() {
        gameObject.layer = defaultLayer;
    }
}

