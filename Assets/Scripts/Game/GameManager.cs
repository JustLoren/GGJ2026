using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [DoNotSerialize]
    public int CurrentPlayerIndex;
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
    }

    public virtual void NextTurn()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Count;
        var card = players[CurrentPlayerIndex].GetPlayedCard();

        if (card != null)
        {
            trick.Add(card);
        }
        else
        {
            //Game over, someone can't play a card.
            DeclareWinner();
        }
    }

    float turnTime = .1f;
    float nextTurnTime = 0f;
    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextTurnTime)
        {
            NextTurn();
            nextTurnTime = Time.time + turnTime;

            if (trick.Count == players.Count)
            {
                ComputeTrickWinner();
            }
        }
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
}
