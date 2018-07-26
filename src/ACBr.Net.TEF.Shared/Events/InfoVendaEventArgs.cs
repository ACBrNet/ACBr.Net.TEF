﻿// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 07-30-2016
//
// Last Modified By : RFTD
// Last Modified On : 07-30-2016
// ***********************************************************************
// <copyright file="InfoTransacaoEventArgs.cs" company="ACBr.Net">
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

namespace ACBr.Net.TEF.Events
{
    /// <summary>
    /// Class InfoECFEventArgs.
    /// </summary>
    public class InfoVendaEventArgs : EventArgs
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoVendaEventArgs"/> class.
        /// </summary>
        /// <param name="operacao">The operacao.</param>
        internal InfoVendaEventArgs(InfoVenda operacao)
        {
            Operacao = operacao;
            Retorno = EstadoVenda.Outro;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets the operacao.
        /// </summary>
        /// <value>The operacao.</value>
        public InfoVenda Operacao { get; private set; }

        /// <summary>
        /// Gets or sets the retorno.
        /// </summary>
        /// <value>The retorno.</value>
        public EstadoVenda Retorno { get; set; }

        /// <summary>
        /// Gets or sets the retorno ecf.
        /// </summary>
        /// <value>The retorno ecf.</value>
        public RetornoECF? RetornoECF { get; set; }

        #endregion Properties
    }
}