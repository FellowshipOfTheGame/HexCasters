using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockStrikeAnimController : MonoBehaviour {

	HexCell orig;
	HexCell dest;
	float time = 0.0f;
	int totalDist;

	[Tooltip("Seconds spent traversing a single cell's distance")]
	public float timePerDistUnit = 0.5f;

	public float rotSpeed = 360f;

	void Update() {
		time += Time.deltaTime;
		transform.Rotate(Vector3.forward, rotSpeed * Time.deltaTime);
		print(transform.rotation);
		transform.position = Vector3.Lerp(
			orig.transform.position,
			dest.transform.position,
			time / (timePerDistUnit * totalDist));
		if (time > timePerDistUnit * totalDist) {
			Destroy(gameObject);
		}
	}

	public void Init(HexCell from, HexCell to) {
		transform.position = from.transform.position;
		orig = from;
		dest = to;
		print("from " + from.position + " to " + to.position);
		// if we add a trail later
		// transform.LookAt(dest.transform);

		totalDist = from.ManhattanDistanceTo(to);
	}
}
