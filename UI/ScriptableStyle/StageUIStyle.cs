using UnityEngine;
using UnityEngine.UI;

namespace UIStyle
{
    [CreateAssetMenu(fileName = "StageUI Sytle", menuName = "Scriptable Style/Stage UI")]
    public class StageUIStyle : ScriptableObject
    {
        public Color parabellumColor, fullMetalColor, hollowPointColor;

        [Header("Button Color")]
        public ColorBlock buttonColors;

        public Sprite[] stageImg;

        public Sprite afterEndingImage;


        [Header("Rank Images")]
        public Sprite rankS;
        public Sprite rankA, rankB, rankC, rankD, rankZero;

        public Sprite GetRankSprite(ScoreRank rank)
        {
            switch (rank)
            {
                case ScoreRank.S:
                    return rankS;

                case ScoreRank.A:
                    return rankA;

                case ScoreRank.B:
                    return rankB;

                case ScoreRank.C:
                    return rankC;
                case ScoreRank.D:
                    return rankD;
                case ScoreRank.Zero:
                    return rankZero;
                default:
                    break;
            }

            return rankZero;
        }
    }
}
