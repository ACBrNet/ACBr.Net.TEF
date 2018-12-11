// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 05-04-2014
//
// Last Modified By : RFTD
// Last Modified On : 02-21-2015
// ***********************************************************************
// <copyright file="RetornoCliSiTef.cs" company="ACBr.Net">
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
using ACBr.Net.Core.Extensions;

namespace ACBr.Net.TEF
{
    /// <inheritdoc />
    public sealed class RetornoCliSiTef : RetornoTEF
    {
        #region Constructors

        public RetornoCliSiTef() : base(TEFTipo.CliSiTef)
        {
        }

        #endregion Constructors

        #region Methods

        internal void GravaInformacao(int identificacao, string informacao)
        {
            var sequencia = 0;

            switch (identificacao)
            {
                case int n1 when n1.IsBetween(141, 142): // 141 - Data Parcela, 142 - Valor Parcela
                case int n2 when n2.IsBetween(600, 607): // Dados do Corresp. Bancário
                case int n3 when n3.IsBetween(611, 624): // Dados do Corresp. Bancário
                    sequencia = 1;
                    while (!LeInformacao(identificacao, sequencia).AsString().IsEmpty())
                        sequencia++;
                    break;
            }

            Conteudo.GravarInformacao(informacao, identificacao, sequencia);
        }

        internal override void ConteudoToProperty()
        {
            ValorTotal = 0;
            ImagemComprovante1aVia.Clear();
            ImagemComprovante2aVia.Clear();
            Debito = false;
            Credito = false;
            Digitado = false;
            var wNumCB = 0;

            foreach (var linha in Conteudo)
            {
                var linStr = linha.Linha.Trim();

                switch (linha.Identificacao)
                {
                    case 29: Digitado = (linStr == "True"); break;
                    case 100:
                        ModalidadePagto = linStr;
                        switch (ModalidadePagto.Substring(0, 2).ToInt32())
                        {
                            case 1: Debito = true; break;
                            case 2: Credito = true; break;
                        }

                        var wTipoOperacao = ModalidadePagto.Substring(2, 2).ToInt32();
                        switch (wTipoOperacao)
                        {
                            case 0: TipoOperacao = RespTipoOperacao.Avista; break;
                            case 1: TipoOperacao = RespTipoOperacao.PreDatado; break;

                            case 2:
                                ParceladoPor = RespParceladoPor.Loja;
                                TipoOperacao = RespTipoOperacao.Parcelado;
                                break;

                            case 3:
                                ParceladoPor = RespParceladoPor.ADM;
                                TipoOperacao = RespTipoOperacao.Parcelado;
                                break;

                            default: TipoOperacao = RespTipoOperacao.Outras; break;
                        }
                        break;

                    case 101: ModalidadeExtenso = linStr; break;
                    case 102: ModalidadePagtoDescrita = linStr; break;
                    case 105:
                        DataHoraTransacaoComprovante = linha.AsDateTime();
                        DataHoraTransacaoHost = DataHoraTransacaoComprovante;
                        DataHoraTransacaoLocal = DataHoraTransacaoComprovante;
                        break;

                    case 120: Autenticacao = linStr; break;
                    case 121: ImagemComprovante1aVia.AddRange(linStr.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)); break;
                    case 122: ImagemComprovante2aVia.AddRange(linStr.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)); break;
                    case 123: TipoTransacao = linha.AsInt32(); break;
                    case 130:
                        Saque = linha.AsDecimal();
                        ValorTotal += Saque;
                        break;

                    case 131: Instituicao = linStr; break;
                    case 132: CodigoBandeiraPadrao = linStr; break;
                    case 133: CodigoAutorizacaoTransacao = linha.AsString(); break;
                    case 134: NSU = linStr; break;
                    case 136: BIN = linha.AsString(); break;
                    case 139: ValorEntradaCDC = linha.AsDecimal(); break;
                    case 140: DataEntradaCDC = linha.AsDate(); break;
                    case 156: Rede = linStr; break;
                    case 501: TipoPessoa = linha.AsInt32() == 0 ? 'J' : 'F'; break;
                    case 502: DocumentoPessoa = linStr; break;
                    case 504: TaxaServico = linha.AsDecimal(); break;
                    case 505: QtdParcelas = linha.AsInt32(); break;
                    case 506: DataPreDatado = linha.AsDate(); break;
                    case 511: QtdParcelas = linha.AsInt32(); break; // Parcelas CDC - Neste caso o campo 505 não é retornado
                    case 515: DataHoraTransacaoCancelada = linha.AsDate(); break;
                    case 516: NSUTransacaoCancelada = linStr; break;
                    case 527: DataVencimento = linha.AsDate(); break;
                    case 589: CodigoOperadoraCelular = linStr; break;
                    case 590: NomeOperadoraCelular = linStr; break;
                    case 591: ValorRecargaCelular = linha.AsDecimal(); break;
                    case 592: NumeroRecargaCelular = linStr; break;

                    case 607:
                        wNumCB = linha.AsInt32();

                        if (wNumCB == 1)
                            CorrespBancarios.Clear();

                        var cb = CorrespBancarios.AddNew();
                        cb.DataVencimento = LeInformacao(600, wNumCB).AsDate(); // Data Vencimento do título - CB
                        cb.ValorPago = LeInformacao(601, wNumCB).AsDecimal(); // Valor Pago do título - CB
                        cb.ValorOriginal = LeInformacao(602, wNumCB).AsDecimal(); // Valor Original do título - CB
                        cb.Acrescimo = LeInformacao(603, wNumCB).AsDecimal(); // Valor do Acréscimo - CB
                        cb.Desconto = LeInformacao(604, wNumCB).AsDecimal(); // Valor do Desconto - CB
                        cb.DataPagamento = LeInformacao(605, wNumCB).AsDate(); // Data contábil do Pagamento - CB
                        cb.NSUTransacaoCB = LeInformacao(611, wNumCB).AsString(); // NSU da Transação CB
                        cb.TipoDocumento = LeInformacao(612, wNumCB).AsInt32(); // Tipo Docto CB - 0:Arrecadação / 1:Título / 2:Tributo
                        cb.NSUCancelamento = LeInformacao(623, wNumCB).AsString(); // NSU para cancelamento - CB
                        cb.Documento = LeInformacao(624, wNumCB).AsString(); // Linha Digitável/ Código de Barras do documento pago
                        break;

                    case 609: CorrespBancarios.TotalTitulos = linha.AsDecimal(); break;
                    case 610: CorrespBancarios.TotalTitulosNaoPago = linha.AsDecimal(); break;

                    case 613:
                        Cheque = linStr.Substring(20, 6);
                        CMC7 = linStr;
                        break;

                    case 626: Banco = linStr; break;
                    case 627: Agencia = linStr; break;
                    case 628: AgenciaDC = linStr; break;
                    case 629: Conta = linStr; break;
                    case 630: ContaDC = linStr; break;

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

                    case 950: NFCeSAT.CNPJCredenciadora = linha.AsString(); break;
                    case 951: NFCeSAT.Bandeira = linha.AsString(); break;
                    case 952: NFCeSAT.Autorizacao = linha.AsString(); break;
                    case 953: NFCeSAT.CodCredenciadora = linha.AsString(); break;
                    case 1002: NFCeSAT.DataExpiracao = linha.AsString(); break;
                    case 1003: NFCeSAT.DonoCartao = linha.AsString(); break;
                    case 1190: NFCeSAT.UltimosQuatroDigitos = linha.AsString(); break;
                    case 4029:
                        Desconto = linha.AsDecimal();
                        ValorTotal -= Desconto;
                        break;
                }
            }

            QtdLinhasComprovante = Math.Max(ImagemComprovante1aVia.Count, ImagemComprovante2aVia.Count);

            // leitura de parcelas conforme nova documentação
            // 141 e 142 foram removidos em Setembro de 2014
            Parcelas.Clear();
            if (QtdParcelas < 1) return;

            var wValParc = (ValorTotal / QtdParcelas).RoundABNT();
            var wTotalParc = 0M;

            for (var i = 0; i < QtdParcelas - 1; i++)
            {
                var parcela = Parcelas.AddNew();
                if (i == 1)
                {
                    var vencimento = LeInformacao(140, i).AsDate();
                    if (vencimento == DateTime.MinValue)
                        vencimento = DataHoraTransacaoHost.Value.AddDays(i * 30);

                    parcela.Vencimento = vencimento;
                    parcela.Valor = LeInformacao(524, i).AsDecimal();
                }
                else
                {
                    var vencimento = LeInformacao(140, i).AsDate();
                    vencimento = vencimento == DateTime.MinValue ?
                        DataHoraTransacaoHost.Value.AddDays(i * 30) :
                        vencimento.AddDays(LeInformacao(508, i).AsInt32());
                    parcela.Vencimento = vencimento;
                    parcela.Valor = LeInformacao(525, i).AsDecimal();
                }

                if (parcela.Valor > 0) continue;

                if (i == QtdParcelas)
                    wValParc = ValorTotal - wTotalParc;
                else
                    wTotalParc = wTotalParc + wValParc;

                parcela.Valor = wValParc;
            }
        }

        #endregion Methods
    }
}