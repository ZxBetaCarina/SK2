using AstekUtility.ServiceLocatorTool;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace RNGSystem
{
    public class RngGenerator : MonoBehaviour, IGameService
    {
        [SerializeField] private int seed;
        [SerializeField] private bool randomizeSeed = false;

        private void Awake()
        {
            if (!ServiceLocator.Instance.IsRegistered<RngGenerator>())
            {
                ServiceLocator.Instance.Register<RngGenerator>(this);
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void OnDestroy()
        {
            ServiceLocator.Instance.Unregister<RngGenerator>();
        }

        public int RandInt(int min, int max, int bitShift)
        {
            int seedGenerated;
            if (!randomizeSeed)
                seedGenerated = seed;
            else
            {
                seedGenerated = SeedBasedRng.SeedGenerator();
                seed = seedGenerated;
            }

            return SeedBasedRng.SeedIntRNG(min, max, seedGenerated << bitShift);
        }
        public float RandFloat(float min, float max, int bitShift)
        {
            int seedGenerated;
            if (!randomizeSeed)
                seedGenerated = seed;
            else
            {
                seedGenerated = SeedBasedRng.SeedGenerator();
                seed = seedGenerated;
            }

            return SeedBasedRng.SeedFloatRNG(min, max, seedGenerated << bitShift);
        }
    }
}
