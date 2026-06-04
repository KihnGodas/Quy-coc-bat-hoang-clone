using System.Collections.Generic;
using UnityEngine;

public sealed class SpellVisualEffect2D : MonoBehaviour
{
    [SerializeField, Min(0.01f)] private float lifetime = 0.35f;
    [SerializeField] private bool fadeOut = true;

    private static Material sharedLineMaterial;
    private readonly List<LineRenderer> lines = new List<LineRenderer>();
    private readonly List<Color> startColors = new List<Color>();
    private float startTime;

    private void Awake()
    {
        startTime = Time.time;
        GetComponentsInChildren(lines);

        foreach (LineRenderer line in lines)
        {
            startColors.Add(line.startColor);
        }
    }

    private void Update()
    {
        EnsureLines();
        float progress = Mathf.Clamp01((Time.time - startTime) / lifetime);

        if (fadeOut)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                Color color = i < startColors.Count ? startColors[i] : Color.white;
                color.a *= 1f - progress;
                lines[i].startColor = color;
                lines[i].endColor = color;
            }
        }

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(float effectLifetime, bool shouldFadeOut)
    {
        lifetime = Mathf.Max(effectLifetime, 0.01f);
        fadeOut = shouldFadeOut;
        startTime = Time.time;
    }

    public static void CreateCircle(string name, Vector2 center, float radius, Color color, float lifetime, float lineWidth = 0.08f, int sortingOrder = 30)
    {
        GameObject root = CreateRoot(name, center, lifetime);
        LineRenderer line = AddLine(root, color, lineWidth, sortingOrder);
        int pointCount = 48;
        line.positionCount = pointCount + 1;

        for (int i = 0; i <= pointCount; i++)
        {
            float angle = (float)i / pointCount * Mathf.PI * 2f;
            Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            line.SetPosition(i, point);
        }
    }

    public static void CreateBurst(string name, Vector2 center, float radius, int rays, Color color, float lifetime, float lineWidth = 0.08f, int sortingOrder = 31)
    {
        GameObject root = CreateRoot(name, center, lifetime);
        rays = Mathf.Max(4, rays);

        for (int i = 0; i < rays; i++)
        {
            float angle = (float)i / rays * 360f;
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
            LineRenderer line = AddLine(root, color, lineWidth, sortingOrder);
            line.positionCount = 2;
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, direction * radius);
        }
    }

    public static void CreateVines(Vector2 center, float radius, Color color, float lifetime)
    {
        CreateCircle("WoodVine_Root_Ring", center, radius, color, lifetime, 0.1f, 32);

        GameObject root = CreateRoot("WoodVine_Root_Lines", center, lifetime);
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f + 18f;
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
            LineRenderer line = AddLine(root, color, 0.09f, 33);
            line.positionCount = 3;
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, direction * radius * 0.45f + Vector2.Perpendicular(direction) * 0.18f);
            line.SetPosition(2, direction * radius);
        }
    }

    public static void CreateSpikes(Vector2 center, float radius, Color color, float lifetime)
    {
        CreateCircle("EarthSpike_Warning_Ring", center, radius, color, lifetime, 0.08f, 32);

        GameObject root = CreateRoot("EarthSpike_Spikes", center, lifetime);
        for (int i = 0; i < 7; i++)
        {
            float x = Mathf.Lerp(-radius * 0.8f, radius * 0.8f, i / 6f);
            float height = radius * (0.65f + 0.25f * Mathf.Sin(i * 1.7f));
            LineRenderer left = AddLine(root, color, 0.09f, 33);
            left.positionCount = 2;
            left.SetPosition(0, new Vector3(x - 0.18f, -radius * 0.2f, 0f));
            left.SetPosition(1, new Vector3(x, height * 0.55f, 0f));

            LineRenderer right = AddLine(root, color, 0.09f, 33);
            right.positionCount = 2;
            right.SetPosition(0, new Vector3(x + 0.18f, -radius * 0.2f, 0f));
            right.SetPosition(1, new Vector3(x, height * 0.55f, 0f));
        }
    }

    public static void CreateProjectileLaunch(string name, Vector2 origin, Vector2 direction, float length, Color color, float lifetime, float lineWidth = 0.1f)
    {
        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        GameObject root = CreateRoot(name, origin, lifetime);
        LineRenderer line = AddLine(root, color, lineWidth, 34);
        line.positionCount = 2;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, direction * length);
    }

    public static void CreateCrescent(Vector2 origin, Vector2 direction, float radius, Color color, float lifetime)
    {
        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        GameObject root = CreateRoot("MetalBlade_Crescent", origin, lifetime);
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        LineRenderer line = AddLine(root, color, 0.12f, 35);
        int pointCount = 18;
        line.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);
            float angle = baseAngle + Mathf.Lerp(-62f, 62f, t);
            Vector2 point = (Vector2)(Quaternion.Euler(0f, 0f, angle) * Vector2.right) * radius;
            line.SetPosition(i, point);
        }
    }

    public static void CreateUltimateWoodRoots(Vector2 center, float radius, Color primaryColor, Color secondaryColor, float lifetime)
    {
        CreateCircle("UltimateWood_OuterSeal", center, radius, primaryColor, lifetime, 0.14f, 40);
        CreateCircle("UltimateWood_InnerSeal", center, radius * 0.55f, secondaryColor, lifetime, 0.1f, 40);

        GameObject root = CreateRoot("UltimateWood_RootCage", center, lifetime);
        for (int i = 0; i < 16; i++)
        {
            float angle = i * 22.5f;
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
            Vector2 side = Vector2.Perpendicular(direction);
            LineRenderer vine = AddLine(root, i % 2 == 0 ? primaryColor : secondaryColor, 0.11f, 41);
            vine.positionCount = 4;
            vine.SetPosition(0, Vector3.zero);
            vine.SetPosition(1, direction * radius * 0.28f + side * 0.2f);
            vine.SetPosition(2, direction * radius * 0.68f - side * 0.22f);
            vine.SetPosition(3, direction * radius);
        }
    }

    public static void CreateUltimateFireNova(Vector2 center, float radius, Color primaryColor, Color secondaryColor, float lifetime)
    {
        CreateCircle("UltimateFire_ShockRing", center, radius, secondaryColor, lifetime, 0.18f, 40);
        CreateBurst("UltimateFire_ExplosionRays", center, radius * 1.15f, 28, primaryColor, lifetime, 0.13f, 41);
        CreateBurst("UltimateFire_InnerFlames", center, radius * 0.6f, 18, secondaryColor, lifetime * 0.85f, 0.18f, 42);
    }

    public static void CreateUltimateFireWarning(Vector2 center, float radius, Color primaryColor, Color secondaryColor, float lifetime)
    {
        CreateCircle("UltimateFire_WarningRing", center, radius, primaryColor, lifetime, 0.12f, 40);
        CreateCircle("UltimateFire_ImpactCore", center, radius * 0.28f, secondaryColor, lifetime, 0.1f, 41);

        GameObject root = CreateRoot("UltimateFire_FallingMeteor", center, lifetime);
        LineRenderer meteor = AddLine(root, secondaryColor, 0.18f, 42);
        meteor.positionCount = 2;
        meteor.SetPosition(0, new Vector3(-radius * 0.35f, radius * 1.35f, 0f));
        meteor.SetPosition(1, Vector3.zero);

        LineRenderer trail = AddLine(root, primaryColor, 0.1f, 41);
        trail.positionCount = 2;
        trail.SetPosition(0, new Vector3(-radius * 0.72f, radius * 1.75f, 0f));
        trail.SetPosition(1, new Vector3(-radius * 0.12f, radius * 0.28f, 0f));
    }

    public static void CreateUltimateEarthQuake(Vector2 center, float radius, Color primaryColor, Color secondaryColor, float lifetime)
    {
        CreateCircle("UltimateEarth_OuterCrack", center, radius, secondaryColor, lifetime, 0.13f, 40);
        CreateCircle("UltimateEarth_MidCrack", center, radius * 0.68f, primaryColor, lifetime, 0.1f, 40);
        CreateCircle("UltimateEarth_CoreCrack", center, radius * 0.34f, secondaryColor, lifetime, 0.08f, 40);

        GameObject root = CreateRoot("UltimateEarth_SpikeField", center, lifetime);
        for (int ring = 0; ring < 3; ring++)
        {
            float ringRadius = radius * (0.35f + ring * 0.25f);
            int count = 8 + ring * 4;
            for (int i = 0; i < count; i++)
            {
                float angle = i * (360f / count) + ring * 12f;
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * Vector2.right;
                Vector2 basePosition = direction * ringRadius;
                Vector2 side = Vector2.Perpendicular(direction);
                float spikeHeight = radius * (0.18f + ring * 0.04f);

                LineRenderer left = AddLine(root, primaryColor, 0.09f, 42);
                left.positionCount = 2;
                left.SetPosition(0, basePosition - side * 0.14f);
                left.SetPosition(1, basePosition + direction * spikeHeight);

                LineRenderer right = AddLine(root, secondaryColor, 0.09f, 42);
                right.positionCount = 2;
                right.SetPosition(0, basePosition + side * 0.14f);
                right.SetPosition(1, basePosition + direction * spikeHeight);
            }
        }
    }

    public static void CreateUltimateWaterStorm(Vector2 center, float radius, Color primaryColor, Color secondaryColor, float lifetime)
    {
        CreateCircle("UltimateWater_OuterCurrent", center, radius, secondaryColor, lifetime, 0.12f, 40);

        GameObject root = CreateRoot("UltimateWater_SpiralCurrents", center, lifetime);
        for (int arm = 0; arm < 5; arm++)
        {
            LineRenderer line = AddLine(root, arm % 2 == 0 ? primaryColor : secondaryColor, 0.12f, 41);
            int pointCount = 34;
            line.positionCount = pointCount;
            float armOffset = arm * 72f;

            for (int i = 0; i < pointCount; i++)
            {
                float t = (float)i / (pointCount - 1);
                float angle = armOffset + t * 270f;
                float currentRadius = Mathf.Lerp(radius * 0.2f, radius, t);
                Vector2 point = (Vector2)(Quaternion.Euler(0f, 0f, angle) * Vector2.right) * currentRadius;
                line.SetPosition(i, point);
            }
        }

        CreateBurst("UltimateWater_Spray", center, radius * 0.85f, 20, primaryColor, lifetime * 0.75f, 0.06f, 42);
    }

    public static void CreateUltimateWaterWave(Vector2 origin, Vector2 direction, float length, float width, Color primaryColor, Color secondaryColor, float lifetime)
    {
        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        Vector2 side = Vector2.Perpendicular(direction);
        GameObject root = CreateRoot("UltimateWater_TidalRectangle", origin, lifetime);

        Vector2 a = side * (width * 0.5f);
        Vector2 b = direction * length + side * (width * 0.5f);
        Vector2 c = direction * length - side * (width * 0.5f);
        Vector2 d = -side * (width * 0.5f);

        LineRenderer border = AddLine(root, secondaryColor, 0.13f, 41);
        border.positionCount = 5;
        border.SetPosition(0, a);
        border.SetPosition(1, b);
        border.SetPosition(2, c);
        border.SetPosition(3, d);
        border.SetPosition(4, a);

        for (int i = 0; i < 7; i++)
        {
            float t = i / 6f;
            float y = Mathf.Lerp(-width * 0.4f, width * 0.4f, t);
            LineRenderer wave = AddLine(root, i % 2 == 0 ? primaryColor : secondaryColor, 0.11f, 42);
            int pointCount = 12;
            wave.positionCount = pointCount;

            for (int p = 0; p < pointCount; p++)
            {
                float x = (float)p / (pointCount - 1) * length;
                float wobble = Mathf.Sin(p * 1.4f + i) * width * 0.06f;
                wave.SetPosition(p, direction * x + side * (y + wobble));
            }
        }
    }

    public static void CreateUltimateMetalJudgement(Vector2 origin, Vector2 direction, float length, Color primaryColor, Color secondaryColor, float lifetime)
    {
        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        Vector2 side = Vector2.Perpendicular(direction);
        GameObject root = CreateRoot("UltimateMetal_JudgementSlashes", origin, lifetime);

        for (int i = 0; i < 5; i++)
        {
            float offset = Mathf.Lerp(-1.2f, 1.2f, i / 4f);
            Vector2 start = side * offset;
            Vector2 end = direction * length - side * offset * 0.35f;
            LineRenderer slash = AddLine(root, i % 2 == 0 ? primaryColor : secondaryColor, 0.16f, 42);
            slash.positionCount = 2;
            slash.SetPosition(0, start);
            slash.SetPosition(1, end);
        }

        CreateCrescent(origin + direction * 0.7f, direction, 1.25f, primaryColor, lifetime);
        CreateProjectileLaunch("UltimateMetal_CenterLine", origin, direction, length, secondaryColor, lifetime * 0.8f, 0.08f);
    }

    public static void CreateUltimateMetalRainWarning(Vector2 center, float radius, Color primaryColor, Color secondaryColor, float lifetime)
    {
        CreateCircle("UltimateMetal_RainWarningOuter", center, radius, primaryColor, lifetime, 0.13f, 40);
        CreateCircle("UltimateMetal_RainWarningInner", center, radius * 0.5f, secondaryColor, lifetime, 0.08f, 41);
        CreateBurst("UltimateMetal_RainSightLines", center, radius * 0.92f, 18, secondaryColor, lifetime, 0.055f, 42);
    }

    public static void CreateUltimateMetalRain(Vector2 center, float radius, int bladeCount, Color primaryColor, Color secondaryColor, float lifetime)
    {
        GameObject root = CreateRoot("UltimateMetal_FallingBladeRain", center, lifetime);
        bladeCount = Mathf.Max(1, bladeCount);

        for (int i = 0; i < bladeCount; i++)
        {
            Vector2 point = Random.insideUnitCircle * radius;
            LineRenderer blade = AddLine(root, i % 2 == 0 ? primaryColor : secondaryColor, 0.09f, 43);
            blade.positionCount = 2;
            blade.SetPosition(0, point + new Vector2(-0.25f, 0.9f));
            blade.SetPosition(1, point + new Vector2(0.25f, -0.35f));
        }
    }

    private static GameObject CreateRoot(string name, Vector2 position, float lifetime)
    {
        GameObject root = new GameObject(name);
        root.transform.position = position;
        SpellVisualEffect2D effect = root.AddComponent<SpellVisualEffect2D>();
        effect.Initialize(lifetime, true);
        return root;
    }

    private static LineRenderer AddLine(GameObject root, Color color, float width, int sortingOrder)
    {
        GameObject lineObject = new GameObject("Line");
        lineObject.transform.SetParent(root.transform, false);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.material = GetLineMaterial();
        line.startColor = color;
        line.endColor = color;
        line.startWidth = width;
        line.endWidth = width;
        line.numCapVertices = 3;
        line.numCornerVertices = 3;
        line.sortingOrder = sortingOrder;
        return line;
    }

    private void EnsureLines()
    {
        if (lines.Count > 0)
        {
            return;
        }

        GetComponentsInChildren(lines);
        foreach (LineRenderer line in lines)
        {
            startColors.Add(line.startColor);
        }
    }

    private static Material GetLineMaterial()
    {
        if (sharedLineMaterial == null)
        {
            Shader shader = Shader.Find("Sprites/Default");
            sharedLineMaterial = new Material(shader != null ? shader : Shader.Find("Default-Line"));
        }

        return sharedLineMaterial;
    }
}
