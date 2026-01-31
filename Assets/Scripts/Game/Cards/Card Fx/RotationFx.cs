using UnityEngine;

public class RotationFx : MonoBehaviour, IMadnessFx
{
    public GameObject CardObject;

    public MadnessFxType FxType => MadnessFxType.ColorSpray;

    private void Init()
    {
        this.enabled = true;

    }

    public void Engage()
    {
        Init();
    }
}
