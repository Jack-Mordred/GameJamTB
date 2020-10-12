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
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        defaultLayer = gameObject.layer;
    }

    private void Start() {
        //agent.enabled = true;
    }

    private void Update() {

        Moovement();

        //HIDING or na
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
        Walk(dir.magnitude);
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
        Dig();
        gameObject.layer = (1 << NavMesh.GetAreaFromName(hiddenLayer));
    }
    void NotHidding() {
        Dig(false);
        gameObject.layer = defaultLayer;
    }

    #region AnimationControler

    Animator animator;
    static readonly string IDLE  = "IDLE";
    static readonly string WALK  = "WALK";
    static readonly string DIG   = "DIG";
    //static readonly string SPROUT   = "SPROUT"; //


    void Walk(float speed) {
        animator.SetFloat(WALK, speed);
    }

    void Dig(bool dig = true) {
        animator.SetBool(DIG,dig);
    }

    #endregion
}

