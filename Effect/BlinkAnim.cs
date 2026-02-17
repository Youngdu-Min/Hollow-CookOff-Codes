using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BlinkAnim : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> blinkTargets;

    public void StartBlinking(float blinkDuration, int loopCount)
    {
        print($"반짝이는 시간 {blinkDuration}");

        // Blinking animation for each object
        foreach (SpriteRenderer renderer in blinkTargets)
        {
            if (renderer == null)
            {
                blinkTargets.Remove(renderer);
                continue;
            }

            // Create a sequence of tweens for blinking each object
            Sequence blinkSequence = DOTween.Sequence();

            // Blinking animation
            blinkSequence.Append(renderer.DOFade(0.5f, blinkDuration / 2)); // Fade out
            blinkSequence.Append(renderer.DOFade(1f, blinkDuration / 2)); // Fade in

            // Set the loop type to infinite so it keeps blinking
            blinkSequence.SetLoops(loopCount, LoopType.Restart);

        }
    }
}
