using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : MonoBehaviour
{
    public GameObject _player;

    public int _health, _fieldOfView;
    public float _lookDistance, _rotationSpeed;

    private RaycastHit hit;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        //CanSeePlayer();
        LookAround();
        CanSeePlayer();
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
        Vector3 dir = (_player.transform.position - transform.position).normalized;

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

    //Draw FOV of enemy
    private void OnDrawGizmosSelected()
    {
        float halfFOV = _fieldOfView * 0.5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position, leftRayDirection * _lookDistance);
        Gizmos.DrawRay(transform.position, rightRayDirection * _lookDistance);
    }
}