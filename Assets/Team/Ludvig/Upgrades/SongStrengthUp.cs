
public class SongStrengthUp : Upgrade
{
    public ButtonSong buttonSong;
    public int AddValue;
    
    public override void ApplyUpgrade()
    {
        buttonSong.maxSpawnRateMultiplier += AddValue;
    }

    
}
