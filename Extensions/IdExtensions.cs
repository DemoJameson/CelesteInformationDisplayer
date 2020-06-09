using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.InformationDisplayer.Extensions {
    public static class IdExtensions {
        private const string Id = "Celeste.Mod.InformationDisplayer.Id";

        public static void SetId(this Entity entity, string id) {
            entity.SetExtendedDataValue(Id, id);
        }

        public static string GetId(this Entity entity) {
            return entity.GetExtendedDataValue<string>(Id);
        }
   
    }
}