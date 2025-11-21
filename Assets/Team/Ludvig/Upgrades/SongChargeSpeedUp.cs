
public class SongChargeSpeedUp : Upgrade
{
    public ButtonSong buttonSong;
    public float AddValue;
    
    public override void ApplyUpgrade()
    {
        buttonSong.fillSpeed += AddValue;
    }

    
}
