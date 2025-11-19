using System;

[Serializable]
public class ShopSettings
{
   public int PriceBase = 12;
   public float PriceGrowthRate = 1.1f;
   public int NumberOwned = 0;
   public int NextPrice = 12;

}
