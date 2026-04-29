using System;

[Serializable]
public class WishemonInstance
{
    public WishemonCard Card;
    public int CurrentPv;
    public int[] CurrentPP;

    public bool IsFainted => CurrentPv <= 0;

    public WishemonInstance(WishemonCard card)
    {
        Card = card;
        CurrentPv = card.MaxPv;
        CurrentPP = new int[card.Attacks.Length];
        for (int i = 0; i < CurrentPP.Length; i++)
            CurrentPP[i] = card.Attacks[i].MaxPP;
    }

    public void Heal()
    {
        CurrentPv = Card.MaxPv;
        for (int i = 0; i < CurrentPP.Length; i++)
            CurrentPP[i] = Card.Attacks[i].MaxPP;
    }
}
