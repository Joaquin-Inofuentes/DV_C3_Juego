using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using CustomInspector;
using System.Collections;
using System.Xml.Serialization;
using DG.Tweening;

/// <summary>
/// TimerManager gestiona 6 cooldowns (3 mágicos + 3 melee) y feedback visual.
/// Incluye flash rojo, shake, y animación visual cuando una habilidad está lista.
/// </summary>
public class TimerManager : MonoBehaviour
{
    [Button(nameof(SetTimerToMax), true)]
    public int IndiceAReiniciar = 0;

    [Header("⚔️ Modo actual")]
    public bool enModoMagico = true;
    public bool magiaBloqueadaPorZona = false;


    [Header("⏱ Timers Individuales (6 habilidades)")]
    public float[] timers = new float[6];
    public float[] maxTimers = new float[6];
    public bool[] isTimerZero = new bool[6];
    public float[] timersPercent = new float[6];

    [Header("🧰 Wrappers de habilidad (padre de icono + overlay)")]
    public RectTransform[] wrappers = new RectTransform[6];

    [Header("✨ Efecto visual cuando la habilidad está lista")]
    public float effectDuration = 0.5f;
    public float pulseSpeed = 4f;
    public float pulseAmount = 0.1f;

    [Header("⚡ Flash cuando se habilita la habilidad")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.15f;

    [Header("👤 Referencia a AccionesJugador")]
    public AccionesJugador _AccionesDeJugador;

    [Header("⚡ Eventos cuando el timer llega a 0")]
    public UnityEvent[] onTimerFinished = new UnityEvent[6];

    private Coroutine[] flashCoroutines = new Coroutine[6];
    private Color[] originalColors;
    private Vector2[] originalIconPositions;
    private Vector2[] originalOverlayPositions;
    private float[] readyEffectTimers = new float[6];
    private bool[] yaHizoEfecto = new bool[6];
    public bool enTransicionVisual = false;
    private Vector2[] originalWrapperPositions;


    public static TimerManager Controler;

    void Awake()
    {
        Controler = this;
    }
    void Start()
    {
        originalColors = new Color[6];
        originalIconPositions = new Vector2[6];
        originalOverlayPositions = new Vector2[6];
        yaHizoEfecto = new bool[6];

        originalWrapperPositions = new Vector2[wrappers.Length];

        // 2) Guarda la posición de cada wrapper
        for (int i = 0; i < wrappers.Length; i++)
        {
            if (wrappers[i] == null) continue;

            // Guardar posición del wrapper
            originalWrapperPositions[i] = wrappers[i].anchoredPosition;

            // Guardar posición de hijos
            var iconRT = wrappers[i].GetChild(0) as RectTransform;
            if (iconRT != null)
                originalIconPositions[i] = iconRT.anchoredPosition;

            var overlayRT = wrappers[i].GetChild(1) as RectTransform;
            if (overlayRT != null)
                originalOverlayPositions[i] = overlayRT.anchoredPosition;

            // Activar solo los del modo actual
            bool esMagia = i <= 2;
            bool perteneceAlModo = enModoMagico ? esMagia : !esMagia;
            bool estaEnCooldown = timers[i] > 0f;
            bool deberiaEstarActivo = perteneceAlModo || estaEnCooldown;
            wrappers[i].gameObject.SetActive(deberiaEstarActivo);
        }


    }

    void Update()
    {

        if (enTransicionVisual) return;
        EjecutarTimers();
    }

    public void EjecutarTimers()
    {
        for (int i = 0; i < 6; i++)
        {
            if (timers[i] > 0f)
            {
                timers[i] -= Time.deltaTime;
                if (timers[i] < 0f) timers[i] = 0f;
            }

            timersPercent[i] = maxTimers[i] > 0f ? timers[i] / maxTimers[i] : 0f;
            isTimerZero[i] = timers[i] <= 0f;

            var wrapper = wrappers[i];
            if (wrapper == null) continue;

            bool esMagia = i <= 2;
            bool perteneceAlModo = enModoMagico ? esMagia : !esMagia;
            bool estaEnCooldown = timers[i] > 0f;
            bool deberiaEstarActivo = perteneceAlModo || estaEnCooldown;

            if (!wrapper.gameObject.activeSelf && deberiaEstarActivo)
            {
                wrapper.gameObject.SetActive(true);
            }

            // Obtenemos el Image del overlay (hijo #1)
            var overlayRT = wrapper.GetChild(1) as RectTransform;
            var overlayImg = overlayRT?.GetComponent<Image>();
            if (overlayImg != null)
                overlayImg.fillAmount = timersPercent[i];

            if (isTimerZero[i] && Mathf.Approximately(timers[i], 0f))
            {
                if (!yaHizoEfecto[i])
                {
                    readyEffectTimers[i] = effectDuration;
                    StartCoroutine(DoFlashEffect(i));
                    onTimerFinished[i]?.Invoke();
                    yaHizoEfecto[i] = true;
                }
            }
            else if (!isTimerZero[i])
            {
                yaHizoEfecto[i] = false; // Se reinicia cuando vuelva a cargar
            }

            // Solo saltamos si NO pertenece al modo actual y NO está en TimerDeCombos
            if (!perteneceAlModo && !estaEnCooldown)
                continue;

            if (readyEffectTimers[i] > 0f)
            {
                readyEffectTimers[i] -= Time.deltaTime;

                var w = wrappers[i];
                if (w != null)
                {
                    float scaleFactor = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
                    w.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
                }

                if (readyEffectTimers[i] <= 0f && w != null)
                {
                    w.localScale = Vector3.one;
                }
            }
            else
            {
                var w = wrappers[i];
                if (w != null)
                    w.localScale = Vector3.one;
            }
        }

        // 🔽 NUEVO BLOQUE: oculta íconos que ya no deberían estar activos
        for (int i = 0; i < 6; i++)
        {
            var wrapper = wrappers[i];
            if (wrapper == null) continue;

            bool esMagia = i <= 2;
            bool perteneceAlModo = enModoMagico ? esMagia : !esMagia;
            bool estaEnCooldown = timers[i] > 0f;
            bool deberiaEstarActivo = perteneceAlModo || estaEnCooldown;

            if (wrapper.gameObject.activeSelf != deberiaEstarActivo)
            {
                wrapper.gameObject.SetActive(deberiaEstarActivo);
            }
        }
    }


    private IEnumerator DoFlashEffect(int index)
    {
        // 1) Toma el wrapper de la habilidad
        var wrapper = wrappers[index];
        if (wrapper == null) yield break;

        // 2) Dentro del wrapper, el primer hijo (GetChild(0)) es tu IconoCompleto
        var iconRT = wrapper.GetChild(0) as RectTransform;
        if (iconRT == null) yield break;


        var image = iconRT.GetComponent<Image>();
        if (image == null) yield break;

        Color original = image.color;

        Color intenseFlash = new Color(flashColor.r * 4f, flashColor.g * 4f, flashColor.b * 4f, 1f);

        float halfDuration = flashDuration / 2f;
        float t = 0f;

        // Fade in
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float lerp = t / halfDuration;
            image.color = Color.Lerp(original, intenseFlash, lerp);
            yield return null;
        }

        t = 0f;

        // Fade out
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float lerp = t / halfDuration;
            image.color = Color.Lerp(intenseFlash, original, lerp);
            yield return null;
        }

        image.color = original;
    }


    public void SetTimerToMax(int index)
    {
        if (index < 0 || index >= 6) return;

        timers[index] = maxTimers[index];

        // Asegurar que el ícono sea visible si está en TimerDeCombos
        bool esMagia = index <= 2;
        bool perteneceAlModo = enModoMagico ? esMagia : !esMagia;
        bool estaEnCooldown = timers[index] > 0f;

        bool deberiaEstarActivo = perteneceAlModo || estaEnCooldown;

        if (wrappers[index] != null && !wrappers[index].gameObject.activeSelf && deberiaEstarActivo)
        {
            wrappers[index].gameObject.SetActive(true);
        }
    }


    public bool IsTimerCharging(int index)
    {
        //Debug.Log("Se pregunto el valor de carga para : " + index, gameObject);
        if (index < 0 || index >= 6) return false;
        return timers[index] > 0f;
    }

    public void MostrarFeedbackNoDisponible(int index)
    {
        // Validación de modo
        bool esMagia = index <= 2;
        if ((enModoMagico && !esMagia) || (!enModoMagico && esMagia))
            return;

        // Validación de índice
        if (index < 0 || index >= wrappers.Length) return;

        // Obtenemos el wrapper
        var wrapper = wrappers[index];
        if (wrapper == null) return;

        // Obtenemos el overlay como hijo #1
        var overlayRT = wrapper.GetChild(1) as RectTransform;
        var overlayImg = overlayRT?.GetComponent<Image>();
        if (overlayImg == null) return;

        // Si ya había una coroutine activa, la detenemos y restauramos estado
        if (flashCoroutines[index] != null)
        {
            StopCoroutine(flashCoroutines[index]);

            // Restauramos color y posición del overlay
            overlayImg.color = originalColors[index];
            overlayRT.anchoredPosition = originalOverlayPositions[index];

            // Restauramos la posición del icono completo (hijo #0)
            var iconRT = wrapper.GetChild(0) as RectTransform;
            if (iconRT != null)
                iconRT.anchoredPosition = originalIconPositions[index];
        }

        // Iniciamos la nueva rutina de flash+shake
        flashCoroutines[index] = StartCoroutine(FlashAndShake(index));
    }


    private IEnumerator FlashAndShake(int index)
    {
        // 1) Obtener el wrapper
        var wrapper = wrappers[index];
        if (wrapper == null) yield break;

        // 2) Obtener el overlay (hijo #1) y su RectTransform
        var overlayRT = wrapper.GetChild(1) as RectTransform;
        var overlayImg = overlayRT?.GetComponent<Image>();
        if (overlayImg == null) yield break;

        // 3) Obtener el icono (hijo #0) y su RectTransform
        var iconRT = wrapper.GetChild(0) as RectTransform;
        if (iconRT == null) yield break;

        // 4) Variables originales
        var origColor = originalColors[index];
        var origOverlayPos = originalOverlayPositions[index];
        var origIconPos = originalIconPositions[index];

        float duration = 0.15f;
        float elapsed = 0f;

        // 5) Loop de shake
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Vector2 shake = Random.insideUnitCircle * 5f;

            // Aplicar shake
            overlayImg.color = Color.red;
            overlayRT.anchoredPosition = origOverlayPos + shake;
            iconRT.anchoredPosition = origIconPos + shake;

            yield return null;
        }

        // 6) Restaurar estado
        overlayImg.color = origColor;
        overlayRT.anchoredPosition = origOverlayPos;
        iconRT.anchoredPosition = origIconPos;

        flashCoroutines[index] = null;
    }

    public static bool CambiandoDeModo;
    public void TransicionarModoVisual()
    {
        CambiandoDeModo = true;
        try
        {

            if (!enTransicionVisual)
            {
                EjecutarTimers();
            }
            else
            {
                ActualizarTimersDuranteTransicion();
            }

            enTransicionVisual = true;
            DOTween.Kill(this);

            bool modoAnterior = enModoMagico;
            enModoMagico = !enModoMagico;

            int[] actuales = modoAnterior ? new[] { 0, 1, 2 } : new[] { 3, 4, 5 };
            int[] nuevos = enModoMagico ? new[] { 0, 1, 2 } : new[] { 3, 4, 5 };
            float d = 0.4f;

            Sequence seq = DOTween.Sequence();

            foreach (int i in actuales)
            {
                RectTransform w = wrappers[i];
                if (w == null) continue;

                CanvasGroup cg = w.GetComponent<CanvasGroup>() ?? w.gameObject.AddComponent<CanvasGroup>();

                w.gameObject.SetActive(true);
                w.localEulerAngles = Vector3.zero;
                cg.alpha = 1;
                w.anchoredPosition = originalWrapperPositions[i];

                // ⚠ Mini secuencia separada para cada ícono
                Sequence mini = DOTween.Sequence();
                mini.Append(cg.DOFade(0, d / 2f));
                mini.Join(w.DOLocalRotate(new Vector3(0, 90, 0), d / 2f, RotateMode.FastBeyond360));

                RectTransform wCopy = w;
                mini.AppendCallback(() => wCopy.gameObject.SetActive(false));
                // Sumás esta mini secuencia al grupo
                seq.Join(mini);
            }

            seq.AppendInterval(0.05f);

            // 2) Animación de entrada de los nuevos
            // 2) Animación de entrada de los nuevos, uno por uno
            foreach (int i in nuevos)
            {
                RectTransform w = wrappers[i];
                CanvasGroup cg = w.GetComponent<CanvasGroup>() ?? w.gameObject.AddComponent<CanvasGroup>();

                w.gameObject.SetActive(true);
                w.anchoredPosition = originalWrapperPositions[i];
                w.localEulerAngles = new Vector3(0, -90, 0);
                cg.alpha = 0;

                // ⚠️ Guardar referencia temporal para el closure (por seguridad)
                RectTransform wTemp = w;
                CanvasGroup cgTemp = cg;

                // Crear una mini secuencia por ícono
                Sequence miniEntrada = DOTween.Sequence();
                miniEntrada.Join(cgTemp.DOFade(1, d / 2f));
                miniEntrada.Join(wTemp.DOLocalRotate(Vector3.zero, d / 2f).SetEase(Ease.OutBack));
                seq.Append(miniEntrada);

            }

            seq.OnComplete(() =>
        {
            enTransicionVisual = false;

            for (int i = 0; i < wrappers.Length; i++)
            {
                if (wrappers[i] != null)
                {
                    wrappers[i].anchoredPosition = originalWrapperPositions[i];

                    // Restaurar visibilidad
                    var cg = wrappers[i].GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        cg.alpha = 1f;
                        cg.interactable = true;
                        cg.blocksRaycasts = true;
                    }

                    // Asegurar que estén activos los del modo correcto
                    bool esMagia = i <= 2;
                    bool perteneceAlModo = enModoMagico ? esMagia : !esMagia;
                    bool estaEnCooldown = timers[i] > 0f;

                    // Mostrar si es del modo actual o si todavía está cargando
                    bool deberiaEstarActivo = perteneceAlModo || estaEnCooldown;
                    wrappers[i].gameObject.SetActive(deberiaEstarActivo);
                }
            }

        });


            seq.Play();
        }
        finally
        {
            CambiandoDeModo = false;
        }

    }
    private void ActualizarTimersDuranteTransicion()
    {
        for (int i = 0; i < 6; i++)
        {
            var wrapper = wrappers[i];
            if (wrapper == null || !wrapper.gameObject.activeInHierarchy) continue;

            float percent = maxTimers[i] > 0f ? timers[i] / maxTimers[i] : 0f;
            var overlayRT = wrapper.GetChild(1) as RectTransform;
            var overlayImg = overlayRT?.GetComponent<Image>();
            if (overlayImg != null)
                overlayImg.fillAmount = percent;
        }
    }



}






