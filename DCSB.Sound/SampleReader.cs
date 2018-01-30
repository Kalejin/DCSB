using NAudio.Wave;

namespace DCSB.SoundPlayer
{
    public class SampleReader : ISampleProvider
    {
        public bool IsDisposed { get; protected set; }

        private readonly IAudioReader _reader;
        private bool _loop;

        public WaveFormat WaveFormat { get; private set; }

        public SampleReader(IAudioReader reader, bool loop)
        {
            _reader = reader;
            WaveFormat = reader.WaveFormat;
            _loop = loop;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (IsDisposed)
                return 0;

            int read = 0;
            while (read < count)
            {
                int bytesRead = _reader.Read(buffer, offset + read, count - read);
                if (bytesRead == 0)
                {
                    if (_reader.Position == 0 || !_loop)
                    {
                        break;
                    }
                    _reader.Position = 0;
                }
                read += bytesRead;
            }

            if (read < count)
            {
                Dispose();
            }
            return read;
        }

        public void Dispose()
        {
            _reader.Dispose();
            IsDisposed = true;
        }
    }
}
