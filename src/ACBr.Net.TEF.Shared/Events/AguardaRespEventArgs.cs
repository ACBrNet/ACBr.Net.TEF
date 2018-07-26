// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 05-04-2014
//
// Last Modified By : RFTD
// Last Modified On : 02-18-2015
// ***********************************************************************
// <copyright file="AguardaRespEventArgs.cs" company="ACBr.Net">
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
    /// Classe AguardaRespEventArgs.
    /// </summary>
	public class AguardaRespEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AguardaRespEventArgs"/> class.
        /// </summary>
        /// <param name="arquivo">The arquivo.</param>
        /// <param name="seguntosTimeout">The seguntos timeout.</param>
        /// <param name="interromper"></param>
        internal AguardaRespEventArgs(string arquivo, int seguntosTimeout, bool interromper = false)
        {
            Arquivo = arquivo;
            SegundosTimeout = seguntosTimeout;
            Interromper = interromper;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the arquivo.
        /// </summary>
        /// <value>The arquivo.</value>
        public string Arquivo { get; private set; }

        /// <summary>
        /// Gets the segundos timeout.
        /// </summary>
        /// <value>The segundos timeout.</value>
		public int SegundosTimeout { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AguardaRespEventArgs"/> is interromper.
        /// </summary>
        /// <value><c>true</c> if interromper; otherwise, <c>false</c>.</value>
		public bool Interromper { get; set; }

        #endregion Properties
    }
}