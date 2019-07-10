using UnityEngine;
using System.Collections;

public class EffectDestroy : MonoBehaviour {

	public float destroyTime;

	// Use this for initialization
	void Awake () {
		Destroy (gameObject, destroyTime);
	}
}
