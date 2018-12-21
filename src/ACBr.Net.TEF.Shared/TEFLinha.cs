// ***********************************************************************
// Assembly         : ACBr.Net.TEFD
// Author           : RFTD
// Created          : 02-18-2015
//
// Last Modified By : RFTD
// Last Modified On : 02-18-2015
// ***********************************************************************
// <copyright file="TEFLinha.cs" company="ACBr.Net">
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

using ACBr.Net.Core.Extensions;
using ACBr.Net.Core.Generics;
using System;
using System.Globalization;

namespace ACBr.Net.TEF
{
    /// <summary>
    /// Classe TEFLinha. Est� classe n�o pode ser herdada.
    /// </summary>
    public sealed class TEFLinha : GenericClone<TEFLinha>
    {
        #region Fields

        /// <summary>
        /// The linha
        /// </summary>
        private string linha;

        #endregion Fields

        #region Constuctor

        /// <summary>
        /// Initializes a new instance of the <see cref="TEFLinha"/> class.
        /// </summary>
        /// <param name="linha">The linha.</param>
        internal TEFLinha(string linha)
        {
            Linha = linha;
        }

        #endregion Constuctor

        #region Propriedades

        /// <summary>
        /// Gets or sets the linha.
        /// </summary>
        /// <value>The linha.</value>
        public string Linha {
            get {
                if (!linha.IsEmpty()) return linha;
                return Identificacao > 0 ? $"{NomeCampo(Identificacao, Sequencia)}={Informacao}" : string.Empty;
            }
            set {
                if (linha == value) return;

                linha = value;

                var linhas = linha.Split(new[] { " = " }, StringSplitOptions.None);
                if (linha.IsEmpty() || linhas.Length == 0)
                {
                    Informacao = string.Empty;
                    Identificacao = 0;
                    Sequencia = 0;
                    return;
                }

                if (linhas.Length < 1) return;
                Chave = linhas[0].Trim();

                if (linhas.Length < 2) return;
                Informacao = linhas[1].Trim();

                var chaves = Chave.Split('-');

                if (chaves.Length < 1) return;
                Identificacao = (short)chaves[0].ToInt32();

                if (chaves.Length < 2) return;
                Sequencia = (short)chaves[1].ToInt32();
            }
        }

        /// <summary>
        /// Gets the identificacao.
        /// </summary>
        /// <value>The identificacao.</value>
        public short Identificacao { get; private set; }

        /// <summary>
        /// Gets the sequencia.
        /// </summary>
        /// <value>The sequencia.</value>
        public short Sequencia { get; private set; }

        /// <summary>
        /// Gets the chave.
        /// </summary>
        /// <value>The chave.</value>
        public string Chave { get; private set; }

        /// <summary>
        /// Gets the informacao.
        /// </summary>
        /// <value>The informacao.</value>
        public string Informacao { get; private set; }

        #endregion Propriedades

        #region Methods

        /// <summary>
        /// Ases the string.
        /// </summary>
        /// <returns>System.String.</returns>
        public string AsString()
        {
            return Informacao;
        }

        /// <summary>
        /// Ases the int32.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int AsInt32()
        {
            return Informacao.ToInt32();
        }

        /// <summary>
        /// Ases the time stamp.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime AsDateTime()
        {
            var data = Informacao.OnlyNumbers();
            return DateTime.ParseExact(data, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Ases the date.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime AsDate()
        {
            try
            {
                var data = Informacao.OnlyNumbers();
                var dia = data.Substring(0, 2).ToInt32();
                var mes = data.Substring(2, 2).ToInt32();
                var ano = data.Substring(4, 4).ToInt32();
                return new DateTime(ano, mes, dia);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Ases the time.
        /// </summary>
        /// <returns>TimeSpan.</returns>
        public TimeSpan AsTime()
        {
            var data = Informacao.OnlyNumbers();
            var hora = data.Substring(0, 2).ToInt32();
            var min = data.Substring(2, 2).ToInt32();
            var sec = data.Substring(4, 2).ToInt32();
            return new TimeSpan(hora, min, sec);
        }

        /// <summary>
        /// Ases the decimal.
        /// </summary>
        /// <returns>System.Decimal.</returns>
        public decimal AsDecimal()
        {
            return Informacao.ToDecimal(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Ases the linha comprovante.
        /// </summary>
        /// <returns>System.String.</returns>
        public string AsLinhaComprovante()
        {
            var comprovante = Informacao.Trim();
            comprovante = comprovante.StartsWith("\"") ? comprovante.Remove(0, 1) : comprovante;
            comprovante = comprovante.EndsWith("\"") ? comprovante.Remove(comprovante.Length - 1, 1) : comprovante;
            return comprovante;
        }

        private static string NomeCampo(short identificacao, short sequencia)
        {
            var casas = Math.Max((identificacao.ToString()).Length, 3);
            return $"{((int)identificacao).ZeroFill(casas)}-{sequencia}";
        }

        #endregion Methods
    }
}