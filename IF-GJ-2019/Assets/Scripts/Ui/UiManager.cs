using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Windows.WebCam;

namespace Assets.Scripts.Ui
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Vector3 mouseWorldPosition;
        private CardMovement card;



        void Update()
        {
            Vector3 mousePosition = Input.mousePosition;

            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            mouseWorldPosition.z = 0;
            card?.OnMouseMove(mouseWorldPosition);

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    card = hit.transform.GetComponent<CardMovement>();
                    StartCoroutine(card.DelayedCardMovement(mouseWorldPosition));
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                card.inertia = true;
                card = null;
            }
        }
    }
}