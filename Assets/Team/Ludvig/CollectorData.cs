using System;

[Serializable]
public class CollectorData
{
   public float TicksPerSecondBase = 1f;
   public float TicksPerSecondMultiplier = 1f;
   public int BaseCollect = 10;
   public float CollectMultiplier = 1f;
   public long CurrentCollect;

}
