﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TomMotos.Classes;
using TomMotos.Model;

namespace TomMotos.view
{
    public partial class FmrOrcamento : Form
    {
        string[] item = new string[7];
        Fmrcaixa lp;
        VendaDAO VendaDAO = new VendaDAO();
        Fmrcaixa fmrCx = new Fmrcaixa();
        ProdutoUsadoDAO ProdutoUsadoDAO = new ProdutoUsadoDAO();
        ServicoDAO ServicoPrestadoDAO = new ServicoDAO();

        public FmrOrcamento(Fmrcaixa lpp)
        {
            InitializeComponent();
            lp = lpp;
        }

        private void FmOrcamento_Load(object sender, EventArgs e)
        {
            dgOrcamento.DataSource = VendaDAO.listarTodosOrcamentos();
        }

        private void dgOrcamento_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            fmrCx.Show();
            carregarVenda(int.Parse(dgOrcamento.CurrentRow.Cells[0].Value.ToString())); // ID VENDA
            this.Close();
        }

        public void carregarVenda(int idVenda)
        {
            carregarProdutoUsados(idVenda); 
            carregarServicosFeitos(idVenda);
            carregarLblTotal(idVenda);
        }

        public void carregarLblTotal(int idVenda)
        {
            string totalVenda = VendaDAO.listarTotalVendaPorId(idVenda);
            fmrCx.lblSubitotal.Text = totalVenda;
        }
        
        public void carregarServicosFeitos(int idVenda)
        {
            try
            {
                int ITEM = 1;
                ArrayList resultado = ServicoPrestadoDAO.listarPorVenda(idVenda);
                for (int i = -1; i < resultado.Count; ITEM++)
                {
                    if (ITEM == 1) i++;
                    string DESCRICAO = resultado[i].ToString(); i++;
                    string VALOR = resultado[i].ToString(); i++;

                    item[0] = ITEM.ToString();
                    item[1] = DESCRICAO; 
                    item[2] = VALOR;

                    fmrCx.dgServicos.Rows.Add(item);
                }
            }
            catch (Exception erro) { MessageBox.Show("Test " + erro); }
        }
        public void carregarProdutoUsados(int idVenda)
        {
            try
            {
                int ITEM = 1;
                ArrayList resultado = ProdutoUsadoDAO.listarPorVenda(idVenda);
                for (int i = -1; i < resultado.Count; ITEM++)
                {
                    if (ITEM == 1) i++;
                    string CODIGO = resultado[i].ToString(); i++;
                    string DESCRICAO = resultado[i].ToString(); i++;
                    string QTD = resultado[i].ToString(); i++;
                    string VLUNIT = resultado[i].ToString(); i++;
                    string DESCONTO = resultado[i].ToString(); i++;

                    item[0] = ITEM.ToString();
                    item[1] = CODIGO;
                    item[2] = DESCRICAO;
                    item[3] = QTD;
                    item[4] = VLUNIT;
                    item[5] = DESCONTO;
                    item[6] = (double.Parse(QTD) * double.Parse(VLUNIT) - double.Parse(QTD) * double.Parse(VLUNIT) * double.Parse(DESCONTO) / 100).ToString();

                    fmrCx.dgProdutos.Rows.Add(item);
                }
            }
            catch (Exception erro) { MessageBox.Show("Test " + erro); }
        }

        private void dgOrcamento_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            try
            {
                string campo = cbxBuscar.Text.ToString();
                string finalSQL = "";
                if (campo == "ID DO ORÇAMENTO") campo = "tb_venda.id_venda";
                if (campo == "NOME DO CLIENTE") campo = "tb_cliente.nome_cliente";
                if (campo == "CPF DO CLIENTE") campo = "tb_cliente.cpf_cliente";
                if (campo != "") finalSQL = "AND " + campo.ToLower() + " like " + "'%" + txtBuscar.Text.ToString() + "%'";
                if (cxbData.Checked)
                {
                    finalSQL = finalSQL + " AND tb_venda.data_venda BETWEEN ' " + dtp1.Value.ToString("yyyy/MM/dd") + " 00:00:00' AND ' " + dtp2.Value.ToString("yyyy/MM/dd") + " " + "23:59:59'";
                }

                dgOrcamento.DataSource = VendaDAO.listarVendaPor(finalSQL);
            }
            catch (Exception erro) { MessageBox.Show("Ouve um Erro " + erro); }
        }

        private void btn_mostrar_tudo_Click(object sender, EventArgs e)
        {
            dgOrcamento.DataSource = VendaDAO.listarTodosOrcamentos();
            txtBuscar.Text = "";
            cbxBuscar.Text = "";
            cxbData.Checked = false;
        }
    }
}