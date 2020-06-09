using System;
using System.Collections.Generic;
using Celeste.Mod.InformationDisplayer.Extensions;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.InformationDisplayer {
    public class InformationDisplayerSaveData : EverestModuleSaveData {
        private static InformationDisplayerSaveData SaveData {
            get => InformationDisplayerModule.SaveData;
            set => InformationDisplayerModule.SaveData = value;
        }

        public Dictionary<string, int> EntityCount { get; set; } = new Dictionary<string, int>();

        public Dictionary<string, HashSet<string>> DifferentEntityCount { get; set; } =
            new Dictionary<string, HashSet<string>>();

        private static void Save() {
            InformationDisplayerModule.Instance.SaveSaveData(SaveData.Index);
        }

        public static void Clear() {
            SaveData = new InformationDisplayerSaveData();
            Save();
        }

        public static void Load() {
            On.Monocle.Entity.ctor_Vector2 += EntityOnCtor_Vector2;
            On.Monocle.Entity.RemoveSelf += EntityOnRemoveSelf;
            On.Celeste.Snowball.Destroy += SnowballOnDestroy;
        }

        public static void Unload() {
            On.Monocle.Entity.ctor_Vector2 -= EntityOnCtor_Vector2;
            On.Monocle.Entity.RemoveSelf -= EntityOnRemoveSelf;
            On.Celeste.Snowball.Destroy -= SnowballOnDestroy;
        }

        private static void EntityOnCtor_Vector2(On.Monocle.Entity.orig_ctor_Vector2 orig, Entity self,
            Vector2 position) {
            orig(self, position);

            if (GetSession() == null) {
                return;
            }

            Session session = GetSession();

            // 构造唯一ID
            // 地图+房间+生成位置
            string id = session.Area + "-" + session.Level + "-" + position;
            self.SetId(id);
        }

        private static void Count(Entity entity) {
            Type type = entity.GetType();
            if (Variable.TypeNeedCount.ContainsKey(type)) {
                string variable = Variable.TypeNeedCount[type];
                if (!SaveData.EntityCount.ContainsKey(variable)) {
                    SaveData.EntityCount[variable] = 0;
                }

                SaveData.EntityCount[variable] = SaveData.EntityCount[variable] + 1;

                if (entity.GetId() != null) {
                    string diffVariable = Variable.GetDifferentVariable(variable);
                    if (!SaveData.DifferentEntityCount.ContainsKey(diffVariable)) {
                        SaveData.DifferentEntityCount[diffVariable] = new HashSet<string>();
                    }

                    SaveData.DifferentEntityCount[diffVariable].Add(entity.GetId());
                }
            }
        }

        private static void EntityOnRemoveSelf(On.Monocle.Entity.orig_RemoveSelf orig, Monocle.Entity self) {
            orig(self);

            Count(self);
        }


        private static void SnowballOnDestroy(On.Celeste.Snowball.orig_Destroy orig, Snowball self) {
            orig(self);

            Count(self);
        }

        private static Session GetSession() {
            Level level = null;
            if (Engine.Scene is Level) {
                level = (Level) Engine.Scene;
            } else if (Engine.Scene is LevelLoader levelLoader) {
                level = levelLoader.Level;
            }

            return level?.Session;
        }
    }
}