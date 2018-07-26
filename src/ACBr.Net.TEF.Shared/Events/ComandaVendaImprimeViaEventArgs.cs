// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 05-04-2014
//
// Last Modified By : RFTD
// Last Modified On : 02-18-2015
// ***********************************************************************
// <copyright file="ComandaECFImprimeViaEventArgs.cs" company="ACBr.Net">
// Esta biblioteca é software livre; você pode redistribuí-la e/ou modificá-la
// sob os termos da Licença Pública Geral Menor do GNU conforme publicada pela
// Free Software Foundation; tanto a versão 2.1 da Licença, ou (a seu critério)
// qualquer versão posterior.
//
// Esta biblioteca é distribuída na expectativa de que seja útil, porém, SEM
// NENHUMA GARANTIA; nem mesmo a garantia implícita de COMERCIABILIDADE OU
// ADEQUAÇÃO A UMA FINALIDADE ESPECÍFICA. Consulte a Licença Pública Geral Menor
// do GNU para mais detalhes. (Arquivo LICENÇA.TXT ou LICENSE.TXT)
//
// Você deve ter recebido uma cópia da Licença Pública Geral Menor do GNU junto
// com esta biblioteca; se não, escreva para a Free Software Foundation, Inc.,
// no endereço 59 Temple Street, Suite 330, Boston, MA 02111-1307 USA.
// Você também pode obter uma copia da licença em:
// http://www.opensource.org/licenses/lgpl-license.php
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace ACBr.Net.TEF.Events
{
    /// <summary>
    /// Classe ComandaECFImprimeViaEventArgs.
    /// </summary>
	public class ComandaVendaImprimeViaEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ComandaVendaImprimeViaEventArgs"/> class.
        /// </summary>
        /// <param name="tipoRelatorio">The tipo relatorio.</param>
        /// <param name="via">The via.</param>
        /// <param name="imagemComprovante">The imagem comprovante.</param>
        internal ComandaVendaImprimeViaEventArgs(TipoRelatorio tipoRelatorio, int via, string[] imagemComprovante)
        {
            TipoRelatorio = tipoRelatorio;
            Via = via;
            ImagemComprovante = imagemComprovante;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the tipo relatorio.
        /// </summary>
        /// <value>The tipo relatorio.</value>
        public TipoRelatorio TipoRelatorio { get; private set; }

        /// <summary>
        /// Gets the via.
        /// </summary>
        /// <value>The via.</value>
		public int Via { get; private set; }

        /// <summary>
        /// Gets the imagem comprovante.
        /// </summary>
        /// <value>The imagem comprovante.</value>
		public string[] ImagemComprovante { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [retorno ecf].
        /// </summary>
        /// <value><c>true</c> if [retorno ecf]; otherwise, <c>false</c>.</value>
		public RetornoEvento Retorno { get; set; }

        #endregion Properties
    }
}