using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        string[] names;
        bool usingController;


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            names = Input.GetJoystickNames();
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            float accel = CrossPlatformInputManager.GetAxis("Axis 5");
            float brake = CrossPlatformInputManager.GetAxis("Axis 4");

#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");

            if (names.Length != 0)
            {
                if (names[0].Contains("Controller"))
                {
                    usingController = true;
                    m_Car.Move(h, accel, brake, handbrake, usingController);
                }
                else
                {
                    usingController = false;
                    m_Car.Move(h, v, v, handbrake, usingController);
                }
            }
            else
            {
                usingController = false;
                m_Car.Move(h, v, v, handbrake, usingController);
            }
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
