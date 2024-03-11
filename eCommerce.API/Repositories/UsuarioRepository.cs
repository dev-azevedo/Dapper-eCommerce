using eCommerce.API.Model;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace eCommerce.API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private IDbConnection _connection;

        public UsuarioRepository()
        {
            _connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=eCommerce;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        public List<Usuario> Get()
        {
            //return _connection.Query<Usuario>("Select * FROM Usuarios").ToList();
            List<Usuario> usuarios = new List<Usuario>();

            string query = "SELECT U.*, C.*, EE.*, D.* FROM Usuarios as U LEFT JOIN Contatos C ON C.UsuarioId = U.Id LEFT JOIN EnderecosEntrega EE ON EE.UsuarioId = U.Id LEFT JOIN UsuariosDepartamentos UD On UD.UsuarioId = U.Id LEFT JOIN Departamentos D ON UD.DepartamentoId = D.Id";
            _connection.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(
                query,
                (usuario, contato, enderecoEntrega, departamento) =>
                {
                    if (usuarios.SingleOrDefault(u => u.Id == usuario.Id) == null)
                    {
                        usuario.EnderecosEntrega = new List<EnderecoEntrega>();
                        usuario.Departamentos = new List<Departamento>();
                        usuario.Contato = contato;
                        usuarios.Add(usuario);
                    }
                    else
                        usuario = usuarios.SingleOrDefault(u => u.Id == usuario.Id);


                    if (usuario.EnderecosEntrega.SingleOrDefault(e => e.Id == enderecoEntrega.Id) == null)
                        usuario.EnderecosEntrega.Add(enderecoEntrega);


                    if (usuario.Departamentos.SingleOrDefault(d => d.Id == departamento.Id) == null)
                        usuario.Departamentos.Add(departamento);


                    return usuario;
                }
             );
            return usuarios;
        }

        public Usuario Get(int id)
        {
            List<Usuario> usuarios = new List<Usuario>();

            string query = "SELECT U.*, C.*, EE.*, D.* FROM Usuarios as U LEFT JOIN Contatos C ON C.UsuarioId = U.Id LEFT JOIN EnderecosEntrega EE ON EE.UsuarioId = U.Id LEFT JOIN UsuariosDepartamentos UD On UD.UsuarioId = U.Id LEFT JOIN Departamentos D ON UD.DepartamentoId = D.Id WHERE U.Id = @Id";
            _connection.Query<Usuario, Contato, EnderecoEntrega, Departamento, Usuario>(
                query,
                (usuario, contato, enderecoEntrega, departamento) =>
                {
                    if (usuarios.SingleOrDefault(u => u.Id == usuario.Id) == null)
                    {
                        usuario.EnderecosEntrega = new List<EnderecoEntrega>();
                        usuario.Departamentos = new List<Departamento>();
                        usuario.Contato = contato;
                        usuarios.Add(usuario);
                    }
                    else
                        usuario = usuarios.SingleOrDefault(u => u.Id == usuario.Id);


                    if (usuario.EnderecosEntrega.SingleOrDefault(e => e.Id == enderecoEntrega.Id) == null)
                        usuario.EnderecosEntrega.Add(enderecoEntrega);


                    if (usuario.Departamentos.SingleOrDefault(d => d.Id == departamento.Id) == null)
                        usuario.Departamentos.Add(departamento);


                    return usuario;
                },
                new {Id = id}
             );

            return usuarios.SingleOrDefault();
        }

        public void Insert(Usuario usuario)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();
            try
            {
                #region Cadastro usuario
                string queryUsuario = "INSERT INTO Usuarios(Nome, Email, Sexo, RG, CPF, NomeMae, SituacaoCadastro, DataCadastro) VALUES(@Nome, @Email, @Sexo, @RG, @CPF, @NomeMae, @SituacaoCadastro, @DataCadastro); SELECT CAST(scope_identity() AS INT)";
                usuario.Id = _connection.Query<int>(queryUsuario, usuario, transaction).Single();
                #endregion

                #region Cadastro contato
                if (usuario.Contato != null)
                {
                    usuario.Contato.UsuarioId = usuario.Id;
                    string queryContato = "INSERT INTO Contatos(UsuarioId, Telefone, Celular) VALUES (@UsuarioId, @Telefone, @Celular); SELECT CAST(scope_identity() AS INT)";
                    usuario.Contato.Id = _connection.Query<int>(queryContato, usuario.Contato, transaction).Single();
                }
                #endregion

                #region Cadastro enderecoEntrega
                if (usuario.EnderecosEntrega != null && usuario.EnderecosEntrega.Count > 0)
                {
                    foreach (var enderecoEntrega in usuario.EnderecosEntrega)
                    {
                        enderecoEntrega.UsuarioId = usuario.Id;
                        string queryEnderecoEntrega = "INSERT INTO EnderecosEntrega (UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(scope_identity() AS INT)";
                        enderecoEntrega.Id = _connection.Query<int>(queryEnderecoEntrega, enderecoEntrega, transaction).Single();
                    }
                }
                #endregion

                #region Cadastro departamento
                if (usuario.Departamentos != null && usuario.Departamentos.Count > 0)
                {
                    foreach (var departamento in usuario.Departamentos)
                    {
                        string queryUsuariosDepartamentos = "INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId)";
                        _connection.Execute(queryUsuariosDepartamentos, new {UsuarioId = usuario.Id, DepartamentoId = departamento.Id}, transaction);
                    }
                }
                #endregion
                
                 transaction.Commit();
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                    //Tratativa para erro no RollBack
                }
            }
            finally { _connection.Close(); }
        }

        public void Update(Usuario usuario)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();
            try
            {
                #region Atualiza usuario
                string queryUsuario = "UPDATE Usuarios SET Nome = @Nome, Email = @Email, Sexo = @Sexo, RG = @RG, CPF = @CPF, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, DataCadastro = @DataCadastro WHERE Id = @Id";
                _connection.Execute(queryUsuario, usuario, transaction);
                #endregion

                #region Atualiza contato
                if (usuario.Contato != null)
                {
                    string queryContato = "UPDATE Contatos SET UsuarioId = @UsuarioId, Telefone = @Telefone, Celular = @Celular WHERE Id = @Id";
                    _connection.Execute(queryContato, usuario.Contato, transaction);
                }
                #endregion

                #region Atualiza enderecoEntrega
                string queryDeleteEnderecoEntrega = "DELETE FROM EnderecosEntrega WHERE UsuarioId = @Id";
                _connection.Execute(queryDeleteEnderecoEntrega, usuario, transaction);

                if (usuario.EnderecosEntrega != null && usuario.EnderecosEntrega.Count > 0)
                {
                    foreach (var enderecoEntrega in usuario.EnderecosEntrega)
                    {
                        enderecoEntrega.UsuarioId = usuario.Id;
                        string queryEnderecoEntrega = "INSERT INTO EnderecosEntrega (UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(scope_identity() AS INT)";
                        enderecoEntrega.Id = _connection.Query<int>(queryEnderecoEntrega, enderecoEntrega, transaction).Single();
                    }
                }
                #endregion

                #region Atualiza departamento
                string queryDeleteUsuarioDepartamento = "DELETE FROM UsuariosDepartamentos WHERE UsuarioId = @Id";
                _connection.Execute(queryDeleteUsuarioDepartamento, usuario, transaction);

                if (usuario.Departamentos != null && usuario.Departamentos.Count > 0)
                {
                    foreach (var departamento in usuario.Departamentos)
                    {
                        string queryUsuariosDepartamentos = "INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, @DepartamentoId)";
                        _connection.Execute(queryUsuariosDepartamentos, new { UsuarioId = usuario.Id, DepartamentoId = departamento.Id }, transaction);
                    }
                }
                #endregion
                
                transaction.Commit();
            }
            catch
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                    //Tratativa para erro no RollBack
                }
            }
            finally { _connection.Close(); }

        }

        public void Delete(int id)
        {
            _connection.Execute("DELETE FROM Usuarios WHERE Id = @Id", new { Id = id });
        }
    }
}
