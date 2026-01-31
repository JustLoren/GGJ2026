using UnityEngine;

public class ColorSprayMask : ICrazyMask
{
    public int QuantityToAffect { get; set; } = 3;
    public int Priority { get; private set; } = 99;

    public void Apply(Player player)
    {
        if (QuantityToAffect > player.Hand.Count)
            throw new UnityException("Wtf you can't color spray that many cards, bozo");

        int affectedCount = 0;
        while (affectedCount < QuantityToAffect)
        {
            var card = player.Hand[Random.Range(0, player.Hand.Count)];

            //See if we've already affected this gal
            if (card.Affected)
                continue;

            var fx = card.GetComponentInChildren<ColorSprayCardFx>();
            fx.Engage();

            affectedCount++;
            card.Affected = true;
        }
    }
}

public interface ICrazyMask
{
    public void Apply(Player player);
    public int Priority { get; }
}
