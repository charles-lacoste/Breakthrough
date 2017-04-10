using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SniperAI : MonoBehaviour
{
    [SerializeField]
    private int _health, _fieldOfView, _damage;

    [SerializeField]
    private float _lookDistance, _rotationSpeed, _fireRate, _scoutTime;

    private GameObject _player;
    private RaycastHit hit;
    private NavMeshAgent _navAgent;
    private List<Vector3> _recentDestinations;

    private bool _alerted, _lookingLeft, _patrolling;
    private float _timeLastShot, _maxScoutTime;

    // Use this for initialization
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _navAgent = GetComponent<NavMeshAgent>();
        _recentDestinations = new List<Vector3>();
        _health = 10;
        _fieldOfView = 10;
        _damage = 5;
        _lookDistance = 60;
        _rotationSpeed = 0.2f;
        _fireRate = 1.5f;
        _scoutTime = 5;
        _maxScoutTime = _scoutTime;
    }

    //If going to cover, raycast towards player, if sees then go to next cover
    // Update is called once per frame
    private void Update()
    {
        /*  Alerted
         *      -> Shoot
         *          -> Reload
         *      -> Tries following player
         *          -> If Out of sight, not alert
         *          -> Shoot
         *
         *  Not Alerted
         *      -> Not patrolling
         *          -> Patrol to points
         *      -> Done patrolling
         *          -> LookAround
         *              -> Finds Player
         *                  -> Alerted
         *                      ->Sends Alert to Infantry within radius
         *
         *
         */
        if (_alerted)
        {
            Shoot();
        }
        else
        {
            if (_scoutTime < 0)
            {
                Patrol();
            }
            else
            {
                if (DestinationReached())
                {
                    LookAround();
                    if (CanSeePlayer())
                    {
                        Alert();
                        //InfantryAI[] infantries = FindObjectsOfType<InfantryAI>();
                        //foreach(var i in infantries)
                        //{
                        //    i.Alert();
                        //}
                        _scoutTime = _maxScoutTime;
                        return;
                    }
                    _scoutTime -= Time.deltaTime;
                }
            }
        }
    }

    private void Shoot()
    {
        Vector3 dir = _player.transform.position - transform.position;
        dir = new Vector3(dir.x, dir.y + 1f, dir.z);

        if (Time.time > _fireRate + _timeLastShot)
        {
            if (Vector3.Angle(new Vector3(dir.x, 0.0f, dir.z), transform.forward) < _fieldOfView * 0.5)
                if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), dir, out hit, _lookDistance))
                {
                    if (hit.collider.tag == "Player")
                    {
                        hit.transform.SendMessage("TakeDamage", _damage, SendMessageOptions.DontRequireReceiver);
                    }
                }
            _timeLastShot = Time.time;
        }
        _alerted = false;
    }

    private bool CanSeePlayer()
    {
        Vector3 dir = _player.transform.position - transform.position;
        dir = new Vector3(dir.x, dir.y + 1f, dir.z);

        if (Vector3.Angle(new Vector3(dir.x, 0.0f, dir.z), transform.forward) < _fieldOfView * 0.5)
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), dir, out hit, _lookDistance))
            {
                if (hit.collider.tag == "Player")
                {
                    Debug.Log("Player in FOV");
                    return true;
                }

                //return (hit.transform.CompareTag("Player"));
            }
        }
        Debug.Log("Can't see Player");
        return false;
    }

    private void Patrol()
    {
        _scoutTime = _maxScoutTime;
        GameObject[] patrolpts = GameObject.FindGameObjectsWithTag("Sniper Patrol");
        Vector3 closestpt = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

        foreach (var p in patrolpts)
        {
            if (Vector3.Distance(p.transform.position, transform.position) < Vector3.Distance(closestpt, transform.position) && !_recentDestinations.Contains(p.transform.position))
            {
                closestpt = p.transform.position;
            }
        }

        _navAgent.SetDestination(closestpt);
        _recentDestinations.Add(closestpt);
        if (_recentDestinations.Count > 1)
            _recentDestinations.RemoveAt(0);
    }

    private bool DestinationReached()
    {
        return _navAgent.remainingDistance == 0;
    }

    public void LookAround()
    {
        if (_lookingLeft)
        {
            transform.Rotate(0, -0.25f, 0);
            if (transform.eulerAngles.y > 120.0f && transform.eulerAngles.y < 130.0f)
                _lookingLeft = false;
        }
        else
        {
            transform.Rotate(0, 0.25f, 0);
            if (transform.eulerAngles.y > 220.0f && transform.eulerAngles.y < 230.0f)
                _lookingLeft = true;
        }
    }

    public void LookAtTarget()
    {
        Vector3 dir = (_player.transform.position - transform.position);
        dir = new Vector3(dir.x, dir.y + 2f, dir.z).normalized;

        dir = Vector3.Slerp(transform.forward, dir, _rotationSpeed * Time.deltaTime);
        dir += transform.position;
        transform.LookAt(dir);
    }

    public void TakeDamage(int value)
    {
        _health -= value;
        Debug.Log(_health);
        if (_health < 1)
        {
            Destroy(gameObject);
        }
    }

    public void Alert()
    {
        _alerted = true;
    }

    //Draw FOV of enemy
    private void OnDrawGizmosSelected()
    {
        float halfFOV = _fieldOfView * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward * _lookDistance;
        Vector3 rightRayDirection = rightRayRotation * transform.forward * _lookDistance;
        Gizmos.DrawRay(transform.position, leftRayDirection * _lookDistance);
        Gizmos.DrawRay(transform.position, rightRayDirection * _lookDistance);
    }
}