using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    private Rigidbody _rb;
    private Animator _anim;
    private CharacterController _cc;
    private Camera _cam;
    private Transform _gunpoint;

    private int _health;
    private float _speed, _sprintSpeed, _gravity, _jumpspeed, _fireRate, _timeLastShot;
    private bool _jumping;

    private Ray ray;
    private RaycastHit hit;
    private int _collectibles;

    // Use this for initialization
    private void Start() {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _cc = GetComponent<CharacterController>();
        _cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _gunpoint = GameObject.Find("GunPoint").transform;
        _health = 20;
        _speed = 4.0f;
        _sprintSpeed = 8.0f;
        _gravity = 500.0f;
        _jumpspeed = 300f;
        _fireRate = 0.25f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update() {
        Aim();
        Move();
        Shoot();
    }

    private void Move() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical);

        if (!Mathf.Approximately(vertical, 0.0f) || !Mathf.Approximately(horizontal, 0.0f)) {
            movement = transform.TransformDirection(movement);
            if (Input.GetKey(KeyCode.LeftShift))
                movement *= _sprintSpeed;
            else
                movement *= _speed;
            //transform.Translate(movement * _speed * Time.deltaTime, Space.Self);
            _anim.SetBool("Running", true);
            //if (!_footsteps.isPlaying)
            //    _footsteps.Play();
        } else {
            //_rb.velocity = Vector3.zero;
            _anim.SetBool("Running", false);
        }
        if (_cc.isGrounded && Input.GetKeyDown(KeyCode.Space) && !_jumping)
            StartCoroutine(Jump());
        if (_jumping)
            movement.y += _jumpspeed * Time.deltaTime;
        else
            movement.y -= _gravity * Time.deltaTime;
        _cc.Move(movement * Time.deltaTime);
    }

    private void Aim() {
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

    private void Shoot() {
        if (Input.GetMouseButton(0) && Time.time > _fireRate + _timeLastShot) {
            ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out hit, 1000 /*Mathf.Infinity */)) {
                if (hit.collider.tag == "Enemy") {
                    hit.transform.SendMessage("TakeDamage", 2, SendMessageOptions.DontRequireReceiver);
                }
                //Debug.Log(hit.collider.name);
                //Debug.DrawLine(transform.position, hit.transform.position, Color.red);
            }
            _timeLastShot = Time.time;
        }
    }

    private IEnumerator Jump() {
        _jumping = true;
        yield return new WaitForSeconds(0.35f);
        _jumping = false;
    }

    public void TakeDamage(int value) {
        _health -= value;
        Debug.Log(_health);
        if (_health < 1) {
            Debug.Log("DEAD");
        }
    }

    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Death")) {
            Debug.Log("Dead");
            transform.position = GameObject.Find("PlayerSpawn").transform.position;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Collectible")) {
            _collectibles++;
            Destroy(col.gameObject);
        }
    }
}