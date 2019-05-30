using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace VersionIncrementer.Version {
    public enum VersionUpdateMethod {
        None,
        Increment,
        SetNumber,
        SetDate
    }
}
