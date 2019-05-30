using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VersionIncrementer.Helper {
    internal static class GeneralHelper {
        public static T[] GetEnumValues<T>() where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().ToArray();
    }
}
