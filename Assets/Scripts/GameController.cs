using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController gc;
    [SerializeField]
    private Camera _menuCam;
    [SerializeField]
    private Canvas _canvas;
    private List<GameObject> _infantries, _snipers;
    [SerializeField]
    private GameObject[] _scoutSpawns, _patrolPoints, _coverPoints, _sniperPoints, _reinforcementPoints, _gasCanSpawn;
    [SerializeField]
    private GameObject _sniper, _infantry, _scout, _player, _gas;
    [SerializeField]
    private Transform _playerSpawn;

    void Awake() {
        if (gc == null)
            gc = this;
    }

    void Start() {

    }

    void Update() {

    }

    public void StartGame() {
        _canvas.gameObject.SetActive(false);
        _menuCam.gameObject.SetActive(false);
        GetComponent<AudioListener>().enabled = false;
        Instantiate(_player, _playerSpawn.position, Quaternion.identity);
        for (int i = 0; i < 9; ++i) {
            Instantiate(_infantry, _patrolPoints[i].transform.position, Quaternion.identity);
        }
        Instantiate(_sniper, _sniperPoints[0].transform.position, Quaternion.identity);
        Instantiate(_sniper, _sniperPoints[1].transform.position, Quaternion.identity);
        GameObject s = Instantiate(_scout, _scoutSpawns[0].transform.position, _scoutSpawns[0].transform.rotation);
        s.GetComponent<ScoutAI>().LookLeft();
        Instantiate(_scout, _scoutSpawns[1].transform.position, _scoutSpawns[1].transform.rotation);
        List<int> nums = new List<int>();
        for (int i = 0; i < 3; ++i) {
            int num = Random.Range(0, 5);
            while (nums.Contains(num)) {
                num = Random.Range(0, 5);
            }
            nums.Add(num);
            Instantiate(_gas, _gasCanSpawn[num].transform.position, Quaternion.identity);
        }

    }

    public void Exit() {
        Application.Quit();
    }

    public void EndGame(bool completed) {
        if (completed) { //git to da choppa
            Debug.Log("ok");
        } else { //u deded

        }
    }

    #region Gets
    public GameObject[] GetPatrolPoints() {
        return _patrolPoints;
    }

    public GameObject[] GetSniperPoints() {
        return _sniperPoints;
    }

    public GameObject[] GetCoverPoints() {
        return _coverPoints;
    }
    #endregion
}
