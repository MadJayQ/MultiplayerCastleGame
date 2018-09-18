using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;


[RequireComponent(typeof(CharacterController2D))]
public class Player : NetworkBehaviour {

	enum MoveDirection {
		MOVE_DOWN = 0,
		MOVE_UP = 1,
		MOVE_LEFT = 2,
		MOVE_RIGHT = 3
	};

	[SerializeField] private GameObject FireballPrefab;
	private CinemachineVirtualCamera m_VirtualCamera;

	private CharacterController2D m_CharacterController;
	private Animator m_Animator;
	[SyncVar] private MoveDirection m_MoveDirection;

	[SyncVar] private bool canFire = true;
	[SyncVar] public int Score = 0;
	private bool shouldFire = false;

	private Vector3 m_SpawnLocation;
	private Quaternion m_SpawnRotation;

	public void Die(bool fromPlayer) {
		if(!isServer) {
			return; //Shine
		}

		if(fromPlayer) {
			Score = (Score > 0) ? Score - 1 : 0; 
		}

		transform.position = m_SpawnLocation;
		transform.rotation = m_SpawnRotation;
	}

	void Start () {
		m_CharacterController = GetComponent<CharacterController2D>();	
		m_Animator = GetComponent<Animator>();
		StartCoroutine(SlowInputUpdate());
		if(isLocalPlayer) {
			m_VirtualCamera = GameObject.Find("VirtualCam").GetComponent<CinemachineVirtualCamera>();
			m_VirtualCamera.Follow = transform;
		}
		m_SpawnLocation = transform.position;
		m_SpawnRotation = transform.rotation;
	}

	[Command]
	public void CmdMove(Vector2 inputVector) {
		if(isServer) {
			if(inputVector.x == 1f) {
				m_MoveDirection = MoveDirection.MOVE_RIGHT;
			} else if(inputVector.x == -1f) {
				m_MoveDirection = MoveDirection.MOVE_LEFT;
			}
			if(inputVector.y == 1f) {
				m_MoveDirection = MoveDirection.MOVE_UP;
			} else if(inputVector.y == -1f) {
				m_MoveDirection = MoveDirection.MOVE_DOWN;
			}
			this.m_CharacterController.SetMovementVector(inputVector);
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if(isServer) {
			if(other.gameObject.tag == "BadGuy") {
				Die(false);
			}
		}
	}

	[Command]
	public void CmdFire() {
		Fire();
	}

	void Fire() {
		if(!canFire) {
			return;
		}
		Vector3 spawnVelocity = Vector3.zero;
		Vector3 spawnEuler = Vector3.zero;
		switch(m_MoveDirection) {
			case MoveDirection.MOVE_DOWN: spawnVelocity = Vector3.down; spawnEuler = new Vector3(0f, 0f, 90f); break;
			case MoveDirection.MOVE_UP: spawnVelocity = Vector3.up; spawnEuler = new Vector3(0f, 0f, -90f); break;
			case MoveDirection.MOVE_LEFT: spawnVelocity = Vector3.left; break;
			case MoveDirection.MOVE_RIGHT: spawnVelocity = Vector3.right; spawnEuler = new Vector3(0f, 0f, 180f); break;
		}
		Quaternion rot = Quaternion.Euler(spawnEuler);
		spawnVelocity *= 10f;
		var fireball = Instantiate(FireballPrefab, transform.position, rot);
		fireball.GetComponent<Rigidbody2D>().velocity = spawnVelocity;
		fireball.GetComponent<Fireball>().owner = GetComponent<NetworkIdentity>();
		NetworkServer.Spawn(fireball);
		canFire = false;
		StartCoroutine(FireballUpdate());
	}

	IEnumerator FireballUpdate() {
		yield return new WaitForSeconds(0.5f);
		canFire = true;
		yield return null;
	}

	IEnumerator SlowInputUpdate() {
		while(true) {
			yield return new WaitForSeconds(50f / 1000f);
			var tempVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); 
			CmdMove(tempVector);
			if(shouldFire) {
				CmdFire();
				shouldFire = false;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space)) {
			Debug.Log("FIRE!");
			shouldFire = true;
		}
	}
}
