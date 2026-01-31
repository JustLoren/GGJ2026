using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //Can be 2-14
    [SerializeField]
    private int _number;
    public MeshRenderer BgMesh;
    public MeshRenderer FgMesh;
    public Transform CardContainer;
    public bool Affected { get; set; } = false;

    public List<Material> CardFaces = new();

    public CardPositioner positioner;

    private void Awake()
    {
        positioner = GetComponent<CardPositioner>();
    }

    public void SetNumber(int number)
    {
        _number = number;

        //Update your visuals to match `number`
        FgMesh.material = CardFaces[number - 2];
    }

    public int GetNumber()
    {
        return _number;
    }
}
