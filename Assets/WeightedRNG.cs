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
        int index = 0;
        int randomValue = Random.Range(0, sum); 
        for (int i = itemsForRNG.Count - 1; i >= 0; i--)
        {
            if (itemsForRNG[i].Probability < randomValue)
            {
                index = i;
                break;
            }

        }

        return itemsForRNG[index].Item;
    }
}
