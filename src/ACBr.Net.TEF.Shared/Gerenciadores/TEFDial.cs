// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 02-18-2015
//
// Last Modified By : RFTD
// Last Modified On : 02-19-2015
// ***********************************************************************
// <copyright file="TEFDial.cs" company="ACBr.Net">
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

namespace ACBr.Net.TEF.Gerenciadores
{
    public sealed class TEFDial : TEFTxt
    {
        #region Fields

        private const string CacbrTefdDialArqTemp = @"C:\TEF_DIAL\req\intpos.tmp";
        private const string CacbrTefdDialArqReq = @"C:\TEF_DIAL\req\intpos.001";
        private const string CacbrTefdDialArqResp = @"C:\TEF_DIAL\resp\intpos.001";
        private const string CacbrTefdDialArqSts = @"C:\TEF_DIAL\resp\intpos.sts";
        private const string CacbrTefdDialGpExeName = @"C:\TEF_DIAL\tef_dial.exe";

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TEFDial" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal TEFDial(ACBrTEF parent) : base(parent, TEFTipo.TEFDial)
        {
            EsperaSts = ACBrTEF.CacbrTefdEsperaSts;
            NumVias = ACBrTEF.CacbrTefdNumVias;
            ArqReq = CacbrTefdDialArqReq;
            ArqResp = CacbrTefdDialArqResp;
            ArqTemp = CacbrTefdDialArqTemp;
            ArqSTS = CacbrTefdDialArqSts;
            GPExeName = CacbrTefdDialGpExeName;
            Name = "TEFDial";
        }

        #endregion Constructor
    }
}