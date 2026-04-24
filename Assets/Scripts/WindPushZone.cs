 using UnityEngine;

  // Empuja al jugador lateralmente mientras esté dentro del trigger.
  // NO usa Rigidbody: modifica directamente la posición X porque tu PlayerMovement
  // también mueve al jugador con transform.Translate. Así ambos sistemas son compatibles.
  public class WindPushZone : MonoBehaviour
  {
      [Tooltip("Velocidad lateral del viento. Positivo = empuja a la derecha, negativo = a la izquierda.")]
      [SerializeField] float windStrength = 2.5f;

      [Tooltip("Límite X derecho del carril (debe coincidir con rightLimit de PlayerMovement).")]
      [SerializeField] float rightLimit = 5.5f;
      [Tooltip("Límite X izquierdo del carril (debe coincidir con leftLimit de PlayerMovement).")]
      [SerializeField] float leftLimit = -5.5f;

      Transform playerInside;

      void OnTriggerEnter(Collider other)
      {
          if (other.CompareTag("Player"))
              playerInside = other.transform;
      }

      void OnTriggerExit(Collider other)
      {
          if (other.CompareTag("Player") && other.transform == playerInside)
              playerInside = null;
      }

      void Update()
      {
          if (playerInside == null) return;

          // Aplicar empuje solo si el jugador no se pasaría del límite
          float nextX = playerInside.position.x + windStrength * Time.deltaTime;
          nextX = Mathf.Clamp(nextX, leftLimit, rightLimit);

          Vector3 p = playerInside.position;
          p.x = nextX;
          playerInside.position = p;
      }
  }