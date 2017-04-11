using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutAI : MonoBehaviour {
    private GameObject _player;
    private int _health, _fieldOfView;
    private float _lookDistance, _rotationSpeed;
    [SerializeField]
    private bool _lookingLeft, _alerted;
    private float height;

    // Use this for initialization
    private void Start() {
        _lookingLeft = false;
        _player = GameObject.FindGameObjectWithTag("Player");
        _health = 4;
    }

    // Update is called once per frame
    private void Update() {
        if (!_alerted) {
            LookAround();
            _alerted = CanSeePlayer();
        }
    }

    private bool CanSeePlayer() {
        Vector3 dir = _player.transform.position - transform.position;
        dir = new Vector3(dir.x, dir.y + 2.4f, dir.z);
        RaycastHit hit;
        if (Vector3.Angle(new Vector3(dir.x, 0.0f, dir.z), transform.forward) < _fieldOfView * 0.5) {
            if (Physics.Raycast(transform.position, dir, out hit, _lookDistance)) {
                if (hit.collider.tag == "Player") {
                    //Call sniper's alert
                    SniperAI[] snipers = FindObjectsOfType<SniperAI>();
                    foreach (SniperAI s in snipers) {
                        s.Alert();
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public void LookAround() {
        if (_lookingLeft) {
            transform.Rotate(0, -0.25f, 0);
            if (transform.eulerAngles.y > 120.0f && transform.eulerAngles.y < 130.0f)
                _lookingLeft = false;
        } else {
            transform.Rotate(0, 0.25f, 0);
            if (transform.eulerAngles.y > 220.0f && transform.eulerAngles.y < 230.0f)
                _lookingLeft = true;
        }
    }

    public void TakeDamage(int value) {
        _health -= value;
        if (_health < 1) {
            Destroy(gameObject);
        }
    }

    public void LookLeft() {
        _lookingLeft = true;
    }

    //Draw FOV of enemy
    private void OnDrawGizmosSelected() {
        float halfFOV = _fieldOfView * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + height, transform.position.z), leftRayDirection * _lookDistance);
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + height, transform.position.z), rightRayDirection * _lookDistance);
    }
}