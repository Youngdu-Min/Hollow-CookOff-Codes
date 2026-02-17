using MoreMountains.Tools;
using UnityEngine;
public class ButtonControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        /*
        switch (EventSystem.current.currentSelectedGameObject.name)
        {
            case "재시작":
                SceneManager.LoadScene("Nedrag_2");
                Time.timeScale = (false) ? 0.0f : 1.0f;

                var playerObj = GameObject.FindGameObjectsWithTag("Player");

                var scripts = playerObj[0].GetComponents<MonoBehaviour>();
                var scripts2 = playerObj[1].GetComponents<MonoBehaviour>();
                foreach (var scirpt2 in scripts2)
                {
                    scirpt2.enabled = false;
                }
                foreach (var scirpt in scripts)
                {
                    scirpt.enabled = false;
                }
                break;
        }  */
    }

    public void ChangeScene(string _scene)
    {
        MMSceneLoadingManager.LoadScene(_scene);
    }

    public void ClearStage()
    {
        SaveDataManager.Instance.StageClear(StageSelectUI.Instance.StageIndex());
    }
}
