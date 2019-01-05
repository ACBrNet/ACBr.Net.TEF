// ***********************************************************************
// Assembly         : ACBr.Net.TEF
// Author           : RFTD
// Created          : 02-18-2015
//
// Last Modified By : RFTD
// Last Modified On : 02-21-2015
// ***********************************************************************
// <copyright file="TEFCliSiTef.cs" company="ACBr.Net">
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
using ACBr.Net.Core;
using ACBr.Net.Core.Exceptions;
using ACBr.Net.Core.Extensions;
using ACBr.Net.Core.InteropServices;
using ACBr.Net.Core.Logging;
using ACBr.Net.TEF.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ACBr.Net.TEF.Gerenciadores
{
    /// <inheritdoc />
    /// <summary>
    /// Classe TEFCliSiTef. Está classe não pode ser herdada.
    /// </summary>
    public sealed class TEFCliSiTef : TEFBase
    {
        #region Constants

        internal const bool CacbrTefdCliSiTefImprimeGerencialConcomitante = false;
        internal const string CacbrTefdCliSiTefPressioneEnter = "PRESSIONE <ENTER>";
        internal const string CacbrTefdCliSiTefTransacaoNaoEfetuada = "Transação não efetuada.";
        internal const string CacbrTefdCliSiTefTransacaoNaoEfetuadaReterCupom = "Transação não efetuada.\r\nFavor reter o Cupom";
        internal const string CacbrTefdCliSiTefTransacaoEfetuadaReImprimir = "Transação TEF efetuada.\r\n" +
                                                                             "Favor reimprimir último Cupom.\r\n" +
                                                                             "{0}\r\n(Para Cielo utilizar os 6 últimos dígitos.)";
        internal const string CacbrTefdCliSiTefNaoInicializado = "CliSiTEF não inicializado";
        internal const string CacbrTefdCliSiTefNaoConcluido = "Requisição anterior não concluida";
        internal const string CacbrTefdCliSiTefErro1 = "Endereço IP inválido ou não resolvido";
        internal const string CacbrTefdCliSiTefErro2 = "Código da loja inválido";
        internal const string CacbrTefdCliSiTefErro3 = "Código de terminal inválido";
        internal const string CacbrTefdCliSiTefErro6 = "Erro na inicialização do TCP/IP";
        internal const string CacbrTefdCliSiTefErro7 = "Falta de memória";
        internal const string CacbrTefdCliSiTefErro8 = "Não encontrou a CliSiTef ou ela está com problemas";

        internal const string CacbrTefdCliSiTefErro10 =
            "Erro de acesso na pasta CliSiTef (possível falta de permissão para escrita) \r\n" +
            "ou o PinPad não está devidamente configurado no arquivo CliSiTef.ini \r\n" +
            "ou parâmetros IdLoja e IdTerminal inválidos";

        internal const string CacbrTefdCliSiTefErro11 = "Dados inválidos passados pela automação.";

        internal const string CacbrTefdCliSiTefErro12 =
            "Modo seguro não ativo (possível falta de configuração no servidor SiTef do arquivo .cha).";

        internal const string CacbrTefdCliSiTefErro13 =
            "Caminho da DLL inválido (o caminho completo das bibliotecas está muito grande).";

        #endregion Constants

        #region InnerTypes

        /// <summary>
        /// Códigos dos comandos retornados pelo SiTef
        /// </summary>
        private enum CommandType
        {
            /// <summary>
            /// Está devolvendo um valor para, se desejado, ser armazenado pela automação
            /// </summary>
            Store,

            /// <summary>
            /// 1	Mensagem para o visor do operador
            /// </summary>
            DisplayOperatorMessage,

            /// <summary>
            /// 2	Mensagem para o visor do cliente
            /// </summary>
            DisplayCustomerMessage,

            /// <summary>
            /// 3	Mensagem para os dois visores
            /// </summary>
            DisplayMessage,

            /// <summary>
            /// 4	Texto que deverá ser utilizado como cabeçalho na apresentação do menu (Comando 21)
            /// </summary>
            DisplayMenuHeader,

            /// <summary>
            /// 11	Deve remover a mensagem apresentada no visor do operador
            /// </summary>
            ClearOperatorMessage = 11,

            /// <summary>
            /// 12	Deve remover a mensagem apresentada no visor do cliente
            /// </summary>
            ClearCustomerMessage,

            /// <summary>
            /// 13	Deve remover mensagem apresentada no visor do operador e do cliente
            /// </summary>
            ClearMessage,

            /// <summary>
            /// 14	Deve limpar o texto utilizado como cabeçalho na apresentação do menu
            /// </summary>
            ClearMenuHeader,

            /// <summary>
            /// 15	Cabeçalho a ser apresentado pela aplicação
            /// </summary>
            DisplayHeader,

            /// <summary>
            /// 16	Deve remover o cabeçalho
            /// </summary>
            ClearHeader,

            /// <summary>
            /// 20	Deve obter uma resposta do tipo SIM/NÃO.
            /// </summary>
            DisplayConfirm = 20,

            /// <summary>
            /// 21	Deve apresentar um menu de opções e permitir que o usuário selecione uma delas. Na chamada o parâmetro Buffer contém as opções no formato 1:texto;2:texto;...i:Texto;... A rotina da aplicação deve apresentar as opções da forma que ela desejar (não sendo necessário incluir os índices 1,2, ...) e após a seleção feita pelo usuário, retornar em Buffer o índice i escolhido pelo operador (em ASCII)
            /// </summary>
            DisplayMenuOptions,

            /// <summary>
            /// 22	Deve aguardar uma tecla do operador. É utilizada quando se deseja que o operador seja avisado de alguma mensagem apresentada na tela
            /// </summary>
            WaitAnyKey,

            /// <summary>
            /// 23	Este comando indica
            /// que a rotina está perguntando para a aplicação se ele deseja interromper o processo de coleta de dados ou não.
            /// Esse código ocorre quando a CliSiTef está acessando algum periférico e permite que a automação interrompa esse acesso (por exemplo: aguardando a passagem de um cartão pela leitora ou a digitação de senha pelo cliente)
            /// </summary>
            CancelPinPadOperation,

            /// <summary>
            /// 29	Deve ser fornecido um campo, sem captura, cujo tamanho está entre TamMinimo e TamMaximo. O campo deve ser devolvido em Buffer
            /// </summary>
            ParameterNeeded = 29,

            /// <summary>
            /// 30	Deve ser lido um campo cujo tamanho está entre TamMinimo e TamMaximo. O campo lido deve ser devolvido em Buffer
            /// </summary>
            TextInputNeeded,

            /// <summary>
            /// 31 Deve ser lido o número de um cheque.
            /// A coleta pode ser feita via leitura de CMC-7 ou pela digitação da primeira linha do cheque. No retorno deve ser devolvido em Buffer “0:” ou “1:” seguido do número coletado manualmente ou pela leitura do CMC-7, respectivamente. Quando o número for coletado manualmente o formato é o seguinte: Compensação (3), Banco (3), Agencia (4), C1 (1), ContaCorrente (10), C2 (1), Numero do Cheque (6) e C3 (1), nesta ordem. Notar que estes campos são os que estão na parte superior de um cheque e na ordem apresentada. Sugerimos que na coleta seja apresentada uma interface que permita ao operador identificar e digitar adequadamente estas informações de forma que a consulta não seja feita com dados errados, retornando como bom um cheque com problemas
            /// </summary>
            CheckInputNeeded,

            /// <summary>
            /// 34 Deve ser lido um campo monetário ou seja, aceita o delimitador de centavos e devolvido no parâmetro Buffer
            /// </summary>
            MoneyInputNeeded = 34,

            /// <summary>
            /// 35 Deve ser lido um código em barras ou o mesmo deve ser coletado manualmente. No retorno Buffer deve conter “0:” ou “1:” seguido do código em barras coletado manualmente ou pela leitora, respectivamente. Cabe ao aplicativo decidir se a coleta será manual ou através de uma leitora. Caso seja coleta manual, recomenda-se seguir o procedimento descrito na rotina ValidaCampoCodigoEmBarras de forma a tratar um código em barras da forma mais genérica possível, deixando o aplicativo de automação independente de futuras alterações que possam surgir nos formatos em barras. No retorno do Buffer também pode ser passado “2:”, indicando que a coleta foi cancelada, porém o fluxo não será interrompido, logo no caso de pagamentos múltiplos, todos os documentados coletados anteriormente serão mantidos e o fluxo retomado, permitindo a efetivação de tais pagamentos.
            /// </summary>
            BarcodeInputNeeded,

            /// <summary>
            /// 41 Análogo ao Comando 30 (TextInputNeeded), porém o campo deve ser coletado de forma mascarada (senha).
            /// </summary>
            PasswordTextInputNeeded = 41,

            /// <summary>
            /// 42	Deve apresentar um menu de opções e permitir que o usuário selecione uma delas.
            /// </summary>
            DisplayIdentifiedMenuOptions
        }

        private sealed class CliSitefClient : ACBrSafeHandle
        {
            #region InnerTypes

            private class DelegatesStdCall
            {
                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int ConfiguraIntSiTefInterativoEx(string enderecoIp, string codigoLoja,
                    string numeroTerminal, short reservado, string parametrosAdicionais);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int IniciaFuncaoSiTefInterativo(int modalidade, string valor, string numeroCuponFiscal,
                    string dataFiscal, string horario, string operador, string paramAdic);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int IniciaFuncaoAASiTefInterativo(int modalidade, string valor,
                    string numeroCuponFiscal,
                    string dataFiscal, string horario, string operador, string paramAdic, string produtos);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate void FinalizaTransacaoSiTefInterativo(int confirma, string numeroCuponFiscal,
                    string dataFiscal, string horario);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int ContinuaFuncaoSiTefInterativo(out CommandType proximoComando, out int tipoCampo,
                    out short tamanhoMinimo, out short tamanhoMaximo, StringBuilder buffer, int tamMaxBuffer,
                    int continuaNavegacao);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int EscreveMensagemPermanentePinPad(string mensagem);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int VerificaPresencaPinPad();

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int LeCartaoDireto(string mensagem, StringBuilder trilha1, StringBuilder trilha2);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int LeCartaoInterativo(string mensagem);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate void InterrompeLeCartaoDireto();

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int LeSenhaDireto(string parametros, StringBuilder senhaCliente);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int LeSimNaoPinPad(string mensagem);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int EnviaRecebeSiTefDireto(short redeDestino, short funcaoSiTef, short offsetCartao,
                    string dadosTx, short tamDadosTx, StringBuilder dadosRx, short tamMaxDadosRx, ref short codigoResposta, short tempoEsperaRx,
                    string cupomFiscal, string dataFiscal, string horario, string operador, short tipoTransacao);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int ObtemQuantidadeTransacoesPendentes(string dataFiscal, string numeroCupon);

                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate int ValidaCampoCodigoEmBarras(string dados, short tipo);
            }

            private class DelegatesCdecl
            {
                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int ConfiguraIntSiTefInterativoEx(string enderecoIp, string codigoLoja,
                    string numeroTerminal, short reservado, string parametrosAdicionais);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int IniciaFuncaoSiTefInterativo(int modalidade, string valor, string numeroCuponFiscal,
                    string dataFiscal, string horario, string operador, string paramAdic);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int IniciaFuncaoAASiTefInterativo(int modalidade, string valor,
                    string numeroCuponFiscal,
                    string dataFiscal, string horario, string operador, string paramAdic, string produtos);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate void FinalizaTransacaoSiTefInterativo(int confirma, string numeroCuponFiscal,
                    string dataFiscal, string horario);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int ContinuaFuncaoSiTefInterativo(out CommandType proximoComando, out int tipoCampo,
                    out short tamanhoMinimo, out short tamanhoMaximo, StringBuilder buffer, int tamMaxBuffer,
                    int continuaNavegacao);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int EscreveMensagemPermanentePinPad(string mensagem);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int VerificaPresencaPinPad();

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int LeCartaoDireto(string mensagem, StringBuilder trilha1, StringBuilder trilha2);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int LeCartaoInterativo(string mensagem);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate void InterrompeLeCartaoDireto();

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int LeSenhaDireto(string parametros, StringBuilder senhaCliente);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int LeSimNaoPinPad(string mensagem);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int EnviaRecebeSiTefDireto(short redeDestino, short funcaoSiTef, short offsetCartao,
                    string dadosTx, short tamDadosTx, StringBuilder dadosRx, short tamMaxDadosRx, ref short codigoResposta, short tempoEsperaRx,
                    string cupomFiscal, string dataFiscal, string horario, string operador, short tipoTransacao);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int ObtemQuantidadeTransacoesPendentes(string dataFiscal, string numeroCupon);

                [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
                public delegate int ValidaCampoCodigoEmBarras(string dados, short tipo);
            }

            #endregion InnerTypes

            #region Constructors

            public CliSitefClient(string path) : base(Path.Combine(path,
                IsWindows ? "CliSiTef32I.dll" : "libclisitef.so"))
            {
                if (IsWindows)
                {
                    AddMethod<DelegatesStdCall.ConfiguraIntSiTefInterativoEx>("ConfiguraIntSiTefInterativoEx");
                    AddMethod<DelegatesStdCall.IniciaFuncaoSiTefInterativo>("IniciaFuncaoSiTefInterativo");
                    AddMethod<DelegatesStdCall.IniciaFuncaoAASiTefInterativo>("IniciaFuncaoAASiTefInterativo");
                    AddMethod<DelegatesStdCall.FinalizaTransacaoSiTefInterativo>("FinalizaTransacaoSiTefInterativo");
                    AddMethod<DelegatesStdCall.ContinuaFuncaoSiTefInterativo>("ContinuaFuncaoSiTefInterativo");
                    AddMethod<DelegatesStdCall.EscreveMensagemPermanentePinPad>("EscreveMensagemPermanentePinPad");
                    AddMethod<DelegatesStdCall.VerificaPresencaPinPad>("VerificaPresencaPinPad");
                    AddMethod<DelegatesStdCall.LeCartaoDireto>("LeCartaoDireto");
                    AddMethod<DelegatesStdCall.LeCartaoInterativo>("LeCartaoInterativo");
                    AddMethod<DelegatesStdCall.InterrompeLeCartaoDireto>("InterrompeLeCartaoDireto");
                    AddMethod<DelegatesStdCall.LeSenhaDireto>("LeSenhaDireto");
                    AddMethod<DelegatesStdCall.LeSimNaoPinPad>("LeSimNaoPinPad");
                    AddMethod<DelegatesStdCall.EnviaRecebeSiTefDireto>("EnviaRecebeSiTefDireto");
                    AddMethod<DelegatesStdCall.ObtemQuantidadeTransacoesPendentes>("ObtemQuantidadeTransacoesPendentes");
                    AddMethod<DelegatesStdCall.ValidaCampoCodigoEmBarras>("ValidaCampoCodigoEmBarras");
                }
                else
                {
                    AddMethod<DelegatesCdecl.ConfiguraIntSiTefInterativoEx>("ConfiguraIntSiTefInterativoEx");
                    AddMethod<DelegatesCdecl.IniciaFuncaoSiTefInterativo>("IniciaFuncaoSiTefInterativo");
                    AddMethod<DelegatesCdecl.IniciaFuncaoAASiTefInterativo>("IniciaFuncaoAASiTefInterativo");
                    AddMethod<DelegatesCdecl.FinalizaTransacaoSiTefInterativo>("FinalizaTransacaoSiTefInterativo");
                    AddMethod<DelegatesCdecl.ContinuaFuncaoSiTefInterativo>("ContinuaFuncaoSiTefInterativo");
                    AddMethod<DelegatesCdecl.EscreveMensagemPermanentePinPad>("EscreveMensagemPermanentePinPad");
                    AddMethod<DelegatesCdecl.VerificaPresencaPinPad>("VerificaPresencaPinPad");
                    AddMethod<DelegatesCdecl.LeCartaoDireto>("LeCartaoDireto");
                    AddMethod<DelegatesCdecl.LeCartaoInterativo>("LeCartaoInterativo");
                    AddMethod<DelegatesCdecl.InterrompeLeCartaoDireto>("InterrompeLeCartaoDireto");
                    AddMethod<DelegatesCdecl.LeSenhaDireto>("LeSenhaDireto");
                    AddMethod<DelegatesCdecl.LeSimNaoPinPad>("LeSimNaoPinPad");
                    AddMethod<DelegatesCdecl.EnviaRecebeSiTefDireto>("EnviaRecebeSiTefDireto");
                    AddMethod<DelegatesCdecl.ObtemQuantidadeTransacoesPendentes>("ObtemQuantidadeTransacoesPendentes");
                    AddMethod<DelegatesCdecl.ValidaCampoCodigoEmBarras>("ValidaCampoCodigoEmBarras");
                }
            }

            #endregion Constructors

            #region Methods

            public int ConfiguraIntSiTefInterativoEx(string enderecoIp, string codigoLoja,
                string numeroTerminal, short reservado, string parametrosAdicionais)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.ConfiguraIntSiTefInterativoEx>();
                    return ExecuteMethod(() =>
                        method(enderecoIp, codigoLoja, numeroTerminal, reservado, parametrosAdicionais));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.ConfiguraIntSiTefInterativoEx>();
                    return ExecuteMethod(() =>
                        method(enderecoIp, codigoLoja, numeroTerminal, reservado, parametrosAdicionais));
                }
            }

            public int IniciaFuncaoSiTefInterativo(int modalidade, string valor, string numeroCuponFiscal,
                string dataFiscal, string horario, string operador, string paramAdic)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.IniciaFuncaoSiTefInterativo>();
                    return ExecuteMethod(() =>
                        method(modalidade, valor, numeroCuponFiscal, dataFiscal, horario, operador, paramAdic));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.IniciaFuncaoSiTefInterativo>();
                    return ExecuteMethod(() =>
                        method(modalidade, valor, numeroCuponFiscal, dataFiscal, horario, operador, paramAdic));
                }
            }

            public int IniciaFuncaoAASiTefInterativo(int modalidade, string valor, string numeroCuponFiscal,
                string dataFiscal, string horario, string operador, string paramAdic, string produtos)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.IniciaFuncaoAASiTefInterativo>();
                    return ExecuteMethod(() => method(modalidade, valor, numeroCuponFiscal, dataFiscal, horario,
                        operador, paramAdic, produtos));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.IniciaFuncaoAASiTefInterativo>();
                    return ExecuteMethod(() => method(modalidade, valor, numeroCuponFiscal, dataFiscal, horario,
                        operador, paramAdic, produtos));
                }
            }

            public void FinalizaTransacaoSiTefInterativo(int confirma, string numeroCuponFiscal, string dataFiscal,
                string horario)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.FinalizaTransacaoSiTefInterativo>();
                    ExecuteMethod(() => method(confirma, numeroCuponFiscal, dataFiscal, horario));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.FinalizaTransacaoSiTefInterativo>();
                    ExecuteMethod(() => method(confirma, numeroCuponFiscal, dataFiscal, horario));
                }
            }

            [HandleProcessCorruptedStateExceptions]
            public int ContinuaFuncaoSiTefInterativo(out CommandType proximoComando, out int tipoCampo,
                out short tamanhoMinimo, out short tamanhoMaximo, StringBuilder buffer, int tamMaxBuffer,
                int continuaNavegacao)
            {
                try
                {
                    if (IsWindows)
                    {
                        var method = GetMethod<DelegatesStdCall.ContinuaFuncaoSiTefInterativo>();
                        return method(out proximoComando, out tipoCampo, out tamanhoMinimo, out tamanhoMaximo, buffer,
                            tamMaxBuffer, continuaNavegacao);
                    }
                    else
                    {
                        var method = GetMethod<DelegatesCdecl.ContinuaFuncaoSiTefInterativo>();
                        return method(out proximoComando, out tipoCampo, out tamanhoMinimo, out tamanhoMaximo, buffer,
                            tamMaxBuffer, continuaNavegacao);
                    }
                }
                catch (Exception exception)
                {
                    this.Log().Error($"{className} - Erro: {exception.Message}", exception);
                    throw new ACBrException(exception, exception.Message);
                }
            }

            public int EscreveMensagemPermanentePinPad(string mensagem)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.EscreveMensagemPermanentePinPad>();
                    return ExecuteMethod(() => method(mensagem));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.EscreveMensagemPermanentePinPad>();
                    return ExecuteMethod(() => method(mensagem));
                }
            }

            public int VerificaPresencaPinPad()
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.VerificaPresencaPinPad>();
                    return ExecuteMethod(() => method());
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.VerificaPresencaPinPad>();
                    return ExecuteMethod(() => method());
                }
            }

            public int LeCartaoDireto(string mensagem, StringBuilder trilha1, StringBuilder trilha2)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.LeCartaoDireto>();
                    return ExecuteMethod(() => method(mensagem, trilha1, trilha2));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.LeCartaoDireto>();
                    return ExecuteMethod(() => method(mensagem, trilha1, trilha2));
                }
            }

            public int LeCartaoInterativo(string mensagem)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.LeCartaoInterativo>();
                    return ExecuteMethod(() => method(mensagem));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.LeCartaoInterativo>();
                    return ExecuteMethod(() => method(mensagem));
                }
            }

            public void InterrompeLeCartaoDireto()
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.InterrompeLeCartaoDireto>();
                    ExecuteMethod(() => method());
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.InterrompeLeCartaoDireto>();
                    ExecuteMethod(() => method());
                }
            }

            public int LeSenhaDireto(string parametros, StringBuilder senhaCliente)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.LeSenhaDireto>();
                    return ExecuteMethod(() => method(parametros, senhaCliente));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.LeSenhaDireto>();
                    return ExecuteMethod(() => method(parametros, senhaCliente));
                }
            }

            public int LeSimNaoPinPad(string mensagem)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.LeSimNaoPinPad>();
                    return ExecuteMethod(() => method(mensagem));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.LeSimNaoPinPad>();
                    return ExecuteMethod(() => method(mensagem));
                }
            }

            [HandleProcessCorruptedStateExceptions]
            public int EnviaRecebeSiTefDireto(short redeDestino, short funcaoSiTef, short offsetCartao,
                string dadosTx, short tamDadosTx, StringBuilder dadosRx, short tamMaxDadosRx, ref short codigoResposta, short tempoEsperaRx,
                string cupomFiscal, string dataFiscal, string horario, string operador, short tipoTransacao)
            {
                try
                {
                    if (IsWindows)
                    {
                        var method = GetMethod<DelegatesStdCall.EnviaRecebeSiTefDireto>();
                        return method(redeDestino, funcaoSiTef, offsetCartao, dadosTx, tamDadosTx, dadosRx, tamMaxDadosRx,
                            ref codigoResposta, tempoEsperaRx, cupomFiscal, dataFiscal, horario, operador, tipoTransacao);
                    }
                    else
                    {
                        var method = GetMethod<DelegatesCdecl.EnviaRecebeSiTefDireto>();
                        return method(redeDestino, funcaoSiTef, offsetCartao, dadosTx, tamDadosTx, dadosRx, tamMaxDadosRx,
                            ref codigoResposta, tempoEsperaRx, cupomFiscal, dataFiscal, horario, operador, tipoTransacao);
                    }
                }
                catch (Exception exception)
                {
                    this.Log().Error($"{className} - Erro: {exception.Message}", exception);
                    throw new ACBrException(exception, exception.Message);
                }
            }

            public int ObtemQuantidadeTransacoesPendentes(string dataFiscal, string numeroCupon)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.ObtemQuantidadeTransacoesPendentes>();
                    return ExecuteMethod(() => method(dataFiscal, numeroCupon));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.ObtemQuantidadeTransacoesPendentes>();
                    return ExecuteMethod(() => method(dataFiscal, numeroCupon));
                }
            }

            public int ValidaCampoCodigoEmBarras(string dados, short tipo)
            {
                if (IsWindows)
                {
                    var method = GetMethod<DelegatesStdCall.ValidaCampoCodigoEmBarras>();
                    return ExecuteMethod(() => method(dados, tipo));
                }
                else
                {
                    var method = GetMethod<DelegatesCdecl.ValidaCampoCodigoEmBarras>();
                    return ExecuteMethod(() => method(dados, tipo));
                }
            }

            #endregion Methods
        }

        #endregion InnerTypes

        #region Fields

        private bool iniciouRequisicao;
        private bool reimpressao;
        private bool cancelamento;
        private string documentosProcessados;
        private string arqBackUp;
        private CliSitefClient client;
        private string documentoFiscal;
        private DateTime? dataHoraFiscal;

        private const int BufferSize = 20000;

        private static readonly NumberFormatInfo NumberFormat = new NumberFormatInfo()
        {
            NumberDecimalDigits = 2,
            NumberDecimalSeparator = ",",
            NumberGroupSeparator = ".",
        };

        #endregion Fields

        #region Events

        public event EventHandler<ExibeMenuEventArgs> OnExibeMenu;

        public event EventHandler<ObtemCampoEventArgs> OnObtemCampo;

        #endregion Events

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TEFCliSiTef" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal TEFCliSiTef(ACBrTEF parent) : base(parent, TEFTipo.CliSiTef)
        {
            iniciouRequisicao = false;
            reimpressao = false;
            cancelamento = false;
            ArqReq = string.Empty;
            ArqResp = string.Empty;
            ArqSTS = string.Empty;
            ArqTemp = string.Empty;
            GPExeName = string.Empty;
            Name = "CliSiTef";

            EnderecoIP = string.Empty;
            CodigoLoja = string.Empty;
            NumeroTerminal = string.Empty;
            Operador = string.Empty;
            Restricoes = string.Empty;

            DocumentoFiscal = string.Empty;
            dataHoraFiscal = null;

            OperacaoATV = 111; // 111 - Teste de comunicação com o SiTef
            OperacaoReImpressao = 112; // 112 - Menu Re-impressão
            OperacaoPRE = 115; // 115 - Pré-autorização
            OperacaoADM = 110; // 110 - Abre o leque das transações Gerenciais
            OperacaoCRT = 0; // A CliSiTef permite que o operador escolha a forma
            // de pagamento através de menus
            OperacaoCHQ = 1; // Cheque
            OperacaoCNC = 200; // 200 Cancelamento Normal: Inicia a coleta dos dados
            // no ponto necessário para fazer o cancelamento de uma
            // transação de débito ou crédito, sem ser necessário
            // passar antes pelo menu de transações administrativas

            documentosProcessados = string.Empty;
            ExibirErroRetorno = false;

            ParametrosAdicionais = new Dictionary<string, string>();
            Respostas = new Dictionary<string, string>();
        }

        #endregion Constructor

        #region Properties

        [Browsable(false)]
        public string PathDLL { get; set; }

        [Browsable(false)]
        public Dictionary<string, string> Respostas { get; }

        public string EnderecoIP { get; set; }

        public string CodigoLoja { get; set; }

        public string NumeroTerminal { get; set; }

        public string Operador { get; set; }

        public int PortaPinPad { get; set; }

        public string Restricoes { get; set; }

        public Dictionary<string, string> ParametrosAdicionais { get; }

        public string DocumentoFiscal
        {
            get => documentoFiscal.IsEmpty() ? documentoFiscal = DateTime.Now.ToString("hhmmss") : documentoFiscal;
            set => documentoFiscal = value;
        }

        public DateTime DataHoraFiscal
        {
            get => dataHoraFiscal ?? DateTime.Now;
            set => dataHoraFiscal = value;
        }

        public int OperacaoATV { get; set; }

        public int OperacaoADM { get; set; }

        public int OperacaoCRT { get; set; }

        public int OperacaoCHQ { get; set; }

        public int OperacaoCNC { get; set; }

        public int OperacaoPRE { get; set; }

        public int OperacaoReImpressao { get; set; }

        public bool ExibirErroRetorno { get; set; }

        #endregion Properties

        #region Methods

        /// <inheritdoc />
        public override void Inicializar()
        {
            if (Inicializado) return;

            Guard.Against<ACBrException>(OnExibeMenu == null, "Evento [OnExibeMenu] não programado");
            Guard.Against<ACBrException>(OnObtemCampo == null, "Evento [OnObtemCampo] não programado");

            client = new CliSitefClient(PathDLL);

            if (ParametrosAdicionais.ContainsKey("PortaPinPad"))
                ParametrosAdicionais["PortaPinPad"] = PortaPinPad.ToString();
            else
                ParametrosAdicionais.Add("PortaPinPad", PortaPinPad.ToString());

            var suportaDesconto = SuportaDesconto();
            if (!ParametrosAdicionais.ContainsKey("VersaoAutomacaoCielo") && suportaDesconto)
                ParametrosAdicionais.Add("VersaoAutomacaoCielo", $"{parent.Identificacao.SoftwareHouse.FillLeft(8)}10");

            var parametros = ParametrosAdicionais.ToAdicParam();

            this.Log().Info($"ConfiguraIntSiTefInterativoEx - EnderecoIP: {EnderecoIP} CodigoLoja: {CodigoLoja} " +
                            $"NumeroTerminal: {NumeroTerminal} Resultado: 0 ParametrosAdicionais: {parametros}");

            var ret = client.ConfiguraIntSiTefInterativoEx(EnderecoIP, CodigoLoja, NumeroTerminal, 0, parametros);
            var erro = string.Empty;
            switch (ret)
            {
                case 1:
                    erro = CacbrTefdCliSiTefErro1;
                    break;

                case 2:
                    erro = CacbrTefdCliSiTefErro2;
                    break;

                case 3:
                    erro = CacbrTefdCliSiTefErro3;
                    break;

                case 6:
                    erro = CacbrTefdCliSiTefErro6;
                    break;

                case 7:
                    erro = CacbrTefdCliSiTefErro7;
                    break;

                case 8:
                    erro = CacbrTefdCliSiTefErro8;
                    break;

                case 10:
                    erro = CacbrTefdCliSiTefErro10;
                    break;

                case 11:
                    erro = CacbrTefdCliSiTefErro11;
                    break;

                case 12:
                    erro = CacbrTefdCliSiTefErro12;
                    break;

                case 13:
                    erro = CacbrTefdCliSiTefErro13;
                    break;
            }

            Guard.Against<ACBrException>(!erro.IsEmpty(), erro);

            this.Log().Info($"{Name} Inicializado CliSiTEF");

            inicializado = true;
        }

        /// <inheritdoc />
        public override void Desinicializar()
        {
            client?.Dispose();
            client = null;
            base.Desinicializar();
        }

        /// <inheritdoc />
        public override void AtivarGP()
        {
            throw new ACBrException("CliSiTef não pode ser ativado localmente");
        }

        /// <inheritdoc />
        public override void VerificaAtivo()
        {
            //Nada a Fazer
        }

        /// <inheritdoc />
        public override void ATV()
        {
            var sts = FazerRequisicao(OperacaoATV, "ATV");

            if (sts == 10000)
                sts = ContinuarRequisicao(CacbrTefdCliSiTefImprimeGerencialConcomitante);

            if (sts != 0)
                AvaliaErro(sts);
            else
                ProcessarResposta();
        }

        /// <inheritdoc />
        public override bool ADM()
        {
            var sts = FazerRequisicao(OperacaoADM, "ADM", restricoes: Restricoes);

            if (sts == 10000)
                sts = ContinuarRequisicao(CacbrTefdCliSiTefImprimeGerencialConcomitante);

            if (sts != 0)
                AvaliaErro(sts);
            else
                ProcessarResposta();

            return sts == 0;
        }

        /// <inheritdoc />
        public override bool CRT(decimal valor, string indicePagamento, string documentoVinculado, int moeda)
        {
            if (valor != 0)
                VerificarTransacaoPagamento(valor);

            var restri = Restricoes;
            if (restri.IsEmpty())
                restri = "[10]"; // 10 - Cheques

            if (documentoVinculado.IsEmpty())
                documentoVinculado = DocumentoFiscal;

            var sts = FazerRequisicao(OperacaoCRT, "CRT", valor, documentoVinculado, restri);

            if (sts == 10000)
                sts = ContinuarRequisicao(CacbrTefdCliSiTefImprimeGerencialConcomitante);

            if (sts != 0)
                AvaliaErro(sts);
            else
                ProcessarRespostaPagamento(indicePagamento, valor);

            return sts == 0;
        }

        /// <inheritdoc />
        public override bool PRE(decimal valor, string indiceFpgEcf, string documentoVinculado, int moeda)
        {
            var sts = FazerRequisicao(OperacaoPRE, "PRE", valor, restricoes: Restricoes);

            if (sts == 10000)
                sts = ContinuarRequisicao(CacbrTefdCliSiTefImprimeGerencialConcomitante);

            if (sts != 0)
                AvaliaErro(sts);
            else
                ProcessarResposta();

            return sts == 0;
        }

        /// <inheritdoc />
        public override bool CHQ(decimal valor, string indicePagamento, string documentoVinculado, string cmc7,
            char tipoPessoa, string documentoPessoa, DateTime? dataCheque, string banco, string agencia,
            string agenciaDc, string conta, string contaDc, string cheque, string chequeDc, string compensacao)
        {
            var formataCampo = new Func<string, int, string>((campo, tamanho) => campo.OnlyNumbers().FillRight(tamanho));

            if (!documentoVinculado.IsEmpty() && valor != 0)
                VerificarTransacaoPagamento(valor);

            Respostas.Add("501", tipoPessoa == 'J' ? "1" : "0");

            if (!documentoPessoa.IsEmpty())
                Respostas.Add("502", documentoPessoa.OnlyNumbers());

            if (dataCheque.HasValue)
                Respostas.Add("506", dataCheque.Value.ToString("ddMMyyyy"));

            if (!cmc7.IsEmpty())
                Respostas.Add("517", $"1:{cmc7}");
            else
                Respostas.Add("517", $"0:{formataCampo(compensacao, 3)}{formataCampo(banco, 3)}{formataCampo(agencia, 4)}" +
                                     $"{formataCampo(agenciaDc, 1)}{formataCampo(conta, 10)}{formataCampo(contaDc, 1)}" +
                                     $"{formataCampo(cheque, 6)}{formataCampo(chequeDc, 1)}");

            var retri = Restricoes;
            if (retri.IsEmpty())
                retri = "[15;25]"; // 15 - Cartão Credito; 25 - Cartao Debito

            if (documentoVinculado.IsEmpty())
                documentoVinculado = DocumentoFiscal;

            var sts = FazerRequisicao(OperacaoCHQ, "CHQ", valor, documentoVinculado, retri);

            if (sts == 10000)
                sts = ContinuarRequisicao(CacbrTefdCliSiTefImprimeGerencialConcomitante);

            if (sts != 0)
                AvaliaErro(sts);
            else
                ProcessarRespostaPagamento(indicePagamento, valor);

            return sts == 0;
        }

        /// <inheritdoc />
        public override bool CNC(string rede, string nsu, DateTime dataHoraTransacao, decimal valor)
        {
            Respostas.Add("146", valor.ToString("0.00", CultureInfo.InvariantCulture));
            Respostas.Add("147", valor.ToString("0.00", CultureInfo.InvariantCulture));
            Respostas.Add("515", dataHoraTransacao.ToString("ddMMyyyy"));
            Respostas.Add("516", nsu);

            var sts = FazerRequisicao(OperacaoCNC, "CNC");

            if (sts == 10000)
                sts = ContinuarRequisicao(CacbrTefdCliSiTefImprimeGerencialConcomitante);

            if (sts != 0)
                AvaliaErro(sts);
            else
                ProcessarResposta();

            return sts == 0;
        }

        /// <inheritdoc />
        public override void CNF(string rede, string nsu, string finalizacao, string documentoVinculado)
        {
            // CliSiTEF não usa Rede, NSU e Finalizacao
            FinalizarTransacao(true, documentoVinculado);
        }

        /// <inheritdoc />
        public override void NCN(string rede, string nsu, string finalizacao, decimal valor, string documentoVinculado)
        {
            // CliSiTEF não usa Rede, NSU e Finalizacao
            FinalizarTransacao(false, documentoVinculado);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        public int DefineMensagemPermanentePinPad(string mensagem)
        {
            Guard.Against<ACBrException>(client == null, CacbrTefdCliSiTefNaoInicializado);

            return client.EscreveMensagemPermanentePinPad(mensagem);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cumpomFiscal"></param>
        /// <returns></returns>
        public int ObtemQuantidadeTransacoesPendentes(DateTime data, string cumpomFiscal)
        {
            Guard.Against<ACBrException>(client == null, CacbrTefdCliSiTefNaoInicializado);

            var sDate = data.ToString("yyyyMMdd");
            return client.ObtemQuantidadeTransacoesPendentes(sDate, cumpomFiscal);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="redeDestino"></param>
        /// <param name="funcaoSiTef"></param>
        /// <param name="offsetCartao"></param>
        /// <param name="dadosTx"></param>
        /// <param name="dadosRx"></param>
        /// <param name="codigoResposta"></param>
        /// <param name="tempoEsperaRx"></param>
        /// <param name="cupomFiscal"></param>
        /// <param name="confirmar"></param>
        /// <returns></returns>
        public int EnviaRecebeSiTefDireto(short redeDestino, short funcaoSiTef, short offsetCartao,
            string dadosTx, ref string dadosRx, ref short codigoResposta, short tempoEsperaRx,
            string cupomFiscal, bool confirmar)
        {
            var aNow = DateTime.Now;
            var dataStr = aNow.ToString("yyyyMMdd");
            var horaStr = aNow.ToString("HHmmss");
            var buffer = new StringBuilder(BufferSize);

            this.Log().Info($"EnviaRecebeSiTefDireto -> Rede: {redeDestino}, Funcao: {funcaoSiTef}, OffSetCartao: {offsetCartao}" +
                            $", DadosTX: {dadosTx}, TimeOut: {tempoEsperaRx},  Cupom: {cupomFiscal}" +
                            $", {(confirmar ? "Confirmar" : "Não Confirmar")}");

            Guard.Against<ACBrException>(client == null, CacbrTefdCliSiTefNaoInicializado);

            var result = client.EnviaRecebeSiTefDireto(redeDestino, funcaoSiTef, offsetCartao, dadosTx, (short)dadosTx.Length,
                buffer, BufferSize, ref codigoResposta, tempoEsperaRx, cupomFiscal, dataStr, horaStr, Operador, (short)(confirmar ? 1 : 0));

            dadosRx = buffer.ToString().Trim();
            return result;
        }

        public int ValidaCampoCodigoEmBarras(string dados, short tipo)
        {
            this.Log().Info($"ValidaCodigoEmBarras -> Dados: {dados}");

            Guard.Against<ACBrException>(client == null, CacbrTefdCliSiTefNaoInicializado);

            return client.ValidaCampoCodigoEmBarras(dados, tipo);
        }

        public bool VerificaPresencaPinPad()
        {
            this.Log().Info("VerificaPresencaPinpad");

            Guard.Against<ACBrException>(client == null, CacbrTefdCliSiTefNaoInicializado);

            //Retornos:
            //1: Existe um PinPad operacional conectado ao micro;
            // 0: Nao Existe um PinPad conectado ao micro;
            // -1: Biblioteca de acesso ao PinPad não encontrada

            return client.VerificaPresencaPinPad() == 1;
        }

        public void FinalizarTransacao(bool confirma, string documentoVinculado)
        {
            Respostas.Clear();
            iniciouRequisicao = false;

            if (reimpressao) return;
            if (documentoVinculado.IsIn(documentosProcessados.Split('|'))) return;

            documentosProcessados += $"{documentoVinculado}|";

            var dataStr = DateTime.Now.ToString("yyyyMMdd");
            var horaStr = DateTime.Now.ToString("hhmmss");
            var finalizacao = (confirma || cancelamento) ? 1 : 0;

            this.Log().Info($"*** FinalizaTransacaoSiTefInterativo. Confirma: {(finalizacao == 1 ? "Sim" : "Não")} " +
                            $"Documento: {documentoVinculado} Data: {dataStr} Hora: {horaStr}");

            client.FinalizaTransacaoSiTefInterativo(finalizacao, documentoVinculado, dataStr, horaStr);

            if (confirma) return;

            string aMsg;
            if (cancelamento)
            {
                aMsg = CacbrTefdCliSiTefTransacaoEfetuadaReImprimir.Substitute(Resposta.NSU);
            }
            else
            {
                EstadoVenda estado;
                try
                {
                    estado = Estado;
                }
                catch (Exception)
                {
                    estado = EstadoVenda.Outro;
                }

                aMsg = estado == EstadoVenda.Outro ? CacbrTefdCliSiTefTransacaoNaoEfetuada : CacbrTefdCliSiTefTransacaoNaoEfetuadaReterCupom;
            }

            parent.DoExibeMsg(OperacaoMensagem.OK, aMsg);
        }

        protected override void VerificarIniciouRequisicao()
        {
            Guard.Against<ACBrException>(!iniciouRequisicao, ACBrTEF.CacbrTefdErroSemRequisicao);
        }

        protected override void ProcessarResposta()
        {
            this.Log().Info($"{Name} ProcessarResposta: {Requisicao.Header}");

            parent.EstadoResp = RespEstado.Processando;

            if (Resposta.QtdLinhasComprovante < 1) return;

            // Cria cópia do Objeto Resp, e salva na lista "RespostasPendentes"
            var respostaPendente = Resposta.Clone();

            try
            {
                parent.RespostasPendentes.Add(respostaPendente);
                ImprimirRelatorio();
                parent.DoOnDepoisConfirmarTransacoes();
            }
            finally
            {
                parent.RespostasPendentes.Clear();
            }
        }

        protected override bool ProcessarRespostaPagamento(string indicePagamento, decimal valor)
        {
            this.Log().InfoFormat("{0} ProcessarRespostaPagamento: {1} Indice:{2} Valor: {3:c}", Name, Resposta.Header,
                indicePagamento, valor);

            //...Se está aqui, então a Transação foi aprovada...
            Resposta.IndicePagamento = indicePagamento;

            // Cria Arquivo de Backup, contendo Todas as Respostas
            CopiarResposta();

            //Cria cópia do Objeto Resp, e salva no ObjectList "RespostasPendentes"
            var respostaPendete = Resposta.Clone();
            parent.RespostasPendentes.Add(respostaPendete);

            return true;
        }

        protected override string CopiarResposta()
        {
            if (arqBackUp.IsEmpty()) return base.CopiarResposta();
            if (!UtilTEF.DeleteFile(arqBackUp)) throw new ACBrException($"Não foi possivel apagar o arquivo \"{arqBackUp}\" de backup!");

            arqBackUp = string.Empty;
            return base.CopiarResposta();
        }

        private int FazerRequisicao(int funcao, string header, decimal valor = 0, string documento = "", string restricoes = "")
        {
            var result = 0;

            Guard.Against<ACBrException>(client == null, CacbrTefdCliSiTefNaoInicializado);

            if (documento.IsEmpty()) documento = DocumentoFiscal;

            Requisicao.DocumentoVinculado = documento;
            Requisicao.ValorTotal = valor;

            Guard.Against<ACBrException>(AguardandoResposta, CacbrTefdCliSiTefNaoConcluido);

            if (restricoes.IndexOf(@"{TipoTratamento=4}", StringComparison.Ordinal) == -1 &&
                header.IsIn("CRT", "CHQ") && SuportaDesconto())
            {
                restricoes += "{TipoTratamento=4}";
            }

            iniciouRequisicao = true;

            var dataHora = DataHoraFiscal;
            var dataStr = dataHora.ToString("yyyyMMdd");
            var horaStr = dataHora.ToString("HHmmss");
            var valorStr = valor.ToString("N2");
            documentosProcessados = string.Empty;

            this.Log().Info($"*** IniciaFuncaoSiTefInterativo. Modalidade: {funcao} Valor: {valorStr} " +
                            $"Documento: {documento} Data: {dataStr} Hora: {horaStr} " +
                            $"Operador: {Operador} Restricoes: {restricoes}");

            result = client.IniciaFuncaoSiTefInterativo(funcao, valorStr, documento, dataStr, horaStr, Operador,
                restricoes);

            Resposta.Clear();
            idSeq++;
            if (documento.IsEmpty())
                documento = idSeq.ToString();

            Resposta.DocumentoVinculado = documento;
            Resposta.Conteudo.GravarInformacao(header, 899, 100);
            Resposta.Conteudo.GravarInformacao(idSeq, 899, 101);
            Resposta.Conteudo.GravarInformacao(documento, 899, 102);
            Resposta.Conteudo.GravarInformacao(Math.Truncate(Math.Round(valor * 100)), 899, 103);
            Resposta.TipoGP = Tipo;

            return result;
        }

        private int ContinuarRequisicao(bool imprimirComprovantes)
        {
            var processaMensagemTela = new Func<string, string>((mensagemProcessar) =>
            {
                mensagemProcessar = mensagemProcessar.Replace("@", Environment.NewLine);
                mensagemProcessar = mensagemProcessar.Replace("/n", Environment.NewLine);
                return mensagemProcessar;
            });

            var continua = 0;
            var captionMenu = string.Empty;
            var gerencialAberto = false;
            var impressaoOk = true;
            var houveImpressao = false;
            iniciouRequisicao = true;
            cancelamento = false;
            reimpressao = false;
            var interromper = false;
            arqBackUp = string.Empty;
            AguardandoResposta = true;
            var fechaGerencialAberto = true;

            try
            {
                parent.BloquearMouseTeclado();

                int result;
                var buffer = new StringBuilder(BufferSize);

                do
                {
                    this.Log().Info($"ContinuaFuncaoSiTefInterativo, Chamando: Continua = {continua} Buffer = {buffer}");

                    result = client.ContinuaFuncaoSiTefInterativo(out var proximoComando, out var tipoCampo, out var tamanhoMinimo,
                        out var tamanhoMaximo, buffer, BufferSize, continua);

                    continua = 0;
                    var mensagem = buffer.ToString().Trim();
                    var respostaSitef = string.Empty;
                    var voltar = false;
                    var digitado = true;

                    this.Log().Info($"ContinuaFuncaoSiTefInterativo, Retornos: STS = {result} ProximoComando = {(int)proximoComando} " +
                                    $"TipoCampo = {tipoCampo} Buffer = {mensagem} Tam.Min = {tamanhoMinimo} Tam.Max = {tamanhoMaximo}");

                    if (result == 10000)
                    {
                        if (tipoCampo > 0 && Respostas.ContainsKey(tipoCampo.ToString()))
                            respostaSitef = Respostas[tipoCampo.ToString()];

                        string mensagemCliente;
                        string mensagemOperador;
                        switch (proximoComando)
                        {
                            case CommandType.Store:
                                ((RetornoCliSiTef)Resposta).GravaInformacao(tipoCampo, mensagem);
                                switch (tipoCampo)
                                {
                                    case 15:
                                        ((RetornoCliSiTef)Resposta).GravaInformacao(tipoCampo, "True"); //Selecionou Debito;
                                        break;

                                    case 25:
                                        ((RetornoCliSiTef)Resposta).GravaInformacao(tipoCampo, "True"); //Selecionou Debito;
                                        break;

                                    case 29:
                                        ((RetornoCliSiTef)Resposta).GravaInformacao(tipoCampo, "True"); // Cartão Digitado;
                                        break;

                                    case 56:
                                    case 57:
                                    case 58:
                                        reimpressao = true;
                                        break;

                                    case 110:
                                        cancelamento = true;
                                        break;

                                    case 121:
                                    case 122:
                                        if (imprimirComprovantes)
                                        {
                                            //Impressão de Gerencial, deve ser Sob demanda
                                            if (!houveImpressao)
                                            {
                                                houveImpressao = true;
                                                arqBackUp = CopiarResposta();
                                            }

                                            impressaoOk = false;
                                            var i = tipoCampo;
                                            try
                                            {
                                                while (!impressaoOk)
                                                {
                                                    try
                                                    {
                                                        while (i <= tipoCampo)
                                                        {
                                                            if (fechaGerencialAberto)
                                                            {
                                                                var estado = Estado;
                                                                switch (estado)
                                                                {
                                                                    case EstadoVenda.CupomVinculado:
                                                                        parent.DoComandaVenda(OperacaoVenda.FechaVinculado);
                                                                        break;

                                                                    case EstadoVenda.RelatorioGerencial:
                                                                        parent.DoComandaVenda(OperacaoVenda.FechaGerencial);
                                                                        break;

                                                                    case EstadoVenda.Venda:
                                                                    case EstadoVenda.Pagamento:
                                                                    case EstadoVenda.NaoFiscal:
                                                                        parent.DoComandaVenda(OperacaoVenda.CancelaCupom);
                                                                        break;
                                                                }

                                                                gerencialAberto = false;
                                                                fechaGerencialAberto = false;

                                                                Guard.Against<ACBrTEFPrintException>(Estado != EstadoVenda.Livre, ACBrTEF.CacbrTefdErroEcfNaoLivre);
                                                            }

                                                            mensagem = Resposta.LeInformacao(i).AsString();
                                                            if (!mensagem.IsEmpty())
                                                            {
                                                                if (!gerencialAberto)
                                                                {
                                                                    parent.DoComandaVenda(OperacaoVenda.AbreGerencial);
                                                                    gerencialAberto = true;
                                                                }
                                                                else
                                                                {
                                                                    parent.DoComandaVenda(OperacaoVenda.PulaLinhas);
                                                                    parent.DoExibeMsg(OperacaoMensagem.DestaqueVia, ACBrTEF.CacbrTefdDestaqueVia.Substitute(1));
                                                                }

                                                                parent.DoVendaImprimeVia(TipoRelatorio.Gerencial,
                                                                    i - 120, mensagem.Split((char)10));
                                                                impressaoOk = true;
                                                            }

                                                            i++;
                                                        }

                                                        if (tipoCampo == 122 && gerencialAberto)
                                                        {
                                                            parent.DoComandaVenda(OperacaoVenda.FechaGerencial);
                                                            gerencialAberto = false;
                                                        }
                                                    }
                                                    catch (ACBrTEFPrintException)
                                                    {
                                                        impressaoOk = false;
                                                    }

                                                    if (impressaoOk) continue;
                                                    if (parent.DoExibeMsg(OperacaoMensagem.YesNo, ACBrTEF.CacbrTefdErroEcfNaoResponde) != ModalResult.Yes) break;

                                                    i = 121;
                                                    fechaGerencialAberto = true;
                                                }
                                            }
                                            finally
                                            {
                                                if (!impressaoOk)
                                                    continua = -1;
                                            }
                                        }
                                        break;

                                    case 133:
                                    case 952:
                                        arqBackUp = CopiarResposta();
                                        break;
                                }
                                break;

                            case CommandType.DisplayOperatorMessage:
                                mensagemOperador = processaMensagemTela(mensagem);
                                parent.DoExibeMsg(OperacaoMensagem.ExibirMsgOperador, mensagemOperador, tipoCampo == 5005);
                                break;

                            case CommandType.DisplayCustomerMessage:
                                mensagemCliente = processaMensagemTela(mensagem);
                                parent.DoExibeMsg(OperacaoMensagem.ExibirMsgCliente, mensagemCliente, tipoCampo == 5005);
                                break;

                            case CommandType.DisplayMessage:
                                mensagemOperador = processaMensagemTela(mensagem);
                                mensagemCliente = mensagemOperador;
                                parent.DoExibeMsg(OperacaoMensagem.ExibirMsgOperador, mensagemOperador, tipoCampo == 5005);
                                parent.DoExibeMsg(OperacaoMensagem.ExibirMsgCliente, mensagemCliente, tipoCampo == 5005);
                                break;

                            case CommandType.DisplayMenuHeader:
                                captionMenu = mensagem;
                                break;

                            case CommandType.ClearOperatorMessage:
                                parent.DoExibeMsg(OperacaoMensagem.RemoverMsgOperador);
                                break;

                            case CommandType.ClearCustomerMessage:
                                parent.DoExibeMsg(OperacaoMensagem.RemoverMsgCliente);
                                break;

                            case CommandType.ClearMessage:
                                parent.DoExibeMsg(OperacaoMensagem.RemoverMsgOperador);
                                parent.DoExibeMsg(OperacaoMensagem.RemoverMsgCliente);
                                break;

                            case CommandType.ClearMenuHeader:
                                captionMenu = string.Empty;
                                break;

                            case CommandType.DisplayConfirm:
                                if (mensagem.IsEmpty())
                                    mensagem = "CONFIRMA ?";

                                respostaSitef = parent.DoExibeMsg(OperacaoMensagem.YesNo, mensagem) == ModalResult.Yes ? "0" : "1";
                                if (tipoCampo == 5013 && respostaSitef == "1") interromper = false;
                                break;

                            case CommandType.DisplayMenuOptions:
                                var itens = mensagem.Split(';');
                                parent.BloquearMouseTeclado(false);

                                var exibeMenuEventArgs = new ExibeMenuEventArgs(captionMenu, itens);
                                OnExibeMenu.Raise(this, exibeMenuEventArgs);

                                voltar = exibeMenuEventArgs.VoltarMenu;
                                if (!voltar)
                                {
                                    if (exibeMenuEventArgs.ItemSelecionado >= 0 && exibeMenuEventArgs.ItemSelecionado < itens.Length)
                                        respostaSitef = itens[exibeMenuEventArgs.ItemSelecionado].Substring(itens[exibeMenuEventArgs.ItemSelecionado].IndexOf(":") - 1, 1);
                                    else
                                        digitado = false;
                                }
                                break;

                            case CommandType.WaitAnyKey:
                                if (mensagem.IsEmpty())
                                    mensagem = CacbrTefdCliSiTefPressioneEnter;

                                parent.DoExibeMsg(OperacaoMensagem.OK, mensagem);
                                break;

                            case CommandType.CancelPinPadOperation:
                                interromper = false;

                                var aguardaRespEventArgs = new AguardaRespEventArgs("23", 0, interromper);
                                parent.DoOnAguardaResp(aguardaRespEventArgs);
                                interromper = aguardaRespEventArgs.Interromper;
                                if (interromper)
                                    continua = -1;
                                break;

                            case CommandType.TextInputNeeded:
                                parent.BloquearMouseTeclado(false);
                                var obterCampoTexto = new ObtemCampoEventArgs(mensagem, tamanhoMinimo, tamanhoMaximo, tipoCampo, OperacaoCampo.String);
                                OnObtemCampo.Raise(this, obterCampoTexto);
                                respostaSitef = obterCampoTexto.Resposta;
                                ((RetornoCliSiTef)Resposta).GravaInformacao(tipoCampo, respostaSitef);
                                parent.BloquearMouseTeclado();
                                break;

                            case CommandType.CheckInputNeeded:
                                parent.BloquearMouseTeclado(false);
                                var obterCampoCheque = new ObtemCampoEventArgs(mensagem, tamanhoMinimo, tamanhoMaximo, tipoCampo, OperacaoCampo.CMC7);
                                OnObtemCampo.Raise(this, obterCampoCheque);
                                respostaSitef = obterCampoCheque.Resposta;
                                parent.BloquearMouseTeclado();
                                break;

                            case CommandType.MoneyInputNeeded:
                                parent.BloquearMouseTeclado(false);
                                var obterCampoDecimal = new ObtemCampoEventArgs(mensagem, tamanhoMinimo, tamanhoMaximo, tipoCampo, OperacaoCampo.Double);
                                OnObtemCampo.Raise(this, obterCampoDecimal);
                                respostaSitef = obterCampoDecimal.Resposta;
                                parent.BloquearMouseTeclado();
                                break;

                            case CommandType.BarcodeInputNeeded:
                                parent.BloquearMouseTeclado(false);
                                var obterCampoBarcode = new ObtemCampoEventArgs(mensagem, tamanhoMinimo, tamanhoMaximo, tipoCampo, OperacaoCampo.BarCode);
                                OnObtemCampo.Raise(this, obterCampoBarcode);
                                respostaSitef = obterCampoBarcode.Resposta;
                                parent.BloquearMouseTeclado();
                                break;
                        }
                    }
                    else
                    {
                        this.Log().Info($"*** ContinuaFuncaoSiTefInterativo, Finalizando: STS = {result}");
                    }

                    if (voltar) continua = 1;
                    else if (!digitado || interromper) continua = -1;

                    if (voltar && result == 10000 || !digitado)
                    {
                        parent.DoExibeMsg(OperacaoMensagem.RemoverMsgOperador);
                        parent.DoExibeMsg(OperacaoMensagem.RemoverMsgCliente);
                    }

                    buffer.Clear();
                    buffer.Append(respostaSitef);
                } while (result == 10000);

                return result;
            }
            finally
            {
                if (gerencialAberto)
                {
                    try
                    {
                        parent.DoComandaVenda(OperacaoVenda.FechaGerencial);
                    }
                    catch (Exception)
                    {
                        impressaoOk = false;
                    }
                }

                if (arqBackUp.IsEmpty() && File.Exists(arqBackUp))
                    UtilTEF.DeleteFile(arqBackUp);

                if (houveImpressao || imprimirComprovantes || cancelamento)
                    FinalizarTransacao(impressaoOk, Resposta.DocumentoVinculado);

                parent.BloquearMouseTeclado(false);

                this.Log().Debug(Resposta.Conteudo.AsString());

                // Transfere valore de "Conteudo" para as propriedades
                Resposta.ConteudoToProperty();

                if (houveImpressao && cancelamento)
                    parent.DoExibeMsg(OperacaoMensagem.OK, CacbrTefdCliSiTefTransacaoEfetuadaReImprimir.Substitute(Resposta.NSU));

                AguardandoResposta = false;
            }
        }

        private void AvaliaErro(int sts)
        {
            if (!ExibirErroRetorno) return;

            var erro = string.Empty;
            switch (sts)
            {
                case -1:
                    erro = "Módulo não inicializado";
                    break;

                case -2:
                    erro = "Operação cancelada pelo operador";
                    break;

                case -3:
                    erro = "Fornecido um código de função inválido";
                    break;

                case -4:
                    erro = "Falta de memória para rodar a função";
                    break;

                case -5:
                    erro = "";
                    break; // 'Sem comunicação com o SiTef' ; // Comentado pois SiTEF já envia a msg de Erro

                case -6:
                    erro = "Operação cancelada pelo usuário";
                    break;

                case -40:
                    erro = "Transação negada pelo SiTef";
                    break;

                case -43:
                    erro = "Falha no pinpad";
                    break;

                case -50:
                    erro = "Transação não segura";
                    break;

                case -100:
                    erro = "Erro interno do módulo";
                    break;

                default:
                    erro = sts < 0
                        ? $"Erros detectados internamente pela rotina ({sts})"
                        : $"Negada pelo autorizador ({sts})";
                    break;
            }

            if (!erro.IsEmpty())
                parent.DoExibeMsg(OperacaoMensagem.OK, erro);
        }

        private bool SuportaDesconto()
        {
            return !parent.Identificacao.SoftwareHouse.IsEmpty() &&
                   parent.EventAssigned(nameof(parent.OnComandaVendaSubtotaliza));
        }

        #endregion Methods
    }
}