using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    /// <summary>
    /// Shuffles the elements of a list in-place using the Fisher-Yates algorithm.
    /// Usage: myCardList.Shuffle();
    /// </summary>
    public static void Shuffle<T>(this List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i
            int randomIndex = Random.Range(0, i + 1);

            // Swap list[i] with list[randomIndex]
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}