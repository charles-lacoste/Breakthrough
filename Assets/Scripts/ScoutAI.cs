using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutAI : MonoBehaviour {
    private GameObject _player;

    public int _health, _fieldOfView;
    public float _lookDistance, _rotationSpeed;
    [SerializeField]
    private bool _lookingLeft, _alerted;
    private RaycastHit hit;
    private Quaternion _leftQ, _rightQ;

    // Use this for initialization
    private void Start() {
        _leftQ = new Quaternion(0, -220.0f, 0, 0);
        _rightQ = new Quaternion(0, -140.0f, 0, 0);
        _player = GameObject.FindGameObjectWithTag("Player");
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
        dir = new Vector3(dir.x, dir.y + 2f, dir.z);
        
        if (Vector3.Angle(new Vector3(dir.x, 0.0f, dir.z), transform.forward) < _fieldOfView * 0.5) {
            if (Physics.Raycast(transform.position, dir, out hit, _lookDistance)) {
                if (hit.collider.tag == "Player") {
                    //Call sniper's alert
                    SniperAI[] snipers = FindObjectsOfType<SniperAI>();
                    foreach (SniperAI s in snipers) {
                        //call s.Alert();
                    }
                    return true;
                }
            }
        }
        Debug.Log("Can't see Player");
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
        Debug.Log(_health);
        if (_health < 1) {
            Destroy(gameObject);
        }
    }

    //Draw FOV of enemy
    private void OnDrawGizmosSelected() {
        float halfFOV = _fieldOfView * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position, leftRayDirection * _lookDistance);
        Gizmos.DrawRay(transform.position, rightRayDirection * _lookDistance);
    }
}