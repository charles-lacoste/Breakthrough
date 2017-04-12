using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public static GameController gc = null;
    [SerializeField]
    private Camera _menuCam;
    [SerializeField]
    private Canvas _mainMenuCanvas;
    [SerializeField]
    private Text _contextText;
    private List<GameObject> _infantries, _snipers;
    [SerializeField]
    private GameObject[] _scoutSpawns, _patrolPoints, _coverPoints, _sniperPoints, _reinforcementPoints, _gasCanSpawn;
    [SerializeField]
    private GameObject _sniper, _infantry, _scout, _player, _gas;
    [SerializeField]
    private Transform _playerSpawn;
    [SerializeField]
    private GameObject _explosion;
    private bool _paused;

    void Awake() {
        if (gc == null)
            gc = this;
    }

    void Start() {
        _infantries = new List<GameObject>();
    }

    public void StartGame() {
        GetComponent<AudioListener>().enabled = false;
        _mainMenuCanvas.gameObject.SetActive(false);
        _menuCam.gameObject.SetActive(false);
        Instantiate(_player, _playerSpawn.position, Quaternion.identity);
        List<int> nbs = new List<int>();
        for (int i = 0; i < 9; ++i) {
            int r = Random.Range(0, _patrolPoints.Length - 1);
            while (nbs.Contains(r)) {
                r = Random.Range(0, _patrolPoints.Length - 1);
            }
            nbs.Add(r);
            _infantries.Add(Instantiate(_infantry, _patrolPoints[r].transform.position, Quaternion.identity));
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

    public IEnumerator EndGame(bool completed) {
        if (completed) { //git to da choppa
            Debug.Log("ok");
        } else { //u deded
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            _paused = true;
            yield return new WaitForSeconds(3.0f);
            SceneManager.LoadScene("GameScene");
        }
    }

    public bool IsGamePaused() {
        return _paused;
    }

    public void RemoveInfantry(GameObject infantry) {
        _infantries.Remove(infantry);
        if (_infantries.Count == 3) {
            for (int i = 0; i < 3; ++i) {
                _infantries.Add(Instantiate(_infantry, _reinforcementPoints[i].transform.position, Quaternion.identity));
                _infantries.Add(Instantiate(_infantry, _reinforcementPoints[i].transform.position + new Vector3(4, 0, 4), Quaternion.identity));
            }
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

    public GameObject GetExplosion() {
        return _explosion;
    }
    #endregion
}
