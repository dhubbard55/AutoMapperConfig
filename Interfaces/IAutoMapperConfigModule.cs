using AutoMapper;

namespace AutoMapperConfig.Interfaces
{
    public interface IAutoMapperConfigModule
    {
        void Load(IConfiguration configuration);
    }
}