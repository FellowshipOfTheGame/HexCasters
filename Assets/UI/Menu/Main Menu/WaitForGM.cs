using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForGM : MonoBehaviour {
    public List<GameObject> objects;

    public bool done;

    void Start() {
        GameManager.AfterInit(() => done = true);
        StartCoroutine(Wait());
    }

    IEnumerator Wait() {
        while (!done) {
            yield return null;
        }
        foreach (var obj in objects) {
            obj.SetActive(true);
        }
    }
}
