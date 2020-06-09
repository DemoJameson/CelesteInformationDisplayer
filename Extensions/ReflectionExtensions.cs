using System;
using System.Reflection;

namespace Celeste.Mod.InformationDisplayer.Extensions {
    public static class ReflectionExtensions {
        public static FieldInfo GetPrivateFieldInfo(this Type type, string fieldName) {
            return type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static MethodInfo GetPrivateMethodInfo(this Type type, string methodName) {
            return type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        }
        
        
        public static object Invoke(this MethodInfo methodInfo, Object obj, params object[] parameters) {
            return methodInfo.Invoke(obj, parameters);
        }
    }
}