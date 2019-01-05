using System;
using System.Windows.Forms;
using ACBr.Net.TEF.Events;

namespace ACBr.Net.TEF.Demo
{
    public partial class Form1 : Form
    {
        private ACBrTEF tef;

        public Form1()
        {
            InitializeComponent();
            tef = new ACBrTEF();
            tef.OnExibeMensagem += TefOnOnExibeMensagem;
            tef.OnComandaVenda += TefOnOnComandaVenda;
            tef.OnComandaVendaAbreVinculado += TefOnOnComandaVendaAbreVinculado;
            tef.OnComandaVendaImprimeVia += TefOnOnComandaVendaImprimeVia;
            tef.OnInfoVenda += TefOnOnInfoVenda;
        }

        private void TefOnOnInfoVenda(object sender, InfoVendaEventArgs e)
        {
            switch (e.Operacao)
            {
                case InfoVenda.SubTotal:
                    e.Valor = 0;
                    break;

                case InfoVenda.EstadoVenda:
                    e.EstadoVenda = EstadoVenda.Livre;
                    break;

                case InfoVenda.TotalAPagar:
                    e.Valor = 10;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TefOnOnComandaVendaImprimeVia(object sender, ComandaVendaImprimeViaEventArgs e)
        {
            //
        }

        private void TefOnOnComandaVendaAbreVinculado(object sender, ComandaVendaAbreVinculadoEventArgs e)
        {
            //
        }

        private void TefOnOnComandaVenda(object sender, ComandaVendaEventArgs e)
        {
            //
        }

        private void TefOnOnExibeMensagem(object sender, ExibeMensagemEventArgs e)
        {
            switch (e.Operacao)
            {
                case OperacaoMensagem.OK:
                    e.Result = (ModalResult)MessageBox.Show(e.Mensagem, "", MessageBoxButtons.OK);
                    break;

                case OperacaoMensagem.YesNo:
                    e.Result = (ModalResult)MessageBox.Show(e.Mensagem, "", MessageBoxButtons.YesNo);
                    break;

                case OperacaoMensagem.ExibirMsgOperador:
                    break;

                case OperacaoMensagem.RemoverMsgOperador:
                    break;

                case OperacaoMensagem.ExibirMsgCliente:
                    break;

                case OperacaoMensagem.RemoverMsgCliente:
                    break;

                case OperacaoMensagem.DestaqueVia:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void btnInicializar_Click(object sender, EventArgs e)
        {
            tef.Inicializar(TEFTipo.TEFDial);
        }

        private void btnATV_Click(object sender, EventArgs e)
        {
            tef.ATV();
        }

        private void btnADM_Click(object sender, EventArgs e)
        {
            tef.ADM();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tef.CRT(10, "1");
        }
    }
}