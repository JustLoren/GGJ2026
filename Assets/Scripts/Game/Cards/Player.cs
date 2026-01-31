using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject HandContainer;
    public GameObject PlayLocation;
    public List<Card> Hand = new();
    public Card CardPrefab;
    [DoNotSerialize]
    public int Points;

    private void Start()
    {

    }

    public virtual void Update()
    {

    }

    public virtual bool HasCardSelected()
    {
        return true;
    }

    public virtual Card GetPlayedCard()
    {
        if (Hand.Count == 0)
            return null;

        //Come up with some plan to choose card sensibly

        var cardIndex = Random.Range(0, Hand.Count);

        var card = Hand[cardIndex];
        Hand.RemoveAt(cardIndex);

        return card;
    }

    public virtual void InitializeHand()
    {
        //We are already initialized - what are you doing?
        if (Hand.Count > 0)
        {
            Debug.LogError("We are already initialized - what are you doing?");
            return;
        }

        Points = 0;

        for (int i = 2; i <= 14; i++)
        {
            var newCard = Instantiate(CardPrefab, HandContainer.transform);

            newCard.transform.position += new Vector3(0, 0, .01f * i);

            newCard.SetNumber(i);
            Hand.Add(newCard);
        }
    }
}
