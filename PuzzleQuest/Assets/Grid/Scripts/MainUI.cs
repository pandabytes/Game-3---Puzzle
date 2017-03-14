using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour {

    public Text ScoreText;

    public void SetScoreText(int amount)
    {
        ScoreText.text = amount.ToString();
    }
}
