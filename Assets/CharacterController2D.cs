using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D ))]
public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private LayerMask m_WallLayer;
	[SerializeField] private float m_MoveSpeed = 1.3f;

	private Vector3 m_MovementVector;

	void Start() {

	}

	void Update() {

	}

	public void SetMovementVector(Vector2 movement) {
		this.m_MovementVector = movement;
	}

	public void SetMovementVector(float moveX, float moveY) {
		this.m_MovementVector = new Vector2(moveX, moveY);
	}

	Vector3 TransformMovementVector() {
		var direction = m_MovementVector * m_MoveSpeed * Time.fixedDeltaTime;
		var magnitude = direction.magnitude;
		if(magnitude == 0f) {
			return Vector3.zero;
		}
		return (direction * m_MoveSpeed / magnitude) * Time.fixedDeltaTime;
	}

	void FixedUpdate() {
		var currentPosition = transform.position;
		var deltaPosition = TransformMovementVector();
		var targetPosition = currentPosition + deltaPosition;
		var rayDistance = deltaPosition.magnitude;
		var rayDirection = deltaPosition.normalized;
		var raycastHit = Physics2D.Raycast(currentPosition, rayDirection, rayDistance, m_WallLayer);
		if(!raycastHit) {
			transform.Translate(deltaPosition);
		}		
	}
}