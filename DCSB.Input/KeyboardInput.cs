using DCSB.Utils;
using System;
using System.Collections.Generic;
using System.Timers;

namespace DCSB.Input
{
    public class KeyboardInput
    {
        public delegate void KeyboardHookCallback(VKey key, List<VKey> pressedKeys);

        public event KeyboardHookCallback KeyDown;
        public event KeyboardHookCallback KeyUp;
        public event KeyboardHookCallback KeyPress;

        private Dictionary<VKey, VKey> numpadKeys = new Dictionary<VKey, VKey>()
        {
            { VKey.INSERT, VKey.NUMPAD0 },
            { VKey.DELETE, VKey.DECIMAL },
            { VKey.END, VKey.NUMPAD1 },
            { VKey.DOWN, VKey.NUMPAD2 },
            { VKey.PAGE_DOWN, VKey.NUMPAD3 },
            { VKey.LEFT, VKey.NUMPAD4 },
            { VKey.CLEAR, VKey.NUMPAD5 },
            { VKey.RIGHT, VKey.NUMPAD6 },
            { VKey.HOME, VKey.NUMPAD7 },
            { VKey.UP, VKey.NUMPAD8 },
            { VKey.PAGE_UP, VKey.NUMPAD9 }
        };
        private bool shiftNumpadCorrection;
        private Timer shiftNumpadTimer = new Timer();
        private RawInput rawInput;

        private List<VKey> pressedKeys = new List<VKey>();

        public KeyboardInput(IntPtr handle)
        {
            rawInput = new RawInput(handle);
            
            rawInput.KeyPressed += OnKeyPressed;

            shiftNumpadTimer.Elapsed += (x, y) => {
                if (shiftNumpadCorrection)
                {
                    shiftNumpadCorrection = false;
                    Key_Up(VKey.SHIFT);
                }
            };
        }

        private void OnKeyPressed(object sender, InputEventArg e)
        {
            VKey key = (VKey)e.KeyPressEvent.VKey;
            if (e.KeyPressEvent.Message == 256 || e.KeyPressEvent.Message == 260)
            {
                if (!(key == VKey.SHIFT && pressedKeys.Contains(key)))
                {
                    if (shiftNumpadCorrection && numpadKeys.ContainsKey(key))
                    {
                        shiftNumpadTimer.Stop();
                        shiftNumpadCorrection = false;
                        Key_Down(numpadKeys[key]);
                    }
                    else
                    {
                        Key_Down(key);
                    }
                }
            }
            else if (e.KeyPressEvent.Message == 257 || e.KeyPressEvent.Message == 261)
            {
                if (key == VKey.SHIFT)
                {
                    shiftNumpadTimer.Interval = 5;
                    shiftNumpadTimer.Start();
                    shiftNumpadCorrection = true;
                }
                else if (!pressedKeys.Contains(key) && numpadKeys.ContainsKey(key))
                {
                    Key_Up(numpadKeys[key]);
                }
                else
                {
                    Key_Up(key);
                }
            }
        }

        private void Key_Down(VKey key)
        {
            if (!pressedKeys.Contains(key))
            {
                pressedKeys.Add(key);
                KeyPress?.Invoke(key, pressedKeys);
            }
            KeyDown?.Invoke(key, pressedKeys);
        }

        private void Key_Up(VKey key)
        {
            KeyUp?.Invoke(key, pressedKeys);
            if (pressedKeys.Contains(key))
            {
                pressedKeys.Remove(key);
            }
        }
    }
}
