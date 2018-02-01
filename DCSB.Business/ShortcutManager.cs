using DCSB.Models;
using DCSB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DCSB.Business
{
    public class ShortcutManager
    {
        private ConfigurationModel _configurationModel;
        private SoundManager _soundManager;

        private Random _random;

        public ShortcutManager(ConfigurationModel configurationModel, SoundManager soundManager)
        {
            _configurationModel = configurationModel;
            _soundManager = soundManager;
            _random = new Random();
        }

        public void KeyDown(VKey key, List<VKey> pressedKeys)
        {
            if (_configurationModel.Enable == DisplayOption.Counters || _configurationModel.Enable == DisplayOption.Both)
            {
                Shortcut shortcut = ResolveShortcut(key, pressedKeys, new List<Shortcut>(){
                    _configurationModel.CounterShortcuts.Next,
                    _configurationModel.CounterShortcuts.Previous,
                    _configurationModel.CounterShortcuts.Increment,
                    _configurationModel.CounterShortcuts.Decrement,
                    _configurationModel.CounterShortcuts.Reset
                });
                if (shortcut != null && shortcut.Command.CanExecute(null))
                {
                    shortcut.Command.Execute(null);
                }
            }

            if (_configurationModel.Enable == DisplayOption.Sounds || _configurationModel.Enable == DisplayOption.Both)
            {
                Shortcut shortcut = ResolveShortcut(key, pressedKeys, new List<Shortcut>(){
                    _configurationModel.SoundShortcuts.Pause,
                    _configurationModel.SoundShortcuts.Continue,
                    _configurationModel.SoundShortcuts.Stop
                });
                if (shortcut != null && shortcut.Command.CanExecute(null))
                {
                    shortcut.Command.Execute(null);
                }
            }
        }

        public void KeyPress(VKey key, List<VKey> pressedKeys)
        {
            if (_configurationModel.Enable == DisplayOption.Sounds || _configurationModel.Enable == DisplayOption.Both)
            {
                Sound sound= ResolveShortcut(key, pressedKeys, _configurationModel.SelectedPreset.SoundCollection.Where(x => x.Files.Count != 0));
                if (sound != null)
                {
                    _configurationModel.SelectedPreset.SelectedSound = sound;
                    _soundManager.Play(sound);
                }
            }

            Preset preset = ResolveShortcut(key, pressedKeys, _configurationModel.PresetCollection);
            if (preset != null)
            {
                _configurationModel.SelectedPreset = preset;
            }
        }

        private T ResolveShortcut<T>(VKey key, IEnumerable<VKey> pressedKeys, IEnumerable<T> items) where T : IBindable
        {
            return items.Where(x => x.Keys.Contains(key) && x.Keys.All(y => pressedKeys.Contains(y))).OrderBy(x => x.Keys.Count).LastOrDefault();
        }
    }
}
