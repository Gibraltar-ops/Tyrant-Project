
public class ProgressEventTrigger : Upgrader
{
    private bool _canBuy = true;

    public delegate void OnProgressEvent(ProgressEvent progressEvent);
    public static event OnProgressEvent onProgressEvent;

    public ProgressEvent ProgressEvent;

    public override void ApplyUpgrade(bool maxedOut)
    {
        if (!_canBuy)
        {
            return;
        }

        if (maxedOut)
        {
            _canBuy = false;
        }

        onProgressEvent?.Invoke(ProgressEvent);
        base.ApplyUpgrade(maxedOut);
    }


}

public enum ProgressEvent
{
    SetStage1,
    SetStage2,
    SetStage3,
    UnlockLarynx,
    UnlockBroodChamber,
    UnlockKebabArms,
}

