using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Assets.Scripts.Ui;
using UnityEngine;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

public class CardMovement : MonoBehaviour
{
    public bool inertia;
    private bool isSelected;

    public SpriteRenderer sprite;

    [SerializeField] public Vector3 offset;
    [SerializeField] private Vector3 mouseWorldPosition;
    private Vector3 position;
    private bool selected;
    private BoxCollider collider;

    public CardConfiguration configuration;

    public event Action<CardMovement> Played;
    public bool Selected
    {
        get => selected;
        set
        {
            if (selected != value)
            {
                selected = value;
                this.transform.position = value ? position + new Vector3(0,1,-2) / 10 : position;
            }
        }
    }

    private void Awake()
    {
        sprite = this.GetComponent<SpriteRenderer>();
        collider = this.GetComponent<BoxCollider>();
    }

    public void Initialize(CardConfiguration configuration)
    {
        this.configuration = configuration;
        this.sprite.sprite = configuration.sprite;
    }

    void Update()
    { }

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

        if (goingToBePlayed)
        {
            Played?.Invoke(this);
            Destroy(this.gameObject);
            yield break;
        }

        inertia = false;

        while (distanceDelta >= 0.01)
        {
            distanceDelta = (transform.position - position).magnitude / 10;
            distanceDelta = distanceDelta < 0.01f ? 0.01f : distanceDelta;
            transform.position = Vector3.Lerp(transform.position, position, distanceDelta);
            yield return new WaitForEndOfFrame();
        }

        transform.position = position;
    }

    public void OnMouseMove(Vector3 vector3)
    {
        this.mouseWorldPosition = vector3;
    }

    public void SetPosition(Vector3 pos)
    {
        this.position = pos;
        transform.position = pos;
    }

    public void OnTriggerEnter(Collider other)
    {
        this.goingToBePlayed = other.gameObject.GetComponent<PlayArea>() != null;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayArea>() != null)
        {
            this.goingToBePlayed = false;
        }
    }

    private bool goingToBePlayed;
}