// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 05-04-2014
//
// Last Modified By : RFTD
// Last Modified On : 02-21-2015
// ***********************************************************************
// <copyright file="ACBrTEF.cs" company="ACBr.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2016 Grupo ACBr.Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using ACBr.Net.Core;
using ACBr.Net.Core.Exceptions;
using ACBr.Net.Core.Extensions;
using ACBr.Net.Core.Logging;
using ACBr.Net.TEF.Events;
using ACBr.Net.TEF.Gerenciadores;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ACBr.Net.TEF
{
    /// <summary>
    /// Classe ACBrTEF. Está classe não pode ser herdada.
    /// </summary>
    [ToolboxBitmap(typeof(ACBrTEF), @"ACBr.Net.TEF.ico.bmp")]
    [TypeConverter(typeof(ACBrExpandableObjectConverter))]
    // ReSharper disable once InconsistentNaming
    public sealed class ACBrTEF : ACBrComponent, IACBrLog
    {
        #region Constantes

        internal const int CacbrTefdEsperaSts = 7;
        internal const int CacbrTefdEsperaMinimaMensagemFinal = 5;
        internal const int CacbrTefdEsperaSleep = 250;
        internal const int CacbrTefdNumVias = 2;
        internal const string CacbrTefdDestaqueVia = "Destaque a {0}ª Via";
        internal const string CacbrTefdErroEcfNaoLivre = "ECF não está LIVRE";
        internal const string CacbrTefdErroEcfNaoResponde = "Erro na impressão.\r\nDeseja tentar novamente ?";
        internal const string CacbrTefdErroEcfNaoRespondeInfo = "Impressora não responde.\r\nDeseja continuar ?";
        internal const string CacbrTefdErroNaoAtivo = "O gerenciador padrão {0} não está ativo !";
        internal const string CacbrTefdErroSemRequisicao = "Nenhuma Requisição Iniciada";
        internal const string CacbrTefdErroOutraFormaPagamento = "Gostaria de continuar a transação com outra(s) forma(s) de pagamento ?";

        internal const string CacbrTefdCliSiTefTransacaoEfetuadaReImprimir = "Transação TEF efetuada.\r\nFavor reimprimir último Cupom.\r\n" +
                                                                               "{0}\r\n(Para Cielo utilizar os 6 últimos dígitos.)";

        #endregion Constantes

        #region Eventos

        /// <summary>
        /// Occurs when [on aguarda resp].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<AguardaRespEventArgs> OnAguardaResp;

        /// <summary>
        /// Occurs when [on exibe mensagem].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<ExibeMensagemEventArgs> OnExibeMensagem;

        /// <summary>
        /// Occurs when [on desbloqueia mouse teclado].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<BloqueiaMouseTecladoEventArgs> OnBloqueiaMouseTeclado;

        /// <summary>
        /// Occurs when [on restaura foco aplicacao].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<ExecutaAcaoEventArgs> OnRestauraFocoAplicacao;

        /// <summary>
        /// Occurs when [on limpa teclado].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<ExecutaAcaoEventArgs> OnLimpaTeclado;

        /// <summary>
        /// Occurs when [on comanda ecf].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<ComandaVendaEventArgs> OnComandaVenda;

        /// <summary>
        /// Occurs when [on comanda ecf subtotaliza].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<ComandaVendaSubtotalizaEventArgs> OnComandaVendaSubtotaliza;

        /// <summary>
        /// Occurs when [on comanda ecf pagamento].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<ComandaPagamentoVendaEventArgs> OnComandaPagamentoVenda;

        /// <summary>
        /// Occurs when [on comanda ecf abre vinculado].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<ComandaVendaAbreVinculadoEventArgs> OnComandaVendaAbreVinculado;

        /// <summary>
        /// Occurs when [on comanda ecf imprime via].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<ComandaVendaImprimeViaEventArgs> OnComandaVendaImprimeVia;

        /// <summary>
        /// Occurs when [on information ecf].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<InfoVendaEventArgs> OnInfoVenda;

        /// <summary>
        /// Occurs when [on antes finalizar requisicao].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<AntesFinalizarRequisicaoEventArgs> OnAntesFinalizarRequisicao;

        /// <summary>
        /// Occurs when [on depois confirmar transacoes].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<DepoisConfirmarTransacoesEventArgs> OnDepoisConfirmarTransacoes;

        /// <summary>
        /// Occurs when [on antes cancelar transacao].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<AntesCancelarTransacaoEventArgs> OnAntesCancelarTransacao;

        /// <summary>
        /// Occurs when [on depois cancelar transacoes].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<DepoisCancelarTransacoesEventArgs> OnDepoisCancelarTransacoes;

        /// <summary>
        /// Occurs when [on muda estado req].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<MudaEstadoReqEventArgs> OnMudaEstadoReq;

        /// <summary>
        /// Occurs when [on muda estado resp].
        /// </summary>
        [Category("Geral")]
        public event EventHandler<MudaEstadoRespEventArgs> OnMudaEstadoResp;

        #endregion Eventos

        #region Fields

        private TEFBase selectedTEF;
        private TEFTipo gpAtual;
        private TEFBase[] gerenciadores;
        private RespEstado estadoResp;
        private ReqEstado estadoReq;
        private DateTime? tempoInicialMensagemOperador;
        private DateTime? tempoInicialMensagemCliente;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the about.
        /// </summary>
        /// <value>The about.</value>
        [Category("Geral")]
        public string About => $@"ACBrTEF v{Assembly.GetExecutingAssembly().GetName().Version}";

        /// <summary>
        /// Gets or sets the path backup.
        /// </summary>
        /// <value>The path backup.</value>
        [Category("Geral")]
        public string PathBackup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic ativar].
        /// </summary>
        /// <value><c>true</c> if [automatic ativar]; otherwise, <c>false</c>.</value>
        [Category("Geral")]
        public bool AutoAtivar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [multiplos cartoes].
        /// </summary>
        /// <value><c>true</c> if [multiplos cartoes]; otherwise, <c>false</c>.</value>
        [Category("Geral")]
        public bool MultiplosCartoes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [CHQ em gerencial].
        /// </summary>
        /// <value><c>true</c> if [CHQ em gerencial]; otherwise, <c>false</c>.</value>
        [Category("Geral")]
        public bool ChqEmGerencial { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [exibir MSG autenticacao].
        /// </summary>
        /// <value><c>true</c> if [exibir MSG autenticacao]; otherwise, <c>false</c>.</value>
        [Category("Geral")]
        public bool ExibirMsgAutenticacao { get; set; }

        /// <summary>
        /// Gets a value indicating whether [teclado bloqueado].
        /// </summary>
        /// <value><c>true</c> if [teclado bloqueado]; otherwise, <c>false</c>.</value>
        [Category("Geral")]
        public bool TecladoBloqueado { get; internal set; }

        /// <summary>
        /// Gets or sets the troco maximo.
        /// </summary>
        /// <value>The troco maximo.</value>
        [Category("Geral")]
        public decimal TrocoMaximo { get; set; }

        /// <summary>
        /// Gets or sets the espera sleep.
        /// </summary>
        /// <value>The espera sleep.</value>
        [Category("Geral")]
        public int EsperaSleep { get; set; }

        /// <summary>
        /// Gets or sets the espera STS.
        /// </summary>
        /// <value>The espera STS.</value>
        [Category("Geral")]
        public int EsperaSts { get; set; }

        /// <summary>
        /// Gets or sets the espera minima mensagem final.
        /// </summary>
        /// <value>The espera minima mensagem final.</value>
        [Category("Geral")]
        public int EsperaMinimaMensagemFinal { get; set; }

        [Category("Geral")]
        public bool ConfirmarAntesDosComprovantes { get; set; }

        [Category("Geral")]
        public bool ConfirmarDepoisDosComprovantes { get; set; }

        [Category("Geral")]
        public bool ImprimirViaClienteReduzida { get; set; }

        /// <summary>
        /// Gets or sets the number vias.
        /// </summary>
        /// <value>The number vias.</value>
        [Category("Geral")]
        public int NumVias { get; set; }

        /// <summary>
        /// Gets or sets the numero maximo cartoes.
        /// </summary>
        /// <value>The numero maximo cartoes.</value>
        [Category("Geral")]
        public int NumeroMaximoCartoes { get; set; }

        /// <summary>
        /// Gets the identificacao.
        /// </summary>
        /// <value>The identificacao.</value>
        [Category("Identificação")]
        public IdentificacaoTEF Identificacao { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether [suporta saque].
        /// </summary>
        /// <value><c>true</c> if [suporta saque]; otherwise, <c>false</c>.</value>
        [Category("Identificação")]
        public bool SuportaSaque { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [suporta desconto].
        /// </summary>
        /// <value><c>true</c> if [suporta desconto]; otherwise, <c>false</c>.</value>
        [Category("Identificação")]
        public bool SuportaDesconto { get; set; }

        [Category("Identificação")]
        public bool SuportaReajusteValor { get; set; }

        [Category("Geral")]
        public bool AutoFinalizarCupom { get; set; }

        [Category("Geral")]
        public bool IsDFe { get; set; }

        /// <summary>
        /// Gets the req.
        /// </summary>
        /// <value>The req.</value>
        [Browsable(false)]
        public RequisicaoTEF Requisicao { get; internal set; }

        /// <summary>
        /// Gets or sets the estado req.
        /// </summary>
        /// <value>The estado req.</value>
        [Category("Geral")]
        public ReqEstado EstadoReq
        {
            get => estadoReq;
            set
            {
                estadoReq = value;
                OnMudaEstadoReq.Raise(this, new MudaEstadoReqEventArgs(value));
            }
        }

        /// <summary>
        /// Gets the resp.
        /// </summary>
        /// <value>The resp.</value>
        [Browsable(false)]
        public RetornoTEF Resposta { get; internal set; }

        /// <summary>
        /// Gets or sets the estado resp.
        /// </summary>
        /// <value>The estado resp.</value>
        [Category("Geral")]
        public RespEstado EstadoResp
        {
            get => estadoResp;
            set
            {
                estadoResp = value;
                OnMudaEstadoResp.Raise(this, new MudaEstadoRespEventArgs(value));
            }
        }

        /// <summary>
        /// Gets the respostas pendentes.
        /// </summary>
        /// <value>The respostas pendentes.</value>
        [Browsable(false)]
        public RetornoTEFCollection RespostasPendentes { get; internal set; }

        /// <summary>
        /// Gets the tef dial.
        /// </summary>
        /// <value>The tef dial.</value>
        [Category("GP"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TEFDial TEFDial { get; private set; }

        [Category("GP"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TEFDisc TEFDisc { get; private set; }

        /// <summary>
        /// Gets the tef cli si tef.
        /// </summary>
        /// <value>The tef cli si tef.</value>
        [Category("GP"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TEFCliSiTef TEFCliSiTef { get; private set; }

        /// <summary>
        /// Gets or sets the gp atual.
        /// </summary>
        /// <value>The gp atual.</value>
        [Browsable(false)]
        public TEFTipo GpAtual
        {
            get => gpAtual;
            set
            {
                if (gpAtual == value)
                    return;

                gpAtual = value;

                switch (gpAtual)
                {
                    case TEFTipo.TEFDial:
                        selectedTEF = TEFDial;
                        break;

                    case TEFTipo.TEFDisc:
                        selectedTEF = TEFDisc;
                        break;

                    case TEFTipo.CliSiTef:
                        selectedTEF = TEFCliSiTef;
                        break;

                    default:
                        selectedTEF = null;
                        break;
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializars the specified gp.
        /// </summary>
        /// <param name="gp">The gp.</param>
        public void Inicializar(TEFTipo gp = TEFTipo.Nenhum)
        {
            Guard.Against<ACBrException>(OnExibeMensagem == null, "Evento [OnExibeMsg] não programado");
            Guard.Against<ACBrException>(OnComandaVendaImprimeVia == null, "Evento [OnComandaECFImprimeVia] não programado");

            if (!IsDFe)
            {
                Guard.Against<ACBrException>(OnComandaVenda == null, "Evento [OnComandaECF] não programado");
                Guard.Against<ACBrException>(OnComandaVendaAbreVinculado == null, "Evento [OnComandaECFAbreVinculado] não programado");
                Guard.Against<ACBrException>(OnInfoVenda == null, "Evento [OnInfoECF] não programado");
            }

            if (!Directory.Exists(PathBackup))
            {
                Directory.CreateDirectory(PathBackup);
            }

            Guard.Against<ACBrException>(!Directory.Exists(PathBackup), "Diretório de Backup não existente:{0}{1}", Environment.NewLine, PathBackup);

            if (gp == TEFTipo.Nenhum)
            {
                var erros = new StringBuilder();
                foreach (var tefClass in gerenciadores.Where(x => x.Habilitado))
                {
                    try
                    {
                        tefClass.Inicializado = true;
                    }
                    catch (Exception e)
                    {
                        this.Log().ErrorFormat("Erro ao inicializar GP {0}{1}{2}", tefClass.Name, Environment.NewLine, e);
                        erros.AppendLine(e.Message);
                    }
                }

                Guard.Against<ACBrException>(!erros.ToString().IsEmpty(), erros.ToString());
            }
            else
            {
                try
                {
                    GpAtual = gp;
                    selectedTEF.Inicializado = true;
                    selectedTEF.Habilitado = true;
                }
                catch (Exception e)
                {
                    this.Log().ErrorFormat("Erro ao inicializar GP {0}{1}{2}", selectedTEF.Name, Environment.NewLine, e);
                    throw new ACBrException("Erro ao inicializar GP", e);
                }
            }
        }

        /// <summary>
        /// DESs the inicializar.
        /// </summary>
        /// <param name="gp">The gp.</param>
        public void DesInicializar(TEFTipo gp)
        {
            if (gp == TEFTipo.Nenhum)
            {
                var erros = new StringBuilder();
                foreach (var tefClass in gerenciadores.Where(x => x.Habilitado))
                {
                    try
                    {
                        tefClass.Inicializado = false;
                    }
                    catch (Exception e)
                    {
                        this.Log().ErrorFormat("Erro ao desinicializar GP {0}{1}{2}", tefClass.Name, Environment.NewLine, e);
                        erros.AppendLine(e.Message);
                    }
                }

                Guard.Against<ACBrException>(!erros.ToString().IsEmpty(), erros.ToString());
            }
            else
            {
                try
                {
                    GpAtual = gp;
                    selectedTEF.Inicializado = false;
                }
                catch (Exception e)
                {
                    this.Log().ErrorFormat("Erro ao desinicializar GP {0}{1}{2}", selectedTEF.Name, Environment.NewLine, e);
                    throw new ACBrException("Erro ao desinicializar GP", e);
                }
            }
        }

        /// <summary>
        /// Ativars the gp.
        /// </summary>
        /// <param name="gp">The gp.</param>
        public void AtivarGp(TEFTipo gp)
        {
            if (gp == TEFTipo.Nenhum)
            {
                var erros = new StringBuilder();
                foreach (var tefClass in gerenciadores.Where(x => x.Habilitado))
                {
                    try
                    {
                        tefClass.AtivarGP();
                    }
                    catch (Exception e)
                    {
                        this.Log().ErrorFormat("Erro ao ativar GP {0}{1}{2}", tefClass.Name, Environment.NewLine, e);
                        erros.AppendLine(e.Message);
                    }
                }

                Guard.Against<ACBrException>(!erros.ToString().IsEmpty(), erros.ToString());
            }
            else
            {
                try
                {
                    GpAtual = gp;
                    selectedTEF.AtivarGP();
                }
                catch (Exception e)
                {
                    this.Log().ErrorFormat("Erro ao ativar GP {0}{1}{2}", selectedTEF.Name, Environment.NewLine, e);
                    throw new ACBrException("Erro ao ativar GP", e);
                }
            }
        }

        /// <summary>
        /// CRTs the specified valor.
        /// </summary>
        /// <param name="valor">The valor.</param>
        /// <param name="indicePagamento">The indice fp g_ ecf.</param>
        /// <param name="documentoVinculado">The documento vinculado.</param>
        /// <param name="moeda">The moeda.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CRT(decimal valor, string indicePagamento, string documentoVinculado = "", int moeda = 0)
        {
            Guard.Against<Exception>(selectedTEF == null, "Nenhum GP selecionado.");

            return selectedTEF.CRT(valor, indicePagamento, documentoVinculado, moeda);
        }

        public bool PRE(decimal valor, string indicePagamento, string documentoVinculado = "", int moeda = 0)
        {
            Guard.Against<Exception>(selectedTEF == null, "Nenhum GP selecionado.");

            return selectedTEF.PRE(valor, indicePagamento, documentoVinculado, moeda);
        }

        /// <summary>
        /// CHQs the specified valor.
        /// </summary>
        /// <param name="valor">The valor.</param>
        /// <param name="indicePagamento">The indice fp g_ ecf.</param>
        /// <param name="documentoVinculado">The documento vinculado.</param>
        /// <param name="cmc7">The cm c7.</param>
        /// <param name="tipoPessoa">The tipo pessoa.</param>
        /// <param name="documentoPessoa">The documento pessoa.</param>
        /// <param name="dataCheque">The data cheque.</param>
        /// <param name="banco">The banco.</param>
        /// <param name="agencia">The agencia.</param>
        /// <param name="agenciaDc">The agencia dc.</param>
        /// <param name="conta">The conta.</param>
        /// <param name="contaDc">The conta dc.</param>
        /// <param name="cheque">The cheque.</param>
        /// <param name="chequeDc">The cheque dc.</param>
        /// <param name="compensacao">The compensacao.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CHQ(decimal valor, string indicePagamento, string documentoVinculado = "", string cmc7 = "",
            char tipoPessoa = 'F', string documentoPessoa = "", DateTime? dataCheque = null, string banco = "",
            string agencia = "", string agenciaDc = "", string conta = "", string contaDc = "", string cheque = "",
            string chequeDc = "", string compensacao = "")
        {
            Guard.Against<Exception>(selectedTEF == null, "Nenhum GP selecionado.");

            if (!dataCheque.HasValue) dataCheque = DateTime.Now;

            return selectedTEF.CHQ(valor, indicePagamento, documentoVinculado, cmc7, tipoPessoa, documentoPessoa,
                                 dataCheque.Value, banco, agencia, agenciaDc, conta, contaDc, cheque, chequeDc, compensacao);
        }

        /// <summary>
        /// Atvs the specified gp.
        /// </summary>
        /// <param name="gp">The gp.</param>
        public void ATV(TEFTipo gp = TEFTipo.Nenhum)
        {
            if (gp != TEFTipo.Nenhum)
                GpAtual = gp;

            Guard.Against<Exception>(selectedTEF == null, "Nenhum GP selecionado.");

            selectedTEF.ATV();
        }

        /// <summary>
        /// Adms the specified gp.
        /// </summary>
        /// <param name="gp">The gp.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ADM(TEFTipo gp = TEFTipo.Nenhum)
        {
            if (gp != TEFTipo.Nenhum)
                GpAtual = gp;

            Guard.Against<Exception>(selectedTEF == null, "Nenhum GP selecionado.");

            return selectedTEF.ADM();
        }

        /// <summary>
        /// CNCs the specified rede.
        /// </summary>
        /// <param name="rede">The rede.</param>
        /// <param name="nsu">The nsu.</param>
        /// <param name="dataHoraTransacao">The data hora transacao.</param>
        /// <param name="valor">The valor.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CNC(string rede, string nsu, DateTime dataHoraTransacao, decimal valor)
        {
            Guard.Against<Exception>(selectedTEF == null, "Nenhum GP selecionado.");

            return selectedTEF.CNC(rede, nsu, dataHoraTransacao, valor);
        }

        /// <summary>
        /// CNFs the specified rede.
        /// </summary>
        /// <param name="rede">The rede.</param>
        /// <param name="nsu">The nsu.</param>
        /// <param name="finalizacao">The finalizacao.</param>
        /// <param name="documentoVinculado">The documento vinculado.</param>
        public void CNF(string rede, string nsu, string finalizacao, string documentoVinculado = "")
        {
            Guard.Against<Exception>(selectedTEF == null, "Nenhum GP selecionado.");

            selectedTEF.CNF(rede, nsu, finalizacao, documentoVinculado);
        }

        /// <summary>
        /// NCNs the specified rede.
        /// </summary>
        /// <param name="rede">The rede.</param>
        /// <param name="nsu">The nsu.</param>
        /// <param name="finalizacao">The finalizacao.</param>
        /// <param name="valor">The valor.</param>
        /// <param name="documentoVinculado">The documento vinculado.</param>
        public void NCN(string rede, string nsu, string finalizacao, decimal valor = 0M, string documentoVinculado = "")
        {
            Guard.Against<Exception>(selectedTEF == null, "Nenhum GP selecionado.");

            selectedTEF.NCN(rede, nsu, finalizacao, valor, documentoVinculado);
        }

        /// <summary>
        /// Cancelars the transacoes pendentes.
        /// </summary>
        public void CancelarTransacoesPendentes()
        {
            this.Log().Info("CancelarTransacoesPendentes");

            // Ajustando o mesmo valor nas Classes de TEF, caso elas usem o valor default
            try
            {
                foreach (var gerenciador in gerenciadores.Where(x => x.Habilitado))
                {
                    gerenciador.CancelarTransacoesPendentes();
                }
            }
            finally
            {
                try
                {
                    DoOnDepoisCancelarTrasacoes();
                }
                finally
                {
                    RespostasPendentes.Clear();
                }
            }
        }

        /// <summary>
        /// Confirmars the transacoes pendentes.
        /// </summary>
        /// <param name="apagarRespostasPendentes"></param>
        public void ConfirmarTransacoesPendentes(bool apagarRespostasPendentes = true)
        {
            this.Log().Info("ConfirmarTransacoesPendentes");

            foreach (var pendente in RespostasPendentes)
            {
                try
                {
                    GpAtual = pendente.TipoGP;
                    if (!pendente.CNFEnviado)
                    {
                        CNF(pendente.Rede, pendente.NSU, pendente.Finalizacao, pendente.DocumentoVinculado);
                        pendente.CNFEnviado = true;
                    }

                    ApagaEVerifica(pendente.ArqRespPendente);
                    ApagaEVerifica(pendente.ArqBackup);
                }
                catch (Exception e)
                {
                    //Exceção Muda... Fica em Loop até conseguir confirmar e apagar Backup
                    this.Log().Debug(e);
                }
            }

            try
            {
                DoOnDepoisConfirmarTransacoes();
            }
            finally
            {
                if (apagarRespostasPendentes)
                    RespostasPendentes.Clear();
            }
        }

        /// <summary>
        /// Imprime as transações pendentes
        /// </summary>
        public void ImprimirTransacoesPendentes()
        {
            if (RespostasPendentes.Count < 1) return;

            this.Log().Info("ImprimirTransacoesPendentes");

            var est = DoEstadoVenda();

            var funcLiberarVenda = new Action(() =>
            {
                if (est == EstadoVenda.Livre) return;

                switch (est)
                {
                    case EstadoVenda.Venda:
                    case EstadoVenda.Pagamento:
                    case EstadoVenda.NaoFiscal:
                        FinalizarCupom(false);
                        break;

                    case EstadoVenda.RelatorioGerencial:
                        DoComandaVenda(OperacaoVenda.FechaGerencial);
                        break;

                    case EstadoVenda.CupomVinculado:
                        DoComandaVenda(OperacaoVenda.FechaVinculado);
                        break;
                }

                Guard.Against<ACBrException>(DoEstadoVenda() != EstadoVenda.Livre, CacbrTefdErroEcfNaoLivre);
            });

            funcLiberarVenda();

            if (ConfirmarAntesDosComprovantes)
                ConfirmarTransacoesPendentes(false);

            var impressaoOk = false;
            var gerencial = false;
            var removerMsg = false;
            var gerencialAberto = false;
            var msgAutenticacaoAExibir = string.Empty;

            try
            {
                while (!impressaoOk)
                {
                    try
                    {
                        try
                        {
                            if (gerencial)
                            {
                                est = DoEstadoVenda();
                                funcLiberarVenda();

                                gerencialAberto = false;
                                for (var j = 0; j < RespostasPendentes.Count; j++)
                                {
                                    var pendente = RespostasPendentes[j];
                                    GpAtual = pendente.TipoGP;
                                    var tempoInicio = DateTime.Now;

                                    var nVias = selectedTEF.NumVias;
                                    if (!pendente.ImagemComprovante2aVia.Any())
                                        nVias = 1;

                                    if (!pendente.ImagemComprovante1aVia.Any())
                                        nVias = 0;

                                    if (nVias > 0)
                                    {
                                        if (pendente.TextoEspecialOperador.IsEmpty())
                                        {
                                            removerMsg = true;
                                            DoExibeMsg(OperacaoMensagem.ExibirMsgOperador, pendente.TextoEspecialOperador);
                                        }

                                        if (pendente.TextoEspecialCliente.IsEmpty())
                                        {
                                            removerMsg = true;
                                            DoExibeMsg(OperacaoMensagem.ExibirMsgCliente, pendente.TextoEspecialCliente);
                                        }
                                    }
                                    else
                                    {
                                        if (!pendente.TextoEspecialOperador.IsEmpty() ||
                                            !pendente.TextoEspecialCliente.IsEmpty())
                                        {
                                            var mensagem = pendente.TextoEspecialOperador;
                                            if (!mensagem.IsEmpty())
                                                mensagem += Environment.NewLine + Environment.NewLine;

                                            mensagem += pendente.TextoEspecialCliente;
                                            DoExibeMsg(OperacaoMensagem.OK, mensagem);
                                        }
                                    }

                                    if (!gerencialAberto && NumVias > 0)
                                    {
                                        DoComandaVenda(OperacaoVenda.AbreGerencial);
                                        gerencialAberto = true;
                                    }

                                    var i = 1;
                                    while (i < NumVias)
                                    {
                                        DoVendaImprimeVia(TipoRelatorio.Gerencial, i,
                                            i == 1 ? pendente.ImagemComprovante1aVia : pendente.ImagemComprovante2aVia);

                                        if (i < NumVias || j < RespostasPendentes.Count - 1)
                                        {
                                            DoComandaVenda(OperacaoVenda.PulaLinhas);
                                            DoExibeMsg(OperacaoMensagem.DestaqueVia, CacbrTefdDestaqueVia.Substitute(i));
                                        }

                                        i++;
                                    }

                                    //Removendo a mensagem do Operador
                                    if (removerMsg)
                                    {
                                        AguardarTempoMinimoDeExibicao(tempoInicio);
                                        DoExibeMsg(OperacaoMensagem.RemoverMsgOperador, string.Empty);
                                        DoExibeMsg(OperacaoMensagem.RemoverMsgCliente, string.Empty);
                                    }

                                    if (ExibirMsgAutenticacao && !pendente.Autenticacao.IsEmpty())
                                    {
                                        msgAutenticacaoAExibir = $"Favor anotar no verso do Cheque:{Environment.NewLine}{pendente.Autenticacao}";
                                    }

                                    if (j < RespostasPendentes.Count - 1 && !msgAutenticacaoAExibir.IsEmpty())
                                    {
                                        DoExibeMsg(OperacaoMensagem.OK, msgAutenticacaoAExibir);
                                        msgAutenticacaoAExibir = string.Empty;
                                    }
                                }

                                if (gerencialAberto)
                                    DoComandaVenda(OperacaoVenda.FechaGerencial);
                            }
                            else
                            {
                                var ordem = -1;
                                var pagamentos = RespostasPendentes.OrderedAndGrouped;
                                for (var k = 0; k < pagamentos.Length; k++)
                                {
                                    var pagamento = pagamentos[k];
                                    if (pagamento.OrdemPagamento >= 999)
                                        gerencial = true;

                                    for (var j = 0; j < RespostasPendentes.Count; j++)
                                    {
                                        var pendente = RespostasPendentes[j];
                                        if (pagamento.OrdemPagamento != pendente.OrdemPagamento)
                                            continue;

                                        GpAtual = pendente.TipoGP;
                                        var tempoInicio = DateTime.Now;

                                        var nVias = selectedTEF.NumVias;
                                        if (!pendente.ImagemComprovante2aVia.Any())
                                            nVias = 1;

                                        if (!pendente.ImagemComprovante1aVia.Any())
                                            nVias = 0;

                                        if (nVias > 0)
                                        {
                                            if (pendente.TextoEspecialOperador.IsEmpty())
                                            {
                                                removerMsg = true;
                                                DoExibeMsg(OperacaoMensagem.ExibirMsgOperador, pendente.TextoEspecialOperador);
                                            }

                                            if (pendente.TextoEspecialCliente.IsEmpty())
                                            {
                                                removerMsg = true;
                                                DoExibeMsg(OperacaoMensagem.ExibirMsgCliente, pendente.TextoEspecialCliente);
                                            }
                                        }
                                        else
                                        {
                                            if (!pendente.TextoEspecialOperador.IsEmpty() ||
                                                !pendente.TextoEspecialCliente.IsEmpty())
                                            {
                                                var mensagem = pendente.TextoEspecialOperador;
                                                if (!mensagem.IsEmpty())
                                                    mensagem += Environment.NewLine + Environment.NewLine;

                                                mensagem += pendente.TextoEspecialCliente;
                                                DoExibeMsg(OperacaoMensagem.OK, mensagem);
                                            }
                                        }

                                        if (nVias > 0 && ordem != pendente.OrdemPagamento)
                                        {
                                            ordem = pendente.OrdemPagamento;
                                            if (gerencial)
                                            {
                                                DoComandaVenda(OperacaoVenda.AbreGerencial);
                                                gerencialAberto = true;
                                            }
                                            else
                                            {
                                                DoVendaAbreVinculado(pendente.DocumentoVinculado, pagamento.IndicePagamento, pagamento.ValorTotal);
                                            }
                                        }

                                        var i = 1;
                                        while (i <= nVias)
                                        {
                                            DoVendaImprimeVia(gerencial ? TipoRelatorio.Gerencial : TipoRelatorio.Vinculado, i,
                                                i == 1 ? pendente.ImagemComprovante1aVia : pendente.ImagemComprovante2aVia);

                                            if (i < NumVias || j < RespostasPendentes.Count - 1)
                                            {
                                                DoComandaVenda(OperacaoVenda.PulaLinhas);
                                                DoExibeMsg(OperacaoMensagem.DestaqueVia, CacbrTefdDestaqueVia.Substitute(i));
                                            }

                                            i++;
                                        }

                                        //Removendo a mensagem do Operador
                                        if (removerMsg)
                                        {
                                            AguardarTempoMinimoDeExibicao(tempoInicio);
                                            DoExibeMsg(OperacaoMensagem.RemoverMsgOperador, string.Empty);
                                            DoExibeMsg(OperacaoMensagem.RemoverMsgCliente, string.Empty);
                                        }

                                        if (ExibirMsgAutenticacao && !pendente.Autenticacao.IsEmpty())
                                        {
                                            msgAutenticacaoAExibir = $"Favor anotar no verso do Cheque:{Environment.NewLine}{pendente.Autenticacao}";
                                        }

                                        if (j < RespostasPendentes.Count - 1 && !msgAutenticacaoAExibir.IsEmpty())
                                        {
                                            DoExibeMsg(OperacaoMensagem.OK, msgAutenticacaoAExibir);
                                            msgAutenticacaoAExibir = string.Empty;
                                        }
                                    }

                                    if (ordem <= -1) continue;

                                    if (gerencialAberto)
                                    {
                                        DoComandaVenda(OperacaoVenda.FechaGerencial);
                                        gerencialAberto = false;
                                    }
                                    else
                                    {
                                        DoComandaVenda(OperacaoVenda.FechaVinculado);
                                    }
                                }
                            }

                            impressaoOk = true;
                        }
                        finally
                        {
                            if (removerMsg)
                            {
                                DoExibeMsg(OperacaoMensagem.RemoverMsgOperador, string.Empty);
                                DoExibeMsg(OperacaoMensagem.RemoverMsgCliente, string.Empty);
                            }
                        }
                    }
                    catch (ACBrException)
                    {
                        impressaoOk = false;
                    }

                    if (!impressaoOk)
                        if (DoExibeMsg(OperacaoMensagem.YesNo, CacbrTefdErroEcfNaoResponde) != ModalResult.Yes) break;

                    gerencial = true;
                }
            }
            finally
            {
                if (ConfirmarAntesDosComprovantes || !impressaoOk)
                {
                    try
                    {
                        DoComandaVenda(OperacaoVenda.CancelaCupom);
                    }
                    catch (Exception)
                    {
                        //
                    }

                    CancelarTransacoesPendentes();
                }
                else
                {
                    if (ConfirmarDepoisDosComprovantes) ConfirmarTransacoesPendentes();
                }

                BloquearMouseTeclado(false);
                if (!msgAutenticacaoAExibir.IsEmpty())
                    DoExibeMsg(OperacaoMensagem.OK, msgAutenticacaoAExibir);
            }

            if (ConfirmarAntesDosComprovantes | ConfirmarDepoisDosComprovantes) RespostasPendentes.Clear();
        }

        /// <summary>
        /// Finaliza o cupom
        /// </summary>
        /// <param name="desbloqueia">Se <c>true</c> [desbloqueia] o mouse/teclado no termino.</param>
        // ReSharper disable once FunctionComplexityOverflow
        public void FinalizarCupom(bool desbloqueia = true)
        {
            var impressaoOk = false;
            this.Log().InfoFormat("FinalizarCupom [{0}] Desbloquear mouse/teclado no termino", desbloqueia);

            try
            {
                while (!impressaoOk)
                {
                    try
                    {
                        BloquearMouseTeclado();
                        try
                        {
                            var estNaoFiscal = EstadoVenda.NaoFiscal;
                            var est = DoEstadoVenda();
                            while (est != EstadoVenda.Livre)
                            {
                                // É não fiscal ? Se SIM, vamos passar por todas as fases...
                                if (est == estNaoFiscal)
                                {
                                    switch (estNaoFiscal)
                                    {
                                        case EstadoVenda.NaoFiscal:
                                            estNaoFiscal = EstadoVenda.Venda;
                                            break;

                                        case EstadoVenda.Venda:
                                            estNaoFiscal = EstadoVenda.Pagamento;
                                            break;

                                        case EstadoVenda.Pagamento:
                                            estNaoFiscal = EstadoVenda.NaoFiscal;
                                            break;
                                    }

                                    est = estNaoFiscal;
                                }

                                try
                                {
                                    switch (est)
                                    {
                                        case EstadoVenda.Venda:
                                            SubtotalizaVenda(RespostasPendentes.TotalDesconto);
                                            break;

                                        case EstadoVenda.Pagamento:
                                            var pagamentos = RespostasPendentes.OrderedAndGrouped;
                                            var ordem = 0;
                                            foreach (var pgt in pagamentos)
                                            {
                                                ordem++;
                                                if (pgt.OrdemPagamento != 0)
                                                {
                                                    ordem = pgt.OrdemPagamento;
                                                    continue;
                                                }

                                                if (DoOnInfoVendaAsDecimal(InfoVenda.SubTotal) > 0 && pgt.ValorTotal > 0)
                                                    PagamentoVenda(pgt.IndicePagamento, pgt.ValorTotal);

                                                var indice = pgt.IndicePagamento;
                                                foreach (var pendente in RespostasPendentes
                                                    .Where(x => x.IndicePagamento == indice))
                                                {
                                                    if (pendente.Header == "CHQ" && ChqEmGerencial)
                                                    {
                                                        pendente.OrdemPagamento = 999;
                                                        ordem--;
                                                    }
                                                    else
                                                    {
                                                        pendente.OrdemPagamento = ordem;
                                                    }
                                                }
                                            }

                                            if (DoOnInfoVendaAsDecimal(InfoVenda.SubTotal) > 0)
                                            {
                                                if (DoOnInfoVendaAsDecimal(InfoVenda.TotalAPagar) > 0)
                                                {
                                                    DoComandaVenda(OperacaoVenda.ImprimePagamentos);
                                                    if (DoOnInfoVendaAsDecimal(InfoVenda.SubTotal) > 0) break;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }

                                            DoComandaVenda(OperacaoVenda.FechaCupom);
                                            break;

                                        case EstadoVenda.NaoFiscal: //Usado apenas no Fechamento de NaoFiscal
                                            if (DoOnInfoVendaAsDecimal(InfoVenda.SubTotal) > 0)
                                            {
                                                if (DoOnInfoVendaAsDecimal(InfoVenda.TotalAPagar) > 0)
                                                {
                                                    DoComandaVenda(OperacaoVenda.ImprimePagamentos);
                                                    if (DoOnInfoVendaAsDecimal(InfoVenda.SubTotal) > 0)
                                                        break;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }

                                            DoComandaVenda(OperacaoVenda.FechaCupom);
                                            break;

                                        default:
                                            throw new ACBrException("ECF deve estar em Venda ou Pagamento");
                                    }
                                }
                                catch (Exception exception)
                                {
                                    // A condição abaixo, será True se não for Cupom Nao Fiscal,
                                    // ou se já tentou todas as fases do Cupom Nao Fiscal
                                    // (SubTotaliza, Pagamento, Fechamento)...
                                    // Se for NaoFiscal não deve disparar uma exceção até ter
                                    // tentado todas as fases descritas acima, pois o ACBrECF
                                    // não é capaz de detectar com precisão a fase atual do
                                    // Cupom Não Fiscal (poucos ECFs possuem flags para isso)
                                    if (estNaoFiscal == EstadoVenda.NaoFiscal)
                                    {
                                        throw new ACBrException("", exception);
                                    }
                                }

                                est = DoEstadoVenda();
                            }

                            impressaoOk = true;
                        }
                        finally
                        {
                            if (desbloqueia)
                            {
                                BloquearMouseTeclado(false);
                            }
                        }
                    }
                    catch (ACBrException)
                    {
                        impressaoOk = false;
                    }
                    catch (Exception e)
                    {
                        this.Log().Error(e);
                        throw;
                    }

                    if (impressaoOk) continue;
                    if (DoExibeMsg(OperacaoMensagem.YesNo, CacbrTefdErroEcfNaoResponde) == ModalResult.Yes) continue;

                    try
                    {
                        DoComandaVenda(OperacaoVenda.CancelaCupom);
                    }
                    catch (Exception e)
                    {
                        this.Log().Error(e);
                    }

                    break;
                }
            }
            finally
            {
                if (!impressaoOk)
                {
                    CancelarTransacoesPendentes();
                }
            }
        }

        #endregion Methods

        #region Internal Methods

        /// <summary>
        /// Restaurars the foco aplicacao.
        /// </summary>
        internal void RestaurarFocoAplicacao()
        {
            var e = new ExecutaAcaoEventArgs();
            OnRestauraFocoAplicacao.Raise(this, e);

            if (e.Tratado) return;

            //Testar se funciona.
            var handle = Process.GetCurrentProcess().MainWindowHandle;
            UtilTEF.BringWindowToFocus(handle);
        }

        /// <summary>
        /// Limpars the teclado.
        /// </summary>
        internal void LimparTeclado()
        {
            var e = new ExecutaAcaoEventArgs();
            OnLimpaTeclado.Raise(this, e);

            //Testar se funciona.
            if (e.Tratado) return;

            UtilTEF.CleanKeyboardBuffer();
        }

        /// <summary>
        /// Bloquears the mouse teclado.
        /// </summary>
        /// <param name="bloqueia">if set to <c>true</c> [desbloqueia].</param>
        internal void BloquearMouseTeclado(bool bloqueia = true)
        {
            TecladoBloqueado = bloqueia;
            this.Log().InfoFormat("BloquearMouseTeclado: {0}", bloqueia);

            var e = new BloqueiaMouseTecladoEventArgs(bloqueia);
            OnBloqueiaMouseTeclado.Raise(this, e);

            if (!bloqueia)
            {
                LimparTeclado();
            }

            //Testar se funciona.
            if (e.Tratado) return;

            UtilTEF.BlockInput(bloqueia);
        }

        /// <summary>
        /// Does the on antes finalizar requisicao.
        /// </summary>
        internal void DoOnAntesFinalizarRequisicao()
        {
            var e = new AntesFinalizarRequisicaoEventArgs(selectedTEF.Requisicao);
            OnAntesFinalizarRequisicao.Raise(this, e);
        }

        /// <summary>
        /// Does the on aguarda resp.
        /// </summary>
        /// <param name="e">The <see cref="AguardaRespEventArgs" /> instance containing the event data.</param>
        internal void DoOnAguardaResp(AguardaRespEventArgs e)
        {
            OnAguardaResp.Raise(this, e);
        }

        /// <summary>
        /// Does the on depois confirmar transacoes.
        /// </summary>
        internal void DoOnDepoisConfirmarTransacoes()
        {
            var e = new DepoisConfirmarTransacoesEventArgs(RespostasPendentes);
            OnDepoisConfirmarTransacoes.Raise(this, e);
        }

        /// <summary>
        /// Does the on depois cancelar trasacoes.
        /// </summary>
        internal void DoOnDepoisCancelarTrasacoes()
        {
            var e = new DepoisCancelarTransacoesEventArgs(RespostasPendentes);
            OnDepoisCancelarTransacoes.Raise(this, e);
        }

        /// <summary>
        /// Does the on antes cancelar transacao.
        /// </summary>
        /// <param name="respostaCancelada">The resposta cancelada.</param>
        internal void DoOnAntesCancelarTransacao(RetornoTEF respostaCancelada)
        {
            var e = new AntesCancelarTransacaoEventArgs(respostaCancelada);
            OnAntesCancelarTransacao.Raise(this, e);
        }

        /// <summary>
        /// Does the exibe MSG.
        /// </summary>
        /// <param name="operacao">The operacao.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="manterTempoMinimo"></param>
        /// <returns>ModalResult.</returns>
        internal ModalResult DoExibeMsg(OperacaoMensagem operacao, string msg = "", bool manterTempoMinimo = false)
        {
            Guard.Against<ACBrException>(OnExibeMensagem == null, "Evento [OnExibeMsg] não programado");

            this.Log().InfoFormat("{0} DoExibeMsg: Oper: {1}  Mensagem: {2}", selectedTEF.Name, operacao, msg);

            try
            {
                var tempoInicial = operacao == OperacaoMensagem.ExibirMsgCliente ? tempoInicialMensagemCliente : tempoInicialMensagemOperador;

                // A mensagem anterior fixou um Tempo mínimo de exibição ?
                if (tempoInicial.HasValue)
                    AguardarTempoMinimoDeExibicao(tempoInicial.Value);

                if (operacao.IsIn(OperacaoMensagem.OK, OperacaoMensagem.YesNo, OperacaoMensagem.DestaqueVia))
                {
                    RestaurarFocoAplicacao();
                }

                var oldTecladoBloqueado = TecladoBloqueado;

                if (oldTecladoBloqueado && operacao.IsIn(OperacaoMensagem.OK, OperacaoMensagem.YesNo))
                {
                    BloquearMouseTeclado(false);
                }

                var e = new ExibeMensagemEventArgs(operacao, msg);
                OnExibeMensagem.Raise(this, e);

                if (oldTecladoBloqueado && e.Operacao.IsIn(OperacaoMensagem.OK, OperacaoMensagem.YesNo))
                {
                    BloquearMouseTeclado();
                }

                if (manterTempoMinimo)
                    tempoInicial = DateTime.Now;
                else
                    tempoInicial = null;

                if (operacao == OperacaoMensagem.ExibirMsgCliente)
                    tempoInicialMensagemCliente = tempoInicial;
                else
                    tempoInicialMensagemOperador = tempoInicial;

                return e.Result;
            }
            catch (Exception exception) when (!(exception is ACBrException))
            {
                throw new ACBrException(exception.Message, exception);
            }
        }

        internal EstadoVenda DoEstadoVenda()
        {
            if (IsDFe) return EstadoVenda.Livre;

            var e = new InfoVendaEventArgs(InfoVenda.EstadoVenda);
            DoOnInfoVenda(e);

            Guard.Against<ACBrException>(!Enum.IsDefined(typeof(EstadoVenda), e.EstadoVenda),
                         "Retorno de [OnInfoEcf( ineEstadoECF, Retorno )] deve ser:" + Environment.NewLine +
                         "[L] = Livre" + Environment.NewLine +
                         "[V] = Venda de Itens" + Environment.NewLine +
                         "[P] - Pagamento (ou SubTotal efetuado)" + Environment.NewLine +
                         "[C] ou [R] - CDC ou Cupom Vinculado" + Environment.NewLine +
                         "[G] ou [R] - Relatório Gerencial" + Environment.NewLine +
                         "[N] - Recebimento Não Fiscal" + Environment.NewLine +
                         "[O] - Outro");

            return e.EstadoVenda;
        }

        internal bool PagamentoVenda(string indice, decimal valor)
        {
            this.Log().InfoFormat("{0} ECFPagamento: Indice: {1} Valor: {2:c}", selectedTEF.Name, indice, valor);
            var e = new ComandaPagamentoVendaEventArgs(indice, valor);

            try
            {
                OnComandaPagamentoVenda.Raise(this, e);
            }
            catch (Exception)
            {
                e.Retorno = RetornoEvento.ErroExecucao;
            }

            if (e.Retorno == RetornoEvento.Sucesso) return true;
            var erro = e.Retorno == RetornoEvento.ErroExecucao
                    ? "Erro ao executar [OnComandaECFPagamento]"
                    : "[OnComandaECFPagamento] não tratada";

            this.Log().Error(erro);
            throw new ACBrTEFPrintException(erro);
        }

        internal bool SubtotalizaVenda(decimal descontoAcrescimo)
        {
            this.Log().InfoFormat("{0} ECFSubtotaliza: DescAcres: {1:c}", selectedTEF.Name, descontoAcrescimo);
            var e = new ComandaVendaSubtotalizaEventArgs(descontoAcrescimo);

            try
            {
                OnComandaVendaSubtotaliza.Raise(this, e);
            }
            catch (Exception)
            {
                e.Retorno = RetornoEvento.ErroExecucao;
            }

            if (e.Retorno == RetornoEvento.Sucesso)
            {
                // Deve imprimir Dinheiro antes dos pagamentos do TEF
                DoComandaVenda(OperacaoVenda.ImprimePagamentos);
                return true;
            }

            var erro = e.Retorno == RetornoEvento.ErroExecucao
                ? "Erro ao executar [OnComandaECFSubtotaliza]"
                : "[OnComandaECFSubtotaliza] não tratada";

            this.Log().Error(erro);
            throw new ACBrException(erro);
        }

        internal decimal DoOnInfoVendaAsDecimal(InfoVenda operacao)
        {
            var e = new InfoVendaEventArgs(operacao);
            DoOnInfoVenda(e);
            return e.Valor;
        }

        internal void DoOnInfoVenda(InfoVendaEventArgs e)
        {
            Guard.Against<ACBrException>(OnInfoVenda == null, "Evento OnInfoECF não programado");
            try
            {
                OnInfoVenda.Raise(this, e);
            }
            catch (Exception exception)
            {
                throw new ACBrException(exception.Message, exception);
            }
        }

        internal void DoVendaAbreVinculado(string documentoVinculado, string indicePagamento, decimal valor)
        {
            if (IsDFe) return;

            Guard.Against<ACBrTEFPrintException>(OnComandaVendaAbreVinculado == null, "Evento [OnComandaECFAbreVinculado] não programado");

            try
            {
                var e = new ComandaVendaAbreVinculadoEventArgs(documentoVinculado, indicePagamento, valor);
                OnComandaVendaAbreVinculado.Raise(this, e);
            }
            catch (Exception exception)
            {
                throw new ACBrTEFPrintException(exception.Message, exception);
            }
        }

        internal void DoComandaVenda(OperacaoVenda operacao)
        {
            if (IsDFe) return;

            Guard.Against<NullReferenceException>(OnComandaVenda == null, "Evento [OnComandaECF] não programado");

            this.Log().InfoFormat("{0} ComandaECF: Oper: {1}", selectedTEF.Name, operacao);

            var e = new ComandaVendaEventArgs(operacao, selectedTEF.Resposta);
            OnComandaVenda.Raise(this, e);
            if (e.Retorno == RetornoEvento.Sucesso) return;

            var erro = string.Format(e.Retorno == RetornoEvento.ErroExecucao ?
                "Erro ao executar Operação: [{0}]" : "Operação [{0}] não tratada em [OnComandaECF]", operacao);

            this.Log().Error(erro);
            throw new ACBrException(erro);
        }

        internal void DoVendaImprimeVia(TipoRelatorio relatorio, int via, IEnumerable<string> imagemComprovante)
        {
            Guard.Against<NullReferenceException>(OnComandaVendaImprimeVia == null, "Evento [OnComandaECFImprimeVia] não programado");

            this.Log().InfoFormat("{0} ECFImprimeVia: {1} Via: {2}", selectedTEF.Name, relatorio, via);

            var e = new ComandaVendaImprimeViaEventArgs(relatorio, via, imagemComprovante.ToArray());
            OnComandaVendaImprimeVia.Raise(this, e);

            if (e.Retorno == RetornoEvento.Sucesso) return;

            var erro = e.Retorno == RetornoEvento.ErroExecucao ?
                "Erro ao executar [OnComandaECFImprimeVia]" : "[OnComandaECFImprimeVia] não tratada";

            this.Log().Error(erro);
            throw new ACBrException(erro);
        }

        internal void ApagaEVerifica(string arquivo)
        {
            if (!File.Exists(arquivo))
                return;

            UtilTEF.DeleteFile(arquivo);
            Guard.Against<ACBrException>(File.Exists(arquivo), "Erro ao apagar o arquivo: {0}", arquivo);
        }

        private void AguardarTempoMinimoDeExibicao(DateTime data)
        {
            // Verifica se Mensagem Ficou pelo menos por 5 segundos
            var tempoCorrido = (int)(DateTime.Now - data).TotalSeconds;
            while (tempoCorrido < EsperaMinimaMensagemFinal)
            {
                this.Log().Info($"{selectedTEF.Name} AguardarTempoMinimoDeExibicao: {tempoCorrido:###.##}");

                // Avisa a aplicação, que está em Espera
                if (this.EventAssigned(nameof(OnAguardaResp)))
                    OnAguardaResp.Raise(this, new AguardaRespEventArgs("TempoMinimoMensagemFinal", tempoCorrido));

                Thread.Sleep(EsperaSleep);
                tempoCorrido = (int)(DateTime.Now - data).TotalSeconds;
            }
        }

        #endregion Internal Methods

        #region Override Methods

        /// <summary>
        /// Called when [initialize].
        /// </summary>
        protected override void OnInitialize()
        {
            TEFDial = new TEFDial(this);
            TEFDisc = new TEFDisc(this);
            TEFCliSiTef = new TEFCliSiTef(this);

            gerenciadores = new TEFBase[]
            {
                TEFDial,
                TEFDisc,
                TEFCliSiTef
            };

            GpAtual = TEFTipo.Nenhum;
            Identificacao = new IdentificacaoTEF();

            RespostasPendentes = new RetornoTEFCollection(this);

            PathBackup = $"{Assembly.GetExecutingAssembly().GetPath()}\\TEF";
            AutoAtivar = true;
            ExibirMsgAutenticacao = true;
            MultiplosCartoes = false;
            NumeroMaximoCartoes = 0;
            NumVias = CacbrTefdNumVias;
            EsperaSts = CacbrTefdEsperaSts;
            EsperaSleep = CacbrTefdEsperaSleep;
            EsperaMinimaMensagemFinal = CacbrTefdEsperaMinimaMensagemFinal;
            TecladoBloqueado = false;
            ChqEmGerencial = false;
            TrocoMaximo = 0;
            SuportaDesconto = true;
            SuportaSaque = true;
        }

        /// <summary>
        /// Called when [disposing].
        /// </summary>
        protected override void OnDisposing()
        {
        }

        #endregion Override Methods
    }
}