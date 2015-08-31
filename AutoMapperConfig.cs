using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System;
using AutoMapperConfig.Interfaces;
using AutoMapper;

namespace AutoMapperConfig
{
    public class AutoMapperConfig
    {
        private static bool _executeCalled;
        private static List<Type> _types;

        public static void Execute(params IAutoMapperConfigModule[] configModules)
        {
            if (_executeCalled)
            {
                return;
            }

            _types = Assembly.GetCallingAssembly()
                             .GetReferencedAssemblies()
                             .SelectMany(a => Assembly.Load(a).GetTypes())
                             .Union(Assembly.GetCallingAssembly().GetTypes()).ToList();

            LoadCustomMappings();
            LoadFromMappings();
            LoadToMappings();

            if(configModules != null)
            {
                configModules.ToList().ForEach(c => c.Load(Mapper.Configuration));
            }

            _executeCalled = true;
        }

        private static void LoadCustomMappings()
        {
            var maps = (from t in _types
                        from i in t.GetInterfaces()
                        where typeof(ICustomMapping).IsAssignableFrom(t) &&
                              !t.IsAbstract &&
                              !t.IsInterface
                        select t).ToArray();

            foreach (var map in maps)
            {
                var instance = (ICustomMapping)Activator.CreateInstance(map);
                instance.CreateMapping(Mapper.Configuration);
            }
        }

        private static void LoadFromMappings()
        {
            var maps = (from t in _types
                        from i in t.GetInterfaces()
                        where i.IsGenericType &&
                              (i.GetGenericTypeDefinition() == typeof(IMapFrom<,,>) || 
                               i.GetGenericTypeDefinition() == typeof(IMapFrom<>)) &&
                              !t.IsAbstract && !t.IsInterface
                        select new
                        {
                            Source = i.GetGenericArguments()[0],
                            Destination = t,
                            SourceNaming = i.GetGenericArguments().Count() > 1 
                                               ? i.GetGenericArguments()[1] 
                                               : typeof(PascalCaseNamingConvention),
                            DestinationNaming = i.GetGenericArguments().Count() > 1 
                                                    ? i.GetGenericArguments()[2] 
                                                    : typeof(PascalCaseNamingConvention)
                        }).ToArray();

            foreach (var map in maps)
            {
                Mapper.Configuration.SourceMemberNamingConvention = (INamingConvention)Activator.CreateInstance(map.SourceNaming);
                Mapper.Configuration.DestinationMemberNamingConvention = (INamingConvention)Activator.CreateInstance(map.DestinationNaming);

                Mapper.CreateMap(map.Source, map.Destination);
            }
        }
        
        private static void LoadToMappings()
        {
            var maps = (from t in _types
                        from i in t.GetInterfaces()
                        where i.IsGenericType &&
                                (i.GetGenericTypeDefinition() == typeof(IMapTo<,,>) ||
                                 i.GetGenericTypeDefinition() == typeof(IMapTo<>)) &&
                                !t.IsAbstract && !t.IsInterface
                        select new
                        {
                            Source = t,
                            Destination = i.GetGenericArguments()[0],
                            SourceNaming = i.GetGenericArguments().Count() > 1
                                               ? i.GetGenericArguments()[1]
                                               : typeof(PascalCaseNamingConvention),
                            DestinationNaming = i.GetGenericArguments().Count() > 1
                                                    ? i.GetGenericArguments()[2]
                                                    : typeof(PascalCaseNamingConvention)
                        }).ToArray();

            foreach (var map in maps)
            {
                Mapper.Configuration.SourceMemberNamingConvention = (INamingConvention)Activator.CreateInstance(map.SourceNaming);
                Mapper.Configuration.DestinationMemberNamingConvention = (INamingConvention)Activator.CreateInstance(map.DestinationNaming);

                Mapper.CreateMap(map.Source, map.Destination);
            }
        }
    }
}