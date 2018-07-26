// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 05-04-2014
//
// Last Modified By : RFTD
// Last Modified On : 02-18-2015
// ***********************************************************************
// <copyright file="InfoECF.cs" company="ACBr.Net">
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

namespace ACBr.Net.TEF
{
    /// <summary>
    /// Enum InfoECF
    /// </summary>
    public enum InfoVenda
    {
        /// <summary>
        /// Valor do Saldo restante "A Pagar" do Cupom
        /// </summary>
		SubTotal,

        /// <summary>
        /// Estado do ECF "L" Livre, "V" Em Venda de Itens,
        /// "P" Em Pagamento,
        /// "C" CDC ou Vinculado
        /// "G" Relatório Gerencial
        /// "N" Não Fiscal (em qq fase, pois é dificil detectar a fase)
        /// "O" Outro
        /// </summary>
		EstadoVenda,

        /// <summary>
        /// Valor Total de Pagamentos registrados, na Aplicação, e não enviados ao ECF
        /// </summary>
		TotalAPagar
    }
}