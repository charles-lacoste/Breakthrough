using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperAI : MonoBehaviour
{
    public int _health, _fieldOfView, _damage;
    public float _lookDistance, _rotationSpeed, _fireRate;

    private GameObject _player;
    private RaycastHit hit;

    private bool _alerted;
    private float _timeLastShot;

    // Use this for initialization
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _health = 10;
        _fieldOfView = 10;
        _damage = 5;
        _lookDistance = 100;
        _rotationSpeed = 0.2f;
        _fireRate = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        /*  Alerted
         *      -> Shoot
         *
         *  Not Alerted
         *      -> Scout (LookAround)
         *          -> Finds Player
         *              -> Alerted
         *
         *
         */
        if (_alerted)
        {
            Shoot();
        }
        else
        {
            LookAround();
            if (CanSeePlayer())
            {
                Alert();
            }
        }
    }

    private void Shoot()
    {
        Vector3 dir = _player.transform.position - transform.position;
        dir = new Vector3(dir.x, dir.y + 2f, dir.z);

        if (Time.time > _fireRate + _timeLastShot)
        {
            if (Vector3.Angle(dir, transform.forward) < _fieldOfView * 0.5)
                if (Physics.Raycast(transform.position, dir, out hit, _lookDistance))
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
        dir = new Vector3(dir.x, dir.y + 2f, dir.z);

        //Debug.DrawRay(transform.position, dir);
        if (Vector3.Angle(dir, transform.forward) < _fieldOfView * 0.5)
        {
            if (Physics.Raycast(transform.position, dir, out hit, _lookDistance))
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

    public void LookAround()
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
            Debug.Log("DEAD");
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