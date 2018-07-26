// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 02-18-2015
//
// Last Modified By : RFTD
// Last Modified On : 02-19-2015
// ***********************************************************************
// <copyright file="TEFDisc.cs" company="ACBr.Net">
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
    public sealed class TEFDisc : TEFTxt
    {
        #region Fields

        private const string CacbrTefdDiscArqTemp = @"C:\TEF_Disc\req\intpos.tmp";
        private const string CacbrTefdDiscArqReq = @"C:\TEF_Disc\req\intpos.001";
        private const string CacbrTefdDiscArqResp = @"C:\TEF_Disc\resp\intpos.001";
        private const string CacbrTefdDiscArqSts = @"C:\TEF_Disc\resp\intpos.sts";
        private const string CacbrTefdDiscGpExeName = @"C:\TEF_Disc\tef_Disc.exe";

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TEFDisc" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal TEFDisc(ACBrTEF parent) : base(parent, TEFTipo.TEFDisc)
        {
            EsperaSts = ACBrTEF.CacbrTefdEsperaSts;
            NumVias = ACBrTEF.CacbrTefdNumVias;
            ArqReq = CacbrTefdDiscArqReq;
            ArqResp = CacbrTefdDiscArqResp;
            ArqTemp = CacbrTefdDiscArqTemp;
            ArqSTS = CacbrTefdDiscArqSts;
            GPExeName = CacbrTefdDiscGpExeName;
            Name = "TEFDisc";
        }

        #endregion Constructor
    }
}