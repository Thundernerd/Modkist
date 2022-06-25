using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Debug = UnityEngine.Debug;

namespace ModIO
{
    public static class MethodLogger
    {
        public static void LogCall()
        {
            StackTrace stackTrace = new StackTrace(0);
            StackFrame frame = stackTrace.GetFrame(1);
            MethodBase methodBase = frame.GetMethod();

            string declaringTypeName = methodBase.DeclaringType.Name;
            string methodName = methodBase.Name;
            
            if (methodBase.Name == "MoveNext" && methodBase.DeclaringType.Name.StartsWith("<"))
            {
                declaringTypeName = methodBase.DeclaringType.DeclaringType.Name;
                methodName = Regex.Match(methodBase.DeclaringType.Name, @"(?!<)([A-Z])\w+(?=>)").Value;
            }

            Debug.Log($"[TRACE] {declaringTypeName}.{methodName}");
        }
    }
}
