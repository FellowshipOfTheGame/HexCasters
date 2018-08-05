using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public float speed = 1.0f;
	public Rigidbody2D rb;

	void Update() {
		var horizontal = Input.GetAxisRaw("Horizontal");
		var vertical = Input.GetAxisRaw("Vertical");
		var direction = new Vector2(horizontal, vertical);
		rb.AddForce(speed * direction);
	}
}
