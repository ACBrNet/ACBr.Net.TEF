// ***********************************************************************
// Assembly         : ACBr.Net.TEFD
// Author           : RFTD
// Created          : 02-18-2015
//
// Last Modified By : RFTD
// Last Modified On : 02-18-2015
// ***********************************************************************
// <copyright file="TEFBase.cs" company="ACBr.Net">
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ACBr.Net.TEF
{
    /// <summary>
    /// Classe TEFBase.
    /// </summary>
    public abstract class TEFBase : IACBrLog
    {
        #region Fields

        protected ACBrTEF parent;
        protected bool inicializado;
        protected int idSeq;
        protected bool salvarArquivoBackup;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TEFBase"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        protected TEFBase(ACBrTEF parent, TEFTipo tipo)
        {
            this.parent = parent;
            AutoAtivarGp = true;
            ArqReq = string.Empty;
            ArqTemp = string.Empty;
            ArqResp = string.Empty;
            ArqSTS = string.Empty;
            Tipo = tipo;
            idSeq = (int)DateTime.Now.TimeOfDay.TotalSeconds;
            EsperaSts = ACBrTEF.CacbrTefdEsperaSts;
            NumVias = ACBrTEF.CacbrTefdNumVias;

            AguardandoResposta = false;
            salvarArquivoBackup = true;

            Resposta = CriarResposta(Tipo);
            Requisicao = new RequisicaoTEF();

            this.parent.EstadoReq = ReqEstado.Nenhum;
            this.parent.EstadoResp = RespEstado.Nenhum;
        }

        #endregion Constructor

        #region Propriedades

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic ativar gp].
        /// </summary>
        /// <value><c>true</c> if [automatic ativar gp]; otherwise, <c>false</c>.</value>
        public bool AutoAtivarGp { get; set; }

        /// <summary>
        /// Gets or sets the espera STS.
        /// </summary>
        /// <value>The espera STS.</value>
        public int EsperaSts { get; set; }

        /// <summary>
        /// Gets or sets the arq temporary.
        /// </summary>
        /// <value>The arq temporary.</value>
        public string ArqTemp { get; set; }

        /// <summary>
        /// Gets or sets the arq req.
        /// </summary>
        /// <value>The arq req.</value>
        public string ArqReq { get; set; }

        /// <summary>
        /// Gets or sets the arq STS.
        /// </summary>
        /// <value>The arq STS.</value>
        public string ArqSTS { get; set; }

        /// <summary>
        /// Gets or sets the arq resp.
        /// </summary>
        /// <value>The arq resp.</value>
        public string ArqResp { get; set; }

        /// <summary>
        /// Gets or sets the name of the gp executable.
        /// </summary>
        /// <value>The name of the gp executable.</value>
        public string GPExeName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TEFBase"/> is habilitado.
        /// </summary>
        /// <value><c>true</c> if habilitado; otherwise, <c>false</c>.</value>
        public bool Habilitado { get; set; }

        /// <summary>
        /// Gets the tipo.
        /// </summary>
        /// <value>The tipo.</value>
        public TEFTipo Tipo { get; protected set; }

        /// <summary>
        /// Gets the estado ecf.
        /// </summary>
        /// <value>The estado ecf.</value>
        public EstadoVenda Estado => parent.DoEstadoVenda();

        /// <summary>
        /// Gets or sets the number vias.
        /// </summary>
        /// <value>The number vias.</value>
        public int NumVias
        {
            get => parent.NumVias;
            protected set => parent.NumVias = value;
        }

        /// <summary>
        /// Gets the req.
        /// </summary>
        /// <value>The req.</value>
        public RequisicaoTEF Requisicao { get; protected set; }

        /// <summary>
        /// Gets the resp.
        /// </summary>
        /// <value>The resp.</value>
        public RetornoTEF Resposta
        {
            get => parent.Resposta;
            protected set => parent.Resposta = value;
        }

        /// <summary>
        /// Gets a value indicating whether [aguardando resposta].
        /// </summary>
        /// <value><c>true</c> if [aguardando resposta]; otherwise, <c>false</c>.</value>
        public bool AguardandoResposta { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TEFBase"/> is inicializado.
        /// </summary>
        /// <value><c>true</c> if inicializado; otherwise, <c>false</c>.</value>
        public bool Inicializado
        {
            get => inicializado;
            set
            {
                if (value)
                    Inicializar();
                else
                    Desinicializar();
            }
        }

        #endregion Propriedades

        #region Methods

        #region Protected

        /// <summary>
        /// Iniciars the requisicao.
        /// </summary>
        /// <param name="aHeader">a header.</param>
        /// <param name="aid">The aid.</param>
        /// <exception cref="ACBrException">Requisição anterior não concluida</exception>
        protected virtual void IniciarRequisicao(string aHeader, int aid = 0)
        {
            Guard.Against<ACBrException>(AguardandoResposta, "Requisição anterior não concluida");

            this.Log().InfoFormat("{0} IniciarRequisicao: {1}", Name, aHeader);

            // Verificando se a Classe de TEF está Inicializado
            VerificaInicializado();

            // Transação a ser enviada é pagamento ? (CRT ou CHQ)
            if (TransacaoEPagamento(aHeader))
            {
                //Para TEF DISCADO tradicional, se for MultiplosCartoes, precisa
                //confirma a transação anterior antes de enviar uma nova
                if (parent.MultiplosCartoes)    // É multiplos cartoes ?
                    ConfirmarTransacoesAnteriores();
            }

            //VisaNET exige um ATV antes de cada transação
            if (aHeader != "ATV")
                VerificaAtivo();

            parent.EstadoReq = ReqEstado.Iniciando;

            //Limpando da Memória os dados do Objeto de Requisicao e Resposta
            Requisicao.Clear();
            Resposta.Clear();

            ApagaEVerifica(ArqTemp);  // Apagando Arquivo Temporario anterior //
            ApagaEVerifica(ArqReq);  // Apagando Arquivo de Requisicao anterior //
            ApagaEVerifica(ArqResp);  // Apagando Arquivo de Resposta anterior //
            ApagaEVerifica(ArqSTS);  // Apagando Arquivo de Status anterior //

            if (aid > 0)
                idSeq = aid;
            else
                idSeq++;

            Requisicao.Header = aHeader;
            Requisicao.Id = idSeq;
            Requisicao.Conteudo.GravarArquivo(ArqTemp);
        }

        /// <summary>
        /// Adicionars the identificacao.
        /// </summary>
        protected virtual void AdicionarIdentificacao()
        {
            var temIdentificacao = false;

            var identificacao = $"{parent.Identificacao.NomeAplicacao} {parent.Identificacao.VersaoAplicacao}".Trim();
            if (!identificacao.IsEmpty())
            {
                Requisicao.GravarInformacao(identificacao, 701);
                temIdentificacao = true;
            }

            if (!parent.Identificacao.RegistroCertificacao.IsEmpty())
            {
                Requisicao.GravarInformacao(parent.Identificacao.RegistroCertificacao, 738);
                temIdentificacao = true;
            }

            if (!parent.Identificacao.RazaoSocial.IsEmpty())
            {
                Requisicao.GravarInformacao(parent.Identificacao.RazaoSocial, 716);
                temIdentificacao = true;
            }

            var operacoes = 0;

            if (parent.SuportaSaque) operacoes += 1;
            if (parent.SuportaDesconto) operacoes += 2;
            if (parent.SuportaReajusteValor) operacoes += 3;

            if (temIdentificacao && operacoes > 0)
                Requisicao.GravarInformacao(operacoes.ToString(), 706);
        }

        /// <summary>
        /// Finalizars the requisicao.
        /// </summary>
        /// <exception cref="ACBrException">
        /// Falha na comunicação com o Gereciador Padrão: + Name
        /// or
        /// </exception>
        protected virtual void FinalizarRequisicao()
        {
            VerificarIniciouRequisicao();
            parent.EstadoReq = ReqEstado.CriandoArquivo;
            parent.DoOnAntesFinalizarRequisicao();

            this.Log().InfoFormat("{0} FinalizarRequisicao: {1}, Fechando arquivo: {2}", Name, Requisicao.Header, ArqTemp);

            Requisicao.GravarInformacao("0", 999, 999);
            Requisicao.Conteudo.GravarArquivo(ArqTemp);

            if (this.Log().IsDebugEnabled)
            {
                this.Log().Info(Resposta.Conteudo.Conteudo.AsString());
            }

            this.Log().InfoFormat("{0} FinalizarRequisicao: {1}, Renomeando: {2} para: {3}", Name, Requisicao.Header, ArqTemp, ArqReq);

            try
            {
                File.Move(ArqTemp, ArqReq);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Erro ao Renomear:{0}{1}para:{0}{2}", Environment.NewLine, ArqTemp, ArqReq);
                throw new ACBrException(msg, ex);
            }

            parent.EstadoReq = ReqEstado.AguardandoResposta;
            var tempoFimEspera = DateTime.Now.AddSeconds(parent.EsperaSts);
            var interromper = false;
            AguardandoResposta = true;
            try
            {
                this.Log().InfoFormat("{0} FinalizarRequisicao: {1}, Aguardando: {2}", Name, Requisicao.Header, parent.EsperaSts);
                do
                {
                    Thread.Sleep(parent.EsperaSleep);
                    var e = new AguardaRespEventArgs(ArqSTS, DateTime.Now.Subtract(tempoFimEspera).Seconds);
                    parent.DoOnAguardaResp(e);
                    interromper = e.Interromper;
                } while (!File.Exists(ArqSTS) && DateTime.Now > tempoFimEspera && !interromper);
            }
            finally
            {
                AguardandoResposta = false;
                var e = new AguardaRespEventArgs(ArqSTS, -1)
                {
                    Interromper = interromper
                };
                parent.DoOnAguardaResp(e);
            }

            this.Log().InfoFormat("{0} FinalizarRequisicao: {1}, Fim da Espera de: {2} {3}", Name, Requisicao.Header,
                File.Exists(ArqSTS) ? "Recebido" : "Não recebido");

            Guard.Against<ACBrTEFGPNaoRespondeException>(!File.Exists(ArqSTS), ACBrTEF.CacbrTefdErroNaoAtivo, Name);

            parent.EstadoReq = ReqEstado.ConferindoResposta;
            this.Log().InfoFormat("{0} FinalizarRequisicao: {1}, Verificando conteudo de: {2}", Name, Requisicao.Header, ArqSTS);

            Resposta.LeArquivo(ArqSTS);
            ApagaEVerifica(ArqTemp);
            ApagaEVerifica(ArqSTS);

            if (!VerificarRespostaRequisicao())
            {
                ApagaEVerifica(ArqResp);
                Requisicao.Clear();
                Resposta.Clear();
                throw new ACBrException("Falha na comunicação com o Gereciador Padrão:" + Name);
            }

            parent.EstadoReq = ReqEstado.Finalizada;
        }

        /// <summary>
        /// Verificars the resposta requisicao.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool VerificarRespostaRequisicao()
        {
            var resultado = true;
            if (Resposta is RetornoTEFTxt resp)
            {
                resultado = Resposta.Header == Requisicao.Header && resp.Id == Requisicao.Id && resp.TrailerOk;
            }

            return resultado;
        }

        /// <summary>
        /// Lers the resposta requisicao.
        /// </summary>
        protected virtual void LerRespostaRequisicao()
        {
            VerificarIniciouRequisicao();
            Resposta.Clear();

            if (Requisicao.Header.IsIn("CNF", "ATV"))
            {
                if (parent.TecladoBloqueado)
                {
                    parent.BloquearMouseTeclado(false);
                }
            }

            parent.EstadoResp = RespEstado.AguardandoResposta;
            var interromper = false;
            var ok = false;

            try
            {
                while (!ok && !interromper)
                {
                    var tempoInicioEspera = DateTime.Now;
                    AguardandoResposta = true;

                    try
                    {
                        this.Log().InfoFormat("{0} LerRespostaRequisicao: {1}, Aguardando: {2}", Name, Requisicao.Header, ArqResp);

                        do
                        {
                            Thread.Sleep(parent.EsperaSleep); // Necessário Para não sobrecarregar a CPU //
                            if (!parent.EventAssigned(nameof(parent.OnAguardaResp))) continue;

                            var e = new AguardaRespEventArgs(ArqSTS, DateTime.Now.Subtract(tempoInicioEspera).Seconds);
                            parent.DoOnAguardaResp(e);
                            interromper = e.Interromper;
                        } while (!File.Exists(ArqResp) && !interromper);

                        this.Log().InfoFormat("{0} LerRespostaRequisicao: {1}, Fim da Espera de: {2}", Name, Requisicao.Header,
                            File.Exists(ArqResp) ? "Recebido" : "Não recebido");
                    }
                    finally
                    {
                        AguardandoResposta = false;
                        if (parent.EventAssigned(nameof(parent.OnAguardaResp)))
                        {
                            var e = new AguardaRespEventArgs(ArqSTS, -1) { Interromper = interromper };
                            parent.DoOnAguardaResp(e);
                        }
                    }

                    this.Log().InfoFormat("{0} LerRespostaRequisicao: {1}, Verificando conteudo de: {2}", Name, Requisicao.Header, ArqResp);

                    Resposta.LeArquivo(ArqResp);
                    ok = VerificarRespostaRequisicao();

                    if (!ok)
                    {
                        this.Log().InfoFormat("{0} LerRespostaRequisicao: {1}, Arquivo inválido desprezado: {2}{3}{4}",
                            Name, Requisicao.Header, ArqResp, Environment.NewLine, Resposta.Conteudo.Conteudo.AsString());
                        Resposta.Clear();
                        UtilTEF.DeleteFile(ArqResp);
                    }

                    if (this.Log().IsDebugEnabled)
                        this.Log().Debug(Resposta.Conteudo.AsString());
                }
            }
            finally
            {
                Resposta.TipoGP = Tipo;
                parent.EstadoResp = RespEstado.Nenhum;
                UtilTEF.DeleteFile(ArqReq); // Apaga a Requisicao (caso o G.P. nao tenha apagado)
            }
        }

        /// <summary>
        /// Finalizars the resposta.
        /// </summary>
        /// <param name="apagarArqResp">if set to <c>true</c> [apagar arq resp].</param>
        protected virtual void FinalizarResposta(bool apagarArqResp)
        {
            parent.EstadoResp = RespEstado.Concluida;
            this.Log().InfoFormat("{0} FinalizarResposta: {1}", Name, Requisicao.Header);

            if (apagarArqResp)
                ApagaEVerifica(ArqResp);

            Requisicao.Clear();
            Resposta.Clear();
        }

        /// <summary>
        /// Criars the resposta.
        /// </summary>
        /// <returns>Resp.</returns>
        protected RetornoTEF CriarResposta(TEFTipo tipo)
        {
            switch (tipo)
            {
                case TEFTipo.CliSiTef: return new RetornoCliSiTef();
                default: return new RetornoTEFTxt(tipo);
            }
        }

        /// <summary>
        /// Copiars the resposta.
        /// </summary>
        /// <returns>System.String.</returns>
        protected virtual string CopiarResposta()
        {
            var i = 0;
            string file;
            do
            {
                i++;
                file = Path.Combine(parent.PathBackup, $@"ACBr_{Name}_{i:000}.tef");
            } while (File.Exists(file));

            this.Log().InfoFormat("{0} CopiarResposta: {1} - {2} Arq: {3}", Name, Resposta.Header, Resposta.Id, file);

            Resposta.Conteudo.GravarArquivo(file);
            Resposta.ArqBackup = file;
            return file;
        }

        /// <summary>
        /// Processars the resposta.
        /// </summary>
        protected virtual void ProcessarResposta()
        {
            VerificarIniciouRequisicao();
            this.Log().InfoFormat("{0} ProcessarResposta: {1}", Name, Requisicao.Header);
            parent.EstadoResp = RespEstado.Processando;

            if (Resposta.QtdLinhasComprovante > 0)
            {
                try
                {
                    var respostaPendente = Resposta.Clone();
                    parent.RespostasPendentes.Add(respostaPendente);
                    ImprimirRelatorio();
                    parent.DoOnDepoisConfirmarTransacoes();
                }
                finally
                {
                    parent.RespostasPendentes.Clear();
                }
            }
            else
            {
                if (Resposta.TextoEspecialOperador.IsEmpty()) return;
                parent.DoExibeMsg(OperacaoMensagem.OK, Resposta.TextoEspecialOperador);
            }
        }

        /// <summary>
        /// Processars the resposta pagamento.
        /// </summary>
        /// <param name="indicePagamento">The indice fp g_ ecf.</param>
        /// <param name="valor">The valor.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool ProcessarRespostaPagamento(string indicePagamento, decimal valor)
        {
            LerRespostaRequisicao();
            this.Log().InfoFormat("{0} ProcessarRespostaPagamento: {1} Indice:{2} Valor: {3:c}", Name, Resposta.Header,
                indicePagamento, valor);

            var ret = Resposta.TransacaoAprovada;
            var ultimaTransacao = valor >= parent.RespostasPendentes.SaldoRestante;

            //Se a transação não foi aprovada, faz tratamento e sai
            if (!Resposta.TransacaoAprovada)
            {
                ProcessarResposta(); //Exibe a Mensagem ao Operador
                FinalizarResposta(true); //True = Apaga Arquivo de Resposta

                //Ja tem RespostasPendentes ?
                if (!ultimaTransacao || parent.RespostasPendentes.Count <= 0) return ret;
                if (parent.DoExibeMsg(OperacaoMensagem.YesNo, ACBrTEF.CacbrTefdErroOutraFormaPagamento) ==
                    ModalResult.Yes) return ret;

                parent.DoComandaVenda(OperacaoVenda.CancelaCupom);
                parent.CancelarTransacoesPendentes();

                return ret;
            }

            //...Se está aqui, então a Transação foi aprovada...
            Resposta.IndicePagamento = indicePagamento;

            //Cria Arquivo de Backup, contendo inclusive informações internas como :
            //899 - 001 : CNFEnviado (S, N)
            //899 - 002 : IndiceFPG_ECF : String
            //899 - 003 : OrdemPagamento : Integer
            CopiarResposta();

            //Cria cópia do Objeto Resp, e salva no ObjectList "RespostasPendentes"
            var respostaPendete = Resposta.Clone();
            respostaPendete.ArqRespPendente = ArqResp;
            respostaPendete.ViaClienteReduzida = parent.ImprimirViaClienteReduzida;
            parent.RespostasPendentes.Add(respostaPendete);

            return ret;
        }

        /// <summary>
        /// Verificars the iniciou requisicao.
        /// </summary>
        /// <exception cref="ACBrException"></exception>
        protected virtual void VerificarIniciouRequisicao()
        {
            Guard.Against<ACBrException>(Requisicao.Header.IsEmpty(), ACBrTEF.CacbrTefdErroSemRequisicao);
        }

        /// <summary>
        /// Imprimir o relatorio.
        /// </summary>
        protected virtual void ImprimirRelatorio()
        {
            VerificarIniciouRequisicao();

            if (Resposta.QtdLinhasComprovante < 1) return;

            this.Log().InfoFormat("{0} IniciarRequisicao: {1}", Name, Resposta.Header);

            if (salvarArquivoBackup)
            {
                CopiarResposta();
            }

            var impressaoOk = false;
            var removeMsg = false;
            var inicio = DateTime.Now;

            try
            {
                parent.BloquearMouseTeclado();

                while (!impressaoOk)
                {
                    try
                    {
                        var estado = Estado;
                        if (estado != EstadoVenda.Livre)
                        {
                            switch (estado)
                            {
                                case EstadoVenda.CupomVinculado:
                                    parent.DoComandaVenda(OperacaoVenda.FechaVinculado);
                                    break;

                                case EstadoVenda.RelatorioGerencial:
                                    parent.DoComandaVenda(OperacaoVenda.FechaGerencial);
                                    break;

                                case EstadoVenda.Venda:
                                case EstadoVenda.Pagamento:
                                case EstadoVenda.NaoFiscal:
                                    parent.DoComandaVenda(OperacaoVenda.CancelaCupom);
                                    break;
                            }

                            Guard.Against<ACBrTEFPrintException>(Estado != EstadoVenda.Livre, ACBrTEF.CacbrTefdErroEcfNaoLivre);
                        }

                        var gerencialAberto = false;
                        inicio = DateTime.Now;

                        if (!Resposta.TextoEspecialOperador.IsEmpty())
                        {
                            removeMsg = true;
                            parent.DoExibeMsg(OperacaoMensagem.ExibirMsgOperador, Resposta.TextoEspecialOperador);
                        }

                        if (!Resposta.TextoEspecialCliente.IsEmpty())
                        {
                            removeMsg = true;
                            parent.DoExibeMsg(OperacaoMensagem.ExibirMsgCliente, Resposta.TextoEspecialCliente);
                        }

                        for (var i = 1; i <= NumVias; i++)
                        {
                            var imagemComprovante = i == 1 ? Resposta.ImagemComprovante1aVia : Resposta.ImagemComprovante2aVia;
                            if (!imagemComprovante.Any()) continue;

                            if (!gerencialAberto)
                            {
                                parent.DoComandaVenda(OperacaoVenda.AbreGerencial);
                                gerencialAberto = true;
                            }
                            else
                            {
                                if (i != 1)
                                {
                                    parent.DoComandaVenda(OperacaoVenda.PulaLinhas);
                                }

                                var msg = string.Format(ACBrTEF.CacbrTefdDestaqueVia, i);
                                parent.DoExibeMsg(OperacaoMensagem.DestaqueVia, msg);
                            }

                            parent.DoVendaImprimeVia(TipoRelatorio.Gerencial, i, imagemComprovante.ToArray());
                        }

                        if (gerencialAberto)
                        {
                            parent.DoComandaVenda(OperacaoVenda.FechaGerencial);
                        }

                        impressaoOk = true;
                    }
                    catch (ACBrTEFPrintException)
                    {
                        impressaoOk = false;
                    }
                    finally
                    {
                        if (removeMsg)
                        {
                            // Verifica se Mensagem Ficou pelo menos por 5 segundos
                            if (impressaoOk)
                            {
                                while (DateTime.Now.Subtract(inicio).Seconds < 5)
                                    Thread.Sleep(parent.EsperaSleep);
                            }

                            parent.DoExibeMsg(OperacaoMensagem.ExibirMsgOperador);
                            parent.DoExibeMsg(OperacaoMensagem.ExibirMsgCliente);
                        }
                    }

                    if (impressaoOk) continue;
                    if (parent.DoExibeMsg(OperacaoMensagem.YesNo, ACBrTEF.CacbrTefdErroEcfNaoResponde) != ModalResult.Yes) break;
                }
            }
            finally
            {
                var arqBackup = Resposta.ArqBackup;
                while (File.Exists(arqBackup))
                {
                    try
                    {
                        if (impressaoOk)
                            CNF();
                        else
                            NCN();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }

                    UtilTEF.DeleteFile(arqBackup);
                }

                parent.BloquearMouseTeclado(false);
            }

            Guard.Against<ACBrTEFPrintException>(!impressaoOk, "Impressão de Relatório Falhou");
        }

        /// <summary>
        /// Confirmars the e solicitar impressao transacoes pendentes.
        /// </summary>
        protected virtual void ConfirmarESolicitarImpressaoTransacoesPendentes()
        {
            var arquivosVerficar = new List<string>();

            parent.RespostasPendentes.Clear();

            // Achando Arquivos de Backup deste GP
            arquivosVerficar.AddRange(Directory.EnumerateFiles(parent.PathBackup, "ACBr_" + Name + "_*.tef"));
            var nsUs = string.Empty;
            var exibeMsg = arquivosVerficar.Any();

            while (arquivosVerficar.Count > 0)
            {
                try
                {
                    if (!File.Exists(arquivosVerficar[0]))
                    {
                        arquivosVerficar.RemoveAt(0);
                        continue;
                    }

                    Resposta.LeArquivo(arquivosVerficar[0]);

                    CNF();

                    var respostaConfirmada = Resposta.Clone();
                    parent.RespostasPendentes.Add(respostaConfirmada);

                    if (!Resposta.NSU.IsEmpty())
                        nsUs += $"NSU: {Resposta.NSU}{Environment.NewLine}";

                    UtilTEF.DeleteFile(arquivosVerficar[0]);
                    arquivosVerficar.RemoveAt(0);
                }
                catch (Exception)
                {
                    //
                }
            }

            parent.DoOnDepoisConfirmarTransacoes();
            parent.RespostasPendentes.Clear();

            if (exibeMsg)
            {
                parent.DoExibeMsg(OperacaoMensagem.OK,
                    ACBrTEF.CacbrTefdCliSiTefTransacaoEfetuadaReImprimir.Substitute(nsUs));
            }
        }

        /// <summary>
        /// Verificars the transacao pagamento.
        /// </summary>
        /// <param name="valor">The valor.</param>
        protected virtual void VerificarTransacaoPagamento(decimal valor)
        {
            valor = valor.RoundABNT();
            Guard.Against<ACBrException>(valor < 0, "Valor inválido");

            if (!parent.IsDFe)
            {
                Guard.Against<ACBrException>(!Estado.IsIn(EstadoVenda.Venda, EstadoVenda.Pagamento, EstadoVenda.NaoFiscal) &&
                                             !parent.IsDFe, "ECF deve estar em Estado de \"Venda\", \"Pagamento\" ou \"Não Fiscal\"");

                var saldoAPagar = parent.DoOnInfoVendaAsDecimal(InfoVenda.SubTotal);
                saldoAPagar -= parent.DoOnInfoVendaAsDecimal(InfoVenda.TotalAPagar);
                parent.RespostasPendentes.SaldoAPagar = saldoAPagar;
                parent.RespostasPendentes.SaldoRestante = valor;

                parent.RespostasPendentes.SaldoRestante = valor;

                if (parent.TrocoMaximo <= 0)
                {
                    Guard.Against<ACBrException>(valor > parent.RespostasPendentes.SaldoRestante,
                        "Operação TEF deve ser limitada a Saldo restante a Pagar");
                }
                else
                {
                    Guard.Against<ACBrException>(valor > parent.RespostasPendentes.SaldoRestante + parent.TrocoMaximo,
                        "Operação TEF permite Troco Máximo de {0:c}", parent.TrocoMaximo);
                }
            }

            Guard.Against<ACBrException>(parent.MultiplosCartoes && parent.NumeroMaximoCartoes > 0 &&
                                         parent.RespostasPendentes.Count >= parent.NumeroMaximoCartoes,
                                         "Multiplos Cartões Limitado a {0}", parent.NumeroMaximoCartoes);
            if (this is TEFTxt)
            {
                // Tem multiplos Cartoes ?
                // Valor é diferente do Saldo Restante a Pagar ?
                // Está no último cartão ?
                Guard.Against<ACBrException>(parent.MultiplosCartoes && parent.NumeroMaximoCartoes > 0 &&
                                            valor != parent.RespostasPendentes.SaldoRestante &&
                                            parent.NumeroMaximoCartoes - parent.RespostasPendentes.Count <= 1,
                    "Multiplos Cartões Limitado a {0}.{1}Esta Operação TEF deve ser igual ao Saldo a Pagar.",
                    parent.NumeroMaximoCartoes, Environment.NewLine);
            }
        }

        /// <summary>
        /// Transacaoes the e pagamento.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool TransacaoEPagamento(string header)
        {
            return header.IsIn("CRT", "CHQ");
        }

        #endregion Protected

        #region Public

        /// <summary>
        /// Verificas the inicializado.
        /// </summary>
        /// <exception cref="ACBrException"></exception>
        public void VerificaInicializado()
        {
            Guard.Against<ACBrException>(!inicializado, "Gerenciador Padrão: {0} não foi inicializado", Name);
        }

        /// <summary>
        /// Inicializars this instance.
        /// </summary>
        public virtual void Inicializar()
        {
            if (Inicializado)
                return;

            ApagaEVerifica(ArqTemp);  // Apagando Arquivo Temporario anterior //
            ApagaEVerifica(ArqReq);   // Apagando Arquivo de Requisicao anterior //
            ApagaEVerifica(ArqSTS);   // Apagando Arquivo de Status anterior //

            inicializado = true;
            this.Log().InfoFormat("{0} Inicializado", Name);

            // Verificando se o arquivo de Resposta é invalido ou seja, gerado quando
            //clica-se em 9 - CANCELAR sem selecionar nenhuma Bandeira }
            if (File.Exists(ArqResp))
            {
                Resposta.LeArquivo(ArqResp);
                var info = Resposta.LeInformacao(9).AsString().ToUpper();

                // Amex retorna 101 e não FF
                if ("|101|FF|".Contains(info))
                    ApagaEVerifica(ArqResp);

                Resposta.Clear();
            }

            VerificarTransacoesPendentes(parent.ConfirmarAntesDosComprovantes);
            VerificaAtivo();
        }

        /// <summary>
        /// DESs the inicializar.
        /// </summary>
        public virtual void Desinicializar()
        {
            inicializado = false;
            this.Log().InfoFormat("{0} Desinicializado", Name);
        }

        /// <summary>
        /// Ativars the gp.
        /// </summary>
        /// <exception cref="ACBrException">Nome do executável do Gerenciador Padrão não definido</exception>
        public virtual void AtivarGP()
        {
            Guard.Against<ACBrException>(GPExeName.IsEmpty(), "Nome do executável do Gerenciador Padrão não definido");

            VerificaInicializado();

            Process.Start(GPExeName);
            Thread.Sleep(2000);
            parent.RestaurarFocoAplicacao();
        }

        /// <summary>
        /// Verificas the ativo.
        /// </summary>
        public virtual void VerificaAtivo()
        {
            try
            {
                ATV();
            }
            catch (ACBrTEFGPNaoRespondeException)
            {
                if (AutoAtivarGp)
                {
                    const string msg = "O Gerenciador Padrão não está ativo e será ativado automaticamente!";
                    parent.DoExibeMsg(OperacaoMensagem.OK, msg);
                    AtivarGP();
                    ATV();
                }
            }
            catch (Exception e)
            {
                this.Log().ErrorFormat("Erro ao verificar GP Ativo.{0}{1}", Environment.NewLine, e);
                throw;
            }
        }

        public virtual void VerificarTransacoesPendentes(bool aVerificarCupom)
        {
            if (!aVerificarCupom)
            {
                CancelarTransacoesPendentes();
                return;
            }

            EstadoVenda estado;

            try
            {
                estado = Estado;
            }
            catch (Exception)
            {
                estado = EstadoVenda.Outro;
                // Se o ECF estiver desligado, será retornado 'EstadoVenda.Outro', o que fará o código
                // abaixo Cancelar Todas as Transações Pendentes, porém, pelo Roteiro do
                // TEF dedicado, é necessário confirmar a Transação se o Cupom foi
                // finalizado com sucesso.
                // Criar um arquivo de Status que seja atualizado no Fim do Cupom e no
                // inicio do CCD, de maneira que seja possível identificar o Status do
                // Documento no ECF indepentende do mesmo estar ou não ligado
                // Como alteranativa, é possível implementar código no Evento "OnInfoVenda"
                // para buscar o Status do Documento no Banco de dados da sua aplicação, e
                // responder diferente de 'EstadoVenda.Outro',   (Veja exemplo nos fontes do TEFDDemo) }
            }

            parent.GpAtual = Tipo;

            // Cupom Ficou aberto?? ...Se SIM, Cancele tudo... //
            // NAO, Cupom Fechado, Pode confirmar e Mandar aviso para re-imprimir //
            if (estado.IsIn(EstadoVenda.Venda, EstadoVenda.Pagamento, EstadoVenda.NaoFiscal, EstadoVenda.Outro))
                CancelarTransacoesPendentes();
            else
                ConfirmarESolicitarImpressaoTransacoesPendentes();
        }

        /// <summary>
        /// Cancelars the transacoes pendentes class.
        /// </summary>
        /// <exception cref="ACBrException">
        /// CNC nao efetuado
        /// or
        /// Erro ao cancelar transações
        /// </exception>
        public virtual void CancelarTransacoesPendentes()
        {
            this.Log().InfoFormat("{0} ProcessarResposta: {1}", Name, Requisicao.Header);

            var arquivosVerificar = new List<string>();
            var respostasCanceladas = new List<RetornoTEF>();

            //Achando Arquivos de Backup deste GP
            var searchPattern = $"ACBr_{Name}_*.tef";
            arquivosVerificar.AddRange(Directory.GetFiles(parent.PathBackup, searchPattern));

            //Vamos processar primeiro os CNCs e ADMs, e as Não Confirmadas
            arquivosVerificar = arquivosVerificar.OrderBy(x =>
            {
                var resp = CriarResposta(Tipo);
                resp.LeArquivo(x);
                return resp.Header.IsIn("CNC", "ADM") || resp.CNFEnviado;
            }).ToList();

            //Adicionando Arquivo de Resposta deste GP (se ainda não foi apagado)
            if (File.Exists(ArqResp)) arquivosVerificar.Add(ArqResp);

            while (arquivosVerificar.Count > 0)
            {
                if (!File.Exists(arquivosVerificar[0]))
                {
                    arquivosVerificar.RemoveAt(0);
                    continue;
                }

                Resposta.LeArquivo(arquivosVerificar[0]);

                //Verificando se essa Resposta já foi cancela em outro arquivo
                var jaCancelado = respostasCanceladas.Exists(x => x.Rede == Resposta.Rede &&
                                                                  x.NSU == Resposta.NSU &&
                                                                  x.Finalizacao == Resposta.Finalizacao &&
                                                                  x.ValorTotal == Resposta.ValorTotal);

                if (jaCancelado)
                {
                    UtilTEF.DeleteFile(arquivosVerificar[0]);
                    arquivosVerificar.RemoveAt(0);
                    continue;
                }

                //Criando cópia da Resposta Atual
                var respostaCancelada = Resposta.Clone();

                //Enviando NCN ou CNC

                try
                {
                    if (parent.EventAssigned(nameof(parent.OnAntesCancelarTransacao)))
                        parent.DoOnAntesCancelarTransacao(respostaCancelada);
                }
                catch (Exception e)
                {
                    //Nao deixa exceptions em OnAntesCancelarTransacao interromper
                    this.Log().Error(e);
                }

                if (Resposta.CNFEnviado && Resposta.Header != "CHQ")
                    Guard.Against<ACBrException>(!CNC(), "CNC nao efetuado");
                else
                    NCN();

                UtilTEF.DeleteFile(arquivosVerificar[0]);
                arquivosVerificar.RemoveAt(0);

                //Adicionando na lista de Respostas Canceladas
                respostasCanceladas.Add(respostaCancelada);
            }

            if (parent.EventAssigned(nameof(parent.OnDepoisCancelarTransacoes)))
                parent.DoOnDepoisCancelarTrasacoes();
        }

        /// <summary>
        /// Confirmars the transacoes anteriores.
        /// </summary>
        public virtual void ConfirmarTransacoesAnteriores()
        {
            this.Log().Info("ConfirmarTransacoesAnteriores");

            //Se for Multiplos Cartoes precisa confirmar a transação anterior antes de
            //enviar uma Nova

            foreach (var pendente in parent.RespostasPendentes
                .Where(x => !x.CNFEnviado && x.TipoGP == parent.GpAtual && TransacaoEPagamento(x.Header)))
            {
                CNF(pendente.Rede, pendente.NSU, pendente.Finalizacao, pendente.DocumentoVinculado);
                pendente.CNFEnviado = true;
                if (!pendente.ArqBackup.IsEmpty())
                {
                    pendente.Conteudo.GravarArquivo(pendente.ArqBackup);   //True = DoFlushToDisk
                }

                ApagaEVerifica(pendente.ArqRespPendente);
            }
        }

        /// <summary>
        /// Atvs this instance.
        /// </summary>
        public virtual void ATV()
        {
            IniciarRequisicao("ATV");
            FinalizarRequisicao();
        }

        /// <summary>
        /// Adms this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool ADM()
        {
            IniciarRequisicao("ADM");
            AdicionarIdentificacao();
            FinalizarRequisicao();

            LerRespostaRequisicao();

            try
            {
                ProcessarResposta();      //Faz a Impressão e / ou exibe Mensagem ao Operador
            }
            finally
            {
                FinalizarResposta(true); //True = Apaga Arquivo de Resposta
            }

            return Resposta.TransacaoAprovada;
        }

        /// <summary>
        /// CRTs the specified valor.
        /// </summary>
        /// <param name="valor">The valor.</param>
        /// <param name="indicePagamento">The indice fp g_ ecf.</param>
        /// <param name="documentoVinculado">The documento vinculado.</param>
        /// <param name="moeda">The moeda.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool CRT(decimal valor, string indicePagamento, string documentoVinculado, int moeda)
        {
            VerificarTransacaoPagamento(valor);
            IniciarRequisicao("CRT");
            Requisicao.DocumentoVinculado = documentoVinculado;
            Requisicao.ValorTotal = valor;
            Requisicao.Moeda = moeda;
            AdicionarIdentificacao();
            FinalizarRequisicao();

            var ret = ProcessarRespostaPagamento(indicePagamento, valor);
            return ret;
        }

        public virtual bool PRE(decimal valor, string indiceFpgEcf, string documentoVinculado, int moeda)
        {
            IniciarRequisicao("PRE");
            Requisicao.DocumentoVinculado = documentoVinculado;
            Requisicao.ValorTotal = valor;
            Requisicao.Moeda = moeda;
            AdicionarIdentificacao();
            FinalizarRequisicao();
            LerRespostaRequisicao();

            try
            {
                ProcessarResposta();      //Faz a Impressão e / ou exibe Mensagem ao Operador
            }
            finally
            {
                FinalizarResposta(true); //True = Apaga Arquivo de Resposta
            }

            return Resposta.TransacaoAprovada;
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
        public virtual bool CHQ(decimal valor, string indicePagamento, string documentoVinculado, string cmc7,
            char tipoPessoa, string documentoPessoa, DateTime? dataCheque, string banco, string agencia,
            string agenciaDc, string conta, string contaDc, string cheque, string chequeDc, string compensacao)
        {
            VerificarTransacaoPagamento(valor);
            IniciarRequisicao("CHQ");
            Requisicao.DocumentoVinculado = documentoVinculado;
            Requisicao.ValorTotal = valor;
            Requisicao.CMC7 = cmc7;
            if (!tipoPessoa.IsEmpty()) Requisicao.TipoPessoa = tipoPessoa;
            Requisicao.DocumentoPessoa = documentoPessoa;
            Requisicao.DataCheque = dataCheque;
            Requisicao.Banco = banco;
            Requisicao.Agencia = agencia;
            Requisicao.AgenciaDC = agenciaDc;
            Requisicao.Conta = conta;
            Requisicao.ContaDC = contaDc;
            Requisicao.Cheque = cheque;
            Requisicao.ChequeDC = chequeDc;
            Requisicao.Moeda = 0;            // Moeda 0 = Real
            AdicionarIdentificacao();
            FinalizarRequisicao();

            LerRespostaRequisicao();

            bool ret;

            try
            {
                if (Resposta.QtdLinhasComprovante <= 0 && Resposta.StatusTransacao == "0")
                    ProcessarResposta(); //Faz a Impressão e/ou exibe Mensagem ao Operador
            }
            finally
            {
                ret = ProcessarRespostaPagamento(indicePagamento, valor);
            }

            return ret;
        }

        /// <summary>
        /// CNCs this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool CNC()
        {
            //Salvando dados da Resposta Atual
            var oldResp = Resposta.Clone();
            IniciarRequisicao("CNC");
            Requisicao.DocumentoVinculado = oldResp.DocumentoVinculado;
            Requisicao.ValorTotal = oldResp.ValorTotal;
            Requisicao.CMC7 = oldResp.CMC7;
            if (!oldResp.TipoPessoa.IsEmpty()) Requisicao.TipoPessoa = oldResp.TipoPessoa;
            Requisicao.DocumentoPessoa = oldResp.DocumentoPessoa;
            Requisicao.DataCheque = oldResp.DataCheque.GetValueOrDefault();
            Requisicao.Rede = oldResp.Rede;
            Requisicao.NSU = oldResp.NSU;
            Requisicao.DataHoraTransacaoComprovante = oldResp.DataHoraTransacaoComprovante.GetValueOrDefault();
            Requisicao.Banco = oldResp.Banco;
            Requisicao.Agencia = oldResp.Agencia;
            Requisicao.AgenciaDC = oldResp.AgenciaDC;
            Requisicao.Conta = oldResp.Conta;
            Requisicao.ContaDC = oldResp.ContaDC;
            Requisicao.Cheque = oldResp.Cheque;
            Requisicao.ChequeDC = oldResp.ChequeDC;
            Requisicao.Moeda = oldResp.Moeda;
            AdicionarIdentificacao();
            FinalizarRequisicao();

            LerRespostaRequisicao();
            var ret = Resposta.TransacaoAprovada;

            try
            {
                ProcessarResposta(); //Faz a Impressão e/ou exibe Mensagem ao Operador
            }
            finally
            {
                FinalizarResposta(true); //True = Apaga Arquivo de Resposta
            }

            return ret;
        }

        /// <summary>
        /// CNCs the specified rede.
        /// </summary>
        /// <param name="rede">The rede.</param>
        /// <param name="nsu">The nsu.</param>
        /// <param name="dataHoraTransacao">The data hora transacao.</param>
        /// <param name="valor">The valor.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool CNC(string rede, string nsu, DateTime dataHoraTransacao, decimal valor)
        {
            IniciarRequisicao("CNC");
            Requisicao.ValorTotal = valor;
            Requisicao.Rede = rede;
            Requisicao.NSU = nsu;
            Requisicao.DataHoraTransacaoComprovante = dataHoraTransacao;
            AdicionarIdentificacao();
            FinalizarRequisicao();

            LerRespostaRequisicao();
            var ret = Resposta.TransacaoAprovada;
            try
            {
                ProcessarResposta(); //Faz a Impressão e/ou exibe Mensagem ao Operador
            }
            finally
            {
                FinalizarResposta(true); //True = Apaga Arquivo de Resposta
            }

            return ret;
        }

        /// <summary>
        /// CNFs this instance.
        /// </summary>
        public void CNF()
        {
            CNF(Resposta.Rede, Resposta.NSU, Resposta.Finalizacao, Resposta.DocumentoVinculado);
        }

        /// <summary>
        /// CNFs the specified rede.
        /// </summary>
        /// <param name="rede">The rede.</param>
        /// <param name="nsu">The nsu.</param>
        /// <param name="finalizacao">The finalizacao.</param>
        /// <param name="documentoVinculado">The documento vinculado.</param>
        public virtual void CNF(string rede, string nsu, string finalizacao, string documentoVinculado)
        {
            IniciarRequisicao("CNF");
            Requisicao.DocumentoVinculado = documentoVinculado;
            Requisicao.Rede = rede;
            Requisicao.NSU = nsu;
            Requisicao.Finalizacao = finalizacao;
            FinalizarRequisicao();
        }

        /// <summary>
        /// NCNs this instance.
        /// </summary>
        public void NCN()
        {
            NCN(Resposta.Rede, Resposta.NSU, Resposta.Finalizacao, Resposta.ValorTotal, Resposta.DocumentoVinculado);
        }

        /// <summary>
        /// NCNs the specified rede.
        /// </summary>
        /// <param name="rede">The rede.</param>
        /// <param name="nsu">The nsu.</param>
        /// <param name="finalizacao">The finalizacao.</param>
        /// <param name="valor">The valor.</param>
        /// <param name="documentoVinculado">The documento vinculado.</param>
        public virtual void NCN(string rede, string nsu, string finalizacao, decimal valor, string documentoVinculado)
        {
            IniciarRequisicao("NCN");
            Requisicao.DocumentoVinculado = documentoVinculado;
            Requisicao.Rede = rede;
            Requisicao.NSU = nsu;
            Requisicao.Finalizacao = finalizacao;
            FinalizarRequisicao();

            var msgStr = new StringBuilder();
            msgStr.AppendLine("Última Transação TEF foi cancelada");
            msgStr.AppendLine("");
            msgStr.AppendLine("");

            if (!string.IsNullOrEmpty(rede))
                msgStr.AppendLine($"Rede: {rede}");

            if (!string.IsNullOrEmpty(nsu))
                msgStr.AppendLine($"NSU: {nsu}");

            if (valor > 0)
                msgStr.AppendLine($"Valor: {valor:C2}");

            var msg = msgStr.ToString();
            msg = msg.TrimEnd(Environment.NewLine.ToCharArray());
            parent.DoExibeMsg(OperacaoMensagem.OK, msg);
        }

        /// <summary>
        /// Apagas the e verifica.
        /// </summary>
        /// <param name="arquivo">The arquivo.</param>
        /// <exception cref="ACBrException">
        /// </exception>
        protected virtual void ApagaEVerifica(string arquivo)
        {
            if (arquivo.IsEmpty()) return;

            UtilTEF.DeleteFile(arquivo);
            Guard.Against<ACBrException>(File.Exists(arquivo), "Erro ao apagar o arquivo:{0}{1}", Environment.NewLine, arquivo);
        }

        #endregion Public

        #endregion Methods
    }
}