using System;
using System.Collections.Generic;
using Unity.Cinemachine;

[Serializable]
public class HarvesterData
{  public string Tag;
   public bool Active;
   
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
   public float Upgrade_perhumanmultiplier; 
}

//OLD LEGACY
[Serializable]
public class CollectorData
{
   public List<CollectorType> Types;
   public Currency CollectedCurrency;
   public int BaseCollect = 10;
   public float CollectMultiplier = 1f;
   public int CollectAdd = 0;

}
