using UnityEngine;
using UnityEngine.UI;

public class CustomButtonKey : Button
{
    [SerializeField]
    public KeyCode triggerKey = KeyCode.Space; // 사용할 특정 키패드를 설정합니다.
    

    private void Update()
    {

        if (Input.GetKeyDown(triggerKey))
        {
            onClick.Invoke(); // 버튼을 클릭한 것과 동일한 동작을 수행합니다.
        }
    }
}
