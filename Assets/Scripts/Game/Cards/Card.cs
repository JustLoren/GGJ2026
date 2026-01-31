using UnityEngine;

public class Card : MonoBehaviour
{
    //Can be 2-14
    [SerializeField]
    private int _number;
    public MeshRenderer BgMesh;
    public MeshRenderer FgMesh;

    public void SetNumber(int number)
    {
        _number = number;

        //Update your visuals to match `number`
    }

    public int GetNumber()
    {
        return _number;
    }
}
