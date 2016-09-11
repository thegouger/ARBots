using UnityEngine;
using System.Collections;

public class FireScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            // Spawn projectile
            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.transform.parent = gameObject.transform;
            projectile.transform.Translate(new Vector3(0.0f, -0.04f, -0.035f));
            projectile.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);
            projectile.layer = 10;

            // Add force to projectile
            Rigidbody rigidBody = projectile.AddComponent<Rigidbody>();
            rigidBody.mass = 20.0f;
            rigidBody.AddForce(projectile.transform.forward * 1.0f);
            Debug.Log("Creating projectile");
        }
	}
}
