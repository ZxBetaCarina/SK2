using UnityEngine;

namespace SK2Gameplays
{
    public class SlotSwapGame : MonoBehaviour
    {
        private int? _selectedCylinder;

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    TouchShift();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                TouchShift();
                
                
            }
        }

        private void TouchShift()
        {
            print("Turn on shift button");
            ImageCylinderSpawner.INSTANCE.Shift();
            
        }

        public void CylinderSelected(int cylinderSelected)
        {
            _selectedCylinder = cylinderSelected;
        }
    }
}
