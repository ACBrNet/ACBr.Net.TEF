using System;

namespace ACBr.Net.TEF.Events
{
    public class DepoisConfirmarTransacoesEventArgs : EventArgs
    {
        #region Constructor

        internal DepoisConfirmarTransacoesEventArgs(RetornoTEFCollection respostasPendentes)
        {
            RespostasPendentes = respostasPendentes;
        }

        #endregion Constructor

        #region Properties

        public RetornoTEFCollection RespostasPendentes { get; private set; }

        #endregion Properties
    }
}