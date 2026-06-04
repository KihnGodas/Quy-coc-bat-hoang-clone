# Snapshot thông số project trước Phase 8

File này lưu lại thông số hiện tại trước khi bắt đầu Phase 8 UI.

Ngày tạo snapshot: 2026-06-01.

## Phạm vi đã đọc

- `Assets/Scenes/CombatPrototype.unity`
- `Assets/Prefabs/Enemy.prefab`
- `Assets/Prefabs/Projectile.prefab`
- `Assets/Prefabs/SkillProjectile.prefab`
- `ProjectSettings/TagManager.asset`

Không ghi nhận các file recovery trong `Assets/_Recovery`.

## Tags và Layers

Tags hiện có:

- `Player`
- `Enemy`
- `Projectile`

Layers hiện có:

- Layer 8: `Player`
- Layer 9: `Enemy`
- Layer 10: `Projectile`
- Layer 11: `Wall`

## Scene CombatPrototype

### GameManager

Transform:

- Position: `(0, 0, 0)`
- Scale: `(1, 1, 1)`

Component `EnemySpawner`:

- Enemy Prefab: `Assets/Prefabs/Enemy.prefab`
- Player: `Player`
- Spawn Interval: `2`
- Max Alive Enemies: `8`
- Spawn Radius: `7`
- Min Distance From Player: `3`
- Spawn Around Player: bật
- Left Wall: `Wall_Left`
- Right Wall: `Wall_Right`
- Top Wall: `Wall_Top`
- Bottom Wall: `Wall_Bottom`
- Spawn From Arena Edges: bật
- Edge Spawn Padding: `1`
- Spawn Position Attempts: `20`
- Wall Padding: `0.5`

### Main Camera

Transform:

- Position: `(0, 0, -10)`
- Scale: `(1, 1, 1)`

Camera:

- Orthographic Size: `9`

### Player

Object:

- Name: `Player`
- Tag: `Player`
- Layer: `Player`

Transform:

- Position: `(0, 0, 0)`
- Scale: `(1.35, 1.35, 1.6875)`

SpriteRenderer:

- Sorting Order: `1`
- Color: `(0.25, 0.65, 1, 1)`
- Size: `(1, 1)`

Rigidbody2D:

- Body Type: Dynamic
- Gravity Scale: `0`
- Constraints: Freeze Rotation Z

CircleCollider2D:

- Radius: `0.5`
- Is Trigger: tắt

Component `PlayerMovement2D`:

- Move Speed: `5`

Component `PlayerAim2D`:

- Aim Camera: `Main Camera`
- Fire Point: `FirePoint`
- Fire Point Distance: `1`

Component `PlayerCombat2D`:

- Basic Projectile Prefab: `Assets/Prefabs/Projectile.prefab`
- Basic Fire Cooldown: `0.25`
- Skill Projectile Prefab: `Assets/Prefabs/SkillProjectile.prefab`
- Skill Cooldown: `3`

Component `PlayerHealth2D`:

- Max Health: `100`
- Current Health: `100`
- Disable Controls On Death: bật

Component `PlayerDash2D`:

- Dash Speed: `30`
- Dash Duration: `0.3`
- Dash Cooldown: `1`
- Player Layer Name: `Player`
- Enemy Layer Name: `Enemy`

### FirePoint

Object:

- Name: `FirePoint`
- Parent: `Player`
- Tag: `Untagged`
- Layer: `Player`

Transform:

- Local Position: `(0.9045, 0, 0)`
- Local Scale: `(0.6291253, 0.32, 1.28)`

SpriteRenderer:

- Sorting Order: `2`
- Color: `(1, 0.85, 0.2, 1)`
- Size: `(1, 1)`

## Walls trong CombatPrototype

### Wall_Top

Object:

- Name: `Wall_Top`
- Layer: `Wall`

Transform:

- Position: `(-0.43, 10.71, 0)`
- Scale: `(52, 1, 1)`

SpriteRenderer:

- Sorting Order: `0`
- Color: `(0.32, 0.35, 0.38, 1)`

BoxCollider2D:

- Is Trigger: tắt
- Size: `(1, 1)`

### Wall_Bottom

Object:

- Name: `Wall_Bottom`
- Layer: `Wall`

Transform:

- Position: `(-0.43, -11.03, 0)`
- Scale: `(52, 1, 1)`

SpriteRenderer:

- Sorting Order: `0`
- Color: `(0.32, 0.35, 0.38, 1)`

BoxCollider2D:

- Is Trigger: tắt
- Size: `(1, 1)`

### Wall_Left

Object:

- Name: `Wall_Left`
- Layer: `Wall`

Transform:

- Position: `(-25.5, 0, 0)`
- Scale: `(1, 22, 1)`

SpriteRenderer:

- Sorting Order: `0`
- Color: `(0.32, 0.35, 0.38, 1)`

BoxCollider2D:

- Is Trigger: tắt
- Size: `(1, 1)`

### Wall_Right

Object:

- Name: `Wall_Right`
- Layer: `Wall`

Transform:

- Position: `(25.5, 0, 0)`
- Scale: `(1, 22, 1)`

SpriteRenderer:

- Sorting Order: `0`
- Color: `(0.32, 0.35, 0.38, 1)`

BoxCollider2D:

- Is Trigger: tắt
- Size: `(1, 1)`

## Prefab Enemy

File: `Assets/Prefabs/Enemy.prefab`

Object:

- Name: `Enemy`
- Tag: `Enemy`
- Layer: `Enemy`

Transform:

- Local Position: `(3.12, 0.07, 0)`
- Local Scale: `(1, 1, 1)`

SpriteRenderer:

- Sorting Order: `0`
- Color: `(0.8627452, 0.1921569, 0.1960784, 1)`
- Size: `(1, 1)`

Rigidbody2D:

- Body Type: Dynamic
- Gravity Scale: `0`
- Constraints: Freeze Rotation Z

BoxCollider2D:

- Is Trigger: tắt
- Size: `(1, 1)`

Component `EnemyChaseAI2D`:

- Move Speed: `2.5`
- Rotate To Move Direction: tắt

Component `SimpleHealth`:

- Max Health: `50`
- Destroy On Death: bật

Component `EnemyContactDamage2D`:

- Contact Damage: `10`
- Attack Cooldown: `1`

## Prefab Basic Projectile

File: `Assets/Prefabs/Projectile.prefab`

Object:

- Name: `Projectile`
- Tag: `Projectile`
- Layer: `Projectile`

Transform:

- Local Position: `(0.92, 0, 0)`
- Local Scale: `(0.5, 0.5, 0.5)`

SpriteRenderer:

- Sorting Order: `0`
- Color: `(1, 1, 1, 1)`

Rigidbody2D:

- Body Type: Kinematic
- Gravity Scale: `0`

CircleCollider2D:

- Is Trigger: bật
- Radius: `0.5`

Component `Projectile2D`:

- Speed: `12`
- Lifetime: `1.5`
- Damage: `10`
- Destroy On Hit: bật

## Prefab Skill Projectile

File: `Assets/Prefabs/SkillProjectile.prefab`

Object:

- Name: `SkillProjectile`
- Tag: `Projectile`
- Layer: `Projectile`

Transform:

- Local Position: `(0, 0, 0)`
- Local Scale: `(1, 1, 1)`

SpriteRenderer:

- Sorting Order: `4`
- Color: `(0.45, 0.8, 1, 1)`

Rigidbody2D:

- Body Type: Kinematic
- Gravity Scale: `0`

CircleCollider2D:

- Is Trigger: bật
- Radius: `0.5`

Component `Projectile2D`:

- Speed: `8`
- Lifetime: `2`
- Damage: `50`
- Destroy On Hit: bật

## Ghi chú cho Phase 8

Phase 8 UI cần đọc từ các component hiện có:

- HP Player: `PlayerHealth2D.CurrentHealth` và `PlayerHealth2D.MaxHealth`
- Skill cooldown: `PlayerCombat2D.SkillCooldownRemaining`, `PlayerCombat2D.IsSkillReady`
- Dash cooldown: `PlayerDash2D.DashCooldownRemaining`, `PlayerDash2D.IsDashReady`

Khi làm Phase 8, không overwrite các thông số đã snapshot ở trên nếu không có yêu cầu rõ ràng.
