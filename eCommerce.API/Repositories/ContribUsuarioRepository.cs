using eCommerce.API.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Dapper.Contrib.Extensions;
using System.Linq;
using System;

namespace eCommerce.API.Repositories
{
    public class ContribUsuarioRepository : IUsuarioRepository
    {
        private IDbConnection _connection;

        public ContribUsuarioRepository()
        {
            _connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=eCommerce;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        public List<Usuario> Get() => _connection.GetAll<Usuario>().ToList();

        public Usuario Get(int id) => _connection.Get<Usuario>(id);

        public void Insert(Usuario usuario) => usuario.Id = Convert.ToInt32(_connection.Insert(usuario));

        public void Update(Usuario usuario) => _connection.Update(usuario);

        public void Delete(int id) => _connection.Delete(Get(id));
        
    }
}
