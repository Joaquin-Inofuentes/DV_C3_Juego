using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionesDeClases
{

    public static bool ContieneComponente<TComponent, TValue>(this IList<TValue> lista)
        where TComponent : Component
        where TValue : Component
    {
        return lista.Any(e => e != null && e.gameObject.GetComponent<TComponent>() != null);
    }

    public static int IndiceConComponente<TComponent, TValue>(this IList<TValue> lista)
    where TComponent : Component
    where TValue : Component
    {
        for (int i = 0; i < lista.Count; i++)
        {
            var e = lista[i];
            if (e != null && e.gameObject.GetComponent<TComponent>() != null)
                return i;
        }
        return -1;
    }
}
