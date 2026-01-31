public interface IMadnessFx
{
    public MadnessFxType FxType { get; }
    public void Engage();
}

public enum MadnessFxType
{
    ColorSpray = 0,
    NumberChange = 1,
    Rotation = 2,
    ColorFade = 3,
}
