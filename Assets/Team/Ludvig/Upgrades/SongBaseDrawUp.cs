
public class SongBaseDrawUp : Upgrade
{
    public ButtonSong buttonSong;
    public float AddValue;
    
    public override void ApplyUpgrade()
    {
        buttonSong.minSpawnRateMultiplier += AddValue;
    }

    
}
