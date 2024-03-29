﻿using MySql.Data.MySqlClient;
using System;
using System.Collections;
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
    class ServicoDAO
    {
        MySqlConnection conexao = ConnectionFactory.getConnection();

        public ServicoDAO()
        {
        }

        #region METODO CADASTRAR

        public void cadastrarServicoPrestado(ServicoModel obj)
        {
            try
            {
                string insert = @"CALL criacaoServicoPrestado(@descricao_servico_prestado, @valor_servico_prestado, @fk_venda_id)";

                MySqlCommand executacmdsql = new MySqlCommand(insert, conexao);
                executacmdsql.Parameters.AddWithValue("@descricao_servico_prestado", obj.descricao_servico_prestado);
                executacmdsql.Parameters.AddWithValue("@valor_servico_prestado", obj.valor_servico_prestado);
                executacmdsql.Parameters.AddWithValue("@fk_venda_id", obj.fk_venda_id);

                conexao.Open();
                executacmdsql.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception erro)
            {
                MessageBox.Show("Criaçao serviço prestado "+ erro.Message);
            }

            conexao.Close();
        }


        #endregion

        #region METODO LISTAR POR ID

        public ArrayList listarPorVenda(int idVenda)
        {
            MySqlConnection conexao = ConnectionFactory.getConnection();
            ArrayList resultado = new ArrayList();

            try
            {
                string select = @"
                                select tb_servico_prestado.descricao_servico_prestado, tb_servico_prestado.valor_servico_prestado, tb_venda.fk_veiculo_id, tb_venda.fk_cliente_id from tb_servico_prestado
                                inner join tb_venda on tb_servico_prestado.fk_venda_id = tb_venda.id_venda 
                                where tb_venda.id_venda = " + idVenda + ";";

                MySqlCommand executacmdsql = new MySqlCommand(select, conexao);

                conexao.Open();
               
               
                    MySqlDataAdapter da = new MySqlDataAdapter(executacmdsql);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    CaixaModel.fk_cliente_id = ds.Tables[0].Rows[0]["fk_cliente_id"].ToString();
                    CaixaModel.fk_veiculo_id = ds.Tables[0].Rows[0]["fk_veiculo_id"].ToString();
                }

                MySqlDataReader reader = executacmdsql.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        resultado.Add(Convert.ToString(reader[0].ToString()));
                        resultado.Add(Convert.ToDouble(reader[1].ToString()));
                    }
                    conexao.Close();
                }
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro: " + erro);
            }
            conexao.Close();
            return resultado;
            
        }
        #endregion
    }
}
 