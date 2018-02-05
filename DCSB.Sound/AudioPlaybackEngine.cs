using DCSB.Models;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DCSB.SoundPlayer
{
    public class AudioPlaybackEngine
    {
        private readonly WaveOutEvent _outputDevice;
        private readonly MixingSampleProvider _mixer;

        private int _volumePowBase = 100;

        public float Volume
        {
            get { return RevertVolume(_outputDevice.Volume); }
            set { _outputDevice.Volume = AdjustVolume(value); }
        }

        public bool Overlap { get; set; }

        public AudioPlaybackEngine(int deviceNumber)
        {
            _outputDevice = new WaveOutEvent() { DeviceNumber = deviceNumber };
            _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2)) { ReadFully = true };
            _outputDevice.Init(_mixer);
            _outputDevice.Play();
        }

        public void PlaySound(string fileName, float volume, bool loop)
        {
            if (!Overlap)
            {
                Stop();
            }

            IAudioReader input;
            try
            {
                input = new FileReader(fileName);
            }
            catch (COMException)
            {
                input = new OggFileReader(fileName);
            }

            SampleReader reader = new SampleReader(input, loop);
            AddMixerInput(reader, volume);

            if (_outputDevice.PlaybackState != PlaybackState.Playing)
            {
                _outputDevice.Play();
            }
        }

        public void Pause()
        {
            if (_outputDevice.PlaybackState == PlaybackState.Playing)
            {
                _outputDevice.Pause();
            }
        }

        public void Continue()
        {
            if (_outputDevice.PlaybackState == PlaybackState.Paused)
            {
                _outputDevice.Play();
            }
        }

        public void Stop()
        {
            _mixer.RemoveAllMixerInputs();
            _outputDevice.Stop();
            _outputDevice.Init(_mixer);
        }

        public IList<OutputDevice> EnumerateDevices()
        {
            IList<OutputDevice> devices = new List<OutputDevice>
            {
                new OutputDevice(-1, "Default Output Device")
            };
            for (int n = 0; n < WaveOut.DeviceCount; n++)
            {
                devices.Add(new OutputDevice(n, WaveOut.GetCapabilities(n).ProductName));
            }
            return devices;
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == _mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && _mixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException($"Channel conversion from {input.WaveFormat.Channels} to {_mixer.WaveFormat.Channels} is not supported.");
        }

        private ISampleProvider ConvertToRightSampleRate(ISampleProvider input)
        {
            if (input.WaveFormat.SampleRate == _mixer.WaveFormat.SampleRate)
            {
                return input;
            }
            return new WdlResamplingSampleProvider(input, _mixer.WaveFormat.SampleRate);
        }

        private void AddMixerInput(ISampleProvider input, float volume)
        {
            ISampleProvider convertedInput = ConvertToRightSampleRate(ConvertToRightChannelCount(input));
            VolumeSampleProvider volumeSampleProvider = new VolumeSampleProvider(convertedInput) { Volume = AdjustVolume(volume) };
            _mixer.AddMixerInput(volumeSampleProvider);
        }

        private float AdjustVolume(float volume)
        {
            return (float)((Math.Pow(_volumePowBase, volume) - 1) / (_volumePowBase - 1));
        }

        private int RevertVolume(float volume)
        {
            return (int)(Math.Log(volume * (_volumePowBase - 1) + 1) / Math.Log(_volumePowBase));
        }

        public void Dispose()
        {
            _outputDevice.Dispose();
        }
    }
}
