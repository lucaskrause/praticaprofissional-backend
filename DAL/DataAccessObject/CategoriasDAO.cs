﻿using DAL.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DataAccessObject
{
    public class CategoriasDAO : DAO<Categorias>
    {
        public CategoriasDAO() : base()
        {
        }

        public override async Task<IList<Categorias>> ListarTodos()
        {
            using (var conexao = GetCurrentConnection())
            {
                try
                {
                    string sql = @"SELECT * FROM categorias WHERE status = 'Ativo' ORDER BY codigo;";

                    conexao.Open();

                    NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                    List<Categorias> list = await GetResultSet(command);
                    return list;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public override async Task<Categorias> BuscarPorID(int codigo)
        {
            using (var conexao = GetCurrentConnection())
            {
                try
                {
                    string sql = @"SELECT * FROM categorias WHERE codigo = @codigo AND status = 'Ativo';";

                    conexao.Open();

                    NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                    command.Parameters.AddWithValue("@codigo", codigo);

                    List<Categorias> list = await GetResultSet(command);

                    if(list.Count > 0)
                    {
                        return list[0];
                    }
                    else
                    {
                        throw new Exception("Categoria não encontrada");
                    }
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public override async Task<Categorias> Inserir(Categorias categoria)
        {
            using (var conexao = GetCurrentConnection())
            {
                try
                {
                    conexao.Open();
                    bool exists = await CheckExist(conexao, "categorias", "descricao", categoria.descricao);
                    if (exists)
                    {
                        string sql = @"INSERT INTO categorias(descricao, dtCadastro, dtAlteracao, status) VALUES (@descricao, @dtCadastro, @dtAlteracao, @status) returning codigo;";

                        NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                        command.Parameters.AddWithValue("@descricao", categoria.descricao);
                        command.Parameters.AddWithValue("@dtCadastro", categoria.dtCadastro);
                        command.Parameters.AddWithValue("@dtAlteracao", categoria.dtAlteracao);
                        command.Parameters.AddWithValue("@status", categoria.status);

                        Object idInserido = await command.ExecuteScalarAsync();
                        categoria.codigo = (int)idInserido;
                        return categoria;
                    }
                    else
                    {
                        throw new Exception("Categorias já cadastrada");
                    }
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public override async Task<Categorias> Editar(Categorias categoria)
        {
            using (var conexao = GetCurrentConnection())
            {
                try
                {
                    conexao.Open();
                    bool exists = await CheckExist(conexao, "categorias", "descricao", categoria.descricao, categoria.codigo);
                    if (exists)
                    {
                        string sql = @"UPDATE categorias SET descricao = @descricao, dtAlteracao = @dtAlteracao WHERE codigo = @codigo";

                        NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                        command.Parameters.AddWithValue("@descricao", categoria.descricao);
                        command.Parameters.AddWithValue("@dtAlteracao", categoria.dtAlteracao);
                        command.Parameters.AddWithValue("@codigo", categoria.codigo);

                        await command.ExecuteNonQueryAsync();
                        return categoria;
                    }
                    else
                    {
                        throw new Exception("Categorias já cadastrada");
                    }
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public override async Task<bool> Excluir(Categorias categoria)
        {
            using (var conexao = GetCurrentConnection())
            {
                try
                {
                    string sql = @"DELETE FROM categorias WHERE codigo = @codigo";

                    conexao.Open();

                    NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                    command.Parameters.AddWithValue("@codigo", categoria.codigo);

                    var result = await command.ExecuteNonQueryAsync();
                    return result == 1 ? true : false;
                }
                catch
                {
                    string sql = @"UPDATE categorias SET status = @status, dtAlteracao = @dtAlteracao WHERE codigo = @codigo";

                    NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                    command.Parameters.AddWithValue("@status", categoria.status);
                    command.Parameters.AddWithValue("@dtAlteracao", categoria.dtAlteracao);
                    command.Parameters.AddWithValue("@codigo", categoria.codigo);

                    var result = await command.ExecuteNonQueryAsync();
                    return result == 1 ? true : false;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public override Task<IList<Categorias>> Pesquisar(string str)
        {
            throw new NotImplementedException();
        }
    }
}
