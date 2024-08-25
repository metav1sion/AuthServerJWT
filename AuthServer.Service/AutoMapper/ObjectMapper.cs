using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace AuthServer.Service.AutoMapper
{
    public static class ObjectMapper
    {
        //Uygulama ayağa kalktığında bellekte olmayacak ne zaman çağırırsak o zaman devreye girecek
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoMapper>();
            });
            return config.CreateMapper();
        });
        public static IMapper Mapper => lazy.Value;
    }
}
