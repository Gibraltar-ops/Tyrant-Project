using System;
using System.Collections.Generic;

[Serializable]
public class HarvesterData
{  public string Tag;
   public float TicksPerSecondBase = 1f;
   public float TicksPerSecondMultiplier = 1f;
   public List<CollectorType> CollectorTypes;
}

[Serializable]
public class CollectorType
{
   public Currency CollectedCurrency;
   public int BaseCollect = 10;
   public float CollectMultiplier = 1f;
   public int CollectAdd = 0;
}


[Serializable]
public class CollectorData
{
   public List<CollectorType> Types;
   public Currency CollectedCurrency;
   public int BaseCollect = 10;
   public float CollectMultiplier = 1f;
   public int CollectAdd = 0;

}
