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
    public Text moves;
    public Text currLevel;

    private void Start()
    {
        ResetScene();
    }


    // Update is called once per frame
    void Update()
    {
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // read keyboard input
        movementInput.Normalize(); 

        if(movementInput.sqrMagnitude > 0.5) // check pressed button to move once at a time
        {
            if(_readyForInput)// only true when button pressed once
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

        if(_player.num_moves > 0)
        {
            undoButton.SetActive(true);
        }
        moves.text = "Number of Moves: " + _player.num_moves.ToString();
        currLvl = levelBuilder._currentLevel + 1;
        currLevel.text = "Current Level: " + currLvl.ToString();
    }

    public void NextLevel()
    {
        nextButton.SetActive(false);
        levelBuilder.NextLevel();
        StartCoroutine(ResetSceneAsync());

    }
    public void ResetScene()
    {
        StartCoroutine(ResetSceneAsync());
    }

    public void Undo()
    {
        _player.transform.position = new Vector3(_player.moves.Peek().fromPos.x, _player.moves.Peek().fromPos.y, _player.moves.Peek().fromPos.z);
        Debug.Log("Undoing to: " + _player.transform.position.ToString());
        _player.moves.Pop();
    }

    private bool IsLevelComplete()
    {
        Box[] boxes = FindObjectsOfType<Box>();
        foreach(var box in boxes)
        {
            if(!box.arrived)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator ResetSceneAsync()
    {
        if(SceneManager.sceneCount > 1)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync("LevelScene");
            while(!asyncUnload.isDone)
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
        levelBuilder.BuildLevel();
        _player = FindObjectOfType<Player>();
        Debug.Log("Scene Loaded");

    }
 

}
