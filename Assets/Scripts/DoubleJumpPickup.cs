using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class DoubleJumpPickup : MonoBehaviour
{
    [Header("Animación")]
    [SerializeField] float rotateSpeed = 90f;
    [SerializeField] float bobSpeed    = 2f;
    [SerializeField] float bobHeight   = 0.25f;

    internal static Shader s_litShader;

    Vector3 spawnPos;

    void Awake()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius    = 1.2f;
        BuildVisual();
    }

    void Start() => spawnPos = transform.position;

    void BuildVisual()
    {
        GameObject inner = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        inner.name = "DJIcon_Inner";
        inner.transform.SetParent(transform, false);
        inner.transform.localScale = Vector3.one * 0.6f;
        Destroy(inner.GetComponent<Collider>());

        Material matInner = new Material(s_litShader);
        matInner.color = new Color(1f, 0.55f, 0.1f, 1f);
        matInner.EnableKeyword("_EMISSION");
        matInner.SetColor("_EmissionColor", new Color(1f, 0.4f, 0f) * 2f);
        inner.GetComponent<Renderer>().material = matInner;

        GameObject outer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        outer.name = "DJIcon_Outer";
        outer.transform.SetParent(transform, false);
        outer.transform.localScale = Vector3.one * 1.1f;
        Destroy(outer.GetComponent<Collider>());

        Material matOuter = new Material(s_litShader);
        matOuter.SetFloat("_Surface",   1f);
        matOuter.SetFloat("_Blend",     0f);
        matOuter.SetFloat("_SrcBlend",  5f);
        matOuter.SetFloat("_DstBlend", 10f);
        matOuter.SetFloat("_ZWrite",    0f);
        matOuter.renderQueue = 3000;
        matOuter.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        matOuter.color = new Color(1f, 0.55f, 0.1f, 0.2f);
        matOuter.EnableKeyword("_EMISSION");
        matOuter.SetColor("_EmissionColor", new Color(1f, 0.35f, 0f) * 0.8f);
        outer.GetComponent<Renderer>().material = matOuter;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
        float newY = spawnPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerDoubleJump dj = other.GetComponent<PlayerDoubleJump>();
        if (dj == null) dj = other.GetComponentInParent<PlayerDoubleJump>();

        if (dj != null) dj.Activate();
        gameObject.SetActive(false);
    }
}
