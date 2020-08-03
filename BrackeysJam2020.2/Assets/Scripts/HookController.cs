using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    //Instances
    [SerializeField]
    LineRenderer wire;

    //Values
    [SerializeField]
    float hookSlowdownSpeed = 0.25f;
    [SerializeField]
    float hookMinVelocity = 0.25f;

    //Components
    Rigidbody2D myRbody;
    Collider2D myCollider;

    //Variables
    float windSpeed = 0f;
    float startSpeed = 0f;
    Transform boat;
    Vector2 previousHookVelocity;
    public bool isWinding = false;
    List<LineRenderer> listWires = new List<LineRenderer>();

    private void Awake()
    {
        myRbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(listWires.Count > 0)
        {
            listWires[0].SetPosition(0, boat.position);
            listWires[listWires.Count - 1].SetPosition(1, transform.position);
        }

        if(isWinding)
        {
            Winding();
        }
        else
        {
            myRbody.velocity -= myRbody.velocity * hookSlowdownSpeed * Time.deltaTime;

            if (myRbody.velocity.magnitude <= hookMinVelocity)
                myRbody.velocity = Vector2.zero;

            previousHookVelocity = myRbody.velocity;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 v = Vector3.Reflect(transform.up * -1f, collision.contacts[0].normal);
        float rot = 180 - Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, 0f, rot);

        LineRenderer newWire = Instantiate(wire) as LineRenderer;
        newWire.SetPosition(0, transform.position);
        listWires.Add(newWire);

        myRbody.velocity = transform.up * previousHookVelocity.magnitude * -1f;
    }

    public void Launch()
    {
        myCollider.isTrigger = false;
        myRbody.velocity = transform.up * startSpeed * -1f;

        LineRenderer newWire = Instantiate(wire) as LineRenderer;
        listWires.Add(newWire);
    }

    void Winding()
    {
        transform.position = Vector2.MoveTowards(transform.position, listWires[listWires.Count - 1].GetPosition(0), windSpeed * Time.deltaTime);

        if(transform.position == listWires[listWires.Count - 1].GetPosition(0))
        {
            Destroy(listWires[listWires.Count - 1].gameObject);
            listWires.RemoveAt(listWires.Count - 1);

            if (listWires.Count == 0)
                StopWinding();
        }
    }

    public void SetValues(float originalSpeed, Transform boatTransform)
    {
        startSpeed = originalSpeed;
        boat = boatTransform;
    }

    public bool IsHookMoving()
    {
        if (myRbody.velocity.sqrMagnitude > 0f)
            return true;

        return false;
    }

    public void StartWinding()
    {
        myCollider.isTrigger = true;
        isWinding = true;
    }

    public void StopWinding()
    {
        Debug.Log("lala");
        isWinding = false;
    }

    public void SetWindingSpeed(float value)
    {
        windSpeed = value;
    }

    public bool IsWinding()
    {
        return isWinding;
    }

    public void RemoveHook()
    {
        gameObject.SetActive(false);
    }
}