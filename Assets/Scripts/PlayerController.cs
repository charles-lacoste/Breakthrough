using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    Animator _anim;
    Rigidbody _rb;
    float _speed;

    // Use this for initialization
    void Start() {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _speed = 4.0f;
    }

    // Update is called once per frame
    void Update() {
        //Aim();
        Move();
    }

    void Move() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (!Mathf.Approximately(vertical, 0.0f) || !Mathf.Approximately(horizontal, 0.0f)) {
            _rb.velocity = new Vector3(horizontal, 0, vertical) * _speed;
            _anim.SetBool("Running", true);
            //if (!_footsteps.isPlaying)
            //    _footsteps.Play();
        } else {
            _rb.velocity = Vector3.zero;
            _anim.SetBool("Running", false);
        }
    }

    void Aim() {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.z, 0);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.z, lookPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.down); // Turns Right
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up); //Turns Left
    }
}
