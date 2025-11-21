
public class SongSustainUp : Upgrade
{
    public ButtonSong buttonSong;
    public float MultiplyValue;
    
    public override void ApplyUpgrade()
    {
        buttonSong.drainSpeed *= MultiplyValue;
    }

    
}
