using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject bullet;

    private Rigidbody _rb;
    private Animator _anim;
    private CharacterController _cc;
    private Camera _cam;

    private float _speed;

    // Use this for initialization
    private void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _cc = GetComponent<CharacterController>();
        _cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        _speed = 4.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Aim();
        Move();
        Shoot();
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical);

        if (!Mathf.Approximately(vertical, 0.0f) || !Mathf.Approximately(horizontal, 0.0f))
        {
            //if (_cc.isGrounded)
            {
                movement = transform.TransformDirection(movement);
                movement *= _speed;
                //transform.Translate(movement * _speed * Time.deltaTime, Space.Self);
                _anim.SetBool("Running", true);
                //if (!_footsteps.isPlaying)
                //    _footsteps.Play();
            }
        }
        else
        {
            //_rb.velocity = Vector3.zero;
            _anim.SetBool("Running", false);
        }
        _cc.Move(movement * Time.deltaTime);
    }

    void Aim()
    {
        transform.forward = _cam.transform.forward;
        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);

        /*
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.z, 0);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float angle = Mathf.Atan2(lookPos.z, lookPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.down); // Turns Right
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up); //Turns Left
        */
    }

    void Shoot()
    {
        if (Input.GetMouseButton(0))
        {
            //Instantiate(bullet, transform.forward, Quaternion.identity);
            //GameObject c = (GameObject)Instantiate(bullet, _gunpos.transform.position, _gunpos.transform.rotation);
            //GameObject c = (GameObject)Instantiate(bullet, _cam.transform.forward, _cam.transform.rotation);
        }
        else
        {
        }
    }
}