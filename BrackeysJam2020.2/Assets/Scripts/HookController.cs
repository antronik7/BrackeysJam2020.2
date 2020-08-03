using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    //Values
    [SerializeField]
    float hookSlowdownSpeed = 0.25f;
    [SerializeField]
    float hookMinVelocity = 0.25f;

    //Components
    Rigidbody2D myRbody;
    LineRenderer myLineRenderer;

    //Variables
    float startSpeed = 0f;
    Transform boat;
    Vector2 previousHookVelocity;

    private void Awake()
    {
        myRbody = GetComponent<Rigidbody2D>();
        myLineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Launch();
    }

    // Update is called once per frame
    void Update()
    {
        myLineRenderer.SetPosition(0, boat.position);
        myLineRenderer.SetPosition(myLineRenderer.positionCount - 1, transform.position);

        myRbody.velocity -= myRbody.velocity * hookSlowdownSpeed * Time.deltaTime;

        if (myRbody.velocity.magnitude <= hookMinVelocity)
            myRbody.velocity = Vector2.zero;

        previousHookVelocity = myRbody.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 v = Vector3.Reflect(transform.up * -1f, collision.contacts[0].normal);
        Debug.Log(transform.up * -1f);
        float rot = 180 - Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, 0f, rot);

        myLineRenderer.SetPosition(myLineRenderer.positionCount - 1, transform.position);
        ++myLineRenderer.positionCount;

        myRbody.velocity = transform.up * previousHookVelocity.magnitude * -1f;
    }

    void Launch()
    {
        myRbody.velocity = transform.up * startSpeed * -1f;
    }

    public void SetValues(float originalSpeed, Transform boatTransform)
    {
        startSpeed = originalSpeed;
        boat = boatTransform;
    }
}
