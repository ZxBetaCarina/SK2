using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SK2
{
    [CreateAssetMenu(menuName = "Custom SO/ Temp Data")]
    public class TemporaryDataStorage : ScriptableObject
    {
        [field: SerializeField] public float _currentBet { get; private set; }
        public float SetCurrentBet { set { _currentBet = value; } }
    }
}
