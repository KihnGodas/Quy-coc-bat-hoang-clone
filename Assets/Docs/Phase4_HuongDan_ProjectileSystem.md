# Phase 4 - Projectile System

## Muc tieu

Phase nay hoan thien he thong projectile dung chung cho Player, Enemy va Boss sau nay.

Projectile system can ho tro:

- Projectile bay thang.
- Projectile ban don.
- Projectile ban spread/fan.
- Projectile ban radial 360 do.
- Projectile boomerang bay ra roi quay lai.

He thong moi van giu tuong thich voi Phase 3 Player Weapon System. `WeaponController` van co the dung `ProjectileSpawner.SpawnSpread()` nhu cu.

## File da tao

- `Assets/Script/Combat/ProjectilePattern.cs`
- `Assets/Script/Combat/BoomerangProjectile2D.cs`
- `Assets/Docs/Phase4_HuongDan_ProjectileSystem.md`

## File da sua

- `Assets/Script/Combat/Projectile2D.cs`
- `Assets/Script/Combat/ProjectileSpawner.cs`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

File doc cu `Assets/Docs/Phase4_HuongDan_EnemyCoBan.md` da duoc thay bang doc Phase 4 dung theo roadmap goc: Projectile System. Enemy Base AI se thuoc Phase 6, cac loai enemy cu the thuoc Phase 7.

## Mechanic hoat dong ra sao

### Projectile2D

`Projectile2D` la projectile base:

- Co `speed`.
- Co `damage`.
- Co `lifetime`.
- Co `targetLayers`.
- Co crit config.
- Tu di chuyen theo `moveDirection`.
- Tu destroy sau lifetime.
- Khi cham object dung layer va co `IDamageable`, goi `TakeDamage(DamageInfo)`.
- Bo qua va cham voi owner/source root de projectile khong tu danh nguoi ban.

Class nay da duoc doi tu `sealed` thanh base class co the ke thua. API cu van duoc giu:

- `Launch()`
- `Init()`
- `SetTargetLayers()`
- `ConfigureCrit()`

### ProjectilePattern

`ProjectilePattern` gom:

```text
Single
Spread
Radial
Boomerang
```

Dung de enemy/boss sau nay chon kieu ban ma khong can hard-code nhieu flow rieng.

### ProjectileSpawner

`ProjectileSpawner` hien co:

- `SpawnSingle()`
- `SpawnSpread()`
- `SpawnRadial()`
- `SpawnBoomerang()`
- `SpawnPattern()`

`SpawnPattern()` la wrapper dung cho enemy/boss sau nay:

- `Single`: goi `SpawnSingle`.
- `Spread`: goi `SpawnSpread`.
- `Radial`: goi `SpawnRadial`.
- `Boomerang`: goi `SpawnBoomerang`.

### BoomerangProjectile2D

`BoomerangProjectile2D` ke thua `Projectile2D`.

Quy trinh:

1. Projectile bay ra theo huong ban dau.
2. Sau `returnDelay`, neu co `returnTarget`, projectile doi huong bay ve target.
3. Neu `returnToOwner` bat va khong gan target rieng, projectile quay ve source/owner.
4. Van dung damage/collision/lifetime cua `Projectile2D`.

## Cach dung trong code sau nay

### Ban projectile don

```csharp
projectileSpawner.SpawnPattern(
    ProjectilePattern.Single,
    projectilePrefab,
    null,
    position,
    direction,
    1,
    0f,
    damage,
    speed,
    lifetime,
    source,
    false,
    0f,
    1f,
    targetLayers);
```

### Ban spread

```csharp
projectileSpawner.SpawnPattern(
    ProjectilePattern.Spread,
    projectilePrefab,
    null,
    position,
    direction,
    7,
    80f,
    damage,
    speed,
    lifetime,
    source,
    false,
    0f,
    1f,
    targetLayers);
```

### Ban radial 360 do

```csharp
projectileSpawner.SpawnPattern(
    ProjectilePattern.Radial,
    projectilePrefab,
    null,
    position,
    Vector2.right,
    15,
    0f,
    damage,
    speed,
    lifetime,
    source,
    false,
    0f,
    1f,
    targetLayers);
```

### Ban boomerang

```csharp
projectileSpawner.SpawnPattern(
    ProjectilePattern.Boomerang,
    null,
    boomerangProjectilePrefab,
    position,
    direction,
    1,
    0f,
    damage,
    speed,
    lifetime,
    source,
    false,
    0f,
    1f,
    targetLayers,
    0.45f,
    source.transform);
```

## Cach kiem tra trong Unity

1. Mo Unity Editor.
2. Cho Unity compile xong.
3. Mo `Assets/Scenes/CombatScene.unity`.
4. Bam Play.
5. Test lai Phi Kiem va Kiem khi cua Phase 3:
   - FlyingSword normal ban spread.
   - FlyingSword skill ban fan.
   - Sword skill ban 1 projectile.

Ket qua dung:

- Cac projectile Phase 3 van bay va gay damage nhu cu.
- Khong co loi `Missing Script`.
- Khong co loi compile trong Console.

## Test case cu the

### Test 1 - Projectile cu khong bi hong

Buoc test:

1. Chon `Player`.
2. Set `WeaponController.currentWeapon = FlyingSword`.
3. Bam Play.
4. Giu Left Mouse.

Ket qua dung:

- Player ban projectile spread.
- Projectile trung Enemy thi Enemy mat mau.
- Projectile tu destroy sau lifetime hoac sau khi trung target.

### Test 2 - Radial method compile va san sang dung

Buoc test code/dev:

1. Goi `ProjectileSpawner.SpawnPattern(ProjectilePattern.Radial, ...)` tu script test hoac enemy/boss sau nay.
2. Set `count = 15`.

Ket qua dung:

- Spawn 15 projectile toa 360 do.
- Projectile dung chung target layer va damage config.

### Test 3 - Boomerang method compile va san sang dung

Buoc test code/dev:

1. Tao prefab projectile co component `BoomerangProjectile2D`.
2. Gan prefab vao `ProjectileSpawner.defaultBoomerangProjectilePrefab` hoac truyen vao `SpawnBoomerang`.
3. Goi `SpawnPattern(ProjectilePattern.Boomerang, ...)`.

Ket qua dung:

- Projectile bay ra theo huong ban dau.
- Sau `returnDelay`, projectile quay ve `returnTarget` hoac source.
- Projectile van tu destroy theo lifetime.

## Thao tac thu cong neu can prefab boomerang

Phase nay chi tao script, chua tao prefab boomerang rieng de tranh dung cham prefab/scene khi chua can test enemy/boss.

Neu can tao prefab boomerang thu cong:

1. Duplicate `Assets/Prefabs/Projectile.prefab`.
2. Doi ten thanh `BoomerangProjectile.prefab`.
3. Tren prefab moi, thay hoac them component `BoomerangProjectile2D`.
4. Giu `Rigidbody2D` la Kinematic.
5. Giu Collider2D la Trigger.
6. Gan prefab nay vao `ProjectileSpawner.defaultBoomerangProjectilePrefab`.

## Ghi chu verify

Da chay:

```text
dotnet build Assembly-CSharp.csproj
```

Ket qua:

```text
Build succeeded.
0 Error(s)
```

Co warning tu package Unity trong `Library/PackageCache`, khong phai tu script Phase 4 moi tao.

Chua verify truc tiep bang Play Mode trong Unity Editor.
