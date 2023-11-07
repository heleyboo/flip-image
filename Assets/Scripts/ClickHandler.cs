using UnityEngine;

namespace Tuong
{
    public class ClickHandler: MonoBehaviour
    {
        public void OnButtonClick(int index)
        {
            // Handle the button click using the delegate
            ButtonClickDelegate?.Invoke(index);
        }

        public delegate void ButtonClick(int index);
        public static event ButtonClick ButtonClickDelegate;
    }
}