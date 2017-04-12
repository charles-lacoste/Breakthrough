using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
    private Animator _anim;
    private CharacterController _cc;
    private Camera _cam;
    private ShooterGameCamera _camScript;
    private Transform _gunpoint;

    private int _health;
    private float _speed, _sprintSpeed, _gravity, _jumpspeed, _fireRate, _timeLastShot;
    private bool _jumping;

    private Ray ray;
    private RaycastHit hit;
    private int _collectibles;

    [SerializeField]
    private AudioSource _walkingSrc, _shootingSrc, _auxSrc;
    [SerializeField]
    private AudioClip _hitmarkerFx, _reloadFx, _shootFx, _hurtFx, _jumpFx, _footStep;

    private void Start() {

        _anim = GetComponent<Animator>();
        _cc = GetComponent<CharacterController>();
        _cam = GetComponentInChildren<Camera>();
        _camScript = _cam.gameObject.GetComponent<ShooterGameCamera>();
        _gunpoint = GameObject.Find("GunPoint").transform;
        _health = 20;
        _speed = 4.0f;
        _sprintSpeed = 8.0f;
        _gravity = 500.0f;
        _jumpspeed = 300f;
        _fireRate = 0.25f;
        AudioSource[] audioSrcArray = GetComponentsInChildren<AudioSource>();
        _walkingSrc = audioSrcArray[0];
        _shootingSrc = audioSrcArray[1];
        _auxSrc = audioSrcArray[2];
        _shootingSrc.clip = _shootFx;

        Cursor.lockState = CursorLockMode.Locked;
    }

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
            if (!_walkingSrc.isPlaying) {
                _walkingSrc.clip = _footStep;
                _walkingSrc.Play();
            }
            if (Input.GetKey(KeyCode.LeftShift))
                movement *= _sprintSpeed;
            else
                movement *= _speed;
            _anim.SetBool("Running", true);
            //if (!_footsteps.isPlaying)
            //    _footsteps.Play();
        } else {
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
        if (Input.GetMouseButton(1)) {
            _cam.fieldOfView = 20;
            _camScript.SetHorizontalSensitivity(200f);
        } else {
            _cam.fieldOfView = 60;
            _camScript.SetHorizontalSensitivity(500f);
        }
    }

    private void Shoot() {
        if (Input.GetMouseButton(0) && Time.time > _fireRate + _timeLastShot) {
            _shootingSrc.Play();
            ray = _cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out hit, 1000)) {
                if (hit.collider.tag == "Enemy") {
                    _auxSrc.clip = _hitmarkerFx;
                    if (!_auxSrc.isPlaying)
                        _auxSrc.Play();
                    hit.transform.SendMessage("TakeDamage", 2, SendMessageOptions.DontRequireReceiver);
                }
            }
            _timeLastShot = Time.time;
        }
    }

    private IEnumerator Jump() {
        _jumping = true;
        _auxSrc.clip = _jumpFx;
        if (!_auxSrc.isPlaying)
            _auxSrc.Play();
        yield return new WaitForSeconds(0.35f);
        _jumping = false;
    }

    public void TakeDamage(int value) {
        _auxSrc.clip = _hurtFx;
        if (!_auxSrc.isPlaying)
            _auxSrc.Play();
        _health -= value;
        if (_health < 1) {
            GameController.gc.EndGame(false);
        }
    }

    public int CollectibleCount() {
        return _collectibles;
    }

    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Death")) {
            _auxSrc.clip = _hurtFx;
            _auxSrc.Play();
            transform.position = GameObject.Find("PlayerSpawn").transform.position;
        } else if (col.gameObject.layer == LayerMask.NameToLayer("Collectible")) {
            _collectibles++;
            Destroy(col.gameObject);
        } else if (_collectibles == 3 && col.gameObject.tag == "Helicopter")
            GameController.gc.EndGame(true);
    }
}