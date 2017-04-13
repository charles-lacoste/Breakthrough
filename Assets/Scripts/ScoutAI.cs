using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutAI : MonoBehaviour {
    private GameObject _player;
    private int _health, _fieldOfView;
    private float _lookDistance;
    private bool _lookingLeft, _alerted;

    // Use this for initialization
    private void Start() {
        _lookingLeft = false;
        _player = GameObject.FindGameObjectWithTag("Player");
        _health = 4;
        _fieldOfView = 45;
        _lookDistance = 50;
    }

    // Update is called once per frame
    private void Update() {
        LookAround();
        if (!_alerted)
            _alerted = CanSeePlayer();
    }

    private bool CanSeePlayer() {
        Vector3 dir = _player.transform.position - transform.position;
        RaycastHit hit;
        if (Vector3.Angle(new Vector3(dir.x, 0.0f, dir.z), transform.forward) < _fieldOfView * 0.5) {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 2.2f, transform.position.z), dir, out hit, _lookDistance)) {
                if (hit.collider.tag == "Player") {
                    //Call sniper's alert
                    StartCoroutine(FindObjectOfType<PlayerController>().Alert("You have been spotted!"));
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
            Instantiate(GameController.gc.GetExplosion(), new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z), Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void LookLeft() {
        _lookingLeft = true;
    }
}