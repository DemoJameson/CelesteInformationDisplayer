using System;
using System.IO;
using On.Monocle;

namespace Celeste.Mod.InformationDisplayer {
    public class InformationDisplayerModule : EverestModule {
        public InformationDisplayerModule() {
            Instance = this;
        }

        public override Type SaveDataType => typeof(InformationDisplayerSaveData);
        public override Type SettingsType => typeof(InformationDisplayerSettings);

        public static InformationDisplayerModule Instance;
        public static InformationDisplayerSaveData SaveData {
            get {
                // failsafe: if DifferentSeeker is null, initialize it. THIS SHOULD NEVER HAPPEN, but already happened in a case of a corrupted save.
                if (((InformationDisplayerSaveData) Instance._SaveData)?.DifferentEntityCount == null) {
                    Logger.Log("InformationDisplayer",
                        "WARNING: SaveData was null. This should not happen. Initializing it to an empty save data.");
                    Instance._SaveData = new InformationDisplayerSaveData();
                }

                return (InformationDisplayerSaveData) Instance._SaveData;
            }
            set => Instance._SaveData = value;
        }
        public static InformationDisplayerSettings Settings => (InformationDisplayerSettings) Instance._Settings;

        public override void Load() {
            InformationDisplayer.Load();
            InformationDisplayerSaveData.Load();
            On.Celeste.Level.Update += LevelOnUpdate;
        }

        public override void Unload() {
            InformationDisplayer.Unload();
            InformationDisplayerSaveData.Unload();
            On.Celeste.Level.Update -= LevelOnUpdate;
        }

        private void LevelOnUpdate(On.Celeste.Level.orig_Update orig, Level self) {
            orig(self);
            if (!self.Paused) {
                if (Settings.ButtonEnabled.Button.Pressed) {
                    ReloadSettings();
                    Settings.Enabled = !Settings.Enabled;
                    SaveSettings();
                }

                if (Settings.ButtonClearData.Button.Pressed) {
                    InformationDisplayerSaveData.Clear();
                }
            }
        }

        private void ReloadSettings() {
            InformationDisplayerSettings newSettings = (InformationDisplayerSettings) SettingsType
                .GetConstructor(Everest._EmptyTypeArray)?.Invoke(Everest._EmptyObjectArray);
            string path = UserIO.GetSaveFilePath("modsettings-" + Metadata.Name);
            if (!File.Exists(path))
                return;
            try {
                using (Stream stream = File.OpenRead(path)) {
                    using (StreamReader streamReader = new StreamReader(stream))
                        YamlHelper.DeserializerUsing(newSettings).Deserialize(streamReader, this.SettingsType);
                }
            } catch {
                // ignored
            }

            if (newSettings != null) {
                Settings.UpdateFrom(newSettings);
            }
        }
    }
}