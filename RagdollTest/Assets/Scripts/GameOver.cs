using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {
    Text _textComp;
    public void Awake()
    {
        _textComp = GetComponent<Text>();
        _textComp.text = string.Format("GAME OVER \n Your Score: {0} \n High Score: {1}", ScoreManager.Instance.GetScore(), ScoreManager.Instance.GetHighScore());
    }

}
