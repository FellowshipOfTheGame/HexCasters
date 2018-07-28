using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelAligned : MonoBehaviour {

	private SpriteRenderer rend;

	void Awake() {
		rend = GetComponent<SpriteRenderer>();
	}

	void LateUpdate() {
		if (rend.sprite != null) {
			Vector3 newPosition = Vector3.zero;
			float ppu = rend.sprite.pixelsPerUnit;

			newPosition.x = (Mathf.Round(transform.position.x * ppu) / ppu);
			newPosition.y = (Mathf.Round(transform.position.y * ppu) / ppu);

			transform.position = newPosition;
		}
	}
}
