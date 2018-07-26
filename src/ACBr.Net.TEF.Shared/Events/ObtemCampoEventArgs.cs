using System;

namespace ACBr.Net.TEF.Events
{
    public class ObtemCampoEventArgs : EventArgs
    {
        #region Constructor

        internal ObtemCampoEventArgs(string titulo, int tamanhoMinimo, int tamanhoMaximo, int tipoCampo, OperacaoCampo operacao)
        {
            Titulo = titulo;
            TamanhoMinimo = tamanhoMinimo;
            TamanhoMaximo = tamanhoMaximo;
            TipoCampo = tipoCampo;
            Operacao = operacao;
        }

        #endregion Constructor

        #region Properties

        public string Titulo { get; private set; }

        public int TamanhoMinimo { get; private set; }

        public int TamanhoMaximo { get; private set; }

        public int TipoCampo { get; private set; }

        public OperacaoCampo Operacao { get; private set; }

        public string Resposta { get; set; }

        public bool Digitado { get; set; }

        public bool VoltarMenu { get; set; }

        #endregion Properties
    }
}