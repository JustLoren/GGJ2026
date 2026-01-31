using UnityEngine;
using UnityEngine.InputSystem;

public class HumanPlayer : Player
{
    private PlayerInput playerInput;

    public string nextActionName = "Next Card";
    private InputAction nextAction;

    public string previousActionName = "Previous Card";
    private InputAction previousAction;

    public string selectActionName = "Select Card";
    private InputAction selectAction;

    private HandFanner _handFanner;

    private void Awake()
    {
        _handFanner = GetComponent<HandFanner>();

        if (!playerInput)
            playerInput = GetComponentInParent<PlayerInput>();
    }

    public override void Update()
    {
        if (nextAction.WasPressedThisFrame())
            _handFanner.SelectNext();


        if (previousAction.WasPressedThisFrame())
            _handFanner.SelectPrevious();

        if (selectAction.WasPressedThisFrame())
            hasChosenCard = true;
    }

    private void OnEnable()
    {
        if (playerInput)
        {
            nextAction = playerInput.actions[nextActionName];
            if (nextAction == null)
                Debug.LogError($"Could not find action '{nextActionName}'. Check your Input Actions asset.");
            else
                nextAction.Enable();

            previousAction = playerInput.actions[previousActionName];
            if (previousAction == null)
                Debug.LogError($"Could not find action '{previousActionName}'. Check your Input Actions asset.");
            else
                previousAction.Enable();

            selectAction = playerInput.actions[selectActionName];
            if (selectAction == null)
                Debug.LogError($"Could not find action '{selectActionName}'. Check your Input Actions asset.");
            else
                selectAction.Enable();
        }
    }

    private void OnDisable()
    {
        nextAction?.Disable();
        previousAction?.Disable();
        selectAction?.Disable();
    }

    private void Start()
    {
        // Ensure we start with a valid selection
        _handFanner.SelectIndex(0);
    }

    public override Card GetPlayedCard()
    {
        // Prefer selected card if available
        var selected = _handFanner.GetSelectedCard();

        _handFanner.ShowSelected = false;

        Hand.Remove(selected);
        _handFanner.FanHand();

        hasChosenCard = false;

        return selected;
    }

    public override void InitializeHand()
    {
        base.InitializeHand();

        _handFanner.SelectIndex(0);
        _handFanner.FanHand();
    }

    private bool hasChosenCard = false;
    public override bool HasCardSelected()
    {
        return hasChosenCard;
    }
}
