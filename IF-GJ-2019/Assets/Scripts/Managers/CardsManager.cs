using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardsManager : MonoBehaviour
{
    private List<CardMovement> cards = new List<CardMovement>();
    private BoxCollider collider;

    public GameObject cardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        collider = this.GetComponent<BoxCollider>();
        for (int i = 0; i < 10; i++)
        {
            var cardMovement = Instantiate(cardPrefab).GetComponent<CardMovement>();
            cardMovement.sprite.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            cardMovement.sprite.sortingOrder = cards.Count - Math.Abs(i - 5);
            cards.Add(cardMovement);
            cardMovement.Played += CardMovementOnPlayed;
        }

        PositionCards();
    }

    private void CardMovementOnPlayed(CardMovement card)
    {
        cards.Remove(card);
        PositionCards();
    }

    private void PositionCards()
    {
        Vector3 center = this.transform.position - collider.center;
        float delta = collider.size.x - 1;
        Vector3 left = center - new Vector3(delta / 2, 0, 0);

        for (int i = 0; i < cards.Count; i++)
        {
            // cards[i].transform.position = left + i * new Vector3(delta / cards.Count, 0, 0);
            cards[i].SetPosition(left + i * new Vector3(delta / cards.Count, 0, 0));
        }
    }

    // Update is called once per frame
    void Update()
    { }

    public void OnMouseHover(Vector3 mouseWorldPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseWorldPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f, LayerMask.GetMask("Card")))
        {
            var card = hit.transform.GetComponent<CardMovement>();

            int index = cards.FindIndex(x => x == card);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].sprite.sortingOrder = cards.Count - Math.Abs(i - index);
                cards[i].Selected = i == index;
            }
        }
    }
}