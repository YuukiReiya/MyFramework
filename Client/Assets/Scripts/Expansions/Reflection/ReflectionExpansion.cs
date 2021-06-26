using System;
using System.Linq;
using System.Reflection;

namespace Expansion
{
    public static class ReflectionExpansion
    {
        public static object Invoke(this MethodBase self, object obj, object[] parameters, Action preCallback = null, Action postCallback = null)
        {
            preCallback?.Invoke();
            var method = self.Invoke(obj, parameters);
            postCallback?.Invoke();
            return method;
        }
    }
}