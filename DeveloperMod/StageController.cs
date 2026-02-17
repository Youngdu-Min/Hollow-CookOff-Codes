using UnityEngine;

namespace DeveloperMode
{
    public class StageController : MonoBehaviour
    {


        public void SetClearStage(int stage)
        {


            SaveDataManager.Instance.SetClearStage(stage);
            print($"스테이지 {SaveDataManager.Instance.StageToString(stage)}까지 해금됨");


        }

        public void ResetStages()
        {
            SaveDataManager.Instance.SetClearStage(0);

            print("스테이지 진행도 초기화");
        }

        public void Save()
        {
            SaveDataManager.Instance.Save();
        }

        public void Load()
        {
            SaveDataManager.Instance.Load();
        }

        public void ResetAll()
        {
            SaveDataManager.Instance.ResetSaveData();
        }

        public void UnlockEveryWeapon()
        {
            SaveDataManager.Instance.SetEnableEveryWeapon();
        }

    }


}