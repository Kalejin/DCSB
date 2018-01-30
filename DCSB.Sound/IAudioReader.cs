using NAudio.Wave;

namespace DCSB.SoundPlayer
{
    public interface IAudioReader
    {
        WaveFormat WaveFormat { get; }
        long Position { get; set; }

        int Read(float[] buffer, int offset, int count);
        void Dispose();
    }
}
