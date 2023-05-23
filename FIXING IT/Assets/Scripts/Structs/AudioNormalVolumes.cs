namespace FixingIt.Structs
{
    public struct AudioNormalVolumes
    {
        public float GeneralVolume;
        public float MusicVolume;
        public float SFXVolume;

        public AudioNormalVolumes(float general, float music, float sfx)
        {
            GeneralVolume = general;
            MusicVolume = music;
            SFXVolume = sfx;
        }

        public override string ToString()
        {
            return $"GeneralVolume: {GeneralVolume}. MusicVolume: {MusicVolume}. SFXVolume: {SFXVolume}";
        }
    }
}
