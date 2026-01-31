using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [DoNotSerialize]
    public int CurrentPlayerIndex = 0;
    public List<Player> players = new();
    private List<Card> trick = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        foreach (var player in players)
        {
            player.InitializeHand();
        }

        StartCoroutine(CollectCards());
    }

    public virtual void NextTurn()
    {
        var card = players[CurrentPlayerIndex].GetPlayedCard();

        if (card != null)
        {
            trick.Add(card);

            nextTurnTime = Time.time + turnTime;
            if (trick.Count == players.Count)
            {
                ComputeTrickWinner();
            }
        }

        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Count;
    }

    float turnTime = .1f;
    float nextTurnTime = 0f;
    // Update is called once per frame
    void Update()
    {

    }
    private void ComputeTrickWinner()
    {
        var highestNumber = trick[0].GetNumber();
        var winningPlayer = 0;

        // Every player has played into this trick, let's calculate it.
        for (int i = 1; i < trick.Count; i++)
        {
            if (trick[i].GetNumber() > highestNumber)
            {
                highestNumber = trick[i].GetNumber();
                winningPlayer = i;
            }
        }

        //Now we know who won
        Debug.Log($"{players[winningPlayer].name} won the trick!");
        players[winningPlayer].Points++;

        foreach (var card in trick)
        {
            Destroy(card.gameObject);
        }

        trick.Clear();
    }

    private void DeclareWinner()
    {
        var winner = players.First(p => p.Points == players.Max(pm => pm.Points));

        Debug.Log($"We have a winner: {winner.name} with {winner.Points} points. Good job!");

        InitializeGame();
    }

    private IEnumerator CollectCards()
    {
        while (true)
        {
            yield return new WaitUntil(() => players[CurrentPlayerIndex].HasCardSelected());

            NextTurn();

            if (players[CurrentPlayerIndex].Hand.Count == 0)
            {
                //No more cards in hand
                DeclareWinner();
                break;
            }
        }
    }
}
