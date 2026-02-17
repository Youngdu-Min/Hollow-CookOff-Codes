using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    #region Animator
    public static void AnimatorStop(this Animator animator)
    {
        animator.enabled = false;
    }

    public static void AnimatorPlay(this Animator animator)
    {
        animator.enabled = true;
    }

    public static void AnimatorReset(this Animator animator)
    {
        animator.Rebind();
        animator.Update(0);
    }
    #endregion

    public static void ForEach<T>(this T[] array, Action<T> action)
    {
        Array.ForEach<T>(array, action);
    }
    public static bool Contain<T>(this T[] array, T value)
    {
        return Array.IndexOf(array, value) != -1;
    }

    public static void PrintDictionary<TKey, TValue>(this Dictionary<TKey, TValue> myDictionary)
    {
        foreach (var pair in myDictionary)
        {
            Debug.Log("Key: " + pair.Key + " Value: " + pair.Value);
        }
    }
}
