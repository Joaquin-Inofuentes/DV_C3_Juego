namespace RTS.Comunicacion
{
    public interface IPingSender<T>
    {
        void EnviarPing(TipoMensaje tipo);
    }
}