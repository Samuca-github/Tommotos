﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TomMotos.Conexao;
using TomMotos.Model;

namespace TomMotos.Classes
{
    class CargoDAO
    {
        MySqlConnection conexao = ConnectionFactory.getConnection();

        public CargoDAO()
        {
        }

        #region METODO LISTAR
        public DataTable ListarTodosCargos()
        {

            string sql = @"select id_cargo as 'ID',nome_cargo as 'CARGO',salario_cargo as 'SALARIO' from tb_cargo";

            MySqlCommand executacmdsql = new MySqlCommand(sql, conexao);

            conexao.Open();
            executacmdsql.ExecuteNonQuery();

            MySqlDataAdapter da = new MySqlDataAdapter(executacmdsql);

            DataTable tabelaCargo = new DataTable();
            da.Fill(tabelaCargo);

            conexao.Close();

            return tabelaCargo;
        }
        #endregion

        #region METODO CADASTRAR

        public void cadastrarCargo(CargoModel obj)
        {
         
            if (obj.nome == "" || obj.salario == 0.00)
            {
                MessageBox.Show("Preencha todos valores Obrigatorio! = *");

            }

            else
            {


                try
                {
                    string insert = @"CALL criacaoCargo(@nome, @salario)";

                    MySqlCommand executacmdsql = new MySqlCommand(insert, conexao);
                    executacmdsql.Parameters.AddWithValue("@nome", obj.nome);
                    executacmdsql.Parameters.AddWithValue("@salario", obj.salario);

                    conexao.Open();
                    executacmdsql.ExecuteNonQuery();
                    MessageBox.Show("Cadastrado com sucesso!");
                    conexao.Close();
                }
                catch (Exception erro)
                {
                    MessageBox.Show("Cadastrado não Realizado! " + erro.Message);

                }
               
            }
            conexao.Close();
        }


        #endregion

        #region METODO ALTERAR
        public void alterar(CargoModel obj)
        {
            try
            {

                string update = @"CALL UpdateCargo(@nome, @salario, @id)";
                MySqlCommand executacmdsql = new MySqlCommand(update, conexao);

                executacmdsql.Parameters.AddWithValue("@nome", obj.nome);
                executacmdsql.Parameters.AddWithValue("@salario", obj.salario);
                executacmdsql.Parameters.AddWithValue("@id", obj.id);
                conexao.Open();
                executacmdsql.ExecuteNonQuery();
                conexao.Close();
                MessageBox.Show("Alterado com Sucesso!");
            }
            catch (Exception erro)
            {
                MessageBox.Show("Aconteceu um Erro" + erro.Message);
            }

            conexao.Close();

        }
        #endregion

        #region METODO EXCLUIR
        public void Excluir(CargoModel obj)
        {
            try
            {

                string update = @"Delete from tb_cargo where id_cargo=@id";
                MySqlCommand executacmdsql = new MySqlCommand(update, conexao);

                executacmdsql.Parameters.AddWithValue("@id", obj.id);
                conexao.Open();
                executacmdsql.ExecuteNonQuery();

                conexao.Close();
            }
            catch (Exception erro)
            {
                if(erro.ToString().Contains("Cannot delete or update"))
                MessageBox.Show("O cargo está em uso");
                else MessageBox.Show("erro "+erro.Message);
            }
            conexao.Close();

        }
        #endregion
    }
}
