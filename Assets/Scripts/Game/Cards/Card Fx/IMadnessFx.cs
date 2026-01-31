public interface IMadnessFx
{
    public MadnessFxType FxType { get; }
    public void Engage();
}

public enum MadnessFxType
{
    ColorSpray = 0,
}
