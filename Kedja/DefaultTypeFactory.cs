using System;
using System.Collections.Generic;
using System.Linq;

namespace Kedja {
    internal class DefaultTypeFactory : ITypeFactory {
        private readonly Dictionary<Type, Type> _typeCache = new Dictionary<Type,Type>();

        public T Create<T>() {
            var createType = typeof(T);
            if(createType.IsInterface || createType.IsAbstract) {
                if(_typeCache.ContainsKey(createType))
                    return (T)Activator.CreateInstance(_typeCache[createType]);

                var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
                var type = types.First(t => t.IsClass && !t.IsAbstract && createType.IsAssignableFrom(t));

                if(type == null) {
                    throw new Exception("Type not found");
                }

                _typeCache[createType] = type;
                return (T) Activator.CreateInstance(type);
            }

            return Activator.CreateInstance<T>();
        }
    }
}