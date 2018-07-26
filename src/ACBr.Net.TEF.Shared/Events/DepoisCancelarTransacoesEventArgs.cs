using System;

namespace ACBr.Net.TEF.Events
{
    public class DepoisCancelarTransacoesEventArgs : EventArgs
    {
        #region Constructor

        internal DepoisCancelarTransacoesEventArgs(RetornoTEFCollection respostasPendentes)
        {
            RespostasPendentes = respostasPendentes;
        }

        #endregion Constructor

        #region Properties

        public RetornoTEFCollection RespostasPendentes { get; private set; }

        #endregion Properties
    }
}