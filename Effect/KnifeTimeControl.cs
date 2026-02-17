using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnifeTimeControl : MonoBehaviour
{

    //생성자 
    public GameObject m_goPrefab = null;
    List<Transform> m_objectList = new List<Transform>();
    List<GameObject> m_pirouetteBarList = new List<GameObject>();
    public GameObject t_pirouettebar;
    Camera m_cam = null;

    [Header("Ability1")]
    public Image abilityImage1;
    public float cooldown1 = 5;
    bool isCooldown;
    public KeyCode ability1;


    // Start is called before the first frame update
    void Start()
    {
        // 생성하는거 
        m_cam = Camera.main;
        GameObject t_objects = GameObject.Find("newPixelCorgi");

        m_objectList.Add(t_objects.transform);
        t_pirouettebar = Instantiate(m_goPrefab, t_objects.transform.position, Quaternion.identity, transform);
        t_pirouettebar.gameObject.SetActive(false);
        m_pirouetteBarList.Add(t_pirouettebar);

        var _knifeTime = GameObject.Find("KnifeTime");
        if (_knifeTime != null && _knifeTime.TryGetComponent(out Image img))
        {
            abilityImage1 = img;
            abilityImage1.fillAmount = 0;
        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_objectList.Count; i++)
        {
            m_pirouetteBarList[i].transform.position = m_cam.WorldToScreenPoint(m_objectList[i].position + new Vector3(0, 1.5f, 0));
        }
    }

    void Ability1()
    {
        if (Input.GetKey(ability1) && isCooldown == false)
        {
            isCooldown = true;
        }

        if (isCooldown)
        {
            abilityImage1.fillAmount -= cooldown1 * Time.deltaTime;
            if (abilityImage1.fillAmount <= 0)
            {
                abilityImage1.fillAmount = 0;
                isCooldown = false;
            }
        }
    }

}
