using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedRNG<T>
{
    public List<RNGItem> ItemsForRNG;

    [System.Serializable]
    public class RNGItem
    {
        public T Item;
        public int Probability;
    }
}

public class GetWeightedRNG
{
    public static T GetValue<T>(List<WeightedRNG<T>.RNGItem> itemsForRNG)
    {
        int sum = 0;
        foreach (var item in itemsForRNG)
        {
            sum += item.Probability;
        }

        int randomValue = Random.Range(0, sum); // Generate a random value between 0 and sum-1

        int cumulativeProbability = 0;
        for (int i = 0; i < itemsForRNG.Count; i++)
        {
            cumulativeProbability += itemsForRNG[i].Probability;

            // Check if randomValue falls within the range of the current item's cumulative probability
            if (randomValue < cumulativeProbability)
            {
                return itemsForRNG[i].Item; // Return the selected item
            }
        }

        return default(T);
    }
}
