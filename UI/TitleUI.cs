using UnityEngine;

public class TitleUI : MonoBehaviour
{

    static bool firstStart = true;

    [SerializeField] private GameObject[] showObjects;
    [SerializeField] private GameObject[] hideObjects;
    // Start is called before the first frame update
    void Start()
    {
        if (!firstStart)
            OpenUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
            OpenUI();
    }

    public void OpenUI()
    {
        foreach (var ui in showObjects)
            ui.SetActive(true);
        foreach (var ui in hideObjects)
            ui.SetActive(false);

        gameObject.SetActive(false);
    }


}
