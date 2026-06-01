# Phase 9 - Polish cam giac combat

## Muc tieu

Phase nay them feedback co ban cho combat:

- Enemy nhay mau khi trung dan.
- Enemy bi knockback nhe khi bi trung projectile.
- Player ban co muzzle flash tai `FirePoint`.
- Enemy chet co death effect ngan.

## File da tao

- `Assets/Script/Enemies/EnemyHitFeedback2D.cs`
- `Assets/Script/Combat/MuzzleFlash2D.cs`
- `Assets/Script/Combat/TemporaryEffect2D.cs`
- `Assets/Docs/Phase9_HuongDan_PolishCombat.md`

## File da sua

- `Assets/Script/Core/SimpleHealth.cs`
- `Assets/Script/Combat/Projectile2D.cs`
- `Assets/Script/Player/PlayerCombat2D.cs`
- `Assets/Prefabs/Enemy.prefab`
- `Assets/Scenes/CombatPrototype.unity`

## Thay doi chi tiet

### Enemy hit feedback

Prefab `Enemy` duoc gan them component:

- `EnemyHitFeedback2D`

Thong so hien tai:

- `Flash Duration = 0.08`
- `Knockback Speed = 3.5`
- `Knockback Duration = 0.08`
- `Hit Color = white`

Khi projectile trung Enemy:

1. `Projectile2D` goi `SimpleHealth.TakeDamage(damage, hitDirection)`.
2. `SimpleHealth` phat event `Damaged`.
3. `EnemyHitFeedback2D` nhan event.
4. Enemy nhay mau va bi day nhe theo huong ban.

### Death effect

`SimpleHealth` duoc them death effect don gian.

Khi Enemy chet:

- Tao object tam ten `EnemyDeathEffect`.
- Dung sprite hien tai cua Enemy.
- Phong to va fade out.
- Tu huy sau `0.3s`.

Thong so tren prefab `Enemy`:

- `Spawn Death Effect = true`
- `Death Effect Scale = 1.4`
- `Death Effect Lifetime = 0.3`

### Muzzle flash

Player trong scene `CombatPrototype` duoc gan them:

- `MuzzleFlash2D`

`PlayerCombat2D` co reference toi `MuzzleFlash2D`.

Khi ban basic hoac skill:

- Tao object tam ten `MuzzleFlash`.
- Vi tri tai `FirePoint`.
- Dung sprite cua `FirePoint`.
- Fade out va tu huy sau `0.08s`.

## Test case

### 1. Test basic hit feedback

1. Mo scene `CombatPrototype`.
2. Bam Play.
3. Ban Enemy bang chuot trai.

Ket qua dung:

- Enemy nhay sang mau trang trong thoi gian rat ngan.
- Enemy bi day nhe theo huong vien dan.
- Projectile bien mat sau khi trung Enemy.

### 2. Test skill hit feedback

1. Bam Play.
2. Nhan `Q` de ban skill projectile.
3. De skill trung Enemy.

Ket qua dung:

- Enemy co feedback trung don.
- Skill van gay damage cao hon basic.
- Cooldown skill UI van hoat dong.

### 3. Test death effect

1. Ban Enemy den khi het HP.
2. Quan sat vi tri Enemy chet.

Ket qua dung:

- Enemy bi destroy.
- Co effect ngan tai vi tri Enemy chet.
- Effect tu bien mat sau khoang `0.3s`.
- Spawner tiep tuc spawn Enemy moi.

### 4. Test muzzle flash

1. Bam Play.
2. Ban chuot trai lien tuc.
3. Nhan `Q`.

Ket qua dung:

- Co flash ngan tai `FirePoint` moi lan ban.
- Flash khong lam lech huong projectile.
- Khong co object `MuzzleFlash` ton tai vinh vien trong Hierarchy.

### 5. Test khong anh huong gameplay cu

Kiem tra lai:

- WASD van di chuyen.
- Space van dash va di xuyen Enemy.
- Enemy van duoi Player.
- Enemy cham Player van gay damage theo cooldown.
- HP/Skill/Dash UI Phase 8 van cap nhat.

## Neu can tuning

Co the dieu chinh tren prefab `Enemy`:

- `EnemyHitFeedback2D > Knockback Speed`
- `EnemyHitFeedback2D > Knockback Duration`
- `EnemyHitFeedback2D > Flash Duration`
- `SimpleHealth > Death Effect Scale`
- `SimpleHealth > Death Effect Lifetime`

Co the dieu chinh tren Player trong scene:

- `MuzzleFlash2D > Flash Lifetime`
- `MuzzleFlash2D > Flash Scale Multiplier`
- `MuzzleFlash2D > Flash Color`
