using System;

namespace ACBr.Net.TEF.Events
{
    public class MudaEstadoReqEventArgs : EventArgs
    {
        #region Constructor

        internal MudaEstadoReqEventArgs(ReqEstado estadoReq)
        {
            EstadoReq = estadoReq;
        }

        #endregion Constructor

        #region Properties

        public ReqEstado EstadoReq { get; private set; }

        #endregion Properties
    }
}