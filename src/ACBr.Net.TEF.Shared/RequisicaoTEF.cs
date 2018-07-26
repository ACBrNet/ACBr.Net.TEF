// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 02-18-2015
//
// Last Modified By : RFTD
// Last Modified On : 02-18-2015
// ***********************************************************************
// <copyright file="RequisicaoTEF.cs" company="ACBr.Net">
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
using ACBr.Net.Core.Generics;

namespace ACBr.Net.TEF
{
    /// <summary>
    /// Classe Req.
    /// </summary>
    public sealed class RequisicaoTEF : GenericClone<RequisicaoTEF>
    {
        #region Fields

        private string header;
        private int id;
        private string documentoVinculado;
        private decimal valorTotal;
        private int moeda;
        private string cmc7;
        private char tipoPessoa;
        private string documentoPessoa;
        private DateTime? dataCheque;
        private string rede;
        private string nsu;
        private DateTime? dataHoraTransacaoComprovante;
        private string finalizacao;
        private string banco;
        private string agencia;
        private string agenciaDC;
        private string conta;
        private string contaDC;
        private string cheque;
        private string chequeDC;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RequisicaoTEF"/> class.
        /// </summary>
        internal RequisicaoTEF()
        {
            Conteudo = new TEFArquivo();
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the conteudo.
        /// </summary>
        /// <value>The conteudo.</value>
        public TEFArquivo Conteudo { get; private set; }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string Header
        {
            get => header;
            set
            {
                if (header == value) return;
                header = value;
                Conteudo.GravarInformacao(value, 0);
            }
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id
        {
            get => id;
            set
            {
                if (id == value) return;
                id = value;
                Conteudo.GravarInformacao(value, 1);
            }
        }

        /// <summary>
        /// Gets or sets the documento vinculado.
        /// </summary>
        /// <value>The documento vinculado.</value>
        public string DocumentoVinculado
        {
            get => documentoVinculado;
            set
            {
                if (documentoVinculado == value) return;
                documentoVinculado = value;
                Conteudo.GravarInformacao(value, 2);
            }
        }

        /// <summary>
        /// Gets or sets the valor total.
        /// </summary>
        /// <value>The valor total.</value>
        public decimal ValorTotal
        {
            get => valorTotal;
            set
            {
                if (valorTotal == value) return;
                valorTotal = value;
                Conteudo.GravarInformacao(value, 3);
            }
        }

        /// <summary>
        /// Gets or sets the moeda.
        /// </summary>
        /// <value>The moeda.</value>
        public int Moeda
        {
            get => moeda;
            set
            {
                if (moeda == value) return;
                moeda = value;
                Conteudo.GravarInformacao(value, 4);
            }
        }

        /// <summary>
        /// Gets or sets the cm c7.
        /// </summary>
        /// <value>The cm c7.</value>
        public string CMC7
        {
            get => cmc7;
            set
            {
                if (cmc7 == value) return;
                cmc7 = value;
                Conteudo.GravarInformacao(value, 5);
            }
        }

        /// <summary>
        /// Gets or sets the tipo pessoa.
        /// </summary>
        /// <value>The tipo pessoa.</value>
        public char TipoPessoa
        {
            get => tipoPessoa;
            set
            {
                if (tipoPessoa == value) return;
                tipoPessoa = value;
                Conteudo.GravarInformacao(value, 6);
            }
        }

        /// <summary>
        /// Gets or sets the documento pessoa.
        /// </summary>
        /// <value>The documento pessoa.</value>
        public string DocumentoPessoa
        {
            get => documentoPessoa;
            set
            {
                if (documentoPessoa == value) return;
                documentoPessoa = value;
                Conteudo.GravarInformacao(value, 7);
            }
        }

        /// <summary>
        /// Gets or sets the data cheque.
        /// </summary>
        /// <value>The data cheque.</value>
        public DateTime? DataCheque
        {
            get => dataCheque;
            set
            {
                if (dataCheque == value) return;
                dataCheque = value;

                if (value.HasValue)
                    Conteudo.GravarInformacao(value.Value.Date, 8);
            }
        }

        /// <summary>
        /// Gets or sets the rede.
        /// </summary>
        /// <value>The rede.</value>
        public string Rede
        {
            get => rede;
            set
            {
                if (rede == value) return;
                rede = value;
                Conteudo.GravarInformacao(value, 10);
            }
        }

        /// <summary>
        /// Gets or sets the nsu.
        /// </summary>
        /// <value>The nsu.</value>
        public string NSU
        {
            get => nsu;
            set
            {
                var fNsu = value.OnlyNumbers();
                if (nsu == fNsu) return;

                nsu = fNsu;
                Conteudo.GravarInformacao(fNsu, 12);
            }
        }

        /// <summary>
        /// Gets or sets the data hora transacao comprovante.
        /// </summary>
        /// <value>The data hora transacao comprovante.</value>
        public DateTime? DataHoraTransacaoComprovante
        {
            get => dataHoraTransacaoComprovante;
            set
            {
                if (dataHoraTransacaoComprovante == value) return;

                dataHoraTransacaoComprovante = value;
                if (!value.HasValue) return;

                Conteudo.GravarInformacao(value.Value.Date, 22);
                Conteudo.GravarInformacao(value.Value.TimeOfDay, 23);
            }
        }

        /// <summary>
        /// Gets or sets the finalizacao.
        /// </summary>
        /// <value>The finalizacao.</value>
        public string Finalizacao
        {
            get => finalizacao;
            set
            {
                if (finalizacao == value) return;
                finalizacao = value;
                Conteudo.GravarInformacao(value, 27);
            }
        }

        /// <summary>
        /// Gets or sets the banco.
        /// </summary>
        /// <value>The banco.</value>
        public string Banco
        {
            get => banco;
            set
            {
                if (banco == value) return;
                banco = value;
                Conteudo.GravarInformacao(value, 33);
            }
        }

        /// <summary>
        /// Gets or sets the agencia.
        /// </summary>
        /// <value>The agencia.</value>
        public string Agencia
        {
            get => agencia;
            set
            {
                if (agencia == value) return;
                agencia = value;
                Conteudo.GravarInformacao(value, 34);
            }
        }

        /// <summary>
        /// Gets or sets the agencia dc.
        /// </summary>
        /// <value>The agencia dc.</value>
        public string AgenciaDC
        {
            get => agenciaDC;
            set
            {
                if (agenciaDC == value) return;
                agenciaDC = value;
                Conteudo.GravarInformacao(value, 35);
            }
        }

        /// <summary>
        /// Gets or sets the conta.
        /// </summary>
        /// <value>The conta.</value>
        public string Conta
        {
            get => conta;
            set
            {
                if (conta == value) return;
                conta = value;
                Conteudo.GravarInformacao(value, 36);
            }
        }

        /// <summary>
        /// Gets or sets the conta dc.
        /// </summary>
        /// <value>The conta dc.</value>
        public string ContaDC
        {
            get => contaDC;
            set
            {
                if (contaDC == value) return;
                contaDC = value;
                Conteudo.GravarInformacao(value, 37);
            }
        }

        /// <summary>
        /// Gets or sets the cheque.
        /// </summary>
        /// <value>The cheque.</value>
        public string Cheque
        {
            get => cheque;
            set
            {
                if (cheque == value) return;
                cheque = value;
                Conteudo.GravarInformacao(value, 38);
            }
        }

        /// <summary>
        /// Gets or sets the cheque dc.
        /// </summary>
        /// <value>The cheque dc.</value>
        public string ChequeDC
        {
            get => chequeDC;
            set
            {
                if (chequeDC == value) return;
                chequeDC = value;
                Conteudo.GravarInformacao(value, 39);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            Conteudo.Clear();
            header = string.Empty;
            id = 0;
            documentoVinculado = string.Empty;
            valorTotal = 0;
            moeda = 0;
            cmc7 = string.Empty;
            tipoPessoa = '\0';
            documentoPessoa = string.Empty;
            dataCheque = null;
            rede = string.Empty;
            nsu = string.Empty;
            dataHoraTransacaoComprovante = null;
            finalizacao = string.Empty;
            banco = string.Empty;
            agencia = string.Empty;
            agenciaDC = string.Empty;
            conta = string.Empty;
            contaDC = string.Empty;
            cheque = string.Empty;
            chequeDC = string.Empty;
        }

        /// <summary>
        /// Gravars the informacao.
        /// </summary>
        /// <param name="informacao">The informacao.</param>
        /// <param name="identificação">The identificação.</param>
        /// <param name="sequencia">The sequencia.</param>
        public void GravarInformacao(object informacao, int identificação, int sequencia = 0)
        {
            Conteudo.GravarInformacao(informacao, identificação, sequencia);
        }

        #endregion Methods
    }
}