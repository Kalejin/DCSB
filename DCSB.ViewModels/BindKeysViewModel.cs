using DCSB.Input;
using DCSB.Models;
using DCSB.Utils;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Windows.Input;

namespace DCSB.ViewModels
{
    public class BindKeysViewModel
    {
        private KeyboardInput _keyboardInput;

        public BindKeysViewModel(KeyboardInput keyboardInput)
        {
            _keyboardInput = keyboardInput;
        }

        public IBindable Bindable {get; set; }

        public ICommand ClearKeysCommand
        {
            get { return new RelayCommand(ClearKeys); }
        }
        private void ClearKeys()
        {
            _keyboardInput.KeyUp -= KeyUp;
            Bindable.Keys.Clear();
        }

        private void KeyUp(VKey key, List<VKey> pressedKeys)
        {
            _keyboardInput.KeyUp -= KeyUp;
            Bindable.Keys.Clear();
            foreach (VKey pressedKey in pressedKeys)
                Bindable.Keys.Add(pressedKey);
        }
    }
}
