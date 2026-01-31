using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject HandContainer;
    public List<Card> Hand = new();
    public Card CardPrefab;
    [DoNotSerialize]
    public int Points;

    private void Start()
    {
        for (int i = 2; i <= 14; i++)
        {
            var newCard = GameObject.Instantiate(CardPrefab, HandContainer.transform);
            newCard.SetNumber(i);
            Hand.Add(newCard);
        }
    }

    public Card GetPlayedCard()
    {
        if (Hand.Count == 0)
            return null;

        //Come up with some plan to choose card sensibly

        var cardIndex = Random.Range(0, Hand.Count);

        var card = Hand[cardIndex];
        Hand.RemoveAt(cardIndex);

        return card;

    }
}
