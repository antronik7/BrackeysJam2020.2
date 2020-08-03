using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    //Components
    Rigidbody2D myRbody;

    //Variables
    float startSpeed = 0f;

    private void Awake()
    {
        myRbody = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Launch();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 v = Vector3.Reflect(transform.up * -1f, collision.contacts[0].normal);
        Debug.Log(transform.up * -1f);
        float rot = 180 - Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, 0f, rot);
        Launch();
    }

    void Launch()
    {
        myRbody.velocity = transform.up * startSpeed * -1f;
    }

    public void SetValues(float originalSpeed)
    {
        startSpeed = originalSpeed;
    }
}
