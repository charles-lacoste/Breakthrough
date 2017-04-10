using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class InfantryAI : MonoBehaviour {
    private NavMeshAgent agent;
    private bool _patrol, _getToCover;
    private List<GameObject> _patrolPoints, _coverPoints;
    private List<Vector3> _recentPatrolPoints;
    private int _health;
    private float _lookDistance, _fieldOfView;
    private GameObject _player;

    void Start() {
        _player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        _recentPatrolPoints = new List<Vector3>();
        GameObject[] p = GameObject.FindGameObjectsWithTag("Patrol");
        _patrolPoints = new List<GameObject>(p);
        //GameObject[] c = GameObject.FindGameObjectsWithTag("Cover");
        //_coverPoints = new List<GameObject>(c);
        _patrol = true;
        _getToCover = false;
        _health = 10;
        _fieldOfView = 30.0f;
        _lookDistance = 35.0f;
    }

    void Update() {
        if (_getToCover && agent.remainingDistance == 0) {
            _getToCover = false;
            //reload
        } else {
            if (_patrol && agent.remainingDistance == 0) 
                GoToClosestPatrolPoint();
            if (CanSeePlayer()) {
                _patrol = false;
                //shoot
            }
        }
    }

    private bool CanSeePlayer() {
        Vector3 dir = _player.transform.position - transform.position;
        dir = new Vector3(dir.x, dir.y + 2f, dir.z);
        RaycastHit hit;
        if (Vector3.Angle(new Vector3(dir.x, 0.0f, dir.z), transform.forward) < _fieldOfView * 0.5f) {
            if (Physics.Raycast(transform.position, dir, out hit, _lookDistance)) {
                if (hit.collider.tag == "Player") {
                    return true;
                }
            }
        }
        return false;
    }

    void GoToClosestPatrolPoint() {
        Vector3 closestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        foreach(GameObject p in _patrolPoints) {
            if (Vector3.Distance(p.transform.position, transform.position) < Vector3.Distance(closestPoint, transform.position)
                && !_recentPatrolPoints.Contains(p.transform.position))
                closestPoint = p.transform.position;
        }
        agent.SetDestination(closestPoint);
        _recentPatrolPoints.Add(closestPoint);
        if (_recentPatrolPoints.Count > 3)
            _recentPatrolPoints.RemoveAt(0);
    }

    void GetToCover() {
        _getToCover = true;
        Vector3 closestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        foreach (GameObject p in _patrolPoints) {
            if (Vector3.Distance(p.transform.position, transform.position) < Vector3.Distance(closestPoint, transform.position))
                closestPoint = p.transform.position;
        }
        agent.SetDestination(closestPoint);
    }

    public void TakeDamage(int value) {
        _health -= value;
        if (_health < 3 && !_getToCover)
            GetToCover();
        if (_health < 1) {
            Destroy(gameObject);
        }
    }

    /*
     Sniper alerts, walk towards player - get in line of sight stop moving and shoot
     shoot - get to cover when low on life or ammo
     */
}