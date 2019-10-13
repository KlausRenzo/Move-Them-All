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
        private CardsManager cardsManager;
        [SerializeField] private AudioSource audioSurce;

        void Start()
        {
            this.cardsManager = FindObjectOfType<CardsManager>();
        }

        public bool isEnabled;

        void Update()
        {
            Vector3 mousePosition = Input.mousePosition;

            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            mouseWorldPosition.z = 0;
            card?.OnMouseMove(mouseWorldPosition);

            if (isEnabled)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, 50f, LayerMask.GetMask("CardManager")))
                {
                    cardsManager.OnMouseHover(mousePosition);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (Physics.Raycast(ray, out RaycastHit hit, 50f, LayerMask.GetMask("Card")))
                    {
                        card = hit.transform.GetComponent<CardMovement>();
                        if (card != null)
                        {
                            audioSurce.Play();
                            StartCoroutine(card.DelayedCardMovement(mouseWorldPosition));
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && card != null)
            {
                card.inertia = true;
                card = null;
            }
        }
    }
}