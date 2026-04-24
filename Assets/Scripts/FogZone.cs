using UnityEngine;

  // Activa niebla densa y de color arena mientras el jugador esté dentro del trigger.
  // Guarda el estado previo del RenderSettings para restaurarlo al salir.
  public class FogZone : MonoBehaviour
  {
      [SerializeField] Color fogColor = new Color(0.85f, 0.7f, 0.5f, 1f);
      [SerializeField] float fogDensity = 0.04f;
      [SerializeField] FogMode fogMode = FogMode.ExponentialSquared;

      // Estado original para restaurar al salir
      bool prevFogEnabled;
      Color prevFogColor;
      float prevFogDensity;
      FogMode prevFogMode;

      void OnTriggerEnter(Collider other)
      {
          if (!other.CompareTag("Player")) return;

          // Guardar config actual
          prevFogEnabled = RenderSettings.fog;
          prevFogColor   = RenderSettings.fogColor;
          prevFogDensity = RenderSettings.fogDensity;
          prevFogMode    = RenderSettings.fogMode;

          // Aplicar niebla de tormenta
          RenderSettings.fog        = true;
          RenderSettings.fogColor   = fogColor;
          RenderSettings.fogDensity = fogDensity;
          RenderSettings.fogMode    = fogMode;
      }

      void OnTriggerExit(Collider other)
      {
          if (!other.CompareTag("Player")) return;

          // Restaurar config anterior
          RenderSettings.fog        = prevFogEnabled;
          RenderSettings.fogColor   = prevFogColor;
          RenderSettings.fogDensity = prevFogDensity;
          RenderSettings.fogMode    = prevFogMode;
      }
  }