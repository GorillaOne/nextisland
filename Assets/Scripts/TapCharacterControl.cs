using UnityEngine;
using System.Collections;
using System;
using TouchScript;
using Photon;
using TouchScript.Gestures;
using TouchScript.Hit;

enum MovementState
{
    stopped,
    moving,
    stopping,
}
public class TapCharacterControl : PunBehaviour
{

    public bool InputEnabled { get; set; }

    public const float MaxMovementSpeed = 6f;
    const float ForwardAcceleration = 10f;
    const float TorqueAcceleration = 20f; 
    const float StopDrag = 5f; 
    const float TurnSpeed = 2f;
    const float AnimSpeed = 1f;
    const float StopDistance = 2f;
    const float WalkOnlyDistance = 5f;
    const float WalkAnimSpeed = .2f;
    const float RunAnimSpeed = .8f;

    public float Forward { get; set; }

    Animator animator { get; set; }

    #region private fields
    Vector3 destination = Vector3.zero; //We ignore the Y value, but use a vector3 holder because of the useful functions. 
    MovementState currentMovementState;
    #endregion
    MovementState CurrentMovementState
    {
        get { return currentMovementState; }
        set
        {
            currentMovementState = value;
            OnCurrentMovementStateSet();
        }
    }
    TapGesture tapGesture { get; set; }
    Rigidbody rb { get; set; }
    // Use this for initialization
    void Start()
    {
        animator = this.GetComponentInChildren<Animator>();
        //TouchManager.Instance.TouchesEnded += Instance_TouchesEnded;
        tapGesture = GameObject.FindGameObjectWithTag("Ground").GetComponent<TapGesture>() ?? null;
        if (tapGesture != null)
        {
            tapGesture.Tapped += TapGesture_Tapped;
        }
        rb = GetComponent<Rigidbody>() ?? null;
    }

    public void OnDestroy()
    {
        if (tapGesture != null)
        {
            tapGesture.Tapped -= TapGesture_Tapped;
        }        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateMove()
    {
        UpdateForwardVelocity();         
    }

    private void UpdateForwardVelocity()
    {
        var distance = Vector3.Distance(destination, transform.position);
        if (distance < StopDistance)
        {
            CurrentMovementState = MovementState.stopping;
        }
        else
        {
            var vector = destination - transform.position;
            vector.Normalize();
            vector.y = 0;
            rb.AddForce(vector * ForwardAcceleration, ForceMode.Acceleration);

            if (rb.velocity.magnitude > MaxMovementSpeed)
            {
                vector = rb.velocity;
                vector.Normalize();
                vector *= MaxMovementSpeed;
                rb.velocity = vector;
            }
        }
    }
    private void ApplyTorque()
    {
        //Rigidbody has a locked Y rotation axis, so we can safetly use tweens to rotate the character. 
        iTween.StopByName("torqueTween"); 
        
        var args = new Hashtable();
        args.Add("axis", "y");
        args.Add("name", "torqueTween");
        args.Add("looktarget", destination);
        args.Add("onupdate", "TorqueUpdate");
        iTween.LookTo(gameObject, args);

        
    }

    void TorqueUpdate()
    {
       
    }
    private void OnCurrentMovementStateSet()
    {
        if (CurrentMovementState == MovementState.stopping)
        {
            rb.drag = StopDrag; 
        }
        if (CurrentMovementState == MovementState.moving)
        {            
            rb.drag = 0; 
        }
    }
    public void FixedUpdate()
    {
        if (CurrentMovementState == MovementState.moving) UpdateMove();        
    }


    //void GoToLocation(float x, float z)
    //{
    //    var tweenName = "Movement";
    //    iTween.StopByName(tweenName);
    //    destination = new Vector3(x, transform.position.y, z);
    //    var estTime = Vector3.Distance(destination, transform.position) / MaxMovementSpeed;
    //    estTime = Math.Max(estTime, minimumMoveTime);
    //    var args = new Hashtable();
    //    args.Add("name", tweenName);
    //    args.Add("axis", "y");
    //    args.Add("position", destination);
    //    args.Add("time", estTime);
    //    args.Add("onupdate", "OnTweenUpdate");
    //    args.Add("oncomplete", "OnTweenComplete");
    //    args.Add("easetype", iTween.EaseType.easeInOutQuad);
    //    args.Add("path", new Vector3[2] { transform.position, destination });
    //    args.Add("orienttopath", true);
    //    var lookTime = Math.Min(timeToLook, estTime);
    //    args.Add("looktime", lookTime);

    //    iTween.MoveTo(gameObject, args);

    //    lastFramePosition = transform.position;
    //}

    void GoToLocation(Vector3 loc)
    {
        destination = loc;
        CurrentMovementState = MovementState.moving;
        ApplyTorque(); 
    }

    Vector3 lastFramePosition = Vector3.zero;

    #region events
    private void TapGesture_Tapped(object sender, EventArgs e)
    {
        if (InputEnabled)
        {
            if (photonView.isMine)
            {
                var gesture = (TapGesture)sender;
                TouchHit hit;
                gesture.GetTargetHitResult(out hit);
                GoToLocation(hit.Point);
            }
        }
    }
    #endregion

    void OnTweenUpdate()
    {
        animator.speed = AnimSpeed;
        var distLastFrame = Vector3.Distance(lastFramePosition, transform.position);
        currentMovementSpeed = distLastFrame / Time.fixedDeltaTime;
        percentageOfMaxSpeed = currentMovementSpeed / MaxMovementSpeed;
        animator.SetFloat("Forward", percentageOfMaxSpeed);
        lastFramePosition = transform.position;


        //transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);

    }
    void OnTweenComplete()
    {
        animator.speed = AnimSpeed;
        animator.SetFloat("Forward", 0);
        animator.SetFloat("Direction", 0);
    }
    float currentMovementSpeed;
    float percentageOfMaxSpeed;
}
