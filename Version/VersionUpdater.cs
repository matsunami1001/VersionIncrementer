using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace VersionIncrementer.Version {
    public class VersionUpdater {

        public VersionUpdater() : this(
                new VersionUpdateRule(VersionUpdateMethod.None, null),
                new VersionUpdateRule(VersionUpdateMethod.None, null),
                new VersionUpdateRule(VersionUpdateMethod.None, null),
                new VersionUpdateRule(VersionUpdateMethod.Increment, null)
            ) { }

        public VersionUpdater(VersionUpdateRule majorVersionUpdateRule, VersionUpdateRule minorVersionUpdateRule, VersionUpdateRule buildNumberUpdateRule, VersionUpdateRule revisionUpdateRule) {
            
            MajorVersionUpdateRule = majorVersionUpdateRule ?? throw new ArgumentNullException(nameof(majorVersionUpdateRule));
            MinorVersionUpdateRule = minorVersionUpdateRule ?? throw new ArgumentNullException(nameof(minorVersionUpdateRule));
            BuildNumberUpdateRule = buildNumberUpdateRule ?? throw new ArgumentNullException(nameof(buildNumberUpdateRule));
            RevisionUpdateRule = revisionUpdateRule ?? throw new ArgumentNullException(nameof(revisionUpdateRule));

            MajorVersionUpdateRule.PropertyChanged += (sender, e) => OnRuleChanged((VersionUpdateRule)sender);
            MinorVersionUpdateRule.PropertyChanged += (sender, e) => OnRuleChanged((VersionUpdateRule)sender);
            BuildNumberUpdateRule.PropertyChanged += (sender, e) => OnRuleChanged((VersionUpdateRule)sender);
            RevisionUpdateRule.PropertyChanged += (sender, e) => OnRuleChanged((VersionUpdateRule)sender);
        }

        public VersionUpdateRule MajorVersionUpdateRule { get; }

        public VersionUpdateRule MinorVersionUpdateRule { get; }

        public VersionUpdateRule BuildNumberUpdateRule { get; }

        public VersionUpdateRule RevisionUpdateRule { get; }

        public event Action<VersionUpdateRule> RuleChanged;

        protected virtual void OnRuleChanged(VersionUpdateRule rule) => RuleChanged?.Invoke(rule);

        public Version Update(Version version) {
            version = MajorVersionUpdateRule.Update(version, VersionSection.Major);
            version = MinorVersionUpdateRule.Update(version, VersionSection.Minor);
            version = BuildNumberUpdateRule.Update(version, VersionSection.BuildNumber);
            version = RevisionUpdateRule.Update(version, VersionSection.Revision);

            return version;
        }
    }
}
