using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Fireball : NetworkBehaviour {

	// Use this for initialization

	public NetworkIdentity owner;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// Sent when an incoming collider makes contact with this object's
	/// collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void OnCollisionEnter2D(Collision2D other)
	{
		if(isServer) {
			var player = owner.gameObject.GetComponent<Player>() as Player;
			if(other.gameObject.tag == "Collision") {
				NetworkServer.Destroy(this.gameObject);
			}
			if(other.gameObject.tag == "BadGuy") {
				player.Score++;
				NetworkServer.Destroy(this.gameObject);
				NetworkServer.Destroy(other.gameObject);
			}
			if(other.gameObject.tag == "Player") {
				var otherPlayer = other.gameObject.GetComponent<Player>() as Player;
				if(otherPlayer.GetComponent<NetworkIdentity>() != owner) {
					player.Die(true);
				}
			}
		}
	}
}