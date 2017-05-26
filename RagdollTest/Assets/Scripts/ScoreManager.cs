using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : Singleton<ScoreManager>
{
    // guarantee this will be always a singleton only - can't use the constructor!
    protected ScoreManager() { } 

    private int _score;
    public Component[] comps;
    public bool IsVisible = true;
    Text _textScript;
    string scoreText = "Score:";
    void Awake()
    {
        _textScript = GetComponent<Text>();
        _score = 0;
        _textScript.enabled = IsVisible;
        UpdateScore();
    }
    public void AddScore(int toAdd)
    {
        _score += toAdd;
        UpdateScore();
    }
    public void ResetScore()
    {
        _score = 0;
        UpdateScore();
    }
    void UpdateScore()
    {
        _textScript.text = string.Format("{0} {1}", scoreText, _score);
    }
}
