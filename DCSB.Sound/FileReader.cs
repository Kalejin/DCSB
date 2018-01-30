using NAudio.Wave;

namespace DCSB.SoundPlayer
{
    internal class FileReader : AudioFileReader, IAudioReader
    {
        public FileReader(string fileName) : base(fileName)
        { }
    }
}
