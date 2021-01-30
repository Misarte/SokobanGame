using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level //Single level
{
    public List<string> _rows = new List<string>(); //rows of text define level

    public int Height
    {
        get
        {
            return _rows.Count; //Height of Level is defined by number of rows of text file
        }
    }
    public int Width
    {
        get
        {
            int maxLength = 0;
            foreach (var row in _rows)
            {
                if(row.Length > maxLength)
                {
                    maxLength = row.Length;// Longest line defines width of level so we need to find the max and return it
                }
            }
            return maxLength;
        }
    }
}

public class LevelManager : MonoBehaviour
{
    public string _file;
    public List<Level> _levels;

    private void Awake()
    {
        TextAsset text = (TextAsset)Resources.Load(_file);
        if(!text)
        {
            Debug.Log("Levels file:" + _file + ".txt does not exist!");
            return;
        }
        else
        {
            Debug.Log("Levels imported!");
        }


        string _levelsText = text.text;
        string[] lines;

        lines = _levelsText.Split(new string[] { "\n" }, System.StringSplitOptions.None); //splitting on new line
        _levels.Add(new Level());
        for(long i = 0; i < lines.LongLength; i++)
        {
            string line = lines[i];
            if(line.StartsWith(";"))
            {
                Debug.Log("New level added");
                _levels.Add(new Level());
                continue;
            }
            _levels[_levels.Count - 1]._rows.Add(line);
        }
    }
}
