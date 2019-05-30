using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace VersionIncrementer.Version {
    public struct Version : IEquatable<Version> {

        private ushort major;
        public ushort Major {
            get => major;
            set {
                if (major != value) {
                    major = value;
                    Minor = Minor is null ? null : (ushort?)0;
                    BuildNumber = BuildNumber is null ? null : (ushort?)0;
                    Revision = Revision is null ? null : (ushort?)0;
                }
            }
        }

        private ushort? minor;
        public ushort? Minor {
            get => minor;
            set {
                if (minor == value) {
                    return;
                }
                else if (value is ushort v) {
                    minor = v;
                    BuildNumber = BuildNumber is null ? null : (ushort?)0;
                    Revision = Revision is null ? null : (ushort?)0;
                }
                else {
                    minor = null;
                    BuildNumber = null;
                    Revision = null;
                }
            }
        }

        private ushort? buildNumber;
        public ushort? BuildNumber {
            get => buildNumber;
            set {
                if (buildNumber == value) {
                    return;
                }
                else if (value is ushort v) {
                    Minor = Minor is ushort m ? m : (ushort)0;
                    buildNumber = v;
                    Revision = Revision is null ? null : (ushort?)0;
                }
                else {
                    buildNumber = null;
                    Revision = null;
                }
            }
        }

        private ushort? revision;

        public ushort? Revision {
            get => revision;
            set {
                if (revision == value) {
                    return;
                }
                else if (value is ushort v) {
                    Minor = Minor is ushort m ? m : (ushort)0;
                    BuildNumber = BuildNumber is ushort bn ? bn : (ushort)0;
                    revision = v;
                }
                else {
                    revision = null;
                }
            }
        }

        public Version(ushort major, ushort? minor, ushort? buildNumber, ushort? revision) : this() {
            Major = major;
            Minor = minor;
            BuildNumber = buildNumber;
            Revision = revision;
        }

        public ushort? GetNumber(VersionSection section) {
            switch (section) {
                case VersionSection.Major:
                    return Major;
                case VersionSection.Minor:
                    return Minor;
                case VersionSection.BuildNumber:
                    return BuildNumber;
                case VersionSection.Revision:
                    return Revision;
                default: throw new NotImplementedException();
            }
        }

        public void SetNumber(VersionSection section, ushort? value) {
            switch (section) {
                case VersionSection.Major:
                    Major = value.Value;
                    break;
                case VersionSection.Minor:
                    Minor = value;
                    break;
                case VersionSection.BuildNumber:
                    BuildNumber = value;
                    break;
                case VersionSection.Revision:
                    Revision = value;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool TryParse(string str, out Version result) {
            var nums = str.Split('.');
            if (5 <= nums.Length) {
                result = default;
                return false;
            }

            var versionNums = new ushort?[4];
            for (var i = 0; i < 4; i++) {
                if (nums.Length <= i) {
                    versionNums[i] = null;
                }
                else if (nums[i] == "*") {
                    versionNums[i] = 0;
                }
                else if (!ushort.TryParse(nums[i], out var v)) {
                    result = default;
                    return false;
                }
                else {
                    versionNums[i] = v;
                }
            }

            result = new Version(versionNums[0].Value, versionNums[1], versionNums[2], versionNums[3]);
            return true;
        }

        public override string ToString()
            => string.Join(".", new ushort?[] { Major, Minor, BuildNumber, Revision }.OfType<ushort>().Select(v => v.ToString()).ToArray());

        public override bool Equals(object obj) {
            return obj is Version version && Equals(version);
        }

        public bool Equals(Version other) {
            return Major == other.Major &&
                   EqualityComparer<ushort?>.Default.Equals(Minor, other.Minor) &&
                   EqualityComparer<ushort?>.Default.Equals(BuildNumber, other.BuildNumber) &&
                   EqualityComparer<ushort?>.Default.Equals(Revision, other.Revision);
        }

        public override int GetHashCode() {
            var hashCode = 1605762154;
            hashCode = hashCode * -1521134295 + Major.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(Minor);
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(BuildNumber);
            hashCode = hashCode * -1521134295 + EqualityComparer<ushort?>.Default.GetHashCode(Revision);
            return hashCode;
        }

        public static bool operator ==(Version left, Version right) {
            return left.Equals(right);
        }

        public static bool operator !=(Version left, Version right) {
            return !(left == right);
        }
    }
}
