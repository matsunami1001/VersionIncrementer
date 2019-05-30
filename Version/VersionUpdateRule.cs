using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace VersionIncrementer.Version {
    public class VersionUpdateRule : INotifyPropertyChanged {

        public VersionUpdateRule(VersionUpdateMethod method, string argument) {
            Method = method;
            if (method != VersionUpdateMethod.None)
                Argument = argument;
        }

        private VersionUpdateMethod method;
        public VersionUpdateMethod Method {
            get => method;
            set {
                if (method != value) {
                    Argument = null;
                    method = value;
                    OnPropertyChanged(nameof(Method));
                }
            }
        }

        private string argument = default;
        public string Argument {
            get => argument;
            set {
                if (argument == value)
                    return;
                else if (string.IsNullOrEmpty(value)) {
                    argument = null;
                    return;
                }

                switch (Method) {
                    case VersionUpdateMethod.None:
                        break;
                    case VersionUpdateMethod.Increment:
                    case VersionUpdateMethod.SetNumber:
                        ushort.Parse(value);
                        break;
                    case VersionUpdateMethod.SetDate:
                        DateTime.Now.ToString(value);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                argument = value;
                OnPropertyChanged(nameof(Argument));
            }
        }

        public Version Update(Version version, VersionSection section) => Update(version, section, Method, Argument);

        private static Version Update(Version version, in VersionSection section, in VersionUpdateMethod rule, in string argument) {
            var oldNumber = version.GetNumber(section);
            if (oldNumber is null) return version;

            switch (rule) {
                case VersionUpdateMethod.None:
                    break;
                case VersionUpdateMethod.Increment: {
                    var incrementValue = argument is string a ? ushort.Parse(a) : (ushort)1;
                    var overflow = ushort.MaxValue < oldNumber + incrementValue;
                    if (overflow) {
                        version = Update(version, section, VersionUpdateMethod.SetNumber, "0");

                        if (section == VersionSection.Major)
                            throw new ApplicationException("オーバーフローしました。このバージョンをこれ以上インクリメントすることはできません。");

                        var aboveSection = section - 1;

                        version = Update(version, aboveSection, VersionUpdateMethod.Increment, "1");
                    }
                    else {
                        var newNumber = (ushort)(oldNumber + incrementValue);
                        version.SetNumber(section, newNumber);
                    }
                    break;
                }
                case VersionUpdateMethod.SetNumber: {
                    var newNumber = argument is string a ? ushort.Parse(a) : (ushort)0;
                    version.SetNumber(section, newNumber);
                    break;
                }
                case VersionUpdateMethod.SetDate: {
                    var newNumber = argument is string a ? ushort.Parse(DateTime.Now.ToString(a)) : (ushort)0;
                    version.SetNumber(section, newNumber);
                    break;
                }
                default:
                    throw new NotImplementedException();
            }

            return version;
        }

        #region INotifyPropertyChanged インターフェースとそれに伴う実装
        protected Dictionary<string, PropertyChangedEventArgs> EventArgsCollection = new Dictionary<string, PropertyChangedEventArgs>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            if (!EventArgsCollection.TryGetValue(propertyName, out var e)) {
                e = new PropertyChangedEventArgs(propertyName);
                EventArgsCollection.Add(propertyName, e);
            }
            PropertyChanged?.Invoke(this, e);
        }
        #endregion
    }
}
