using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml.Linq;

using VersionIncrementer.Version;

namespace VersionIncrementer {
    class Program {
        readonly static string ConfigFilePath =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" +
            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "\\" +
            "config.xml";

        [STAThread]
        static int Main(string[] args) {
            if (!ReadExecutionArguments(args, out var projectName, out var assemblyInfo, out var noDialog, out var debug)) {
                return -1;
            };

            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));

            if (!LoadConfig(ConfigFilePath, out var updaterDictionary)) {
                updaterDictionary = new Dictionary<string, VersionUpdater>();
            }

            if (!updaterDictionary.TryGetValue(projectName, out var updater)) {
                updater = new VersionUpdater();
            }

            var currentVersion = Helper.AssemblyInfoFileHelper.ReadAssemblyFileVersion(assemblyInfo);
            Version.Version updatedVersion;
            bool saveConfig;
            if (noDialog) {
                updatedVersion = updater.Update(currentVersion);
                saveConfig = true;
            }
            else {
                var dialog = new Dialog.UpdateDialog();
                dialog.Model.ProjectName = projectName;
                dialog.Model.VersionUpdater = updater;
                dialog.Model.CurrentVersion = currentVersion;

                if (!(dialog.ShowDialog() is bool result) || result == false) {
                    return 1;
                }
                saveConfig = dialog.Model.SaveConfig;
                updatedVersion = dialog.Model.UpdatedVersion;
            }

            if (currentVersion != updatedVersion) {
                Helper.AssemblyInfoFileHelper.WriteAssemblyFileVersion(assemblyInfo, updatedVersion);
            }

            if (saveConfig) {
                updaterDictionary[projectName] = updater;
                SaveConfig(ConfigFilePath, updaterDictionary);
            }
            
            return 0;
        }

        static bool ReadExecutionArguments(string[] args, out string projectName, out string assemblyInfo, out bool noDialog, out bool debug) {
            projectName = null; assemblyInfo = null; noDialog = false; debug = false;

            for (var i = 0; i < args.Length; i++) {
                switch (args[i].ToLower()) {
                    case "--projectname":
                    case "-p":
                        projectName = args[++i];
                        break;

                    case "--assemblyinfo":
                    case "-a":
                        assemblyInfo = args[++i];
                        break;

                    case "--nodialog":
                        noDialog = true;
                        break;
                    case "--debug":
                        debug = true;
                        break;

                    default: throw new ArgumentException();
                }
            }

            if (projectName is null) {
                MessageBox.Show("プロジェクト名が指定されていません。(--projectName <プロジェクト名>)", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (assemblyInfo is null) {
                MessageBox.Show("アセンブリの詳細ファイルが指定されていません (--assemblyInfo <ファイル名>)。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        static bool LoadConfig(string filePath, out Dictionary<string, VersionUpdater> updaterDictionary) {
            try {
                if (!File.Exists(filePath)) {
                    updaterDictionary = null;
                    return false;
                }

                updaterDictionary = new Dictionary<string, VersionUpdater>();
                var doc = XDocument.Load(filePath);

                foreach (var element in doc.Elements()) {
                    var projectName = element.Element("ProjectName").Value;
                    var updateRuleElement = element.Element("Updater");
                    var updateModel = new VersionUpdater(
                        convert(updateRuleElement.Element("MajorVersion")), 
                        convert(updateRuleElement.Element("MinorVersion")), 
                        convert(updateRuleElement.Element("BuildNumber")), 
                        convert(updateRuleElement.Element("RevisionNumber"))
                    );
                    updaterDictionary.Add(projectName, updateModel);
                }

                return true;
            }
            catch {
                updaterDictionary = null;
                return false;
            }

            VersionUpdateRule convert(XElement x) {
                var method = (VersionUpdateMethod)Enum.Parse(typeof(VersionUpdateMethod), x.Element(nameof(VersionUpdateRule.Method)).Value);
                var argument = x.Element(nameof(VersionUpdateRule.Argument)).Value is string s && s != "null" ? s : null;

                return new VersionUpdateRule(method, argument);
            }
        }

        static void SaveConfig(string filePath, Dictionary<string, VersionUpdater> updaterDictionary) {
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

            foreach (var pair in updaterDictionary) {
                var majorRuleElement = convert("MajorVersion", pair.Value.MajorVersionUpdateRule);
                var minorRuleElement = convert("MinorVersion", pair.Value.MinorVersionUpdateRule);
                var buildNumberRuleElement = convert("BuildNumber", pair.Value.BuildNumberUpdateRule);
                var revisionRuleElement = convert("RevisionNumber", pair.Value.RevisionUpdateRule);

                var projectNameElement = new XElement("ProjectName", pair.Key);
                var updaterElement = new XElement("Updater", majorRuleElement, minorRuleElement, buildNumberRuleElement, revisionRuleElement);

                var nameUpdaterPairElement = new XElement("NameUpdaterPair", projectNameElement, updaterElement);
                doc.Add(nameUpdaterPairElement);
            }

            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8)) {
                doc.Save(writer);
            }

            XElement convert(string name, VersionUpdateRule updater) {
                var e = new XElement(name,
                            new XElement(nameof(VersionUpdateRule.Method), updater.Method.ToString()),
                            new XElement(nameof(VersionUpdateRule.Argument), updater.Argument is string s ? s : "null")
                        );
                return e;
            }
        }
    }
}
