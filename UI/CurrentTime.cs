using TMPro;
using UnityEngine;

public class CurrentTime : MonoBehaviour
{

    public TextMeshProUGUI timeText;
    public float gameTime = 0f;


    // Update is called once per frame
    void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        gameTime = ScoreManager.Instance.clearTime;

        int minute = Mathf.FloorToInt(gameTime / 60);
        int second = Mathf.FloorToInt(gameTime % 60);
        int millisecond = Mathf.FloorToInt(gameTime * 100) % 100;

        timeText.text = string.Format("{0:00}:{1:00}.{2:00}", minute, second, millisecond);
    }

}
