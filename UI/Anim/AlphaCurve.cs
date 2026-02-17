using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Toctoc
{
    public class AlphaCurve : MonoBehaviour
    {
        [SerializeField] private AnimationCurve curve;
        private Image[] images;
        private SpriteRenderer[] spriteRenderers;
        private Text[] texts;
        private float curTime;
        [SerializeField] private bool isHideOnEnd;
        [SerializeField] private float hideDelay;
        [SerializeField] private bool isPlayOnEnable;
        private bool isPlaying;

        private void Awake()
        {
            images = GetComponentsInChildren<Image>(true);
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            texts = GetComponentsInChildren<Text>(true);
        }

        private void OnEnable()
        {
            if (isPlayOnEnable)
                Init();
        }

        [ContextMenu("Init")]
        public void Init()
        {
            curTime = 0;
            images?.ForEach(x => x.color = new Color(x.color.r, x.color.g, x.color.b, curve.Evaluate(curTime)));
            spriteRenderers?.ForEach(x => x.color = new Color(x.color.r, x.color.g, x.color.b, curve.Evaluate(curTime)));
            texts?.ForEach(x => x.color = new Color(x.color.r, x.color.g, x.color.b, curve.Evaluate(curTime)));
            isPlaying = true;
        }

        void Update()
        {
            if (!isPlaying)
                return;

            curTime += Time.deltaTime;
            images?.ForEach(x => x.color = new Color(x.color.r, x.color.g, x.color.b, curve.Evaluate(curTime)));
            spriteRenderers?.ForEach(x => x.color = new Color(x.color.r, x.color.g, x.color.b, curve.Evaluate(curTime)));
            texts?.ForEach(x => x.color = new Color(x.color.r, x.color.g, x.color.b, curve.Evaluate(curTime)));

            if (curTime >= curve.keys[curve.length - 1].time)
            {
                isPlaying = false;
                StartCoroutine(Hide());
            }
        }

        IEnumerator Hide()
        {
            if (!isHideOnEnd)
                yield break;

            yield return new WaitForSeconds(hideDelay);
            gameObject.SetActive(false);
        }

        public void SetColor(Color color, bool isTrigger = true)
        {
            images?.ForEach(x => x.color = color);
            texts?.ForEach(x => x.color = color);
            if (isTrigger)
                Init();
        }
    }
}
