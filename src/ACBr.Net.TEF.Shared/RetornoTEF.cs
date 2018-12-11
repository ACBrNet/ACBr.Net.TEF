// ***********************************************************************
// Assembly         : ACBr.Net.TEFD
// Author           : RFTD
// Created          : 05-04-2014
//
// Last Modified By : RFTD
// Last Modified On : 02-18-2015
// ***********************************************************************
// <copyright file="RetornoTEF.cs" company="ACBr.Net">
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

using System;

namespace ACBr.Net.TEF
{
    /// <summary>
    /// Classe RespostaTEF.
    /// </summary>
    public abstract class RetornoTEF
    {
        #region Fields

        private string indicePagamento;
        private int ordemPagamento;
        private bool cnfEnviado;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RetornoTEF"/> class.
        /// </summary>
        internal RetornoTEF(TEFTipo tipo)
        {
            Conteudo = new TEFArquivo();
            ImagemComprovante1aVia = new TEFCollection<string>();
            ImagemComprovante2aVia = new TEFCollection<string>();
            Parcelas = new RespParcelaCollection();
            CorrespBancarios = new RespCBCollection();
            NFCeSAT = new RespNFCeSAT();
            TipoGP = tipo;

            Clear();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
		/// Retorna o conteudo da resposta.
		/// </summary>
        public TEFArquivo Conteudo { get; internal set; }

        /// <summary>
        /// Retorna o header do arquivo resposta.
        /// </summary>
        public string Header { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Retorna o documento vinculado.
        /// </summary>
        public string DocumentoVinculado { get; internal set; }

        /// <summary>
        /// Retorna o valor total.
        /// </summary>
        public decimal ValorTotal { get; internal set; }

        /// <summary>
        /// Retorna o valor original.
        /// </summary>
        public decimal ValorOriginal { get; internal set; }

        /// <summary>
		/// Retorna o valor do saque.
		/// </summary>
        public decimal Saque { get; internal set; }

        /// <summary>
		/// Retorna o valor do desconto.
		/// </summary>
        public decimal Desconto { get; internal set; }

        /// <summary>
		/// Retorna o valor da taxa de serviço.
		/// </summary>
        public decimal TaxaServico { get; internal set; }

        /// <summary>
        /// Retorna a identificação da moeda utilizada.
        /// </summary>
        public int Moeda { get; internal set; }

        /// <summary>
        /// Retorna o número CMC7 do cheque.
        /// </summary>
        public string CMC7 { get; internal set; }

        /// <summary>
        /// Retorna o tipo de pessoa.
        /// </summary>
        public char TipoPessoa { get; internal set; }

        /// <summary>
        /// Retorna o documento da pessoa.
        /// </summary>
        public string DocumentoPessoa { get; internal set; }

        /// <summary>
        /// Retorna retorna a data do cheque se houver.
        /// </summary>
        public DateTime? DataCheque { get; internal set; }

        /// <summary>
        /// Retorna retorna a rede.
        /// </summary>
        public string Rede { get; internal set; }

        /// <summary>
        /// Retorna o numero do NSU.
        /// </summary>
        public string NSU { get; internal set; }

        /// <summary>
        /// Retorna a campo de finalização.
        /// </summary>
        public string Finalizacao { get; internal set; }

        /// <summary>
        /// Retorna a situação da transação.
        /// </summary>
        public string StatusTransacao { get; internal set; }

        /// <summary>
        /// Retorna se a transação foi aprovada.
        /// </summary>
        public virtual bool TransacaoAprovada => true;

        /// <summary>
		/// Retorna o tipo da transação.
		/// </summary>
        public int TipoTransacao { get; internal set; }

        /// <summary>
		/// Retorna o codigo de autorização da transação.
		/// </summary>
        public string CodigoAutorizacaoTransacao { get; internal set; }

        /// <summary>
		/// Retorna o número do lote da transação.
		/// </summary>
        public int NumeroLoteTransacao { get; internal set; }

        /// <summary>
		/// Retorna a data e a hora do host da transação.
		/// </summary>
        public DateTime? DataHoraTransacaoHost { get; internal set; }

        /// <summary>
		/// Retorna a data e a hora local da transação.
		/// </summary>
        public DateTime? DataHoraTransacaoLocal { get; internal set; }

        /// <summary>
		/// Retorna o tipo de parcelamento.
		/// </summary>
        public int TipoParcelamento { get; internal set; }

        /// <summary>
		/// Retorna a quantidade de parcelas do parcelamento.
		/// </summary>
        public int QtdParcelas { get; internal set; }

        /// <summary>
		/// Retorna a data do cheque pré-datado.
		/// </summary>
        public DateTime? DataPreDatado { get; internal set; }

        /// <summary>
		/// Retorna o NSU da transação cancelada.
		/// </summary>
        public string NSUTransacaoCancelada { get; internal set; }

        /// <summary>
		/// Retorna a data e a hora da transação cancelada.
		/// </summary>
        public DateTime? DataHoraTransacaoCancelada { get; internal set; }

        /// <summary>
		/// Retorna a quantidade de linhas do comprovante.
		/// </summary>
        public int QtdLinhasComprovante { get; internal set; }

        /// <summary>
		/// Retorna o texto especial para o operador.
		/// </summary>
        public string TextoEspecialOperador { get; internal set; }

        /// <summary>
		/// Retorna o texto especial para o cliente.
		/// </summary>
        public string TextoEspecialCliente { get; internal set; }

        /// <summary>
		/// Retorna a autenticação.
		/// </summary>
        public string Autenticacao { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public string Banco { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public string Agencia { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public string AgenciaDC { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public string Conta { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public string ContaDC { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public string Cheque { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public string ChequeDC { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public string NomeAdministradora { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public DateTime? DataHoraTransacaoComprovante { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public string Trailer { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string BIN { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string CodigoBandeiraPadrao { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public RespCBCollection CorrespBancarios { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string CodigoOperadoraCelular { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string NomeOperadoraCelular { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public decimal ValorRecargaCelular { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string NumeroRecargaCelular { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public RespParcelaCollection Parcelas { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public TEFCollection<string> ImagemComprovante1aVia { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public TEFCollection<string> ImagemComprovante2aVia { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string ArqBackup { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string ArqRespPendente { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public TEFTipo TipoGP { get; internal set; }

        /// <summary>
        /// retorna
        /// </summary>
        public bool CNFEnviado
        {
            get => cnfEnviado;
            internal set
            {
                if (cnfEnviado == value) return;

                cnfEnviado = value;
                Conteudo.GravarInformacao(value, 899, 1);
            }
        }

        /// <summary>
        /// retorna
        /// </summary>
        public string IndicePagamento
        {
            get => indicePagamento;
            internal set
            {
                if (indicePagamento == value) return;

                indicePagamento = value;
                Conteudo.GravarInformacao(value, 899, 2);
            }
        }

        /// <summary>
        /// retorna
        /// </summary>
        public int OrdemPagamento
        {
            get => ordemPagamento;
            internal set
            {
                if (ordemPagamento == value) return;

                ordemPagamento = value;
                Conteudo.GravarInformacao(value, 899, 3);
            }
        }

        /// <summary>
        /// retorna
        /// </summary>
        public bool ViaClienteReduzida { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public DateTime? DataVencimento { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string Instituicao { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string ModalidadePagto { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string ModalidadePagtoDescrita { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string ModalidadeExtenso { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string CodigoRedeAutorizada { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public bool Debito { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public bool Credito { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public bool Digitado { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public RespParceladoPor ParceladoPor { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public decimal ValorEntradaCDC { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public DateTime? DataEntradaCDC { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public RespTipoOperacao TipoOperacao { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public RespNFCeSAT NFCeSAT { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public int IdPagamento { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public int IdRespostaFiscal { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string SerialPOS { get; internal set; }

        /// <summary>
		/// retorna
		/// </summary>
        public string Estabelecimento { get; internal set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Les the informacao.
        /// </summary>
        /// <param name="identificacao">The identificacao.</param>
        /// <param name="sequencia">The sequencia.</param>
        /// <returns>TEFLinha.</returns>
        public TEFLinha LeInformacao(int identificacao, int sequencia = 0)
        {
            return Conteudo.LeLinha(identificacao, sequencia);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Conteudo.Clear();
            Parcelas.Clear();
            CorrespBancarios.Clear();
            ImagemComprovante1aVia.Clear();
            ImagemComprovante2aVia.Clear();

            Header = string.Empty;
            Id = 0;
            Agencia = string.Empty;
            AgenciaDC = string.Empty;
            Autenticacao = string.Empty;
            Banco = string.Empty;
            Cheque = string.Empty;
            ChequeDC = string.Empty;
            CMC7 = string.Empty;
            CodigoAutorizacaoTransacao = string.Empty;
            Conta = string.Empty;
            ContaDC = string.Empty;
            DataCheque = null;
            DataHoraTransacaoCancelada = null;
            DataHoraTransacaoComprovante = null;
            DataHoraTransacaoHost = null;
            DataHoraTransacaoLocal = null;
            DataPreDatado = null;
            DocumentoPessoa = string.Empty;
            Finalizacao = string.Empty;
            Moeda = 0;
            NomeAdministradora = string.Empty;
            NSU = string.Empty;
            NSUTransacaoCancelada = string.Empty;
            NumeroLoteTransacao = 0;
            QtdLinhasComprovante = 0;
            QtdParcelas = 0;
            Rede = string.Empty;
            StatusTransacao = string.Empty;
            TextoEspecialCliente = string.Empty;
            TextoEspecialOperador = string.Empty;
            TipoPessoa = ' ';
            TipoTransacao = 0;
            Trailer = string.Empty;
            BIN = string.Empty;
            ValorTotal = 0;
            ValorOriginal = 0;
            Saque = 0;
            Desconto = 0;
            TaxaServico = 0;
            DocumentoVinculado = string.Empty;
            TipoParcelamento = 0;
            ValorEntradaCDC = 0;
            DataEntradaCDC = null;
            CodigoBandeiraPadrao = string.Empty;

            CodigoOperadoraCelular = string.Empty;
            NomeOperadoraCelular = string.Empty;
            NumeroRecargaCelular = string.Empty;
            ValorRecargaCelular = 0;

            ParceladoPor = RespParceladoPor.Nenhum;
            TipoOperacao = RespTipoOperacao.Outras;

            Credito = false;
            Debito = false;
            Digitado = false;

            CNFEnviado = false;
            IndicePagamento = string.Empty;
            OrdemPagamento = 0;

            ArqBackup = string.Empty;
            ArqRespPendente = string.Empty;
            ViaClienteReduzida = false;

            NFCeSAT.Clear();
            IdPagamento = 0;
            IdRespostaFiscal = 0;
            SerialPOS = string.Empty;
            Estabelecimento = string.Empty;
        }

        /// <summary>
        /// Les the arquivo.
        /// </summary>
        /// <param name="arquivo">The arquivo.</param>
        public void LeArquivo(string arquivo)
        {
            Clear();
            Conteudo.LerArquivo(arquivo);
            ConteudoToProperty();
        }

        /// <summary>
        /// Conteudoes to property.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        internal abstract void ConteudoToProperty();

        #endregion Methods
    }
}