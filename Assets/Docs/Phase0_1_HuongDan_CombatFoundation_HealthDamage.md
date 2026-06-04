# Phase 0-1 - Combat Foundation va Health Damage

## Muc tieu

Phase nay bat dau chuan hoa prototype combat theo roadmap moi:

- Tao scene `CombatScene` tu `CombatPrototype`.
- Them `ArenaBounds` cho arena 35m x 35m.
- Them bootstrap/debug UI tam.
- Them Health & Damage system dung chung qua `IDamageable` va `DamageInfo`.
- Giu prototype hien tai van chay trong luc migrate.

## File da tao

- `Assets/Scenes/CombatScene.unity`
- `Assets/Script/Combat/ArenaBounds.cs`
- `Assets/Script/Manager/CombatBootstrap.cs`
- `Assets/Script/UI/DebugCombatUI.cs`
- `Assets/Script/Core/IDamageable.cs`
- `Assets/Script/Core/DamageInfo.cs`
- `Assets/Script/Core/Health.cs`
- `Assets/Script/Combat/DamageDealer.cs`

## File da sua

- `Assets/Script/Core/SimpleHealth.cs`
- `Assets/Script/Player/PlayerHealth2D.cs`
- `Assets/Script/Player/PlayerMovement2D.cs`
- `Assets/Script/Player/PlayerDash2D.cs`
- `Assets/Script/Combat/Projectile2D.cs`
- `Assets/Script/Enemies/EnemyContactDamage2D.cs`
- `Assets/Script/Enemies/EnemyChaseAI2D.cs`
- `Assets/Script/Spawning/EnemySpawner.cs`

## Ghi chu tich hop

`SimpleHealth` va `PlayerHealth2D` da implement `IDamageable`, nen projectile/contact damage co the gay sat thuong qua interface moi ma khong can doi prefab ngay lap tuc.

`Health.cs` la component dung chung moi cho cac phase sau. Khi bat dau lam enemy base, boss base hoac weapon system, uu tien dung `Health` thay vi tao health rieng moi.

De dung `ArenaBounds`, tao object trong scene va gan `ArenaBounds`. Mac dinh kich thuoc la `35 x 35`, center o `(0, 0)`. Player, dash, enemy chase va spawner se tu tim `ArenaBounds` khi play neu reference trong Inspector dang trong.

## Test nhanh

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Tao object rong `ArenaBounds` neu scene chua co, gan component `ArenaBounds`.
3. Bam Play.
4. Kiem tra:
   - Player khong di ra ngoai vung 35 x 35.
   - Dash khong day player vuot bien.
   - Projectile van gay damage enemy.
   - Enemy cham player van tru HP.
   - Enemy spawn quanh ria arena neu co `ArenaBounds`.
