using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class ableSpawnData
{
    public GameObject[] spawnob;
    public Vector3[] initPos;


}
[System.Serializable]
public class PlayerSave //체크포인트 도달할때 당시의 체력과 바이오포인트를 저장할때 사용
{
    public float hpsave;
    public float bpsave;

    public PlayerSave(float _hpsave, float _bpsave)
    {
        this.hpsave = _hpsave; this.bpsave = _bpsave;
    }


}

[System.Serializable]
public class StageData 
{
    public string name;
    public int pages;
    public string enemys;



    public StageData(string _name, int _pages, string _enemys)
    { this.name = _name; this.pages = _pages; this.enemys = _enemys; }

}

[System.Serializable]
public class ChatingData // 채팅 데이터 이름과 대사를 입력
{

    public string name;
    public string daesa;
    public ChatingData(string _name, string _dae)
    { this.name = _name; this.daesa = _dae; }

}
[System.Serializable]
public class ChatingData_2 // id로 구별해 채팅데이터를 사용하기 위한것
{
    public int id;
    public List<ChatingData> chat = new List<ChatingData>();
    public ChatingData_2(int _id, List<ChatingData> _dae)
    { this.id = _id; this.chat = _dae; }
}


[System.Serializable]
public class Stages
{

    public List<StageData> stageList = new List<StageData>();
    public List<ChatingData_2> chatlist = new List<ChatingData_2>();
    public PlayerSave ps;



}

public class mobjson : MonoBehaviour
{
    // public PlayerData[] playerData;
    public static mobjson mj;

    public Stages Data;

    [ContextMenu("To Json Data")]
    public void SavePlayerDataToJson()//세이브
    {
        string jsonData = JsonUtility.ToJson(Data, true);
        string path = Path.Combine(Application.dataPath, "Resources/StagesData.json");
        File.WriteAllText(path, jsonData);



    }
    [ContextMenu("To Json Data")]
    public void LoadPlayerDataToJson()//로드
    {

        string path = Path.Combine(Application.dataPath, "Resources/StagesData.json");
        string jsondata = File.ReadAllText(path);
        Data = JsonUtility.FromJson<Stages>(jsondata);



    }

    void Awake()
    {
        LoadPlayerDataToJson();
        mobjson.mj = this;

    }


    // 가장 마지막 함수 실행 전 실행 함수.
    // https://docs.unity3d.com/kr/530/Manual/ExecutionOrder.html
    private void OnDisable()
    {

        SavePlayerDataToJson();
        Debug.Log("종료 json 저장");
    }

}



