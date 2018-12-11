// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 05-04-2014
//
// Last Modified By : RFTD
// Last Modified On : 02-18-2015
// ***********************************************************************
// <copyright file="RespTXT.cs" company="ACBr.Net">
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
using System.Linq;
using ACBr.Net.Core.Extensions;

namespace ACBr.Net.TEF
{
    /// <summary>
    /// Classe RespTxt.
    /// </summary>
    public sealed class RetornoTEFTxt : RetornoTEF
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RetornoTEFTxt"/> class.
        /// </summary>
        internal RetornoTEFTxt(TEFTipo tipo) : base(tipo)
        {
        }

        #endregion Constructor

        #region Properties

        public override bool TransacaoAprovada => StatusTransacao.ToInt32() == 0 || StatusTransacao == "P1";

        /// <summary>
        /// Gets a value indicating whether [trailer ok].
        /// </summary>
        /// <value><c>true</c> if [trailer ok]; otherwise, <c>false</c>.</value>
        public bool TrailerOk => !(string.IsNullOrEmpty(Trailer) || string.IsNullOrWhiteSpace(Trailer));

        #endregion Properties

        #region Methods

        /// <summary>
        /// Passa o conteudo do arquivo para as propriedades da classe
        /// </summary>
        internal override void ConteudoToProperty()
        {
            DataHoraTransacaoComprovante = null;
            ImagemComprovante1aVia.Clear();
            ImagemComprovante2aVia.Clear();
            var usar711 = false;
            var usar713 = false;
            var usar715 = false;
            var temParcela = false;
            var usar29 = false;

            foreach (var linha in Conteudo.OrderBy(x => x.Chave))
            {
                switch (linha.Identificacao)
                {
                    case 0: Header = linha.AsString(); break;
                    case 1: Id = linha.AsInt32(); break;
                    case 2: DocumentoVinculado = linha.AsString(); break;
                    case 3: ValorTotal = linha.AsDecimal(); break;
                    case 4: Moeda = linha.AsInt32(); break;
                    case 5: CMC7 = linha.AsString(); break;
                    case 6: TipoPessoa = linha.AsString()[0]; break;
                    case 7: DocumentoPessoa = linha.AsString(); break;
                    case 8: DataCheque = linha.AsDate(); break;
                    case 9: StatusTransacao = linha.AsString(); break;
                    case 10:
                        Rede = linha.AsString();
                        BIN = linha.AsString();
                        NFCeSAT.UltimosQuatroDigitos = linha.AsString();
                        break;

                    case 11: TipoTransacao = linha.AsInt32(); break;
                    case 12: NSU = linha.AsString(); break;
                    case 13: CodigoAutorizacaoTransacao = linha.AsString(); break;
                    case 14: NumeroLoteTransacao = linha.AsInt32(); break;
                    case 15: DataHoraTransacaoHost = linha.AsDateTime(); break;
                    case 16: DataHoraTransacaoLocal = linha.AsDateTime(); break;
                    case 17:
                        TipoParcelamento = linha.AsInt32();
                        switch (TipoParcelamento)
                        {
                            case 0:
                                ParceladoPor = RespParceladoPor.Loja;
                                break;

                            case 1:
                                ParceladoPor = RespParceladoPor.ADM;
                                break;

                            default:
                                ParceladoPor = RespParceladoPor.Nenhum;
                                break;
                        }
                        break;

                    case 18: QtdParcelas = linha.AsInt32(); break;
                    case 19: temParcela = true; break;
                    case 22: DataHoraTransacaoComprovante = linha.AsDate(); break;
                    case 23: DataHoraTransacaoComprovante = DataHoraTransacaoComprovante + linha.AsTime(); break;
                    case 24: DataPreDatado = linha.AsDate(); break;
                    case 25: NSUTransacaoCancelada = linha.AsString(); break;
                    case 26: DataHoraTransacaoCancelada = linha.AsDateTime(); break;
                    case 27: Finalizacao = linha.AsString(); break;
                    case 28:
                        if (!(usar711 || usar713))
                        {
                            ImagemComprovante1aVia.Clear();
                            QtdLinhasComprovante = linha.AsInt32();
                        }

                        if (!usar715)
                            ImagemComprovante2aVia.Clear();

                        if (linha.Sequencia == 1)
                        {
                            usar29 = true;
                            ImagemComprovante1aVia.Clear();
                            QtdLinhasComprovante = linha.AsInt32();
                            ImagemComprovante2aVia.Clear();
                        }

                        break;

                    case 29:
                        if (usar29)
                        {
                            if (linha.Sequencia <= QtdLinhasComprovante)
                                ImagemComprovante1aVia.Add(linha.AsLinhaComprovante());
                            else
                                ImagemComprovante2aVia.Add(linha.AsLinhaComprovante());
                        }
                        else
                        {
                            if (!(usar711 || usar713))
                                ImagemComprovante1aVia.Add(linha.AsLinhaComprovante());

                            if (!usar715)
                                ImagemComprovante2aVia.Add(linha.AsLinhaComprovante());
                        }

                        break;

                    case 30: TextoEspecialOperador = linha.AsString(); break;
                    case 31: TextoEspecialCliente = linha.AsString(); break;
                    case 32: Autenticacao = linha.AsString(); break;
                    case 33: Banco = linha.AsString(); break;
                    case 34: Agencia = linha.AsString(); break;
                    case 35: AgenciaDC = linha.AsString(); break;
                    case 36: Conta = linha.AsString(); break;
                    case 37: ContaDC = linha.AsString(); break;
                    case 38: Cheque = linha.AsString(); break;
                    case 39: ChequeDC = linha.AsString(); break;
                    case 40: NomeAdministradora = linha.AsString(); break;
                    case 131: Instituicao = linha.AsString(); break;
                    case 132: CodigoBandeiraPadrao = linha.AsString(); break;
                    case 136: BIN = linha.AsString(); break;
                    case 300:
                        switch (linha.Sequencia)
                        {
                            case 1: NFCeSAT.DataExpiracao = linha.AsString(); break;
                            case 2: NFCeSAT.DonoCartao = linha.AsString(); break;
                        }
                        break;

                    case 600: NFCeSAT.CNPJCredenciadora = linha.AsString(); break;
                    case 601: NFCeSAT.Bandeira = linha.AsString(); break;
                    case 602: NFCeSAT.Autorizacao = linha.AsString(); break;
                    case 603: NFCeSAT.CodCredenciadora = linha.AsString(); break;
                    case 707: ValorOriginal = linha.AsDecimal(); break;
                    case 708: Saque = linha.AsDecimal(); break;
                    case 709: Desconto = linha.AsDecimal(); break;
                    case 710:
                        if (linha.AsInt32() > 0 && ViaClienteReduzida)
                        {
                            usar711 = true;
                            ImagemComprovante1aVia.Clear();
                            QtdLinhasComprovante = linha.AsInt32();
                        }
                        break;

                    case 711: if (usar711) ImagemComprovante1aVia.Add(linha.AsLinhaComprovante()); break;
                    case 712:
                        if (linha.AsInt32() > 0 && !ViaClienteReduzida)
                        {
                            usar713 = true;
                            ImagemComprovante1aVia.Clear();
                            QtdLinhasComprovante = linha.AsInt32();
                        }
                        break;

                    case 713: if (usar713) ImagemComprovante1aVia.Add(linha.AsLinhaComprovante()); break;
                    case 714:
                        if (linha.AsInt32() > 0)
                        {
                            usar715 = true;
                            ImagemComprovante2aVia.Clear();
                        }
                        break;

                    case 715: if (usar715) ImagemComprovante2aVia.Add(linha.AsLinhaComprovante()); break;
                    case 899: // Tipos de Uso Interno do ACBrTEF
                        switch (linha.Sequencia)
                        {
                            case 1: CNFEnviado = linha.AsString().ToUpper() == "S"; break;
                            case 2: IndicePagamento = linha.AsString(); break;
                            case 3: OrdemPagamento = linha.AsInt32(); break;
                            case 103: ValorTotal += linha.AsDecimal(); break;
                            case 500: IdPagamento = linha.AsInt32(); break;
                            case 501: IdRespostaFiscal = linha.AsInt32(); break;
                            case 502: SerialPOS = linha.AsString(); break;
                            case 503: Estabelecimento = linha.AsString(); break;
                        }
                        break;

                    case 999: Trailer = linha.AsString(); break;
                }

                Parcelas.Clear();
                if (temParcela)
                {
                    for (var i = 1; i <= QtdParcelas; i++)
                    {
                        var parcela = new RespParcela
                        {
                            Vencimento = LeInformacao(19, i).AsDate(),
                            Valor = LeInformacao(20, i).AsDecimal(),
                            NSUParcela = LeInformacao(21, i).AsString()
                        };
                        Parcelas.Add(parcela);
                    }
                }

                Debito = (TipoTransacao >= 20 && TipoTransacao <= 25) || (TipoTransacao == 40);
                Credito = (TipoTransacao >= 10 && TipoTransacao <= 12);

                switch (TipoTransacao)
                {
                    case 10:
                    case 20:
                    case 23:
                        TipoOperacao = RespTipoOperacao.Avista;
                        break;

                    case 11:
                    case 12:
                    case 22:
                        TipoOperacao = RespTipoOperacao.Parcelado;
                        break;

                    case 21:
                    case 24:
                    case 25:
                        TipoOperacao = RespTipoOperacao.PreDatado;
                        DataPreDatado = LeInformacao(24).AsDate();
                        break;

                    case 40:
                        TipoOperacao = RespTipoOperacao.Parcelado;
                        ParceladoPor = RespParceladoPor.ADM;
                        break;

                    default:
                        TipoOperacao = RespTipoOperacao.Outras;
                        break;
                }
            }
        }

        #endregion Methods
    }
}