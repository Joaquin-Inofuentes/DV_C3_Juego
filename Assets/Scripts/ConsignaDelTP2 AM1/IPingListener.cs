namespace RTS.Comunicacion
{
    public interface IPingListener<T>
    {
        void RecibirPing(T emisor, TipoMensaje tipo);
    }
}