using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelElement //Single level
{
    public string _character;
    public GameObject _prefab;
}

public class LevelBuilder : MonoBehaviour
{
    public int _currentLevel;
    public List<LevelElement> _lvlElements;
    private Level _level;

    GameObject GetPrefab(char c)
    {
        LevelElement elm = _lvlElements.Find(le => le._character == c.ToString());
        if (elm != null)
        {
            return elm._prefab;
        }
        else
        {
            return null;
        }
    }

    public void NextLevel()
    {
        _currentLevel++;
        if (_currentLevel >= GetComponent<LevelManager>()._levels.Count)
        {
            _currentLevel = 0; //go back to first level
        }
    }

    public void BuildLevel()
    {
        _level = GetComponent<LevelManager>()._levels[_currentLevel];
        Debug.Log("Tryig to build...");
        int startX = -_level.Width / 2; // center of screen should be center of level
        int x = startX;
        int y = -_level.Height / 2;
        Debug.Log("Rows are: " + _level._rows.Count.ToString());
        foreach (var row in _level._rows)
        {
            foreach (var ch in row)
            {
                Debug.Log(ch);
                GameObject prefab = GetPrefab(ch);
                if (prefab)
                {
                    Debug.Log("Instantiating prefab...");
                    Debug.Log(prefab.name);
                    Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                x++;
            }
            y++; //new line
            x = startX;//reset x
        }
    }
}
