using System;
using System.Collections;
using UnityEngine;

namespace SK2
{
    public class DoorAnim : MonoBehaviour
    {
        [SerializeField] private DoorData[] _doors;
        [SerializeField] private float _displacementTime;
        public static DoorAnim INSTANCE;
        bool _is_closing;
        [SerializeField] ParticleSystem _door_slam_effect;

        public bool IsAnimRunning { get; private set; } = false;

        private void Awake()
        {
            if (INSTANCE == null)
                INSTANCE = this;
            else
                Destroy(gameObject);

        }

        public IEnumerator DoorTrigger()
        {
            print("Door Anim");
            if (IsAnimRunning) yield break;
            IsAnimRunning = true;
            float timer = _displacementTime;

            Vector3 startPos1 = _doors[0].Door.localPosition;
            Vector3 startPos2 = _doors[1].Door.localPosition;

            Vector3 finalPos1, finalPos2;

            //Detecting Final Position
            if (Mathf.Sqrt((_doors[0].Pos1 - startPos1).sqrMagnitude) < 0.1f)
            {
                //door is at pos 1
                finalPos1 = _doors[0].Pos2;
                finalPos2 = _doors[1].Pos2;
                _is_closing = true;
            }
            else
            {
                //door is at pos 2
                finalPos1 = _doors[0].Pos1;
                finalPos2 = _doors[1].Pos1;
            }

            //Movement from souce to final/destination
            while (timer > 0)
            {
                _doors[0].Door.localPosition = Vector3.Lerp(startPos1, finalPos1, 1 - (timer) / _displacementTime);
                _doors[1].Door.localPosition = Vector3.Lerp(startPos2, finalPos2, 1 - (timer) / _displacementTime);

                timer -= Time.deltaTime;
                yield return null;
            }


            _doors[0].Door.localPosition = finalPos1;
            _doors[1].Door.localPosition = finalPos2;
            IsAnimRunning = false;
            if(_is_closing)
            {
                _is_closing = false;
                _door_slam_effect.Play();
            }
        }
    }

    [Serializable]
    public class DoorData
    {
        public Transform Door;
        public Vector3 Pos1;
        public Vector3 Pos2;
    }
}
