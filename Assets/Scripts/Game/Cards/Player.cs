using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject HandContainer;
    public GameObject PlayLocation;
    public GameObject ScoreLocation;
    public List<Card> Hand = new();
    public Card CardPrefab;
    public GameObject ChipPrefab;
    private int _points;
    public int Points
    {
        get
        {
            return _points;
        }
        set
        {
            _points = value;
            UpdateScoreChips();
        }
    }

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
        foreach(var chip in ScoreChips)
        {
           Destroy(chip); 
        }
        ScoreChips.Clear();

        for (int i = 2; i <= 14; i++)
        {
            var newCard = Instantiate(CardPrefab, HandContainer.transform);

            //newCard.transform.position += new Vector3(0, 0, .01f * i);

            newCard.SetNumber(i);
            Hand.Insert(Random.Range(0, Hand.Count + 1), newCard); // 0.659, -0.013, -0.137
        }
    }

    #region ScoreLocation
    private List<GameObject> ScoreChips = new();
    private void UpdateScoreChips()
    {
        for (int i = ScoreChips.Count; i < Points; i++)
        {
            var newChip = Instantiate(ChipPrefab, this.ScoreLocation.transform);
            ScoreChips.Add(newChip);
            var pos = newChip.transform.localPosition;
            pos.y = 0.03f * i;
            newChip.transform.localPosition = pos;
        }
    }
    #endregion
}
