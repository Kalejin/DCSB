using DCSB.Models;
using DCSB.SoundPlayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DCSB.Business
{
    public class SoundManager
    {
        private AudioPlaybackEngine _primarySoundPlayer;
        private AudioPlaybackEngine _secondarySoundPlayer;
        private Random _random;

        private float _volume = 1f;
        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                if (_primarySoundPlayer != null) _primarySoundPlayer.Volume = _volume * _primaryDeviceVolume;
                if (_secondarySoundPlayer != null) _secondarySoundPlayer.Volume = _volume * _secondaryDeviceVolume;
            }
        }

        private float _primaryDeviceVolume = 1f;
        public float PrimaryDeviceVolume
        {
            get { return _primaryDeviceVolume; }
            set
            {
                _primaryDeviceVolume = value;
                if (_primarySoundPlayer != null) _primarySoundPlayer.Volume = _volume * _primaryDeviceVolume;
            }
        }

        private float _secondaryDeviceVolume = 1f;
        public float SecondaryDeviceVolume
        {
            get { return _secondaryDeviceVolume; }
            set
            {
                _secondaryDeviceVolume = value;
                if (_secondarySoundPlayer != null) _secondarySoundPlayer.Volume = _volume * _secondaryDeviceVolume;
            }
        }

        private bool _overlap;
        public bool Overlap
        {
            get { return _overlap; }
            set
            {
                _overlap = value;
                if (_primarySoundPlayer != null) _primarySoundPlayer.Overlap = value;
                if (_secondarySoundPlayer != null) _secondarySoundPlayer.Overlap = value;
            }
        }

        public SoundManager(ConfigurationModel configurationModel)
        {
            _random = new Random();

            configurationModel.PrimaryOutput = ChangePrimaryOutput(configurationModel.PrimaryOutput);
            configurationModel.SecondaryOutput = ChangeSecondaryOutput(configurationModel.SecondaryOutput);

            Volume = configurationModel.Volume / 100f;
            PrimaryDeviceVolume = configurationModel.PrimaryDeviceVolume / 100f;
            SecondaryDeviceVolume = configurationModel.SecondaryDeviceVolume / 100f;
            Overlap = configurationModel.Overlap;
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
                if (_primarySoundPlayer != null) _primarySoundPlayer.PlaySound(file, sound.Volume / 100f, sound.Loop);
                if (_secondarySoundPlayer != null) _secondarySoundPlayer.PlaySound(file, sound.Volume / 100f, sound.Loop);
            }
            catch (Exception ex)
            {
                sound.Error = ex.ToString();
            }
        }

        public void Pause()
        {
            if (_primarySoundPlayer != null) _primarySoundPlayer.Pause();
            if (_secondarySoundPlayer != null) _secondarySoundPlayer.Pause();
        }

        public void Continue()
        {
            if (_primarySoundPlayer != null) _primarySoundPlayer.Continue();
            if (_secondarySoundPlayer != null) _secondarySoundPlayer.Continue();
        }

        public void Stop()
        {
            if (_primarySoundPlayer != null) _primarySoundPlayer.Stop();
            if (_secondarySoundPlayer != null) _secondarySoundPlayer.Stop();
        }

        private string InstantiateDevice(string deviceName, bool primary, ref AudioPlaybackEngine soundPlayer)
        {
            KeyValuePair<int, string> device = GetDevice(deviceName, primary);

            if (device.Equals(default(KeyValuePair<int, string>)) || device.Value == "Disabled")
            {
                soundPlayer = null;
                return "Disabled";
            }

            soundPlayer = new AudioPlaybackEngine(device.Key);
            return device.Value;
        }

        private string ChangeDevice(string deviceName, float deviceVolume, ref AudioPlaybackEngine soundPlayer)
        {
            if (soundPlayer != null)
            {
                soundPlayer.Stop();
                soundPlayer.Dispose();
            }

            string selectedDeviceName = InstantiateDevice(deviceName, soundPlayer == _primarySoundPlayer, ref soundPlayer);

            if (soundPlayer != null)
            {
                soundPlayer.Overlap = Overlap;
                soundPlayer.Volume = _volume * deviceVolume;
            }

            return selectedDeviceName;
        }

        public string ChangePrimaryOutput(string deviceName)
        {
            return ChangeDevice(deviceName, _primaryDeviceVolume, ref _primarySoundPlayer);
        }

        public string ChangeSecondaryOutput(string deviceName)
        {
            return ChangeDevice(deviceName, _secondaryDeviceVolume, ref _secondarySoundPlayer);
        }

        public ICollection<string> EnumerateDevices()
        {
            return AudioPlaybackEngine.EnumerateDevices().Values;
        }

        public KeyValuePair<int, string> GetDevice(string name, bool primary)
        {
            IDictionary<int, string> devices = AudioPlaybackEngine.EnumerateDevices();
            KeyValuePair<int, string> device = devices.Where(x => x.Value == name).FirstOrDefault();

            if (primary && device.Equals(default(KeyValuePair<int, string>)) && devices.Count > 2)
            {
                return new KeyValuePair<int, string>(-1, devices[-1]);
            }

            return device;
        }

        ~SoundManager()
        {
            if (_primarySoundPlayer != null) _primarySoundPlayer.Dispose();
            if (_secondarySoundPlayer != null) _secondarySoundPlayer.Dispose();
        }
    }
}
