using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ShieldPickup : MonoBehaviour
{
    [Header("Animación del item")]
    [SerializeField] float rotateSpeed = 80f;
    [SerializeField] float bobSpeed    = 1.8f;
    [SerializeField] float bobHeight   = 0.25f;

    [Header("Visual del item")]
    [SerializeField] Color itemColor    = new Color(0.2f, 0.8f, 1f, 1f);
    [SerializeField] Color itemEmission = new Color(0.1f, 0.6f, 1f);
    [SerializeField] float itemScale    = 0.6f;

    internal static Shader s_litShader;

    Vector3 spawnPos;

    void Awake()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius    = 1.2f;
        BuildVisual();
    }

    void Start()
    {
        spawnPos = transform.position;
    }

    void BuildVisual()
    {
        GameObject inner = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        inner.name = "ShieldIcon_Inner";
        inner.transform.SetParent(transform, false);
        inner.transform.localScale = Vector3.one * itemScale;
        Destroy(inner.GetComponent<Collider>());

        Material matInner = new Material(s_litShader);
        matInner.color = itemColor;
        matInner.EnableKeyword("_EMISSION");
        matInner.SetColor("_EmissionColor", itemEmission * 2f);
        inner.GetComponent<Renderer>().material = matInner;

        GameObject outer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        outer.name = "ShieldIcon_Outer";
        outer.transform.SetParent(transform, false);
        outer.transform.localScale = Vector3.one * itemScale * 1.8f;
        Destroy(outer.GetComponent<Collider>());

        Material matOuter = new Material(s_litShader);
        matOuter.SetFloat("_Surface",   1f);
        matOuter.SetFloat("_Blend",     0f);
        matOuter.SetFloat("_SrcBlend",  5f);
        matOuter.SetFloat("_DstBlend", 10f);
        matOuter.SetFloat("_ZWrite",    0f);
        matOuter.renderQueue = 3000;
        matOuter.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        matOuter.color = new Color(0.3f, 0.8f, 1f, 0.2f);
        matOuter.EnableKeyword("_EMISSION");
        matOuter.SetColor("_EmissionColor", itemEmission * 0.8f);
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

        PlayerShield shield = other.GetComponent<PlayerShield>();
        if (shield == null) shield = other.GetComponentInParent<PlayerShield>();
        if (shield == null) shield = other.GetComponentInChildren<PlayerShield>();

        if (shield != null) shield.Activate();
        gameObject.SetActive(false);
    }
}
