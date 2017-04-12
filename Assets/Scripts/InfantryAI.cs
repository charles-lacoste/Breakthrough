using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class InfantryAI : MonoBehaviour {
    private NavMeshAgent _agent;
    public bool _patrol, _getToCover, _attacking, _reloading, _lookAtPlayer;
    private List<Vector3> _recentPatrolPoints;
    private int _health, _damage, _ammo;
    private float _lookDistance, _fieldOfView, _rotationSpeed, _timeLastShot, _fireRate, _waitTime, _lastSpotted;
    private GameObject _player;
    private Animator _anim;

    void Start() {
        _player = GameObject.FindGameObjectWithTag("Player");
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _recentPatrolPoints = new List<Vector3>();
        _patrol = true;
        _getToCover = _reloading = false;
        _health = 10;
        _fireRate = 1.0f;
        _fieldOfView = 100.0f;
        _lookDistance = 35.0f;
        _rotationSpeed = 2.0f;
        _damage = 2;
        _ammo = 10;
        _waitTime = 5.0f;
    }

    void Update() {
        if (!GameController.gc.IsGamePaused()) {
            if (_attacking) {
                LookAtTarget();
                Shoot();
            } else if (_getToCover && _agent.remainingDistance < 0.5f) {
                _agent.ResetPath();
                _getToCover = false;
                _agent.speed = 7;
                _patrol = true;
                if (_ammo == 0)
                    StartCoroutine(Reload());
            } else if (_lookAtPlayer) {
                LookAtPlayer();
            } else if (!_reloading) {
                if (_patrol && _agent.remainingDistance < 0.5f) {
                    _agent.ResetPath();
                    GoToClosestPatrolPoint();
                }
                if (CanSeePlayer()) {
                    _agent.Stop();
                    _anim.SetBool("Running", false);
                    _patrol = false;
                    _attacking = true;
                    _timeLastShot = Time.time; //Wait before shooting, give player a chance
                }
            }
        }
    }

    public void Alert() {
        _patrol = false;
        _agent.SetDestination(_player.transform.position);
    }

    private void Shoot() {
        if (Time.time > _fireRate + _timeLastShot) {
            Vector3 dir = _player.transform.position - transform.position;
            dir = new Vector3(dir.x, dir.y + 2f, dir.z);
            RaycastHit hit;
            if (Vector3.Angle(new Vector3(dir.x, 0.0f, dir.z), transform.forward) < _fieldOfView * 0.5) {
                if (Physics.Raycast(transform.position, dir, out hit, _lookDistance)) {
                    if (hit.collider.tag == "Player") {
                        hit.transform.SendMessage("TakeDamage", _damage, SendMessageOptions.DontRequireReceiver);
                        _lastSpotted = Time.time;
                    }
                    --_ammo;
                    if (_ammo == 0) {
                        _getToCover = true;
                        _attacking = false;
                    }
                }
            }
            _timeLastShot = Time.time;
            if (Time.time > _waitTime + _lastSpotted) {
                _patrol = true;
                _attacking = false;
                _ammo = 10; //Assume reload while patrolling
                _agent.Resume();
                _anim.SetBool("Running", true);
            }
        }
    }

    private IEnumerator Reload() {
        _reloading = true;
        yield return new WaitForSeconds(2.0f);
        _ammo = 10;
        _reloading = false;
    }

    private bool CanSeePlayer() {
        Vector3 dir = _player.transform.position - transform.position;
        dir = new Vector3(dir.x, dir.y + 2f, dir.z);
        RaycastHit hit;
        if (Vector3.Angle(new Vector3(dir.x, 0.0f, dir.z), transform.forward) < _fieldOfView * 0.5f) {
            if (Physics.Raycast(transform.position, dir, out hit, _lookDistance)) {
                if (hit.collider.tag == "Player") {
                    _lastSpotted = Time.time;
                    return true;
                }
            }
        }
        return false;
    }

    private void LookAtPlayer() {
        transform.LookAt(_player.transform.position);
        Vector3 dir = _player.transform.position - transform.position;
        dir = new Vector3(dir.x, dir.y + 2f, dir.z);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, 100)) {
            if (hit.collider.tag == "Player") {
                _lastSpotted = Time.time;
            }
        }
        if (Time.time > _waitTime + _lastSpotted) {
            _lookAtPlayer = false;
            _agent.Resume();
            _anim.SetBool("Running", true);
        }
    }

    private void GoToClosestPatrolPoint() {
        Vector3 closestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        foreach (GameObject p in GameController.gc.GetPatrolPoints()) {
            if (Vector3.Distance(p.transform.position, transform.position) < Vector3.Distance(closestPoint, transform.position)
                && !_recentPatrolPoints.Contains(p.transform.position))
                closestPoint = p.transform.position;
        }
        _agent.SetDestination(closestPoint);
        _anim.SetBool("Running", true);
        _recentPatrolPoints.Add(closestPoint);
        if (_recentPatrolPoints.Count > 5)
            _recentPatrolPoints.RemoveAt(0);
    }

    private void GetToCover() {
        _agent.speed = 8f;
        _getToCover = true;
        Vector3 closestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        Vector3 dir = _player.transform.position - transform.position;
        RaycastHit hit;
        foreach (GameObject p in GameController.gc.GetCoverPoints()) {
            if (Vector3.Distance(p.transform.position, transform.position) < Vector3.Distance(closestPoint, transform.position)) {
                Physics.Raycast(transform.position, dir, out hit);
                if (hit.collider.tag != "Player")
                    closestPoint = p.transform.position;
            }
        }
        _agent.SetDestination(closestPoint);
    }

    private void LookAtTarget() {
        Vector3 dir = _player.transform.position - transform.position;
        dir = new Vector3(dir.x, dir.y + 2f, dir.z);
        dir = Vector3.Slerp(transform.forward, dir, _rotationSpeed * Time.deltaTime);
        dir += transform.position;
        transform.LookAt(dir);
    }

    public void TakeDamage(int value) {
        _health -= value;
        _lookAtPlayer = true;
        _lastSpotted = Time.time;
        _agent.Stop();
        _anim.SetBool("Running", false);
        if (_health < _health / 2 && !_getToCover)
            GetToCover();
        if (_health < 1) {
            GameController.gc.RemoveInfantry(gameObject);
            Instantiate(GameController.gc.GetExplosion(), new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z), Quaternion.identity);
            Destroy(gameObject);
        }
    }
}