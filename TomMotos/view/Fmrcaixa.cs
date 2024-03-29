﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TomMotos.Conexao;
using TomMotos.Classes;
using TomMotos.Model;
using System.Net.Mail;
using System.Net;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Data.Common;
using Microsoft.Win32;

namespace TomMotos.view
{
    public partial class Fmrcaixa : Form
    {
        double desconto;
        string[] SERVICO = new string[4];
        string[] item = new string[7];
        List<string> nota = new List<string>();
        string id_produto, nome_produto;
        int itens = 0, servicos = 0;
        Fmrveiculo fmrveiculo;
        FmrLoading fmrLoading = new FmrLoading();
        VendaDAO caixaDAO = new VendaDAO();
        FiltroDAO Filtro = new FiltroDAO();
        CaixaModel obj = new CaixaModel();
        VendaDAO Cadastro = new VendaDAO();
        ProdutoUsadoDAO produtoDAO = new ProdutoUsadoDAO();
        ServicoDAO servicoDAO = new ServicoDAO();
        CaixaModel objCaixa = new CaixaModel();
        ProdutoUsadoModel objProduto = new ProdutoUsadoModel();
        ServicoModel objServico = new ServicoModel();
        MySqlConnection conexao = ConnectionFactory.getConnection();
        public Fmrcaixa()
        {
            InitializeComponent();
        }
        private void Fmrcaixa_Load(object sender, EventArgs e)
        {
            label12.BackColor = Color.Transparent;
            lblSubitotal.Text = 0.ToString();
            dgProdutos.Columns[2].Width = 200;
            dgServicos.Columns[1].Width = 243;
            cBoxOrcamento.Checked = false;

            FiltroModel.campoWhere = "true";
            dg_func.DataSource = Filtro.buscaFuncionario(true);
        }
        private void FmrFinalizar_venda_FormClosed(object sender, FormClosedEventArgs e)
        {
            finalizarFormCaixa(false);
        }
        public void finalizarFormCaixa(bool orcamento)
        {
            if (CaixaModel.vendaFinalizada || orcamento)
            {
                CaixaModel obj = new CaixaModel();
                obj.desconto = 0;
                CaixaModel.valor_pago = 0;
                obj.total = 0;
                CaixaModel.valorPesquisa = null;
                CaixaModel.totalVenda_orcamento = null;
                CaixaModel.vendaFinalizada = false;
                CaixaModel.eOrcamento = false;
                CaixaModel.fk_cliente_id = null;
                CaixaModel.fk_veiculo_id = null;
                CaixaModel.emailCliente = null;
                FiltroModel.campoWhere = "true";

                this.Controls.Clear();
                this.InitializeComponent();
                this.Fmrcaixa_Load(null, null);

                this.WindowState = FormWindowState.Minimized; // CENTRALIZA 
                this.WindowState = FormWindowState.Maximized; // O FORM
            }
        }

        private void btnFinalizaVenda_Click(object sender, EventArgs e)
        {
            verificarFinalVenda();
        }
        public void carregarLoading()
        {
            try { fmrLoading.Show(); } catch
            {
                fmrLoading = new FmrLoading();
                fmrLoading.Show();
            }
            esperar(700);
        }
        public void fecharLoading()
        {
            fmrLoading.Close();
        }
        public void esperar(int milissegundos)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milissegundos == 0 || milissegundos < 0) return;

            timer1.Interval = milissegundos;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };

            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }

        public void inserirVariaveisObjCaixa()
        {
            objCaixa.validade_orcamento_servico = DateTime.Now;
            if (txtdesc.Text == "0,00") objCaixa.desconto = 0;
            else objCaixa.desconto = double.Parse(txtdesc.Text);
            objCaixa.total = double.Parse(lblSubitotal.Text);
            if (lbl_buscarCliente.Text != "" && lbl_buscarCliente.Text != "(id)") CaixaModel.fk_cliente_id = lbl_buscarCliente.Text;
            else CaixaModel.fk_cliente_id = "1"; // cliente null
            if (lbl_BuscarVeiculo.Text != "" && lbl_BuscarVeiculo.Text != "(id)") CaixaModel.fk_veiculo_id = lbl_BuscarVeiculo.Text; // veiculo null
            else CaixaModel.fk_veiculo_id = "1"; // veiculo null
        }

        public void inserirVariaveisObjProdutoUsado(string idVenda)
        {
                for (int i = 0; i < dgProdutos.Rows.Count - 1; i++)
                {
                    objProduto.fk_produto_id = int.Parse(dgProdutos.Rows[i].Cells[1].Value.ToString());
                    objProduto.quantidade_produto_usado = double.Parse(dgProdutos.Rows[i].Cells[3].Value.ToString());
                    objProduto.desconto_produto_usado = double.Parse(dgProdutos.Rows[i].Cells[5].Value.ToString().Replace("%",""));
                    objProduto.fk_venda_id = int.Parse(idVenda);
                    objProduto.validade_da_garantia_produto = DateTime.Now;

                    produtoDAO.cadastrar(objProduto);
                }
        }
        public void inserirVariaveisObjServicoPrestado(string idVenda)
        {
                for (int i = 0; i < dgServicos.Rows.Count - 1; i++)
                {
                    objServico.descricao_servico_prestado = dgServicos.Rows[i].Cells[1].Value.ToString();
                    objServico.valor_servico_prestado = double.Parse(dgServicos.Rows[i].Cells[2].Value.ToString());
                    objServico.fk_venda_id = int.Parse(idVenda);

                    servicoDAO.cadastrarServicoPrestado(objServico);
                }
            
        }
        public void finalizarOrcamento()
        {
            try
            {
                inserirVariaveisObjCaixa();

                caixaDAO.cadastrarOrcamento(objCaixa);
                
                string idVenda = caixaDAO.listarUltimaVenda();
                inserirVariaveisObjProdutoUsado(idVenda);
                inserirVariaveisObjServicoPrestado(idVenda);
                MessageBox.Show("Orçamento finalizado com sucesso.");
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro - " + erro.Message);
            }
        }
        public void verificarFinalVenda(){

            if (lblSubitotal.Text != "0")
            {
                if (cBoxOrcamento.Checked)
                {
                    FmrFinalizar_Orcamento FmO = new FmrFinalizar_Orcamento(this);
                    FmO.Show();
                }
                else if (dg_funcGet.Rows.Count == 0) MessageBox.Show("Por favor, selecione ao menos um funcionário.");

                else IrParaFinalizar();
            }
        }
        public void FinalizarVenda()
        {
            inserirVariaveisObjCaixa();

            caixaDAO.cadastrarVenda(objCaixa);
            CadastraGrupoFunc();
            string idVenda = caixaDAO.listarUltimaVenda();
            inserirVariaveisObjProdutoUsado(idVenda);
            inserirVariaveisObjServicoPrestado(idVenda);

            caixaDAO.mudarStatusVenda(idVenda, true);
            MessageBox.Show("Venda finalizada com sucesso.");
            finalizarFormCaixa(true);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            EnviarEmail();
        }
        public void EnviarEmail() {
            string htmlString = getHtml(dgProdutos, dgServicos);
            Email(htmlString);
        }
        public string getHtml(DataGridView grid, DataGridView grid2)
        {

                string htmlDivStart = "<link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css' integrity='sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T' crossorigin='anonymous'>" + "<div style=\"max-width: 100%; height: 150px;; background-color: #25293E;\">";
                string htmlImg = "<img src=\'https://ci5.googleusercontent.com/proxy/qUung-ZlIwqg4tyJhKwsh3R0Mwpw59ILZqmGT50Go6eqsXC6OJxv4OYRdckw5i3GnHyfffcVJ93JYCbRSfvG-Sb3ZhzZfNlGkxdj-cvM0Tv7JfeAckZMkTM1crDTrqe-BEyZ92uSx2odQMEFRGUP6o0yUpAkgdvfXdQGqITv1aZkYOZfxKQ=s0-d-e1-ft#https://media.discordapp.net/attachments/468531379798278151/917505687045693510/LOGO_BRANCO.png' style =\"width: 180px; margin-left:auto; margin-right:auto; display:block; \">";
                string htmlDivEnd = "</div><br>";
                string htmlFooter = "<footer  style=\"width:100%; height: 150px; ; background-color: #25293E;bottom: 0;text-align: center;\">" +
                                    "<br>" +
                                    "<a href=\"#\"  style =\"width:180px;margin-left:auto;margin-right:auto;display:block;padding:8px;text-decoration:none;background-color:transparent; color:white; border:1px solid white; \">SAIBA MAIS</a>";
                string htmlFooterEnd = "</footer>";

                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center; margin-left:auto; margin-right:auto; display:block;width:700px;\">";
                string htmlTableEnd = "</table><br>";

                string htmlHeaderRowStart = "<tr style=\"background-color:#25293e; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";

                string htmlTrStart = "<tr style=\"color:#555555;\">";
                string htmlTrEnd = "</tr>";

                string htmlTdStart = "<td style=\"border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";
                string messageBody = "", messageB = "", total = "";
                if (CaixaModel.eOrcamento == true)
                {
                    htmlTdStart = "<td style=\"border-color:black; border-style:solid; border-width:thin; padding: 5px;\">";
                    htmlHeaderRowStart = "<tr style=\"background-color:#d2a06f; color:#ffffff;\">";
                }
                if (grid.RowCount == 0 || grid2.RowCount == 0) return messageBody;


                if (dgProdutos.Rows.Count > 1)
                {

                    messageBody += htmlTableStart;
                    messageBody += htmlHeaderRowStart;
                    messageBody += htmlTdStart + "ITEM" + htmlTdEnd;
                    messageBody += htmlTdStart + "CODIGO" + htmlTdEnd;
                    messageBody += htmlTdStart + "DESCRIÇÃO PRODUTO" + htmlTdEnd;
                    messageBody += htmlTdStart + "QUANTIDADE" + htmlTdEnd;
                    messageBody += htmlTdStart + "VL.UNIT.(R$)" + htmlTdEnd;
                    messageBody += htmlTdStart + "DESCONTO.(%)" + htmlTdEnd;
                    messageBody += htmlTdStart + "VL.ITEM.(R$)" + htmlTdEnd;
                    messageBody += htmlHeaderRowEnd;

                }

                if (dgServicos.Rows.Count > 1)
                {
                    messageB += htmlTableStart;
                    messageB += htmlHeaderRowStart;
                    messageB += htmlTdStart + "    " + htmlTdEnd;
                    messageB += htmlTdStart + "DESCRIÇÃO SERVIÇO" + htmlTdEnd;
                    messageB += htmlTdStart + "VALOR(R$)" + htmlTdEnd;
                    messageB += htmlHeaderRowEnd;


                }
                total += htmlTableStart;
                total += htmlHeaderRowStart;
                total += htmlTdStart + "DATA" + htmlTdEnd;
                if (txtdesc.Text != "0,00" && txtdesc.Text != "") total += htmlTdStart + "DESCONTO FINAL(%)" + htmlTdEnd;
                if (CaixaModel.eOrcamento == false) total += htmlTdStart + "VALOR PAGO(R$)" + htmlTdEnd;
                else total += htmlTdStart + "VALOR ADIANTADO(R$)" + htmlTdEnd;
                total += htmlTdStart + "TOTAL(R$)" + htmlTdEnd;
                if (CaixaModel.eOrcamento == false) total += htmlTdStart + "TROCO(R$)" + htmlTdEnd;
                if (CaixaModel.eOrcamento == true && CaixaModel.valor_pago != 0) total += htmlTdStart + "FALTA PAGAR(R$)" + htmlTdEnd;
                total += htmlHeaderRowEnd;

                total = total + htmlTrStart;
                total = total + htmlTdStart + DateTime.Now.ToString() + htmlTdEnd;
                if (txtdesc.Text != "" && txtdesc.Text != "0,00") total = total + htmlTdStart + string.Format("{0:P}", double.Parse(txtdesc.Text) / 100) + htmlTdEnd;
                total = total + htmlTdStart + string.Format("{0:#,##0.00}", CaixaModel.valor_pago) + htmlTdEnd;
                total = total + htmlTdStart + lblSubitotal.Text.ToString() + htmlTdEnd;
                if (CaixaModel.eOrcamento == false) total = total + htmlTdStart + string.Format("{0:#,##0.00}", CaixaModel.valor_pago - double.Parse(lblSubitotal.Text)) + htmlTdEnd;
                if (CaixaModel.eOrcamento == true && CaixaModel.valor_pago != 0) total = total + htmlTdStart + string.Format("{0:#,##0.00}", CaixaModel.valor_pago - double.Parse(lblSubitotal.Text)) + htmlTdEnd;
                total = total + htmlTrEnd;

                for (int i = 0; i <= grid.RowCount - 2; i++)
                {
                    messageBody = messageBody + htmlTrStart;
                    messageBody = messageBody + htmlTdStart + grid.Rows[i].Cells[0].Value + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + grid.Rows[i].Cells[1].Value + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + grid.Rows[i].Cells[2].Value + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + grid.Rows[i].Cells[3].Value + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + grid.Rows[i].Cells[4].Value + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + grid.Rows[i].Cells[5].Value + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + grid.Rows[i].Cells[6].Value + htmlTdEnd;
                    messageBody = messageBody + htmlTrEnd;

                }
                for (int a = 0; a <= grid2.RowCount - 2; a++)
                {
                    messageB = messageB + htmlTrStart;
                    messageB = messageB + htmlTdStart + grid2.Rows[a].Cells[0].Value + htmlTdEnd;
                    messageB = messageB + htmlTdStart + grid2.Rows[a].Cells[1].Value + htmlTdEnd;
                    messageB = messageB + htmlTdStart + grid2.Rows[a].Cells[2].Value + htmlTdEnd;
                    messageB = messageB + htmlTrEnd;

                }

                string final = htmlDivStart+ htmlImg + htmlDivEnd + messageBody + htmlTableEnd + messageB + htmlTableEnd + total + htmlTableEnd + htmlFooter + htmlFooterEnd;
                return final;
            
        }
  
        public static void Email(string htmlString)
        {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TomMotos");

                string emailRementente = key.GetValue("nomeEmail").ToString();
                string senhaRementente = key.GetValue("senhaEmail").ToString();

                string emailDestinatario = CaixaModel.emailCliente;

                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                message.From = new MailAddress(emailRementente);
                message.To.Add(new MailAddress(emailDestinatario));
                if (CaixaModel.eOrcamento == true) message.Subject = "COMPROVANTE DE ORÇAMENTO";
                else message.Subject = "COMPROVANTE DE VENDA";
                message.IsBodyHtml = true;
                message.Body = htmlString;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(emailRementente, senhaRementente);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
        }

        private void dgServicos_MouseDown(object sender, MouseEventArgs e)
        {
            dgProdutos.ClearSelection();
        }

        private void dgProdutos_MouseDown(object sender, MouseEventArgs e)
        {
            dgServicos.ClearSelection();
        }

        private void Fmrcaixa_MouseClick(object sender, MouseEventArgs e)
        {
            dgProdutos.ClearSelection();
            dgServicos.ClearSelection();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (txtdesc.Text != "0.00" && lblSubitotal.Text != "0" && double.Parse(txtdesc.Text) <= 100)
            {
                desconto = double.Parse(lblSubitotal.Text) * (double.Parse(txtdesc.Text) / 100);
                lblSubitotal.Text = string.Format("{0:#,##0.00}", double.Parse(lblSubitotal.Text) - desconto);               
                txtdesc.Enabled = false;
            }
            else { MessageBox.Show("DESCONTO INVÁLIDO"); txtdesc.Text = "0,00"; }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            lblSubitotal.Text = string.Format("{0:#,##0.00}", double.Parse(lblSubitotal.Text) + desconto);
            desconto = 0;
            txtdesc.Text = "0,00";
            txtdesc.Enabled = true;
        }

        private void txtIdProduto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2) {
                PesquisarProduto();
            }
         
            if (e.KeyData == Keys.F6) txtqtd.Focus();
            if (e.KeyData == Keys.F5) {
                verificarFinalVenda();
            }

            if (e.KeyData == Keys.Enter)
            {
                double valor_produto, valor_total, total, desconto;
                if (lblSubitotal.Text == "") total = 0;
                else total = double.Parse(lblSubitotal.Text);
                string select = "select id_produto, descricao_produto, marca_produto quantidade_produto, valor_produto, imagem_produto  from tb_produto where id_produto = " + txtIdProduto.Text.ToString();
                MySqlCommand executacmdsql = new MySqlCommand(select, conexao);

                MySqlDataAdapter da = new MySqlDataAdapter(executacmdsql);

                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds);

                    if (Convert.IsDBNull(ds.Tables[0].Rows[0]["imagem_produto"]))
                    {
                        ptb_imagem.Image = null;
                    }
                    else
                    {
                        MemoryStream ms = new MemoryStream((byte[])ds.Tables[0].Rows[0]["imagem_produto"]);
                        ptb_imagem.Image = new Bitmap(ms);
                    }
                    id_produto = ds.Tables[0].Rows[0]["id_produto"].ToString();
                    nome_produto = ds.Tables[0].Rows[0]["descricao_produto"].ToString();
                    valor_produto = double.Parse(ds.Tables[0].Rows[0]["valor_produto"].ToString());

                    if (txt_desconto_pro.Text != "")
                    {
                        double desc;
                        desc = (valor_produto * int.Parse(txtqtd.Text));
                        desconto = desc * double.Parse(txt_desconto_pro.Text) / 100;
                        desconto = desc - desconto;
                        item[6] = string.Format("{0:#,##0.00}", desconto);
                        lblSubitotal.Text = string.Format("{0:#,##0.00}", double.Parse(lblSubitotal.Text) + desconto);

                    }
                    else
                    {
                        txt_desconto_pro.Text = "0";
                        total = total + (valor_produto * int.Parse(txtqtd.Text));                        
                        lblSubitotal.Text = string.Format("{0:#,##0.00}", total);
                        valor_total = valor_produto * double.Parse(txtqtd.Text);
                        item[6] = string.Format("{0:#,##0.00}", valor_total);
                    }

                    if (itens == 0) itens = 1;
                    else itens += 1;

                    item[0] = itens.ToString();
                    item[1] = id_produto;
                    item[2] = nome_produto;
                    item[3] = txtqtd.Text;
                    item[4] = string.Format("{0:#,##0.00}", valor_produto);
                    item[5] = string.Format("{0:P}", double.Parse(txt_desconto_pro.Text)/100);
                    dgProdutos.Rows.Add((item));

                    txtIdProduto.Text = "";
                    txtqtd.Text = "1";
                    txtDescServ.Text = "";
                }
                catch (Exception erro)
                {
                    MessageBox.Show("PRODUTO NÃO ENCONTRADO!! "+erro.Message);
                }
            }
        }

        private void Fmrcaixa_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show("Test " + e.KeyValue.ToString());
            //if (e.KeyValue == )
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
        }

        private void txtqtd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
            if (e.KeyData == Keys.Enter) txtIdProduto.Focus();
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
            //MessageBox.Show("Test " + e.KeyValue.ToString());
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Fmrcaixa_Shown(object sender, EventArgs e)
        {
            txtIdProduto.Focus();
        }

        private void txt_pmo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
            if (e.KeyData == Keys.Enter)
            {
                double preco_servico = 0, pt;

                if (txt_pmo.Text != "0,00" || txtDescServ.Text != "")
                {
                    string pmo = txt_pmo.Text;
                    replacepontoevirgula(pmo);

                    try
                    {
                        if (servicos == 0) servicos = 1;
                        else servicos += 1;
                        preco_servico = double.Parse(pmo);
                        pt = double.Parse(lblSubitotal.Text);
                        SERVICO[1] = txtDescServ.Text.ToString().ToUpper();
                        SERVICO[2] = pmo.ToString();

                        SERVICO[0] = servicos.ToString();
                        dgServicos.Rows.Add((SERVICO));
                        pt += preco_servico;
                        lblSubitotal.Text = string.Format("{0:#,##0.00}", pt);
                        txt_pmo.Text = "0,00";
                        txtDescServ.Text = "";
                    }
                    catch (Exception erro)
                    {
                        MessageBox.Show(erro.Message.ToString());
                    }
                }
                else MessageBox.Show("VERIFIQUE SE EXISTE CAMPOS EM BRANCOS");
            }
        }

        public void replacepontoevirgula(string a) {          
            a = a.Replace(".", "").Replace(",", ".");        
        }

        private void btn_buscarVeiculo_Click(object sender, EventArgs e)
        {
            try
            {
               
                CaixaModel.valorPesquisa = "veiculo";
                FmrVeiculo_Cliente fmrVC = new FmrVeiculo_Cliente(this, fmrveiculo);
                fmrVC.Show();
            }
            catch (Exception erro) {
                MessageBox.Show(" "+erro.Message);
            }
        }

        private void btn_BuscarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                label7.Text = "PESQUISA DE CLIENTE";
                CaixaModel.valorPesquisa = "cliente";
                FmrVeiculo_Cliente fmrVC = new FmrVeiculo_Cliente(this, fmrveiculo);
                fmrVC.Show();
            }
            catch (Exception erro)
            {
                MessageBox.Show(" " + erro.Message);
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            lbl_BuscarVeiculo.Text = CaixaModel.fk_veiculo_id.ToString();
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            IrParaFinalizar();
        }
        public void IrParaFinalizar() {
            FmrFinalizar_venda destino = new FmrFinalizar_venda(this, this);
            destino.FormClosed += new FormClosedEventHandler(FmrFinalizar_venda_FormClosed);
            destino.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SalvarPdf();
        }

        private void txt_desconto_pro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
            if (e.KeyData == Keys.Enter)
            {
                txtIdProduto.Focus();
            }
        }

        private void txtPrU_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void txtDescServ_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
            if (e.KeyData == Keys.F6)
            {
                txt_pmo.Focus();
            }
        }

       
        private void txtdesc_KeyDown(object sender, KeyEventArgs e)
        {
           
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void btn_buscarVeiculo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void btn_BuscarCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void btnPesquisarProduto_Click(object sender, EventArgs e)
        {
            PesquisarProduto();
        }
        public void PesquisarProduto() {
            try
            {
                FmrListProdutos fmrLP = new FmrListProdutos(this);
                fmrLP.Show();
            }
            catch (Exception erro)
            {
                MessageBox.Show(" " + erro.Message);
            }
        }

        private void dgProdutos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
            if (e.KeyData == Keys.Delete) {
                Excluir_Produto();
            }
        }

        public void SalvarPdf() {
            try
            {
                string html = getHtml(dgProdutos, dgServicos);
                string nomePDF = "venda" + caixaDAO.listarUltimaVenda() + ".pdf";

                criarPDF(html, nomePDF);
            }
            catch (Exception erro)
            {
                MessageBox.Show(erro.ToString());
            }
        }

        private void dgServicos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2)
            {
                PesquisarProduto();
            }
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
            if (e.KeyData == Keys.Delete)
            {
                Excluir_Servico();
            }
        }

        private void criarPDF(string html, string nomePDF)
        {
            try { 
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\TomMotos");
                string caminhoPDF = key.GetValue("localPDF").ToString();
                if (caminhoPDF == "") { MessageBox.Show("Verifique a pasta dos pdfs.\nPDF não foi salvo"); return; }

                caminhoPDF = caminhoPDF + "\\" + nomePDF;
                var conteudoHTML = String.Format(html, DateTime.Now);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                htmlToPdf.GeneratePdf(conteudoHTML, null, caminhoPDF);
            }
            catch { MessageBox.Show("Verifique a pasta dos pdfs.\nPDF não foi salvo"); }
        }

        private void btndesconto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void btnCancelDesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void txtqtd_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Se a tecla digitada não for número e nem backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 08)
            {
                //Atribui True no Handled para cancelar o evento
                e.Handled = true;
            }
        }
     
        private void txtIdProduto_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Se a tecla digitada não for número e nem backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 08)
            {
                //Atribui True no Handled para cancelar o evento
                e.Handled = true;
            }
        }

        private void txt_desconto_pro_KeyPress(object sender, KeyPressEventArgs e)
        {
            validacaoTxtDAO.FormatarPorcentagem(sender, e);
        }

        private void txtqtd_Leave(object sender, EventArgs e)
        {
            if (txtqtd.Text == "") txtqtd.Text ="1";
        }

        private void txt_pmo_KeyPress(object sender, KeyPressEventArgs e)
        {
            validacaoTxtDAO.FormatarValores(sender, e);
        }

        private void txtdesc_KeyPress(object sender, KeyPressEventArgs e)
        {
            validacaoTxtDAO.FormatarPorcentagem(sender, e);

        }

        private void txt_desconto_pro_Leave(object sender, EventArgs e)
        {
            if (txt_desconto_pro.Text != "")
            {
                if (double.Parse(txt_desconto_pro.Text) > 100 || double.Parse(txt_desconto_pro.Text) < 0)
                {
                    MessageBox.Show("DESCONTO INVÁLIDO");
                    txt_desconto_pro.Text = "";
                }
            }
        }

        public void CadastraGrupoFunc() {
            //FmrAddFunc frmFun = new FmrAddFunc();
            //frmFun.Show(); 
          
                CaixaModel.id_venda = Cadastro.listarUltimaVenda();
                foreach (DataGridViewRow linha in dg_funcGet.Rows)
                {
                    try
                    {
                        CaixaModel.fk_funcionario_id = linha.Cells[0].Value.ToString();
                        Cadastro.cadastrarGrupoFunc(obj);
                    }
                    catch (Exception erro)
                    {
                        MessageBox.Show(erro.Message);
                        return;
                    }                                                        
                }
                CaixaModel.id_venda = "";
        }

        private void dg_func_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dg_func.Columns["Selecionar"].Index)
            {
                dg_func.EndEdit();
                if (dg_func.Rows[e.RowIndex].Cells[0].Value.ToString() == "True")
                {
                    string[] funcionario = new string[3];
                    funcionario[0] = dg_func.CurrentRow.Cells[1].Value.ToString();
                    funcionario[1] = dg_func.CurrentRow.Cells[2].Value.ToString();
                    funcionario[2] = dg_func.CurrentRow.Cells[3].Value.ToString();
                    dg_funcGet.Rows.Add(funcionario);
                }
                else if (dg_func.Rows[e.RowIndex].Cells[0].Value.ToString() == "False")
                {
                    string[] funcionario = new string[3];
                    funcionario[0] = dg_func.CurrentRow.Cells[1].Value.ToString();
                    string rowProduto = funcionario[0];
                    //int VALOR = dg_FuncGet.Rows.Cells["ID"][rowProduto.ToString()];//.Value.ToString() == rowProduto.ToString());
                    for (int v = 0; v < dg_funcGet.Rows.Count; v++)
                    {
                        if (string.Equals(dg_funcGet[0, v].Value as string, rowProduto))
                        {
                            dg_funcGet.Rows.RemoveAt(v);
                            v--; // this just got messy. But you see my point.
                        }
                    }
                }

            }
        }

        private void voltar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void cBoxOrcamento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void txtFuncID_TextChanged(object sender, EventArgs e)
        {
            puxarFuncPorID();
        }

        private void BtnMostrarFunc_Click(object sender, EventArgs e)
        {
            txtFuncID.Text = "";
            puxarFuncPorID();
        }

        private void puxarFuncPorID()
        {
            FiltroModel.campoWhere = "true";
            if (txtFuncID.Text != "") FiltroModel.campoWhere = "tb_funcionario.id_funcionario = " + txtFuncID.Text;
            dg_func.DataSource = Filtro.buscaFuncionario(true);

            for (int i = 0; i < dg_func.Rows.Count; i++) // VERIFICA SE A CHECKBOX DEVE ESTAR MARCADA OU NÃO
            {
                for (int ii = 0; ii < dg_funcGet.Rows.Count; ii++)
                {
                    if (dg_func.Rows[i].Cells[1].Value.ToString() == dg_funcGet.Rows[ii].Cells[0].Value.ToString())
                    {
                        dg_func.Rows[i].Cells[0].Value = true;
                    }
                }
            }
        }

        private void dg_func_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void dg_funcGet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void txtFuncID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void BtnMostrarFunc_KeyDown(object sender, KeyEventArgs e)
        {
               if (e.KeyData == Keys.F5)
            {
                verificarFinalVenda();
            }
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        } 

        private void txtFuncID_KeyPress(object sender, KeyPressEventArgs e)
        {//Se a tecla digitada não for número e nem backspace
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 08)
            {
                //Atribui True no Handled para cancelar o evento
                e.Handled = true;
            }
        }

        private void cBoxOrcamento_CheckedChanged(object sender, EventArgs e)
        {

        }

        public void Excluir_Produto() {
            double subitotal = double.Parse(lblSubitotal.Text);
            if (dgProdutos.SelectedCells.Count > 0)
            {
                if (dgProdutos.CurrentRow.Cells[5].Value != null)
                {
                    try
                    {
                        var result = MessageBox.Show("Deseja deletar o Item "+ dgProdutos.CurrentRow.Cells[0].Value.ToString()+"?", "Deletar",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Information);
                        if (result == DialogResult.Yes)
                        {
                            int rowProduto = dgProdutos.CurrentCell.RowIndex;
                            double rowPreco = double.Parse(dgProdutos.CurrentRow.Cells[6].Value.ToString());
                            subitotal = subitotal - rowPreco;
                            dgProdutos.Rows.RemoveAt(rowProduto);
                            lblSubitotal.Text = string.Format("{0:#,##0.00}", subitotal);
                            dgProdutos.ClearSelection();
                        }
                      
                        
                    }
                    catch (Exception erro)
                    {
                        MessageBox.Show("Erro " + erro.Message);
                    }
                }
            }            

        }
        public void Excluir_Servico()
        {
            double subitotal = double.Parse(lblSubitotal.Text);
            
            if (dgServicos.SelectedCells.Count > 0)
            {
                if (dgServicos.CurrentRow.Cells[2].Value != null)
                {
                    try
                    {
                        var result = MessageBox.Show("Deseja deletar o serviço " + dgServicos.CurrentRow.Cells[0].Value.ToString() + "?", "Deletar",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Information);
                        if (result == DialogResult.Yes)
                        {
                            int rowProduto = dgServicos.CurrentCell.RowIndex;
                            double rowPreco = double.Parse(dgServicos.CurrentRow.Cells[2].Value.ToString());
                            subitotal = subitotal - rowPreco;
                            dgServicos.Rows.RemoveAt(rowProduto);
                            lblSubitotal.Text = string.Format("{0:#,##0.00}", subitotal);
                            dgServicos.ClearSelection();
                        }
                    }
                    catch (Exception erro)
                    {
                        MessageBox.Show("" + erro.Message);
                    }
                }
            }
           

        }

    }
}
