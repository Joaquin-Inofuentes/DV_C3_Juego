using System;

namespace RTS.Comunicacion
{
    public static class EventoPing
    {
        public static Action<IPingSender<EntidadBase>, IPingListener<EntidadBase>, TipoMensaje> OnPing;
    }
}