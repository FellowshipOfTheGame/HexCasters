using System;
using UnityEngine;

public class SpriteSetSelector : MonoBehaviour {
    [Serializable]
    public struct TargetRender {
        public SpriteSet set;
        public SpriteRenderer renderer;
    }

    public TargetRender[] targets;

    void Start() {
        var numOfSprites = targets[0].set.sprites.Length;
        for (var i = 1; i < targets.Length; i++) {
            if (targets[i].set.sprites.Length != numOfSprites) {
                Debug.LogError(
                    "All spritesets must contain the same number of sprites");
                return;
            }
        }
        var selected = UnityEngine.Random.Range(0, numOfSprites);
        foreach (var target in targets) {
            target.renderer.sprite = target.set.sprites[selected];
        }
    }
}