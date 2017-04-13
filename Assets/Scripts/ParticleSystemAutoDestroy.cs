using UnityEngine;
using System.Collections;

public class ParticleSystemAutoDestroy : MonoBehaviour {
    public void Start() {
        Destroy(gameObject, 1.0f);
    }
}