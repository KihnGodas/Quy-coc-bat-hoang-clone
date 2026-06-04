# Phase 2 - Player Movement Dash Bo Phap

## Muc tieu

Phase nay chuan hoa player theo roadmap moi:

- `maxHP = 200`
- `baseDamage = 50`
- `moveSpeed = 12`
- `healthRegen = 1 HP/s`
- Dash bang Space theo distance/duration/cooldown.
- Player tiep tuc aim bang mouse va bi clamp trong `ArenaBounds`.

## File da tao

- `Assets/Script/Player/PlayerStats.cs`
- `Assets/Script/Player/PlayerHealthRegen.cs`
- `Assets/Docs/Phase2_HuongDan_PlayerMovement_Dash_BoPhap.md`

## File da sua

- `Assets/Script/Player/PlayerMovement2D.cs`
- `Assets/Script/Player/PlayerDash2D.cs`
- `Assets/Script/Player/PlayerHealth2D.cs`
- `Assets/Scenes/CombatScene.unity`

## Thay doi chinh

`PlayerMovement2D` uu tien doc speed tu `PlayerStats.MoveSpeed`. Neu khong co `PlayerStats`, no dung `moveSpeed` fallback trong Inspector.

`PlayerHealth2D` co the doc max HP tu `PlayerStats.MaxHP` luc `Awake`.

`PlayerDash2D` dung:

- `dashDistance = 4.5`
- `dashDuration = 0.15`
- `dashCooldown = 2`
- `invulnerableDuration = 0.2`

`PlayerHealthRegen` hoi mau moi frame theo `PlayerStats.HealthRegen`.

## Test nhanh

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Bam Play.
3. Kiem tra:
   - Player HP hien `200 / 200`.
   - WASD di chuyen nhanh hon prototype cu theo speed 12.
   - Space dash ngan, nhanh, cooldown 2s.
   - Khi mat mau, HP hoi lai 1 HP/s.
   - Player khong vuot khoi arena 35 x 35.
