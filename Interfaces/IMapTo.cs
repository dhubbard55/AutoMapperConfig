using AutoMapper;

namespace AutoMapperConfig.Interfaces
{
    public interface IMapTo<Class, SourceNaming, DestNaming> where Class : class
                                                             where SourceNaming : INamingConvention
                                                             where DestNaming : INamingConvention
    {
    }

    public interface IMapTo<Class> where Class : class
    {
    }
}