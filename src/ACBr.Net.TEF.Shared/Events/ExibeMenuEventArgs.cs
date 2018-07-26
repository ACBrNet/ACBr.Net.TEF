using System;

namespace ACBr.Net.TEF.Events
{
    public class ExibeMenuEventArgs : EventArgs
    {
        #region Constructor

        internal ExibeMenuEventArgs(string titulo, string[] opcoes)
        {
            Titulo = titulo;
            Opcoes = opcoes;
            ItemSelecionado = -1;
            VoltarMenu = false;
        }

        #endregion Constructor

        #region Properties

        public string Titulo { get; private set; }

        public string[] Opcoes { get; private set; }

        public int ItemSelecionado { get; set; }

        public bool VoltarMenu { get; set; }

        #endregion Properties
    }
}