using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IocExample
{
    public class Utils
    {
        public static object CreateInstance(Type implementationType, List<object> parameters)
        {
            return Activator.CreateInstance(implementationType, parameters.ToArray());
        }

        public static object CreateInstance(Type implementationType)
        {
            return Activator.CreateInstance(implementationType);
        }

        public static ConstructorInfo GetSingleConstructor(Type implementationType)
        {
            return implementationType.GetConstructors().SingleOrDefault();
        }
    }
}