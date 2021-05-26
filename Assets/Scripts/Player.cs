using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public float turnSpeed = 4.0f;
    public float moveSpeed = 2.0f;
    public float shotSpeed = 1000.0f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;

    [SerializeField]
    private GameObject snowball;
    private float rotX;

    private int health = 6;
    private Slider healthSlider;

    void Update()
    {
        MouseAiming();
        KeyboardMovement();
        Shoot();
    }

    void MouseAiming()
    {
        // get the mouse inputs
        float y = Input.GetAxis("Mouse X") * turnSpeed;
        rotX += Input.GetAxis("Mouse Y") * turnSpeed;

        // clamp the vertical rotation
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        // rotate the camera
        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);
    }

    void KeyboardMovement()
    {
        Vector3 dir = new Vector3(0, 0, 0);

        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");

        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
           
            Debug.Log("Mouse Click");
            var c = Instantiate(snowball,transform.position,snowball.transform.rotation);
            c.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * shotSpeed);

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name.Contains("Snowball"))
        {
            health--;
            healthSlider.value = health;
        }
    }

}