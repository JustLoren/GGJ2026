using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //Can be 2-14
    [SerializeField]
    private int _number;
    public MeshRenderer BgMesh;
    public MeshRenderer FgMesh;

    public List<Material> CardFaces = new();

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
