using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour {

    public int speed = 5;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate () {
        Move();
	}

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 camPostition = transform.position;

        transform.Translate(new Vector3(x, y, 0) * speed * Time.deltaTime);
        camPostition.z = -10;
        cam.transform.position = camPostition;
    }
}
