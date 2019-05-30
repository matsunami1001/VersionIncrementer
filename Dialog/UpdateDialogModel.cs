using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VersionIncrementer;
using VersionIncrementer.Version;

namespace VersionIncrementer.Dialog {

    internal class UpdateDialogModel : INotifyPropertyChanged {

        public IEnumerable<VersionUpdateMethod> Methods { get; } =
            Enum.GetValues(typeof(VersionUpdateMethod)).Cast<VersionUpdateMethod>();

        #region ProjectName
        private string projectName;

        /// <summary>
        /// ProjectName を取得または設定します。
        /// </summary>
        public string ProjectName {
            get => projectName;
            set {
                if (projectName == value) return;
                projectName = value;
                OnPropertyChanged(nameof(ProjectName));
            }
        }
        #endregion

        #region VersionUpdater
        private VersionUpdater versionUpdater;

        /// <summary>
        /// VersionUpdater を取得または設定します。
        /// </summary>
        public VersionUpdater VersionUpdater {
            get => versionUpdater;
            set {
                if (versionUpdater == value) return;
                versionUpdater = value;
                versionUpdater.RuleChanged += rule => UpdateVersion();
                OnPropertyChanged(nameof(VersionUpdater));
            }
        }
        #endregion

        #region CurrentVersion
        private Version.Version currentVersion;

        /// <summary>
        /// CurrentVersion を取得または設定します。
        /// </summary>
        public Version.Version CurrentVersion {
            get => currentVersion;
            set {
                if (currentVersion == value) return;
                currentVersion = value;
                OnPropertyChanged(nameof(CurrentVersion));

                UpdateVersion();
            }
        }
        #endregion

        #region UpdatedVersion
        private Version.Version updatedVersion;

        /// <summary>
        /// UpdatedVersion を取得または設定します。
        /// </summary>
        public Version.Version UpdatedVersion {
            get => updatedVersion;
            set {
                if (updatedVersion == value) return;
                updatedVersion = value;
                OnPropertyChanged(nameof(UpdatedVersion));
            }
        }
        #endregion
        
        #region SaveConfig
        private bool saveConfig = true;

        /// <summary>
        /// SaveConfig を取得または設定します。
        /// </summary>
        public bool SaveConfig {
            get => saveConfig;
            set {
                if (saveConfig == value) return;
                saveConfig = value;
                OnPropertyChanged(nameof(SaveConfig));
            }
        }
        #endregion

        private void UpdateVersion() {
            if (VersionUpdater?.Update(currentVersion) is Version.Version version)
                UpdatedVersion = version;
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
