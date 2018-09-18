using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StupidSpawner : MonoBehaviour {

	[SerializeField] private Transform m_SpawnTransform;
	[SerializeField] private Transform m_TargetTransform;
	[SerializeField] private GameObject m_StupidPrefab;
	[SerializeField] private float m_StupidSpawnRate = 30f;

	// Use this for initialization
	void Start () {
		StartCoroutine(StupidSpawnTimer());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void SpawnStupid() {
		var stupidObject = Instantiate(
			m_StupidPrefab,
			m_SpawnTransform.position,
			m_SpawnTransform.rotation
		);
		var badGuyScript = stupidObject.GetComponent<StupidBadGuy>() as StupidBadGuy;
		badGuyScript.Destination = m_TargetTransform;
	}

	private IEnumerator StupidSpawnTimer() {
		while(true) {
			SpawnStupid();
			yield return new WaitForSeconds(m_StupidSpawnRate);
		}
	}
}
