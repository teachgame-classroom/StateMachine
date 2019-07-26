using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace ReflectionHelper
{
    public delegate object CtorDelegate(object[] paras);

    public class ConstructorHolder<T> where T : class
    {
        public Type type;
        public ConstructorInfo[] ctorInfos;

        public ConstructorHolder()
        {
            this.type = typeof(T);
            ctorInfos = ClassFinder.GetConstructors(this.type);
        }

        public T Invoke(int ctorIdx, object[] paras)
        {
            return ctorInfos[ctorIdx].Invoke(paras) as T;
        }

        public T TryInvoke(object[] paras)
        {
            foreach(ConstructorInfo constructorInfo in ctorInfos)
            {
                ParameterInfo[] parameterInfos = constructorInfo.GetParameters();

                if (parameterInfos.Length == paras.Length)
                {
                    bool isMatch = true;

                    for(int i = 0; i < parameterInfos.Length; i++)
                    {
                        if(parameterInfos[i].ParameterType != paras[i].GetType())
                        {
                            isMatch = false;
                        }
                    }

                    if(isMatch)
                    {
                        return constructorInfo.Invoke(paras) as T;
                    }
                }
            }

            return null;
        }
    }

    public static class ClassFinder
    {
        public static Type[] GetSubclassTypes<T>() where T : class
        {
            List<Type> typeList = new List<Type>();
            List<string> nameList = new List<string>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(T)) && type.IsAbstract == false)
                    {
                        typeList.Add(type);
                        nameList.Add(type.Name);
                    }
                }
            }

            return typeList.ToArray();
        }

        public static Type GetType(string className)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == className)
                    {
                        return type;
                    }
                }
            }

            return null;

        }

        public static Assembly GetAssemblyContains(string className)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == className)
                    {
                        return assembly;
                    }
                }
            }

            return null;
        }

        public static ConstructorInfo[] GetConstructorInfos<T>() where T : class
        {
            Type type = typeof(T);
            return type.GetConstructors();
        }

        public static ConstructorInfo[] GetConstructors(Type type)
        {
            return type.GetConstructors();
        }

        public static ConstructorInfo GetConstructor(Type type, Type[] paraTypes)
        {
            ConstructorInfo ctor = type.GetConstructor(paraTypes);
            return ctor;
        }
    }
}
