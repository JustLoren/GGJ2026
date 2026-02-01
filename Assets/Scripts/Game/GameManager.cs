using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static bool IsPaused = false;
    private PlayerInput playerInput;
    public string discoActionName = "Discoball";
    private InputAction discoAction;
    public GameObject DiscoballPrefab;

    public string pauseActionName = "Pause";
    private InputAction pauseAction;
    public GameObject PauseMenu;
    public GameObject GameOverMenu;
    public GameObject WinUI, LoseUI;

    [DoNotSerialize]
    public int CurrentPlayerIndex = 0;
    public List<Player> players = new();
    private List<Card> trick = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetDiscoball();
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

            var target = players[CurrentPlayerIndex].PlayLocation.transform;
            card.positioner.SetPosition(target.localPosition, target.localRotation, target.localScale);
        }

        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Count;
    }

    public float turnTime = .25f;
    public float waitAfterTrick = 1.25f;
    public float waitAfterRound = 2.5f;
    // Update is called once per frame
    void Update()
    {
        if (discoAction.WasPressedThisFrame())
            ShowDiscoball();

        AnimateDiscoball();

        if (pauseAction.WasPressedThisFrame())
            TogglePause();
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        PauseMenu.SetActive(IsPaused);
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    private void Awake()
    {
        if (!playerInput)
            playerInput = GetComponentInParent<PlayerInput>();
    }

    private void OnEnable()
    {
        if (playerInput)
        {
            discoAction = playerInput.actions[discoActionName];
            if (discoAction == null)
                Debug.LogError($"Could not find action '{discoActionName}'. Check your Input Actions asset.");
            else
                discoAction.Enable();

            pauseAction = playerInput.actions[pauseActionName];
            if (pauseAction == null)
                Debug.LogError($"Could not find action '{pauseActionName}'. Check your Input Actions asset.");
            else
                pauseAction.Enable();
        }
    }

    private void OnDisable()
    {
        discoAction?.Disable();
        pauseAction?.Disable();
    }

    private void OnDestroy()
    {
        //always unpause time
        Time.timeScale = 1f;
        //Always unpause game
        IsPaused = false;
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
        var winningCard = trick[winningPlayer];
        var offset = .01f;
        if (players[winningPlayer] is HumanPlayer)
            offset *= -1f;

        foreach (var card in trick)
        {
            if (card != winningCard)
            {
                card.transform.parent = winningCard.transform.parent;

                card.positioner.SetPosition(
                    winningCard.transform.localPosition + Vector3.back * offset,
                    winningCard.transform.localRotation,
                    winningCard.transform.localScale);
            }
        }
    }

    private void DeclareWinner()
    {
        var winner = players.First(p => p.Points == players.Max(pm => pm.Points));

        Debug.Log($"We have a winner: {winner.name} with {winner.Points} points. Good job!");

        //InitializeGame();
        ShowGameOver(winner is HumanPlayer);
    }

    private void ShowGameOver(bool win)
    {
        GameOverMenu.SetActive(true);
        if (win)
        {
            WinUI.SetActive(true);
        }
        else
        {
            LoseUI.SetActive(true);
        }
        IsPaused = true;
        Time.timeScale = 0f;
    }

    private IEnumerator CollectCards()
    {
        while (true)
        {
            yield return new WaitUntil(() => players[CurrentPlayerIndex].HasCardSelected()
                                             || HumanPlayer.CurrentSanityScore == 0);

            if (HumanPlayer.CurrentSanityScore == 0)
            {
                ShowGameOver(false);
                yield break;
            }

            if (IsPaused)
                yield return new WaitUntil(() => !IsPaused);

            NextTurn();

            yield return new WaitForSeconds(turnTime);


            if (IsPaused)
                yield return new WaitUntil(() => !IsPaused);

            if (trick.Count == players.Count)
            {
                yield return new WaitForSeconds(waitAfterTrick);

                ComputeTrickWinner();

                yield return new WaitForSeconds(waitAfterRound);

                if (IsPaused)
                    yield return new WaitUntil(() => !IsPaused);

                foreach (var card in trick)
                {
                    Destroy(card.gameObject);
                }

                trick.Clear();
            }

            if (players[CurrentPlayerIndex].Hand.Count == 0)
            {

                if (IsPaused)
                    yield return new WaitUntil(() => !IsPaused);

                //No more cards in hand
                DeclareWinner();
                break;
            }
        }
    }

    #region Discoball
    float hiddenYPosition = 6.2f;
    float overTableYPosition = 3.2f;
    float movementSpeed = 3f;
    float distinationYPosition;
    bool discoballNeedsToMove = false;

    private void ResetDiscoball()
    {
        Vector3 pos = DiscoballPrefab.transform.localPosition;
        pos.y = hiddenYPosition;
        DiscoballPrefab.transform.localPosition = pos;

        discoballNeedsToMove = false;
    }
    private void ShowDiscoball()
    {
        if (DiscoballPrefab.activeSelf)
        {
            // discoball is active, move it up, then hide it
            distinationYPosition = hiddenYPosition;
        }
        else
        {
            // discoball is inactive, spin it, and drop it down
            DiscoballPrefab.SetActive(true);
            distinationYPosition = overTableYPosition;
        }
        discoballNeedsToMove = true;
    }
    private void AnimateDiscoball()
    {
        if (DiscoballPrefab.activeSelf)
        {
            if (discoballNeedsToMove)
                MoveDiscoball();

            // spin
            DiscoballPrefab.transform.Rotate(0f, -20f * Time.deltaTime, 0f);
        }
    }
    private void MoveDiscoball()
    {
        Vector3 pos = DiscoballPrefab.transform.localPosition;
        pos.y = Mathf.MoveTowards(pos.y, distinationYPosition, movementSpeed * Time.deltaTime);
        DiscoballPrefab.transform.localPosition = pos;

        if (Mathf.Approximately(pos.y, distinationYPosition))
        {
            if (distinationYPosition == hiddenYPosition)
                DiscoballPrefab.SetActive(false);
            discoballNeedsToMove = false;
        }
    }
    #endregion
}
