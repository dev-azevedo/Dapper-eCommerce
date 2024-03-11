using Dapper.FluentMap.Mapping;
using eCommerce.API.Model;

namespace eCommerce.API.Mappers
{
    public class UsuarioDoisMap : EntityMap<UsuarioDois>
    {
        public UsuarioDoisMap()
        {
            Map(p => p.Cod).ToColumn("Id");
            Map(p => p.NomeCompleto).ToColumn("Nome");
            Map(p => p.NomeCompletoMae).ToColumn("NomeMae");
            Map(p => p.Situacao).ToColumn("SituacaoCadastro");
        }
    }
}
