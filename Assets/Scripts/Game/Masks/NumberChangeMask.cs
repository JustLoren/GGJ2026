using UnityEngine;

public class NumberChangeMask : ICrazyMask
{
    public int QuantityToAffect { get; set; } = 2;
    public int Priority { get; private set; } = 0;

    public void Apply(Player player)
    {
        if (QuantityToAffect > player.Hand.Count)
            throw new UnityException("Wtf you can't change that many cards, bozo");

        int affectedCount = 0;
        while (affectedCount < QuantityToAffect)
        {
            var card = player.Hand[Random.Range(0, player.Hand.Count)];

            //See if we've already affected this gal
            if (card.Affected)
                continue;

            var randomNumber = Random.Range(0, 14);
            while (randomNumber == card.GetNumber())
                randomNumber = Random.Range(0, 14);

            card.SetNumber(randomNumber);

            card.FgMesh.material.color = new Color(1, 0, 0, .85f);

            affectedCount++;
        }
    }
}
