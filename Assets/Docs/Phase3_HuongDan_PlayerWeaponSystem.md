# Phase 3 - Player Weapon System

## Muc tieu

Phase nay them he thong vu khi day du cho Player trong combat 2D top-down:

- Kiem.
- Thuong.
- Phu/Riu.
- Phi Kiem.

He thong moi uu tien dung `PlayerStats.BaseDamage`, `PlayerAim2D.AimDirection`, `Projectile2D`, `IDamageable` va `DamageInfo` da co tu cac phase truoc.

## File da tao

- `Assets/Script/Weapons/WeaponType.cs`
- `Assets/Script/Weapons/WeaponData.cs`
- `Assets/Script/Weapons/WeaponController.cs`
- `Assets/Script/Weapons/WeaponAttackShape.cs`
- `Assets/Script/Weapons/MeleeHitbox.cs`
- `Assets/Script/Weapons/WeaponVisualEffect2D.cs`
- `Assets/Script/Weapons/WeaponSpriteVisual2D.cs`
- `Assets/Script/Weapons/WeaponSkillBase.cs`
- `Assets/Script/Combat/ProjectileSpawner.cs`
- `Assets/Docs/Phase3_HuongDan_PlayerWeaponSystem.md`

## File da sua

- `Assets/Script/Combat/Projectile2D.cs`
- `Assets/Script/Player/PlayerCombat2D.cs`
- `Assets/Script/UI/CombatHUD2D.cs`
- `Assets/Script/Weapons/WeaponController.cs`
- `Assets/Scenes/CombatScene.unity`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

Co them thu muc moi:

- `Assets/Script/Weapons`

Ly do: tach rieng weapon system khoi `Player` va `Combat` de de mo rong sau nay.

## Mechanic hoat dong ra sao

`WeaponController` gan tren Player doc input:

- Left Mouse: danh thuong.
- Q: dung skill vu khi.

`WeaponController` lay damage theo cong thuc:

```text
PlayerStats.BaseDamage * weaponDamageMultiplier
```

Tat ca sat thuong goi qua `IDamageable.TakeDamage(DamageInfo)` de dung chung voi Player, Enemy va Boss sau nay.

Trong `CombatScene`, `PlayerCombat2D.useLegacyCombat` duoc set `false` de tranh viec chuot trai/Q bi ban hai lan. Class `PlayerCombat2D` van duoc giu nguyen de khong lam mat reference cu.

## Dieu chinh tam danh can chien

Sau khi test, Kiem va Phu/Riu can chien bi qua sat enemy: nguoi choi thuong phai cham vao enemy moi gay damage, dan den bi mat mau do `EnemyContactDamage2D`.

Nguyen nhan:

- Goc danh melee truoc do nam tai tam Player.
- `MeleeHitbox` truoc do chu yeu kiem tra tam collider enemy.
- Neu visual/luoi vu khi cham enemy nhung tam enemy chua vao vung danh, don danh van co the bi tinh la truot.

Da sua:

- `WeaponController` them `meleeOriginForwardOffset = 0.45`, chi ap dung cho shape `Cone` va `Rectangle`.
- Skill/AoE hinh tron van lay tam Player.
- Projectile van ban tu `FirePoint`.
- `MeleeHitbox` kiem tra them `ClosestPoint` va cac corner cua bounds enemy de enemy bi trung khi mep collider lot vao vung danh.
- Tang nhe tam danh:
  - Kiem normal range: `2.0 -> 2.7`
  - Phu/Riu normal range: `2.0 -> 2.5`
  - Phu/Riu skill range: `3.0 -> 3.4`

## Hieu ung nhin thay cua vu khi

Sau khi test, logic vu khi da chay nhung mot so don melee khong co bieu hien ro trong Game view. Da them `WeaponVisualEffect2D` tren Player de tao hieu ung tam thoi bang `LineRenderer`.

Hieu ung hien tai:

- Kiem normal: ve vung cone/ban nguyet truoc mat.
- Thuong normal: ve hinh chu nhat dai theo huong aim.
- Thuong skill: ve vong tron AoE quanh Player.
- Phu/Riu normal: ve cone 120 do.
- Phu/Riu skill: ve hinh chu nhat truoc mat.
- Phi Kiem normal/skill: ve cac tia projectile theo fan/spread, dong thoi projectile that van bay nhu truoc.

`WeaponVisualEffect2D` tu tao material runtime bang shader `Sprites/Default`, khong tao asset material moi, nen giam rui ro material bi tim khi merge project. Cac object hieu ung tu huy sau `effectLifetime`.

## Sprite rieng cua vu khi

Da them object con:

```text
Player
|- FirePoint
|- WeaponHolder
|  |- SpriteRenderer
|  `- WeaponSpriteVisual2D
`- WeaponController
```

`Player` van giu logic di chuyen, input, damage va cooldown. `WeaponHolder` chi lo hien thi sprite vu khi, xoay theo huong aim va chay animation ngan khi danh.

Hien tai `WeaponHolder` dung sprite placeholder co san trong scene de tranh tao asset anh moi. Trong `WeaponSpriteVisual2D` co cac slot:

- `Sword Sprite`
- `Spear Sprite`
- `Axe Sprite`
- `Flying Sword Sprite`

Khi co anh vu khi that, chi can keo sprite tu Project window vao cac slot nay. Neu slot nao de trong, script se dung `Fallback Sprite`.

Neu chua co anh vu khi that, `WeaponSpriteVisual2D` se ghep cac sprite placeholder thanh dang rieng cho tung vu khi:

- Kiem: than kiem dai vua, mong, co phan chuoi/chan kiem nho.
- Thuong: than dai nhat, rat mong, co mui thuong rieng o dau.
- Phu/Riu: can ngan hon thuong, co dau riu to hon o phan truoc.
- Phi Kiem: nho, mong, co dau nhon va phan sang nhe.

Hai phan phu `SecondaryWeaponPart` va `AccentWeaponPart` duoc tao runtime khi bam Play, khong phai asset moi trong Project. Khi sau nay gan sprite that cho tung vu khi, cac phan placeholder phu se tu tat de tranh de len art that.

## Thong so vu khi hien tai

### Kiem

- Normal damage multiplier: `1.5`
- Crit chance: `0.1`
- Normal attack: cone/ban nguyet truoc mat, range `2.7m`, angle `180`
- Normal cooldown: `1.5s`
- Skill: ban kiem khi projectile
- Skill damage multiplier: `2.0`
- Projectile speed: `16`
- Projectile lifetime: `2s`
- Skill cooldown: `7s`

### Thuong

- Normal damage multiplier: `1.4`
- Crit chance: `0.1`
- Normal attack: rectangle dai `4m`, rong `0.8m`
- Normal cooldown: `1.4s`
- Skill: circle AoE quanh player, radius `6m`
- Skill damage multiplier: `2.0`
- Skill cooldown: `5s`

### Phu/Riu

- Normal damage multiplier: `2.0`
- Crit chance: `0.05`
- Normal attack: cone truoc mat, range `2.5m`, angle `120`
- Normal cooldown: `2s`
- Skill: rectangle truoc mat dai `3.4m`, rong `2m`
- Skill damage multiplier: `3.0`
- Skill cooldown: `8s`

### Phi Kiem

- Normal damage multiplier: `0.65`
- Crit chance: `0.2`
- Normal attack: ban `3` projectile, spread `18 do`
- Normal cooldown: `1s`
- Skill: ban `7` projectile, fan `80 do`
- Skill damage multiplier: `0.9`
- Projectile speed: `16`
- Projectile lifetime: `1s`
- Skill cooldown: `7s`

Ly do can bang:

- Phi Kiem co nhieu projectile nen tong damage/clear enemy cao hon melee neu de multiplier qua lon.
- Da giam multiplier va tang cooldown skill Phi Kiem de khong vuot qua Kiem/Thuong/Riu qua xa.
- Thuong danh hep hon nen tang nhe normal damage va giam nhe cooldown de cam giac khong bi yeu.

Luu y: GDD/prompt chua ghi ro cooldown skill cho Phi Kiem, nen dang tam dung `7s` sau khi can bang prototype. Neu can doi, sua field trong `WeaponController` hoac config default trong code.

## Cach kiem tra trong Unity

1. Mo scene `Assets/Scenes/CombatScene.unity`.
2. Chon object `Player`.
3. Kiem tra Player co cac component:
   - `WeaponController`
   - `MeleeHitbox`
   - `ProjectileSpawner`
   - `WeaponVisualEffect2D`
4. Kiem tra Player co child `WeaponHolder`.
5. Kiem tra `WeaponHolder` co:
   - `SpriteRenderer`
   - `WeaponSpriteVisual2D`
6. Kiem tra `PlayerCombat2D > Use Legacy Combat` dang tat trong `CombatScene`.
7. Bam Play.
8. Dung chuot de aim.
9. Bam/giu Left Mouse de danh thuong.
10. Bam Q de dung skill.
11. Quan sat HUD hien cooldown skill theo weapon hien tai.
12. Quan sat Game view/Scene view:
    - Don melee phai hien vung danh trong thoi gian rat ngan.
    - Don projectile phai hien tia/fan ban ra tu FirePoint.
    - Sprite tren `WeaponHolder` phai xoay theo huong aim va co animation ngan khi danh.
    - Khi chua gan sprite that, dang vu khi van phai khac nhau: Thuong dai/mong, Riu dau to, Kiem vua, Phi Kiem nho/mong.

## Test case cu the

### Test 1 - Phi Kiem mac dinh

Dieu kien:

- `WeaponController.currentWeapon = FlyingSword`.

Buoc test:

1. Bam Play.
2. Aim ve phia enemy.
3. Giu Left Mouse.

Ket qua dung:

- Player ban 3 projectile theo spread nho.
- Co 3 tia visual mau xanh nhat tu FirePoint.
- `WeaponHolder` hien sprite Phi Kiem, rung/lao nhe khi ban.
- Projectile gay damage enemy qua `IDamageable`.
- Enemy co hit feedback nhu phase truoc.

### Test 2 - Skill Phi Kiem

Buoc test:

1. Bam Play.
2. Bam Q.

Ket qua dung:

- Player ban 7 projectile theo fan 80 do.
- Co fan visual mau cam/vang lon hon normal attack.
- `WeaponHolder` co animation skill manh hon normal.
- HUD hien skill cooldown.

### Test 3 - Kiem

Buoc test thu cong:

1. Chon Player.
2. Doi `WeaponController > Current Weapon` sang `Sword`.
3. Bam Play.
4. De enemy dung truoc mat Player.
5. Bam Left Mouse.
6. Bam Q.

Ket qua dung:

- Left Mouse gay damage trong vung cone/ban nguyet.
- Left Mouse hien vung cone/ban nguyet truoc mat Player.
- Sprite Kiem tren `WeaponHolder` vung theo huong aim.
- Co the dung cach enemy mot khoang ngan de danh trung, khong can cham vao enemy.
- Q ban 1 projectile kiem khi.
- Q hien tia projectile tu FirePoint.

### Test 4 - Thuong

Buoc test thu cong:

1. Doi `Current Weapon` sang `Spear`.
2. Bam Play.
3. Aim ve phia enemy va bam Left Mouse.
4. Bam Q khi enemy o gan Player.

Ket qua dung:

- Left Mouse danh rectangle dai 4m theo huong aim.
- Left Mouse hien khung rectangle dai theo huong aim.
- Sprite Thuong tren `WeaponHolder` dam thang ve phia aim.
- Q gay damage circle quanh Player, radius 6m.
- Q hien vong tron AoE quanh Player.

### Test 5 - Phu/Riu

Buoc test thu cong:

1. Doi `Current Weapon` sang `Axe`.
2. Bam Play.
3. Bam Left Mouse khi enemy o truoc mat.
4. Bam Q khi enemy nam trong vung truoc mat.

Ket qua dung:

- Left Mouse danh cone 120 do.
- Left Mouse hien cone 120 do truoc mat Player.
- Sprite Riu tren `WeaponHolder` vung cham va rong hon.
- Co the dung cach enemy mot khoang ngan de danh trung, khong can cham vao enemy.
- Q danh rectangle 3m x 2m truoc mat.
- Q hien khung rectangle mau skill truoc mat.

## Neu can thao tac thu cong

Neu Unity Editor khong tu refresh script moi:

1. Quay lai Unity Editor.
2. Cho Unity compile xong.
3. Neu Inspector khong hien component moi, chon Player va them thu cong:
   - `MeleeHitbox`
   - `ProjectileSpawner`
   - `WeaponController`
   - Tao child `WeaponHolder` trong Player.
   - Them `SpriteRenderer` va `WeaponSpriteVisual2D` vao `WeaponHolder`.
4. Gan reference:
   - `WeaponController.playerStats` = `PlayerStats`
   - `WeaponController.playerAim` = `PlayerAim2D`
   - `WeaponController.playerMovement` = `PlayerMovement2D`
   - `WeaponController.meleeHitbox` = `MeleeHitbox`
   - `WeaponController.weaponVisualEffect` = `WeaponVisualEffect2D`
   - `WeaponController.weaponSpriteVisual` = `WeaponHolder > WeaponSpriteVisual2D`
   - `WeaponController.projectileSpawner` = `ProjectileSpawner`
   - `WeaponController.muzzleFlash` = `MuzzleFlash2D`
   - `WeaponSpriteVisual2D.weaponRenderer` = `WeaponHolder > SpriteRenderer`
   - `ProjectileSpawner.defaultProjectilePrefab` = `Assets/Prefabs/Projectile.prefab`
5. Set target layer cua `MeleeHitbox`, `ProjectileSpawner`, `WeaponController` la `Enemy`.

## Cach thay sprite vu khi that

1. Import anh vu khi vao Unity, vi du `Assets/Sprites/Weapons`.
2. Chon tung texture.
3. Set `Texture Type = Sprite (2D and UI)`.
4. Bam `Apply`.
5. Chon `Player > WeaponHolder`.
6. Trong `WeaponSpriteVisual2D`, keo sprite vao slot tuong ung:
   - Kiem vao `Sword Sprite`
   - Thuong vao `Spear Sprite`
   - Riu vao `Axe Sprite`
   - Phi Kiem vao `Flying Sword Sprite`
7. Bam Play va doi `WeaponController.currentWeapon` de test tung vu khi.

## Ghi chu verify

Da chay:

```text
dotnet build Assembly-CSharp.csproj --no-restore
```

Ket qua:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

Chua verify truc tiep bang Play Mode trong Unity Editor.
