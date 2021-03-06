﻿using DAL.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DataAccessObject
{
    public class ServicosDAO : DAO<Servicos>
    {
        public ServicosDAO() : base()
        {
        }

        public override async Task<IList<Servicos>> ListarTodos()
        {
            using (var conexao = GetCurrentConnection())
            {
                try
                {
                    string sql = @"SELECT * FROM servicos WHERE status = 'Ativo' ORDER BY codigo;";

                    conexao.Open();

                    NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                    List<Servicos> list = await GetResultSet(command);
                    return list;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public override async Task<Servicos> BuscarPorID(int codigo)
        {
            using (var conexao = GetCurrentConnection())
            {
                try
                {
                    string sql = @"SELECT * FROM servicos WHERE codigo = @codigo AND status = 'Ativo';";

                    conexao.Open();

                    NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                    command.Parameters.AddWithValue("@codigo", codigo);

                    List<Servicos> list = await GetResultSet(command);

                    if(list.Count > 0)
                    {
                        return list[0];
                    }
                    else
                    {
                        throw new Exception("Serviço não encontrado");
                    }
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public override async Task<Servicos> Inserir(Servicos servico)
        {
            using (var conexao = GetCurrentConnection())
            {
                try
                {
                    conexao.Open();
                    bool exists = await CheckExist(conexao, "servicos", "descricao", servico.descricao);
                    if (exists)
                    {
                        string sql = @"INSERT INTO servicos(descricao, valor, dtCadastro, dtAlteracao, status) VALUES (@descricao, @valor, @dtCadastro, @dtAlteracao, @status) returning codigo;";

                        NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                        command.Parameters.AddWithValue("@descricao", servico.descricao);
                        command.Parameters.AddWithValue("@valor", servico.valor);
                        command.Parameters.AddWithValue("@dtCadastro", servico.dtCadastro);
                        command.Parameters.AddWithValue("@dtAlteracao", servico.dtAlteracao);
                        command.Parameters.AddWithValue("@status", servico.status);

                        Object idInserido = await command.ExecuteScalarAsync();
                        servico.codigo = (int)idInserido;

                        return servico;
                    }
                    else
                    {
                        throw new Exception("Serviço já cadastrado");
                    }
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public override async Task<Servicos> Editar(Servicos servico)
        {
            using (var conexao = GetCurrentConnection())
            {
                try
                {
                    conexao.Open();
                    bool exists = await CheckExist(conexao, "servicos", "descricao", servico.descricao, servico.codigo);
                    if (exists)
                    {
                        string sql = @"UPDATE servicos SET descricao = @descricao, valor = @valor, dtAlteracao = @dtAlteracao WHERE codigo = @codigo";

                        NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                        command.Parameters.AddWithValue("@descricao", servico.descricao);
                        command.Parameters.AddWithValue("@valor", servico.valor);
                        command.Parameters.AddWithValue("@dtAlteracao", servico.dtAlteracao);
                        command.Parameters.AddWithValue("@codigo", servico.codigo);

                        await command.ExecuteNonQueryAsync();
                        return servico;
                    }
                    else
                    {
                        throw new Exception("Serviço já cadastrado");
                    }
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public override async Task<bool> Excluir(Servicos servico)
        {
            using (var conexao = GetCurrentConnection())
            {
                try
                {
                    string sql = @"DELETE FROM servicos WHERE codigo = @codigo";

                    conexao.Open();

                    NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                    command.Parameters.AddWithValue("@codigo", servico.codigo);

                    var result = await command.ExecuteNonQueryAsync();
                    return result == 1 ? true : false;
                }
                catch
                {
                    string sql = @"UPDATE servicos SET status = @status, dtAlteracao = @dtAlteracao WHERE codigo = @codigo";

                    NpgsqlCommand command = new NpgsqlCommand(sql, conexao);

                    command.Parameters.AddWithValue("@status", servico.status);
                    command.Parameters.AddWithValue("@dtAlteracao", servico.dtAlteracao);
                    command.Parameters.AddWithValue("@codigo", servico.codigo);

                    var result = await command.ExecuteNonQueryAsync();
                    return result == 1 ? true : false;
                }
                finally
                {
                    conexao.Close();
                }
            }
        }

        public override Task<IList<Servicos>> Pesquisar(string str)
        {
            throw new NotImplementedException();
        }
    }
}
