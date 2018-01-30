using NAudio.Vorbis;

namespace DCSB.SoundPlayer
{
    public class OggFileReader : VorbisWaveReader, IAudioReader
    {
        public OggFileReader(string fileName) : base(fileName)
        { }
    }
}
