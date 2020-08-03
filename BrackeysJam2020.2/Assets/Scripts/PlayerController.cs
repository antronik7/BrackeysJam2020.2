using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum State { Moving, Charging, Throwing, Winding };

    //Instances
    [SerializeField]
    Transform pivot;
    [SerializeField]
    HookController hook;

    //Values
    [SerializeField]
    float boatSpeed = 1f;
    [SerializeField]
    float boatMaxSpeed = 10f;
    [SerializeField]
    float boatSlowdownSpeed = 0.25f;
    [SerializeField]
    float boatMinVelocity = 0.25f;
    [SerializeField]
    float stickChargingValue = 0.9f;
    [SerializeField]
    float stickReleaseMinDistance = 0.25f;
    [SerializeField]
    float hookForce = 1f;
    [SerializeField]
    float chargeSpeed = 1f;
    [SerializeField]
    float maxCharge = 10f;

    //Components
    Rigidbody2D myRbody;

    //Variables
    State currentState = State.Moving;
    float rightStickPreviousMagnitude = 0f;
    float rightStickHorizontalPreviousValue = 0f;
    float rightStickVerticalPreviousValue = 0f;
    float leftStickHorizontalPreviousValue = 0f;
    float leftStickVerticalPreviousValue = 0f;
    float currentCharge = 0f;
    Vector2 previousBoatVelocity;

    private void Awake()
    {
        myRbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ManageInputs();

        Move();

        if (currentState == State.Charging)
        {
            ChargeThrow();
        }

        previousBoatVelocity = myRbody.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 v = Vector3.Reflect(previousBoatVelocity, collision.contacts[0].normal);
        myRbody.velocity = v;
    }

    void SwitchState(State newState)
    {
        switch (newState)
        {
            case State.Moving:
                ShowPivot(false);
                break;
            case State.Charging:
                ShowPivot(true);
                currentCharge = 0f;
                break;
            case State.Throwing:
                ShowPivot(false);
                Throw();
                break;
            case State.Winding:
                break;
            default:
                break;
        }

        currentState = newState;
    }

    void ManageInputs()
    {
        float rightStickMagnitude = new Vector3(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight"), 0f).magnitude;

        if (rightStickMagnitude >= stickChargingValue && rightStickPreviousMagnitude < stickChargingValue)
            SwitchState(State.Charging);

        if (rightStickMagnitude >= stickChargingValue)
            RotatePivot();

        if (currentState == State.Charging)
        {
            if (rightStickPreviousMagnitude - rightStickMagnitude >= stickReleaseMinDistance)
                SwitchState(State.Throwing);
            else if (rightStickMagnitude <= stickChargingValue)
                SwitchState(State.Moving);
        }

        rightStickHorizontalPreviousValue = Input.GetAxis("HorizontalRight");
        rightStickVerticalPreviousValue = Input.GetAxis("VerticalRight");
        leftStickHorizontalPreviousValue = Input.GetAxis("Horizontal");
        leftStickVerticalPreviousValue = Input.GetAxis("Vertical");
        rightStickPreviousMagnitude = rightStickMagnitude;
    }

    void RotatePivot()
    {
        float angleArrow;

        Vector3 joystickVector = new Vector3(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight"), 0f);

        if (Vector3.Dot(Vector3.up, joystickVector) < 0)
            angleArrow = 360 - Vector3.Angle(Vector3.right, joystickVector);
        else
            angleArrow = Vector3.Angle(Vector3.right, joystickVector);

        pivot.eulerAngles = new Vector3(0f, 0f, angleArrow);
    }

    void ShowPivot(bool value)
    {
        pivot.gameObject.SetActive(value);
    }

    void ChargeThrow()
    {
        currentCharge += chargeSpeed * rightStickPreviousMagnitude * Time.deltaTime;
        currentCharge = Mathf.Min(currentCharge, maxCharge);
    }

    void Throw()
    {
        hook.transform.position = transform.position;
        hook.transform.rotation = Quaternion.FromToRotation(hook.transform.up, new Vector3 (rightStickHorizontalPreviousValue, rightStickVerticalPreviousValue, 0f)) * hook.transform.rotation;
        hook.SetValues(currentCharge, transform);
        hook.gameObject.SetActive(true);
    }

    void Move()
    {
        myRbody.velocity -= myRbody.velocity * boatSlowdownSpeed * Time.deltaTime;

        if(leftStickHorizontalPreviousValue == 0f && leftStickVerticalPreviousValue == 0f)
        {
            if (myRbody.velocity.magnitude <= boatMinVelocity)
                myRbody.velocity = Vector2.zero;
        }

        if (currentState == State.Moving)
        {
            myRbody.velocity += new Vector2(leftStickHorizontalPreviousValue, leftStickVerticalPreviousValue) * boatSpeed * Time.deltaTime;
            myRbody.velocity = Vector2.ClampMagnitude(myRbody.velocity, boatMaxSpeed);
        }
    }
}