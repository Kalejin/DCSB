using DCSB.Models;
using DCSB.SoundPlayer;
using System;
using System.Collections.Generic;

namespace DCSB.Business
{
    public class SoundManager
    {
        private AudioPlaybackEngine _primarySoundPlayer;
        private AudioPlaybackEngine _secondarySoundPlayer;
        private Random _random;

        public int Volume
        {
            get { return _primarySoundPlayer.Volume; }
            set
            {
                _primarySoundPlayer.Volume = value;
                if (_secondarySoundPlayer != null) _secondarySoundPlayer.Volume = value;
            }
        }

        public bool Overlap
        {
            get { return _primarySoundPlayer.Overlap; }
            set
            {
                _primarySoundPlayer.Overlap = value;
                if (_secondarySoundPlayer != null) _secondarySoundPlayer.Overlap = value;
            }
        }

        public SoundManager(OutputDevice primaryDevice, OutputDevice secondaryDevice)
        {
            _random = new Random();
            _primarySoundPlayer = new AudioPlaybackEngine(primaryDevice.Number);
            if (secondaryDevice.Number != -2)
            {
                _secondarySoundPlayer = new AudioPlaybackEngine(secondaryDevice.Number);
            }
        }

        public void Play(Sound sound)
        {
            if (sound.Files.Count == 0)
            {
                return;
            }

            string file = sound.Files[_random.Next(sound.Files.Count)];
            try
            {
                _primarySoundPlayer.PlaySound(file, sound.Volume, sound.Loop);
                if (_secondarySoundPlayer != null) _secondarySoundPlayer.PlaySound(file, sound.Volume, sound.Loop);
            }
            catch (Exception ex)
            {
                sound.Error = ex.ToString();
            }
        }

        public void Pause()
        {
            _primarySoundPlayer.Pause();
            if (_secondarySoundPlayer != null) _secondarySoundPlayer.Pause();
        }

        public void Continue()
        {
            _primarySoundPlayer.Continue();
            if (_secondarySoundPlayer != null) _secondarySoundPlayer.Continue();
        }

        public void Stop()
        {
            _primarySoundPlayer.Stop();
            if (_secondarySoundPlayer != null) _secondarySoundPlayer.Stop();
        }

        public void ChangePrimaryDevice(OutputDevice device)
        {
            AudioPlaybackEngine tmp = new AudioPlaybackEngine(device.Number)
            {
                Overlap = Overlap,
                Volume = Volume
            };
            _primarySoundPlayer.Stop();
            _primarySoundPlayer.Dispose();
            _primarySoundPlayer = tmp;
        }

        public void ChangeSecondaryDevice(OutputDevice device)
        {
            if (_secondarySoundPlayer != null)
            {
                _secondarySoundPlayer.Stop();
                _secondarySoundPlayer.Dispose();
            }
            _secondarySoundPlayer = device.Number == -2 ? null : new AudioPlaybackEngine(device.Number)
            {
                Overlap = Overlap,
                Volume = Volume
            };
        }

        public IList<OutputDevice> EnumerateDevices()
        {
            return _primarySoundPlayer.EnumerateDevices();
        }

        ~SoundManager()
        {
            _primarySoundPlayer.Dispose();
            if (_secondarySoundPlayer != null) _secondarySoundPlayer.Dispose();
        }
    }
}
