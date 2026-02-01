using System.Collections.Generic;
using System.Linq;
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

    public string dexterityActionName = "Apply Dexterity";
    private InputAction dexterityAction;

    private HandFanner _handFanner;

    private void Awake()
    {
        _handFanner = GetComponent<HandFanner>();

        if (!playerInput)
            playerInput = GetComponentInParent<PlayerInput>();

        CurrentSanity = MaxSanity;
    }

    public override void Update()
    {
        if (nextAction.WasPressedThisFrame())
            _handFanner.SelectNext();

        if (previousAction.WasPressedThisFrame())
            _handFanner.SelectPrevious();

        if (selectAction.WasPressedThisFrame())
            hasChosenCard = true;

        if (dexterityAction.WasPressedThisFrame())
        {
            //var rotations = GameObject.FindObjectsByType<RotationFx>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            //foreach (var rotation in rotations)
            //{
            //    rotation.AddRotation();
            //}

            var fades = GameObject.FindObjectsByType<ColorFadeCardFx>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var fade in fades)
            {
                fade.ApplyFade();
            }
        }

        DecreaseSanity();
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

            dexterityAction = playerInput.actions[dexterityActionName];
            if (dexterityAction == null)
                Debug.LogError($"Could not find action '{dexterityActionName}'. Check your Input Actions asset.");
            else
                dexterityAction.Enable();
        }
    }

    private void OnDisable()
    {
        nextAction?.Disable();
        previousAction?.Disable();
        selectAction?.Disable();
        dexterityAction?.Disable();
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

        //Enforce mask properties
        var masks = FetchMaskEffects();
        foreach (var mask in masks.OrderBy(m => m.Priority))
            mask.Apply(this);
    }
    private List<ICrazyMask> FetchMaskEffects()
    {
        return new() { new ColorSprayMask(), new NumberChangeMask(), new ColorFadeMask() };
    }

    private bool hasChosenCard = false;
    public override bool HasCardSelected()
    {
        return hasChosenCard;
    }

    #region Sanity Section
    public AnimationCurve SanityDropRate;
    private float _currentSanity;
    public float CurrentSanity
    {
        get
        {
            return _currentSanity;
        }
        set
        {
            _currentSanity = Mathf.Clamp(value, 0f, MaxSanity);
            CurrentSanityScore = _currentSanity / MaxSanity;
        }
    }
    public float MaxSanity = 1000f;
    public static float CurrentSanityScore = 1f;

    private void DecreaseSanity()
    {
        CurrentSanity -= SanityDropRate.Evaluate(0f) * Time.deltaTime;
        Debug.Log($"Current Sanity: {CurrentSanity}");
    }
    #endregion
}
