using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShooterBadGuy : NetworkBehaviour {

	private Rigidbody2D m_RigidBody2D;
	public Transform Destination;
	public bool isAwake = false;

	private Vector2 m_Velocity;

	void Start () {
		m_RigidBody2D = GetComponent<Rigidbody2D>();
	}

	void ChooseDirection() {
		var dir = Random.insideUnitCircle * 4f;
		var mag = dir.magnitude;
		m_Velocity = new Vector2(dir.x * 4f / mag, dir.y * 4f / mag);
		var lookDirection = dir.normalized;
		transform.rotation.SetLookRotation(new Vector3(lookDirection.x, lookDirection.y, 0f));
	}
	/// <summary>
	/// Sent when an incoming collider makes contact with this object's
	/// collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.tag == "Collision" && isAwake) {
			ChooseDirection();
		}
	}
	void Update () {
		if(!isAwake) { //Gosh this is awful lol
			transform.position = Vector3.MoveTowards(transform.position, Destination.position, 0.1f);
			if(Vector3.Distance(transform.position, Destination.position) < 0.1) {
				isAwake = true;
				m_RigidBody2D.simulated = true;
				ChooseDirection();
			}
		}
		else {
			m_RigidBody2D.velocity = m_Velocity;
		}
	}
}
