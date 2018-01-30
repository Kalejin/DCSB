using GalaSoft.MvvmLight;
using System;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace DCSB.Models
{
    public class Counter : ObservableObject
    {
        private string _name;
        [XmlElement(Order = 1)]
        public string Name {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private string _file;
        [XmlElement(Order = 4)]
        public string File
        {
            get { return _file; }
            set
            {
                _file = value;
                RaisePropertyChanged("File");
                ReadFromFile();
                WriteToFile();
            }
        }

        private string _format;
        [XmlElement(Order = 3)]
        public string Format
        {
            get { return _format; }
            set
            {
                _format = value;
                RaisePropertyChanged("Format");
                WriteToFile();
            }
        }

        private int _count;
        [XmlIgnore]
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                RaisePropertyChanged("Count");
                WriteToFile();
            }
        }

        private int _increment;
        [XmlElement(Order = 2)]
        public int Increment
        {
            get { return _increment; }
            set
            {
                _increment = value;
                RaisePropertyChanged("Increment");
            }
        }

        private string _error;
        [XmlIgnore]
        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                RaisePropertyChanged("Error");
            }
        }

        public Counter()
        {
            Format = "{0}";
            Increment = 1;
        }

        private void WriteToFile()
        {
            Error = null;
            if (System.IO.File.Exists(File))
            {
                string formatted;
                try
                {
                    formatted = string.Format(Format, Count);
                    System.IO.File.WriteAllText(File, formatted);
                }
                catch (FormatException)
                {
                    Error = string.Format("Format '{0}' is invalid.", Format);
                }
                catch (UnauthorizedAccessException)
                {
                    Error = string.Format("Unauthorized to access file '{0}'.", File);
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                }
            }
            else
            {
                Error = string.Format("File '{0}' does not exist.", File);
            }
        }

        private void ReadFromFile()
        {
            Error = null;
            if (System.IO.File.Exists(File))
            {
                string fileContent;
                try
                {
                    fileContent = System.IO.File.ReadAllText(File);

                    if (fileContent == "")
                    {
                        return;
                    }

                    string pattern = "^" + Format.Replace("{0}", @"(?<count>\d+)") + "$";
                    Regex regex = new Regex(pattern);
                    Match match = regex.Match(fileContent);

                    if (!match.Success)
                    {
                        Error = string.Format("Format '{0}' does not match file {1} content.", Format, File);
                        return;
                    }

                    Count = int.Parse(match.Groups["count"].Value);
                }
                catch (UnauthorizedAccessException)
                {
                    Error = string.Format("Unauthorized to access file '{0}'.", File);
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                }
            }
            else
            {
                Error = string.Format("File '{0}' does not exist.", File);
            }
        }
    }
}
