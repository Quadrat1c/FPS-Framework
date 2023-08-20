using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public class Turret : MonoBehaviour, IOnHit
    {
        public GameObject target;
        public Transform muzzle;
        public Firearm firearm;
        public float lookSpeed = 50;

        Vector3 enemyPos;

        bool forceFocus = false;
        float forceFocusDuration = 2.0f;
        float forceFocusTimer = 0.0f;
        bool greenLight;

        private void Start()
        {
            Invoke(nameof(GiveGreenLight), Random.Range(1, 10));
        }

        private void GiveGreenLight()
        {
            greenLight = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!greenLight) return;
            if (target == null && FindObjectOfType<CharacterController>()) target = FindObjectOfType<CharacterController>().gameObject;

            if (target != null)
            {
                Vector3 directionToTarget = target.transform.position - transform.position;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                // assuming a 90-degree total field of view
                // 45.0f = 90 degress -- 70.0f = 140 degrees -- 90.0f = 180 degrees -- 180.0f = 360 degrees
                if (forceFocus || angleToTarget < 105.0f)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position, directionToTarget, out hit))
                    {
                        if (hit.transform == target.transform) // Check if the first hit object is the player.
                        {
                            enemyPos = Vector3.Lerp(enemyPos, target.transform.position, Time.deltaTime * lookSpeed);
                            if (muzzle) muzzle.LookAt(enemyPos);
                            firearm.Fire(directionToTarget.normalized);
                        }
                    }

                    if (forceFocus) // reduce the timer when forceFocus is active
                    {
                        forceFocusTimer -= Time.deltaTime;
                        if (forceFocusTimer <= 0.0f)
                        {
                            forceFocus = false;
                        }
                    }
                }
            }
        }

        public void OnHit(HitInfo info)
        {
            forceFocus = true;  // Set the flag to true when hit
            forceFocusTimer = forceFocusDuration;   // reset the timer
            GiveGreenLight();
        }
    }
}
