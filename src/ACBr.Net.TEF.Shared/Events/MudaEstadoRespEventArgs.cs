using System;

namespace ACBr.Net.TEF.Events
{
    public class MudaEstadoRespEventArgs : EventArgs
    {
        #region Constructor

        internal MudaEstadoRespEventArgs(RespEstado estadoResp)
        {
            EstadoResp = estadoResp;
        }

        #endregion Constructor

        #region Properties

        public RespEstado EstadoResp { get; private set; }

        #endregion Properties
    }
}