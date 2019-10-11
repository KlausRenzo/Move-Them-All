using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Assets.Scripts.Ui;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CardMovement : MonoBehaviour
{
    public bool inertia;
    private bool isSelected;

    [SerializeField] public Vector3 offset;
    [SerializeField] private Vector3 mouseWorldPosition;

    void Update()
    {
        //Vector3 mousePosition = Input.mousePosition;

        //mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //mouseWorldPosition.z = 0;

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        //    if (Physics.Raycast(ray, out RaycastHit hit))
        //    {
        //        if (hit.transform == this.transform)
        //        {
        //            isSelected = true;
        //            StartCoroutine(DelayedCardMovement());
        //        }
        //    }
        //}

        //if (!isSelected)
        //{
        //    return;
        //}

        //if (Input.GetMouseButtonUp(0) && isSelected)
        //{
        //    inertia = true;
        //    isSelected = false;
        //}
    }

    public IEnumerator DelayedCardMovement(Vector3 mousePosition)
    {
        Vector3 lastMousePosition;
        offset = transform.position - mousePosition;
        mouseWorldPosition = mousePosition;
        float distanceDelta = 0;
        while (true)
        {
            distanceDelta = (transform.position - mouseWorldPosition - offset).magnitude / 10;
            distanceDelta = distanceDelta < 0.25f ? 0.25f : distanceDelta;
            transform.position = Vector3.Lerp(transform.position, mouseWorldPosition + offset, distanceDelta);
            yield return new WaitForEndOfFrame();
            if (inertia)
            {
                lastMousePosition = mouseWorldPosition;
                break;
            }
        }

        inertia = false;

        while (distanceDelta > 0.1)
        {
            distanceDelta = (transform.position - lastMousePosition - offset).magnitude / 10;
            transform.position = Vector3.Lerp(transform.position, lastMousePosition + offset, distanceDelta);
            yield return new WaitForEndOfFrame();
        }
    }

    public void SetOffset(Vector3 vector3)
    {
        offset = transform.position - mouseWorldPosition;
    }

    public void OnMouseMove(Vector3 vector3)
    {
        this.mouseWorldPosition = vector3;
    }
}