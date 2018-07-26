// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 05-04-2014
//
// Last Modified By : RFTD
// Last Modified On : 02-19-2015
// ***********************************************************************
// <copyright file="RespostasPendentes.cs" company="ACBr.Net">
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


using System.Linq;

namespace ACBr.Net.TEF
{
    /// <summary>
    /// Classe RespostasPendentes.
    /// </summary>
    public class RetornoTEFCollection : TEFCollection<RetornoTEF>
    {
        #region Fields

        /// <summary>
        /// The f parent
        /// </summary>
        private ACBrTEF fParent;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RetornoTEFCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal RetornoTEFCollection(ACBrTEF parent)
        {
            fParent = parent;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the saldo restante.
        /// </summary>
        /// <value>The saldo restante.</value>
        public decimal SaldoRestante { get; internal set; }

        /// <summary>
        /// Gets the total desconto.
        /// </summary>
        /// <value>The total desconto.</value>
        public decimal TotalDesconto { get; internal set; }

        /// <summary>
        /// Gets the total pago.
        /// </summary>
        /// <value>The total pago.</value>
        public decimal TotalPago { get; internal set; }

        /// <summary>
        /// Gets the saldo a pagar.
        /// </summary>
        /// <value>The saldo a pagar.</value>
        public decimal SaldoAPagar { get; internal set; }

        /// <summary>
        /// Retorna o valor total das respostas agrupadas e organizadas.
        /// </summary>
        public RetornoTEFOrderedGrouped[] OrderedAndGrouped => 
            this.Any() ? this.GroupBy(x => new { x.IndicePagamento, x.OrdemPagamento })
                .Select(x => new RetornoTEFOrderedGrouped
                {
                    OrdemPagamento = x.Key.OrdemPagamento,
                    IndicePagamento = x.Key.IndicePagamento,
                    ValorTotal = x.Sum(y => y.ValorTotal)
                }).OrderBy(x => x.OrdemPagamento).ToArray() : new RetornoTEFOrderedGrouped[]{};

        #endregion Properties
    }

}