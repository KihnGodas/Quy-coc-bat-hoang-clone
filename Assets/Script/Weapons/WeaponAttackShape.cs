using UnityEngine;

public static class WeaponAttackShape
{
    public static bool Contains(
        AttackShape shape,
        Vector2 origin,
        Vector2 direction,
        Vector2 point,
        float range,
        float width,
        float angle,
        float radius)
    {
        direction = NormalizeOrRight(direction);

        if (shape == AttackShape.Circle)
        {
            return Vector2.Distance(origin, point) <= radius;
        }

        if (shape == AttackShape.Rectangle)
        {
            return IsInsideRectangle(origin, direction, point, range, width);
        }

        if (shape == AttackShape.Cone)
        {
            return IsInsideCone(origin, direction, point, range, angle);
        }

        return false;
    }

    public static void DrawGizmo(
        AttackShape shape,
        Vector2 origin,
        Vector2 direction,
        float range,
        float width,
        float angle,
        float radius)
    {
        direction = NormalizeOrRight(direction);

        if (shape == AttackShape.Circle)
        {
            Gizmos.DrawWireSphere(origin, radius);
            return;
        }

        if (shape == AttackShape.Rectangle)
        {
            DrawRectangle(origin, direction, range, width);
            return;
        }

        if (shape == AttackShape.Cone)
        {
            DrawCone(origin, direction, range, angle);
        }
    }

    private static bool IsInsideRectangle(Vector2 origin, Vector2 direction, Vector2 point, float length, float width)
    {
        Vector2 toPoint = point - origin;
        float forward = Vector2.Dot(toPoint, direction);
        if (forward < 0f || forward > length)
        {
            return false;
        }

        Vector2 right = new Vector2(-direction.y, direction.x);
        float lateral = Mathf.Abs(Vector2.Dot(toPoint, right));
        return lateral <= width * 0.5f;
    }

    private static bool IsInsideCone(Vector2 origin, Vector2 direction, Vector2 point, float range, float angle)
    {
        Vector2 toPoint = point - origin;
        float distance = toPoint.magnitude;
        if (distance <= 0.0001f)
        {
            return true;
        }

        if (distance > range)
        {
            return false;
        }

        float currentAngle = Vector2.Angle(direction, toPoint);
        return currentAngle <= angle * 0.5f;
    }

    private static void DrawRectangle(Vector2 origin, Vector2 direction, float length, float width)
    {
        Vector2 right = new Vector2(-direction.y, direction.x);
        Vector2 startLeft = origin - right * width * 0.5f;
        Vector2 startRight = origin + right * width * 0.5f;
        Vector2 endLeft = startLeft + direction * length;
        Vector2 endRight = startRight + direction * length;

        Gizmos.DrawLine(startLeft, startRight);
        Gizmos.DrawLine(startRight, endRight);
        Gizmos.DrawLine(endRight, endLeft);
        Gizmos.DrawLine(endLeft, startLeft);
    }

    private static void DrawCone(Vector2 origin, Vector2 direction, float range, float angle)
    {
        Quaternion leftRotation = Quaternion.Euler(0f, 0f, -angle * 0.5f);
        Quaternion rightRotation = Quaternion.Euler(0f, 0f, angle * 0.5f);
        Vector2 left = leftRotation * direction;
        Vector2 right = rightRotation * direction;

        Gizmos.DrawLine(origin, origin + left * range);
        Gizmos.DrawLine(origin, origin + right * range);
        Gizmos.DrawWireSphere(origin, range);
    }

    private static Vector2 NormalizeOrRight(Vector2 direction)
    {
        return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
    }
}
