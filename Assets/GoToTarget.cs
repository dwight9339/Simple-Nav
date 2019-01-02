using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToTarget : MonoBehaviour {
    Transform target;
    NavAgent nav;

	// Use this for initialization
	void Start () {
        target = GameObject.FindWithTag("Target").transform;
        nav = GetComponent<NavAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit2D blocked = Physics2D.Linecast(transform.position, target.position, 1 << 8);

        if (!blocked) {
            if (nav.CheckPath(target.position))
            nav.SetDestination(target.position);
        
        }

	}
}
