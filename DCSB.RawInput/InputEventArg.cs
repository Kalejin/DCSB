using System;

namespace KalejinsAssistant.RawInput
{
    public class InputEventArg : EventArgs
    {
        public InputEventArg(KeyPressEvent arg)
        {
            KeyPressEvent = arg;
        }

        private InputEventArg() { }

        public KeyPressEvent KeyPressEvent { get; private set; }
    }
}
