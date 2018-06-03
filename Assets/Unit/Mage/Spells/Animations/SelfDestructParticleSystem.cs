using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructParticleSystem : MonoBehaviour {

	ParticleSystem sys;

	void Start () {
		sys = GetComponent<ParticleSystem>();
	}
	
	void Update () {
		if (!sys.IsAlive()) {
			Destroy(gameObject);
		}
		print("oi");
	}
}
