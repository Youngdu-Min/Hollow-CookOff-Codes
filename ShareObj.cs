using UnityEngine;

public class ShareObj : MonoBehaviour
{
    public GameObject targetObj;

    private static GameObject targetInstance;

    void Awake()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        if (targetInstance == null)
        {
            print("오브젝트 클론 생성");
            //if (GameObject.Find(targetObj.ToString()))
            //{
            //    Destroy(gameObject);
            //}

            var obj = Instantiate(targetObj);
            if (obj.name.Contains("(Clone)"))
                obj.name = obj.name.Replace("(Clone)", "");

            DontDestroyOnLoad(obj);
            targetInstance = obj;

        }
        else
        {

            Destroy(gameObject);
            return;
        }

    }
}
