// ***********************************************************************
// Assembly         : ACBr.Net.TEFD
// Author           : RFTD
// Created          : 02-18-2015
//
// Last Modified By : RFTD
// Last Modified On : 02-27-2015
// ***********************************************************************
// <copyright file="TEFArquivo.cs" company="ACBr.Net">
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

using ACBr.Net.Core.Exceptions;
using ACBr.Net.Core.Extensions;
using ACBr.Net.Core.Generics;
using ACBr.Net.Core.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ACBr.Net.TEF
{
    /// <summary>
    /// Classe TEFArquivo. Está classe não pode ser herdada.
    /// </summary>
    public sealed class TEFArquivo : GenericClone<TEFArquivo>, IEnumerable<TEFLinha>, IACBrLog
    {
        #region Fields

        /// <summary>
        /// The arquivo
        /// </summary>
        private readonly List<TEFLinha> arquivo;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TEFArquivo" /> class.
        /// </summary>
        internal TEFArquivo()
        {
            arquivo = new List<TEFLinha>();
        }

        #endregion Constructor

        #region Propriedades

        /// <summary>
        /// Retorna o conteúdo do arquivo em array
        /// </summary>
        /// <value>O conteúdo</value>
        public string[] Conteudo => arquivo.Select(x => x.Linha).ToArray();

        /// <summary>
        /// Obtém <see cref="TEFLinha" /> com o indice especificado.
        /// </summary>
        /// <param name="idx">O indice.</param>
        /// <returns>TEFLinha.</returns>
        /// <exception cref="System.IndexOutOfRangeException">Indice fora da série</exception>
        public TEFLinha this[int idx]
        {
            get
            {
                Guard.Against<IndexOutOfRangeException>(idx < 0 || idx >= arquivo.Count, "Indice fora da série");
                return arquivo[idx];
            }
        }

        #endregion Propriedades

        #region Methods

        /// <summary>
        /// Retorna o conteudo do arquivo como string.
        /// </summary>
        /// <returns>System.String.</returns>
        public string AsString()
        {
            return arquivo.OrderBy(x => x.Chave).Select(linha => $"{linha.Chave} = {linha.Informacao}").AsString();
        }

        /// <summary>
        /// Limpa o conteúdo do classe
        /// </summary>
        internal void Clear()
        {
            arquivo.Clear();
        }

        /// <summary>
        /// Grava o arquivo em disco
        /// </summary>
        /// <param name="nomeArquivo">O nome do arquivo.</param>
        internal void GravarArquivo(string nomeArquivo)
        {
            var lines = arquivo.OrderBy(x => x.Chave).Select(linha => $"{linha.Chave} = {linha.Informacao}").ToArray();

            File.WriteAllLines(nomeArquivo, lines, Encoding.ASCII);
        }

        /// <summary>
        /// O arquivo passado
        /// </summary>
        /// <param name="nomeArquivo">O nome do arquivo.</param>
        /// <exception cref="System.IO.FileNotFoundException">Arquivo não encontrado</exception>
        internal void LerArquivo(string nomeArquivo)
        {
            Guard.Against<FileNotFoundException>(!File.Exists(nomeArquivo), "Arquivo não encontrado");

            arquivo.Clear();
            var file = File.ReadAllLines(nomeArquivo);

            try
            {
                foreach (var line in file)
                {
                    arquivo.Add(new TEFLinha(line));
                }
            }
            catch (Exception ex)
            {
                this.Log().Error(ex);
            }
        }

        /// <summary>
        /// Grava a informação no arquivo
        /// </summary>
        /// <param name="informacao">A informação.</param>
        /// <param name="chave">A chave de indentificação.</param>
        internal void GravarInformacao(object informacao, string chave)
        {
            chave = chave.Trim();
            string strInformacao;

            switch (informacao)
            {
                case decimal value:
                    strInformacao = value.ToString(CultureInfo.InvariantCulture).Trim();
                    break;
                case double value:
                    strInformacao = value.ToString(CultureInfo.InvariantCulture).Trim();
                    break;
                case float value:
                    strInformacao = value.ToString(CultureInfo.InvariantCulture).Trim();
                    break;
                case DateTime value:
                    strInformacao = value.ToString(value.TimeOfDay == TimeSpan.Zero ? "ddMMyyyy" : "ddMMhhmmss");
                    break;
                case TimeSpan value:
                    strInformacao = value.ToString("hhmmss");
                    break;
                default:
                    strInformacao = informacao == null ? string.Empty : informacao.ToString().Trim();
                    break;
            }

            var line = arquivo.SingleOrDefault(x => x.Chave == chave);
            if (line != null)
            {
                arquivo.Remove(line);
            }

            if (strInformacao.IsEmpty()) return;

            arquivo.Add(new TEFLinha($"{chave} = {strInformacao}"));
        }

        /// <summary>
        /// Grava a informação no arquivo
        /// </summary>
        /// <param name="informacao">A informação.</param>
        /// <param name="identificacao">O número de identificação.</param>
        /// <param name="sequencia">O número da sequencia.</param>
        internal void GravarInformacao(object informacao, int identificacao, int sequencia = 0)
        {
            var chave = $"{identificacao:000}-{sequencia:000}";
            GravarInformacao(informacao, chave);
        }

        /// <summary>
        /// Retorna uma linha especifica do arquivo.
        /// </summary>
        /// <param name="identificacao">O número de identificação.</param>
        /// <param name="sequencia">O número da sequencia.</param>
        /// <returns>TEFLinha.</returns>
        internal TEFLinha LeLinha(int identificacao, int sequencia = 0)
        {
            var chave = $"{identificacao:000}-{sequencia:000}";
            return arquivo.Find(x => x.Chave == chave);
        }

        #endregion Methods

        #region Enumerator

        /// <inheritdoc />
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<TEFLinha> GetEnumerator()
        {
            return arquivo.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Enumerator
    }
}