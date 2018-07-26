namespace ACBr.Net.TEF
{
	public enum ReqEstado
	{
		#region Documentation

		/// <summary>
		/// Nennhuma Requisição em andamento
		/// </summary>

		#endregion Documentation

		Nenhum,

		#region Documentation

		/// <summary>
		/// Iniciando uma nova Requisicao
		/// </summary>

		#endregion Documentation

		Iniciando,

		#region Documentation

		/// <summary>
		/// Arquivo Temporário de requisição está sendo criado
		/// </summary>

		#endregion Documentation

		CriandoArquivo,

		#region Documentation

		/// <summary>
		/// Requisição Escrita, Aguardando Resposta
		/// </summary>

		#endregion Documentation

		AguardandoResposta,

		#region Documentation

		/// <summary>
		/// Verifica se o STS é válido
		/// </summary>

		#endregion Documentation

		ConferindoResposta,

		#region Documentation

		/// <summary>
		/// Finalizada
		/// </summary>

		#endregion Documentation

		Finalizada
	}
}