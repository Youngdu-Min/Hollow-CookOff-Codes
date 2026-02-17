using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            TriggerGoal();
        }
    }

    [ContextMenu("골")]
    public void TriggerGoal()
    {
        Debug.Log("골");

        ResultUI _ui = FindObjectOfType<ResultUI>();
        _ui.OpenDisplay();
        SaveDataManager.Instance.StageClear(StageSelectUI.Instance.StageIndex());
        ScoreManager.Instance.ShowScore();
    }
}
