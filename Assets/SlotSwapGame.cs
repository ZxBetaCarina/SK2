using UnityEngine;

namespace SK2Gameplays
{
    public class SlotSwapGame : MonoBehaviour
    {
        private int? _selectedCylinder;
        public GameObject shiftpanelchecker; 

        void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    //shiftpanelchecker.SetActive(false);  
                    TouchShift();
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
               // shiftpanelchecker.gameObject.SetActive(false); 
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
