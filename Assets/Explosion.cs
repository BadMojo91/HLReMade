using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    private Animator animator;

    public void EndExplosion() {
        Destroy(gameObject);
    }
}
