using System.Collections.Generic;
using UnityEngine;

public static class ShuffleListExtension
{
    public static void Shuffle<T>(this List<T> list)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var temp = list[i];
            var randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public static void Clock<T>(this List<T> list, int times = 1)
    {
        for (var i = 0; i < times; i++) list.BringToFront(list.Count - 1);
    }

    public static void BringToFront<T>(this List<T> list, int targetIdx)
    {
        (list[0], list[targetIdx]) = (list[targetIdx], list[0]);
    }

    public static void Swap<T>(this List<T> list, int swap, int with)
    {
        (list[swap], list[with]) = (list[with], list[swap]);
    }
}