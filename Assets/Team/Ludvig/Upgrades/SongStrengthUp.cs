
public class SongStrengthUp : Upgrade
{
    public ButtonSong buttonSong;
    public float AddValue;
    
    public override void ApplyUpgrade()
    {
        buttonSong.maxSpawnRateMultiplier += AddValue;
    }

    
}
