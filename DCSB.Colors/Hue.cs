using System;
using System.Windows.Media;

namespace DCSB.Colors
{
    public class Hue
    {
        public Hue(string name, Color color, Color foreground)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Color = color;
            Foreground = foreground;
        }

        public string Name { get; }

        public Color Color { get; }

        public Color Foreground { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
