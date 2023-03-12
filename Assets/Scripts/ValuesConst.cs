public static class ValuesConst
{
    public enum Type
    {
        I, J, L, O, S, T, Z
    }
    public const int ROW = 20, COL = 9,START_BLOCK_POS_Y = 23,SCOREROW = 100;
    public const float SCALE_TETROMINO_FAKE = 0.7f, SCALE_TETROMINO_NEXT = 0.5f;

    public const string AUDIO_NEWGAME = "New Game",
        AUDIO_RESUME = "Resume",
        AUDIO_PAUSE = "Pause",
        AUDIO_GAMEOVER = "Game Over",
        AUDIO_MUSIC = "Music",
        AUDIO_DROP = "Drop",
        AUDIO_ROTATE = "Rotate",
        AUDIO_COLLECT = "Collect";

    public const string MUSIC_VOL = "Music Vol", SFX_VOL = "Sfx Vol";
}