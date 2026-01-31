using System.Collections.Generic;
using UnityEngine;

public class ColorSprayCardFx : MonoBehaviour, IMadnessFx
{
    public MeshRenderer CardNumber;

    private List<MeshRenderer> numberClones = new();

    public List<Color> CloneColors = new();

    public MadnessFxType FxType => MadnessFxType.ColorSpray;

    private void Init()
    {
        foreach (var color in CloneColors)
        {
            var clone = Instantiate(CardNumber, CardNumber.transform.parent);
            clone.material.color = color;
            numberClones.Add(clone);

            clone.gameObject.AddComponent<ObjectJiggler>();
        }
        CardNumber.gameObject.SetActive(false);
        this.enabled = true;
    }

    public void Engage()
    {
        Init();
    }
}
