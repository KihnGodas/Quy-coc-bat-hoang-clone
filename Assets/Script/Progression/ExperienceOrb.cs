using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class ExperienceOrb : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private float lifetime = 12f;
    [SerializeField, Min(0f)] private float attractRadius = 3.5f;
    [SerializeField, Min(0f)] private float pickupRadius = 0.5f;
    [SerializeField, Min(0f)] private float moveSpeed = 8f;
    [SerializeField] private Color orbColor = new Color(0.35f, 0.95f, 1f, 0.95f);
    [SerializeField, Min(0f)] private float experienceAmount = 10f;

    private static Sprite orbSprite;
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private PlayerExperience playerExperience;
    private float spawnTime;

    public static ExperienceOrb Spawn(Vector3 position, float amount)
    {
        GameObject orbObject = new GameObject("ExperienceOrb");
        orbObject.transform.position = position;

        ExperienceOrb orb = orbObject.AddComponent<ExperienceOrb>();
        orb.experienceAmount = Mathf.Max(0f, amount);
        return orb;
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        CircleCollider2D collider = GetComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 1f;

        spriteRenderer.sprite = GetOrbSprite();
        spriteRenderer.color = orbColor;
        spriteRenderer.sortingOrder = 45;

        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - spawnTime >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        ResolvePlayer();
        if (playerExperience == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, playerExperience.transform.position);
        if (distance <= pickupRadius)
        {
            Collect();
            return;
        }

        if (distance <= attractRadius)
        {
            Vector2 direction = (playerExperience.transform.position - transform.position).normalized;
            body.MovePosition(body.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
        {
            return;
        }

        PlayerExperience experience = other.GetComponentInParent<PlayerExperience>();
        if (experience != null)
        {
            playerExperience = experience;
            Collect();
        }
    }

    private void Collect()
    {
        if (playerExperience != null && experienceAmount > 0f)
        {
            playerExperience.AddExperience(experienceAmount);
        }

        Destroy(gameObject);
    }

    private void ResolvePlayer()
    {
        if (playerExperience != null)
        {
            return;
        }

        PlayerExperience found = FindPlayerExperienceInScene();
        if (found != null)
        {
            playerExperience = found;
        }
    }

    private static PlayerExperience FindPlayerExperienceInScene()
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<PlayerExperience>();
#else
        return FindObjectOfType<PlayerExperience>();
#endif
    }

    private static Sprite GetOrbSprite()
    {
        if (orbSprite != null)
        {
            return orbSprite;
        }

        const int size = 16;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                float alpha = Mathf.Clamp01(1f - distance);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        orbSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), Vector2.one * 0.5f, size);
        return orbSprite;
    }
}
