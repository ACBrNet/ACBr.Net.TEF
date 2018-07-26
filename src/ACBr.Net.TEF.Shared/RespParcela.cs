// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 05-04-2014
//
// Last Modified By : RFTD
// Last Modified On : 02-18-2015
// ***********************************************************************
// <copyright file="RespParcela.cs" company="ACBr.Net">
// Esta biblioteca � software livre; voc� pode redistribu�-la e/ou modific�-la
// sob os termos da Licen�a P�blica Geral Menor do GNU conforme publicada pela
// Free Software Foundation; tanto a vers�o 2.1 da Licen�a, ou (a seu crit�rio)
// qualquer vers�o posterior.
//
// Esta biblioteca � distribu�da na expectativa de que seja �til, por�m, SEM
// NENHUMA GARANTIA; nem mesmo a garantia impl�cita de COMERCIABILIDADE OU
// ADEQUA��O A UMA FINALIDADE ESPEC�FICA. Consulte a Licen�a P�blica Geral Menor
// do GNU para mais detalhes. (Arquivo LICEN�A.TXT ou LICENSE.TXT)
//
// Voc� deve ter recebido uma c�pia da Licen�a P�blica Geral Menor do GNU junto
// com esta biblioteca; se n�o, escreva para a Free Software Foundation, Inc.,
// no endere�o 59 Temple Street, Suite 330, Boston, MA 02111-1307 USA.
// Voc� tamb�m pode obter uma copia da licen�a em:
// http://www.opensource.org/licenses/lgpl-license.php
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using ACBr.Net.Core.Generics;

namespace ACBr.Net.TEF
{
    /// <summary>
    /// Classe RespParcela. Est� classe n�o pode ser herdada.
    /// </summary>
    [Serializable]
    public sealed class RespParcela : GenericClone<RespParcela>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RespParcela"/> class.
        /// </summary>
        internal RespParcela()
        {
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the vencimento.
        /// </summary>
        /// <value>The vencimento.</value>
        public DateTime Vencimento { get; set; }

        /// <summary>
        /// Gets or sets the valor.
        /// </summary>
        /// <value>The valor.</value>
		public decimal Valor { get; set; }

        /// <summary>
        /// Gets or sets the nsu parcela.
        /// </summary>
        /// <value>The nsu parcela.</value>
		public string NSUParcela { get; set; }

        #endregion Properties
    }
}