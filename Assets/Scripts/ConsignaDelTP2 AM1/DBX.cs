using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class DBX
{
    static string logFileName = "debug_log.txt";
    static string logPath;

    static DBX()
    {
        // Determinar plataforma en runtime
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            logPath = Application.persistentDataPath + "/" + logFileName;
        }
        else
        {
            logPath = Path.Combine(Application.persistentDataPath, logFileName);
        }
    }

    public static void L() => L("I", "Llegó hasta aquí");

    public static void L(object msg, params object[] extra) => L("I", msg, extra);

    public static void L(string tipo, object msg, params object[] extra)
    {
        var frame = new StackTrace(true).GetFrame(1);
        string s = frame.GetMethod().DeclaringType.Name;
        string m = frame.GetMethod().Name;
        int l = frame.GetFileLineNumber();
        string t = DateTime.Now.ToString("HH:mm:ss");
        int h = System.Threading.Thread.CurrentThread.ManagedThreadId;
        string p = (extra != null && extra.Length > 0) ? "[P: " + string.Join(", ", extra) + "]" : "";
        string rgb = HexColorFromText(s);

        string raw = $"[{t}][{h}][{tipo}][S:{s}][M:{m}][L:{l}] → {msg} {p}";
        string consoleMsg = $"<color=#{rgb}>{raw}</color>";

        switch (tipo)
        {
            case "W": UnityEngine.Debug.LogWarning(consoleMsg); break;
            case "E": UnityEngine.Debug.LogError(consoleMsg); break;
            default: UnityEngine.Debug.Log(consoleMsg); break;
        }

        GuardarLog(raw);
    }

    private static void GuardarLog(string linea)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL solo soporta PlayerPrefs o IndexedDB (a través de UnityWebRequest o JS plugins)
            // Usamos PlayerPrefs como workaround (no escalable, pero útil para pruebas)
            int count = PlayerPrefs.GetInt("log_count", 0);
            PlayerPrefs.SetString("log_" + count, linea);
            PlayerPrefs.SetInt("log_count", count + 1);
            PlayerPrefs.Save();
        }
        else
        {
            File.AppendAllText(logPath, linea + "\n");
        }
    }

    private static string HexColorFromText(string text)
    {
        int hash = text.GetHashCode();
        byte r = (byte)((hash >> 16) & 0xFF);
        byte g = (byte)((hash >> 8) & 0xFF);
        byte b = (byte)(hash & 0xFF);
        return $"{r:X2}{g:X2}{b:X2}";
    }
}