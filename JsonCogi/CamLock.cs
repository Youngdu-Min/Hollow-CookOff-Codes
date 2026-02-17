using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.CorgiEngine
{
    public class CamLock : MonoBehaviour
    {
        public static CamLock camLock;
        [SerializeField] private int savePage;
        public int ableCount = 0;
        [SerializeField] private ableSpawnData[] page;
        [SerializeField] private Transform cameraTarget;
        private BoxCollider2D[] cameraCollider;
        public bool clear;
        private Collider2D trigger;

        [SerializeField]
        private CheckPoint tmpCheckPoint;
        [SerializeField]
        private UnityEvent triggerEvent;
        [SerializeField] private bool onlyCamMove = false;
        [SerializeField] private bool isCameraFollowingOnEnd = true;


        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                //임시 스폰구역 설정
                if (LevelManager.Instance != null && tmpCheckPoint != null)
                    LevelManager.Instance.SetTmpCheckpoint(tmpCheckPoint);

                if (!onlyCamMove)
                    camLock = this.GetComponent<CamLock>();

                MMCameraEvent.Trigger(MMCameraEventTypes.StopFollowing);
                MMCameraEvent.TriggerOther(MMCameraEventTypes.SetTargetOther, cameraTarget);
                MMCameraEvent.Trigger(MMCameraEventTypes.StartOtherFollowing);

                trigger.enabled = false;
                triggerEvent.Invoke();
                AbleSpawning();

            }
        }

        public void Start()
        {
            if (this.name[2] == '1') //첫번째 전투지역일경우
                camLock = this.GetComponent<CamLock>(); //cam을 지금꺼로 지정

            trigger = GetComponent<Collider2D>();
            cameraCollider = cameraTarget.GetComponentsInChildren<BoxCollider2D>(true);

            if (onlyCamMove)
                return;

            for (int i = 0; i < page.Length; i++)
            {
                page[i].initPos = new Vector3[page[i].spawnob.Length];
                for (int j = 0; j < page[i].spawnob.Length; j++)
                    page[i].initPos[j] = page[i].spawnob[j].transform.position;
            }
            //  GM = FindObjectOfType<GameManager>().gameObject; // 게임매니져를 찾아서씀(직접 Insptector창에서 지정하게해도됨)
        }

        public void ManualAbleSpawning()
        {
            AbleSpawning();
        }

        public void AbleSpawning() // 적을 스폰하는 함수
        {
            if (onlyCamMove)
                return;
            if (camLock == null)
                camLock = this.GetComponent<CamLock>();

            if (ableCount > 0) //
                return;

            if (savePage == page.Length) // 적을다 잡았으면
            {
                LevelManager.Instance.RemoveTmpCheckPoint();
                if (isCameraFollowingOnEnd)
                    MMCameraEvent.Trigger(MMCameraEventTypes.StartFollowing); // 카메라가 플레이어 따라 이동하게함
                foreach (var coll in cameraCollider)
                    coll.gameObject.SetActive(false);

                clear = true; // 끝났다는것을 표시
            }
            else
            {
                for (int i = 0; i < page[savePage].spawnob.Length; i++) // 적이 남아있으면 
                {
                    page[savePage].spawnob[i].SetActive(true); // 다음페이지 적을 소환
                    page[savePage].spawnob[i].transform.position = page[savePage].initPos[i];
                    //page[savePage].spawnob[i].transform.localScale = new Vector2(1 / page[savePage].spawnob[i].transform.parent.localScale.x, 1 / page[savePage].spawnob[i].transform.parent.localScale.y);
                }
                ableCount = page[savePage].spawnob.Length;
                savePage++;
            }
        }
        public void AbleSpawnReset() // 적 스폰초기화
        {
            print($"{gameObject} AbleSpawnReset");
            if (clear == false) // 적을다 처리하기전에 플레이어가 죽었을때
            {
                for (savePage--; savePage >= 0; savePage--)
                {
                    for (int i = 0; i < page[savePage].spawnob.Length; i++) //현재 적들을 false 상태로 만듬
                    {
                        page[savePage].spawnob[i].GetComponent<Health>().Revive();
                        page[savePage].spawnob[i].SetActive(false);
                    }
                }
                savePage = 0;
                ableCount = 0;
                trigger.enabled = true;
            }
        }


    }
}
