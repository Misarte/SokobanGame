using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Player _player;
    private bool _readyForInput;
    private int currLvl;
    public LevelBuilder levelBuilder;
    public GameObject nextButton;
    public GameObject undoButton;
    public GameObject yayParticles;
    public Text moves;
    public Text currLevel;
    private GameObject _win_vfx;
    private bool _playedVfx;

    private void Start()
    {
        levelBuilder = FindObjectOfType<LevelBuilder>();
        ResetScene();
        _playedVfx = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // read keyboard input
        movementInput.Normalize();
        if (_player)
        {
            nextButton.SetActive(IsLevelComplete());
            if (!_playedVfx && IsLevelComplete())
            {
                GameObject win_vfx = Instantiate(yayParticles, _player.transform.position, Quaternion.identity);
                _playedVfx = true;
                Destroy(win_vfx, 10.0f);
            }
            
            if (movementInput.sqrMagnitude > 0.5) // check pressed button to move once at a time
            {
                if (_readyForInput)// only true when button pressed once
                {
                    _readyForInput = false;
                    _player.Move(movementInput);
                    nextButton.SetActive(IsLevelComplete());
                }
            }
            else
            {
                _readyForInput = true;
            }

            if (_player.num_moves > 0)
            {
                undoButton.SetActive(true);
            }
            else
            {
                undoButton.SetActive(false);
                Debug.Log("No more moves to undo!");
            }
            moves.text = "NUMBER OF MOVES: " + _player.num_moves.ToString();
            currLvl = levelBuilder._currentLevel + 1;
            currLevel.text = "CURRENT LEVEL: " + currLvl.ToString();
        }
    }

    public void NextLevel()
    {
        nextButton.SetActive(false);
        levelBuilder.NextLevel();
        StartCoroutine(ResetSceneAsync());
        ParticleSystem currentVfx = FindObjectOfType<ParticleSystem>();
        Destroy(currentVfx);
    }
    public void ResetScene()
    {
        StartCoroutine(ResetSceneAsync());
        ParticleSystem currentVfx = FindObjectOfType<ParticleSystem>();
        Destroy(currentVfx);
    }

    public void Undo()
    {
        if (_player.moves.Count > 0)
        {
            if (_player.moves.Peek().withBox)
            {
                _player.transform.position = new Vector3(_player.moves.Peek().fromPos.x, _player.moves.Peek().fromPos.y, _player.moves.Peek().fromPos.z);
                Debug.Log("Undoing to: " + _player.transform.position.ToString() + "undoing Box too!");
                _player.moves.Peek().whichBox.transform.position = new Vector3(_player.moves.Peek().boxPos.x, _player.moves.Peek().boxPos.y, _player.moves.Peek().boxPos.z);
            }
            else
            {
                _player.transform.position = new Vector3(_player.moves.Peek().fromPos.x, _player.moves.Peek().fromPos.y, _player.moves.Peek().fromPos.z);
                Debug.Log("Undoing Only Player to: " + _player.transform.position.ToString());
            }
            _player.moves.Pop();
            --_player.num_moves;
        }
        else
        {
            undoButton.SetActive(false);
            Debug.Log("No more moves to undo!");
        }
    }

    public void Quit()
    {
        Application.Quit();
        _player.moves.Clear();
        Destroy(yayParticles.gameObject);
    }

    private bool IsLevelComplete()
    {
        Box[] boxes = FindObjectsOfType<Box>();
        foreach (var box in boxes)
        {
            if (!box.arrived)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator ResetSceneAsync()
    {
        if (SceneManager.sceneCount > 1)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync("LevelScene");
            while (!asyncUnload.isDone)
            {
                yield return null;
                Debug.Log("Unloading scene...");
            }
            Debug.Log("Unload Done!");
            Resources.UnloadUnusedAssets();
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LevelScene", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
            Debug.Log("Loading scene...");
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("LevelScene"));
        Debug.Log("Build level");
        levelBuilder.BuildLevel();
        _player = FindObjectOfType<Player>();
        _player.moves.Clear();
        Debug.Log("Scene Loaded" + levelBuilder.GetComponent<LevelManager>()._levels.Count.ToString() + "Number of levels");
    }
}