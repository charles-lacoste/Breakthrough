using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public GameObject _player;

    public float _lookDistance;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log(CanSeePlayer());
    }

    private bool CanSeePlayer()
    {
        Vector3 dir = _player.transform.position - transform.position;
        RaycastHit hit;
        dir = new Vector3(dir.x, dir.y + 2f, dir.z);
        Debug.DrawRay(transform.position, dir);
        if (Physics.Raycast(transform.position, dir, out hit, _lookDistance))
        {
            if (hit.collider.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }
}