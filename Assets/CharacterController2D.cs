#define DEBUG_CC2D_RAYS
using UnityEngine;
using System;
using System.Collections.Generic;


[RequireComponent(typeof(BoxCollider2D ))]
public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private LayerMask m_WallLayer;
	[SerializeField] private float m_MoveSpeed = 1.3f;

	private Vector3 m_MovementVector;

	void Start() {

	}

	void Update() {
		m_MovementVector = new Vector3(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical"),
			0f
		);
	}

	void FixedUpdate() {
		var currentPosition = transform.position;
		var deltaPosition = m_MovementVector * m_MoveSpeed * Time.fixedDeltaTime;
		var targetPosition = currentPosition + deltaPosition;
		var rayDistance = deltaPosition.magnitude;
		var rayDirection = deltaPosition.normalized;
		var raycastHit = Physics2D.Raycast(currentPosition, rayDirection, rayDistance, m_WallLayer);
		if(!raycastHit) {
			transform.Translate(deltaPosition);
		}
	}
}