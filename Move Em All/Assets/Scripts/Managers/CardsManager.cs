using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ui;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardsManager : MonoBehaviour
{
    private List<CardMovement> cards = new List<CardMovement>();
    private BoxCollider collider;

    public GameObject cardPrefab;
    [SerializeField] private List<CardConfiguration> cardConfigurations = new List<CardConfiguration>();
    [SerializeField] private GridManager gridManager;


#if UNITY_EDITOR
    [Button(Name = "Get Cards")]
    private void GetAllCards()
    {
        cardConfigurations = Utilities.GetAllInstances<CardConfiguration>().ToList();
    }
#endif
    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        //todo Event
    }

    // Start is called before the first frame update
    public void Initialize(Card[] cardList)
    {
        Cleanup();
        for (var i = 0; i < cardList.Length; i++)
        {
            var cardConfiguration = cardConfigurations.Find(x => x.card.type == cardList[i].type);
            CardMovement cardMovement = Instantiate(cardPrefab).GetComponent<CardMovement>();
            cardMovement.transform.SetParent(this.transform);
            cardMovement.Initialize(cardConfiguration);
            cardMovement.sprite.sortingOrder = -(cards.Count + Math.Abs(i - 5));
            cards.Add(cardMovement);
            cardMovement.Played += CardMovementOnPlayed;
        }


        collider = this.GetComponent<BoxCollider>();

        PositionCards();
    }

    private void Cleanup()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        cards.Clear();

    }

    private void CardMovementOnPlayed(CardMovement card)
    {
        cards.Remove(card);
        PositionCards();
        gridManager.ExecuteCard(card.configuration.card.type);
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
        RaycastHit[] hits = Physics.RaycastAll(ray, 50, LayerMask.GetMask("Card"));
        if (hits.Length > 0)
        {
            var card = hits.OrderBy(x => x.transform.GetComponent<CardMovement>().sprite.sortingOrder).Last().transform.GetComponent<CardMovement>();

            int index = cards.FindIndex(x => x == card);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].sprite.sortingOrder = cards.Count - Math.Abs(i - index);
                cards[i].Selected = i == index;
            }
        }
    }
}