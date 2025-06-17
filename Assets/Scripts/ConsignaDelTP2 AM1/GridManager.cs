using System.Collections.Generic;
using UnityEngine;
using RTS; // Asegúrate de tener acceso al namespace donde está PingManager

namespace RTS.GridSystem
{
    public static class GridManager
    {
        public static Dictionary<Vector2Int, List<EntidadBase>> grilla = new();
        public static Dictionary<EntidadBase, Vector2Int> mapaEntidades = new();
        public static float tamanioCelda = 5f;

        public static Vector2Int PosToCelda(Vector3 pos)
        {
            return new Vector2Int(
                Mathf.FloorToInt(pos.x / tamanioCelda),
                Mathf.FloorToInt(pos.z / tamanioCelda)
            );
        }

        public static void Registrar(EntidadBase e)
        {
            var celda = e.CeldaActual;
            if (!grilla.ContainsKey(celda))
                grilla[celda] = new List<EntidadBase>();
            grilla[celda].Add(e);
            mapaEntidades[e] = celda;
        }

        public static void Remover(EntidadBase e)
        {
            if (mapaEntidades.TryGetValue(e, out var celda))
            {
                grilla[celda]?.Remove(e);
                mapaEntidades.Remove(e);
            }
        }

        public static void ActualizarCelda(EntidadBase e, Vector3 nuevaPos)
        {
            var nueva = PosToCelda(nuevaPos);
            if (mapaEntidades.TryGetValue(e, out var vieja) && nueva != vieja)
            {
                grilla[vieja]?.Remove(e);
                if (!grilla.ContainsKey(nueva))
                    grilla[nueva] = new List<EntidadBase>();
                grilla[nueva].Add(e);
                mapaEntidades[e] = nueva;
            }
        }

        public static List<EntidadBase> ObtenerCercanos(Vector3 pos, float rango)
        {
            Vector2Int centro = PosToCelda(pos);
            int r = Mathf.CeilToInt(rango / tamanioCelda);
            List<EntidadBase> resultado = new();

            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    Vector2Int celda = centro + new Vector2Int(x, y);
                    if (grilla.TryGetValue(celda, out var lista))
                    {
                        //Debug.Log(centro);
                        resultado.AddRange(lista);
                    }
                }
            }

            return resultado;
        }

        // Método para completar el mapa de entidades si está vacío pero hay entidades en PingManager
        public static void CompletarMapaEntidadesSiVacio()
        {
            if (mapaEntidades.Count == 0 && PingManager.todasEntidades.Count > 0)
            {
                foreach (var entidad in PingManager.todasEntidades)
                {
                    if (entidad != null && !mapaEntidades.ContainsKey(entidad))
                    {
                        var celda = entidad.CeldaActual;
                        if (!grilla.ContainsKey(celda))
                            grilla[celda] = new List<EntidadBase>();
                        grilla[celda].Add(entidad);
                        mapaEntidades[entidad] = celda;
                    }
                }
            }
        }
    }
}