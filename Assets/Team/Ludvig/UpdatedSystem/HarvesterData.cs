using System;
using System.Collections.Generic;

[Serializable]
public class HarvesterData
{  public string Tag;
   public bool Active;
   
   public float TicksPerSecondMultiplier = 1f;
   public List<CollectionType> CollectorTypes;
}

[Serializable]
public class CollectionType
{
   public Currency CollectedCurrency;
   public int BaseCollect = 10;
   public float CollectMultiplier = 1f;
   public int CollectAdd = 0;
   public float CollectAdditivePercentage = 0f;
   public float CollectMultiplicativePercentage = 0f;
}

//OLD LEGACY
[Serializable]
public class CollectorData
{
   public List<CollectionType> Types;
   public Currency CollectedCurrency;
   public int BaseCollect = 10;
   public float CollectMultiplier = 1f;
   public int CollectAdd = 0;

}
