namespace SPSA.Autorizadores.Aplicacion.DTO
{
	public enum EstadoMonitor
	{
		CIERRE_REALIZADO = 1,
		PENDIENTE_VALIDACION_CIERRE = 2,
		NO_SE_HA_REALIZADO_CIERRE = 3,
		SI = 4,
		NO = 5
	}

	public enum TipoMonitor
	{
		CIERRE_FIN_DIA = 1,
		CAJA_DEFECTUOSA = 2
	}

	public enum ColorSemaforo
	{
		VERDE = 1,
		NARANJA = 2,
		ROJO = 3
	}
}
