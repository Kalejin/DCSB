using DCSB.Models;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public AudioPlaybackEngine(string deviceName)
        {
            OutputDevice device = EnumerateDevices().Where(x => x.Name == deviceName).FirstOrDefault();
            if (device == null)
            {
                _outputDevice = new WaveOutEvent() { DeviceNumber = -1 };
            }
            else
            {
                _outputDevice = new WaveOutEvent() { DeviceNumber = device.Number };
            }
            
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
            var allDevices = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.All);

            IList<OutputDevice> devices = new List<OutputDevice>
            {
                new OutputDevice(-1, "Default Output Device")
            };
            for (int n = 0; n < WaveOut.DeviceCount; n++)
            {
                string incompleteName = WaveOut.GetCapabilities(n).ProductName;
                MMDevice device = allDevices.Where(x => x.FriendlyName.StartsWith(incompleteName)).FirstOrDefault();
                devices.Add(new OutputDevice(n, device == null ? incompleteName : device.FriendlyName));
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
