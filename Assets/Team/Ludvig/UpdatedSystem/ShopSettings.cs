using System;

[Serializable]
public class ShopSettings
{
   public string Tag;
   public int PriceBase = 12;
   public float PriceGrowthRate = 1.1f;
   public int NumberOwned = 0;
   public int NextPrice = 12;
   public int MaxOwned = 25;

}
