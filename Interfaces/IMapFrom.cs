using AutoMapper;

namespace AutoMapperConfig.Interfaces
{
    public interface IMapFrom<Class, SourceNaming, DestNaming> where Class : class
                                                               where SourceNaming : INamingConvention
                                                               where DestNaming : INamingConvention
    {
    }

    public interface IMapFrom<Class> where Class : class
    {
    }
}