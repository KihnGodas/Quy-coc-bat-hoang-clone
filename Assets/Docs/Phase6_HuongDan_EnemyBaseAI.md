# Phase 6 - Enemy Base AI

## Muc tieu

Phase nay tao nen Enemy Base AI de sau nay co the lam tung loai enemy cu the trong Phase 7 ma khong phai viet lai toan bo logic.

He thong moi ho tro:

- Enemy co data rieng qua `EnemyData`.
- Enemy co state machine co ban.
- Enemy chase Player.
- Enemy melee attack bang `DamageInfo`.
- Enemy van duoc `EnemySpawner` track de dieu kien Victory cua Phase 5 hoat dong dung.

## File da tao

- `Assets/Script/Enemies/EnemyState.cs`
- `Assets/Script/Enemies/EnemyRole.cs`
- `Assets/Script/Enemies/EnemyData.cs`
- `Assets/Script/Enemies/EnemyBase.cs`
- `Assets/Script/Enemies/EnemyMovement.cs`
- `Assets/Script/Enemies/EnemyAttackBase.cs`
- `Assets/Script/Enemies/EnemyMeleeAttack.cs`
- `Assets/ScriptableObjects/EnemyData_KhoiLang.asset`
- `Assets/Docs/Phase6_HuongDan_EnemyBaseAI.md`

## File da sua

- `Assets/Script/Core/SimpleHealth.cs`
- `Assets/Script/Spawning/EnemySpawner.cs`
- `Assets/Prefabs/Enemy.prefab`
- `Assets/Scenes/CombatScene.unity`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

Ghi chu: `Assets/Docs/Phase6_HuongDan_Dash.md` la tai lieu cu cua flow truoc. File nay khong bi xoa va khong bi sua trong phase nay.

## Mechanic hoat dong ra sao

### EnemyState

`EnemyState` gom:

```text
Idle
Chase
Attack
Casting
Stunned
Dead
```

Phase 6 dang dung cac state chinh:

- `Idle` khi chua co target.
- `Chase` khi co Player va ngoai tam danh.
- `Attack` khi da vao tam danh.
- `Dead` khi health bao death.

`Casting` va `Stunned` duoc chuan bi cho enemy co skill ve sau.

### EnemyRole

`EnemyRole` gom:

```text
MeleeChaser
SpeedChaser
Charger
Tank
Berserker
HybridThrower
RangedProjectile
RootMage
```

Phase 6 moi dung `MeleeChaser` cho enemy prototype hien tai. Cac role khac de Phase 7 mo rong.

### EnemyData

`EnemyData` la ScriptableObject chua chi so enemy:

- Enemy Name
- Enemy Role
- Max HP
- Damage
- Move Speed
- Attack Range
- Attack Cooldown
- Rotate To Move Direction
- Arena Padding

Da tao asset:

```text
Assets/ScriptableObjects/EnemyData_KhoiLang.asset
```

Thong so hien tai:

- Enemy Name: `Khoi Lang`
- Role: `MeleeChaser`
- Max HP: `100`
- Damage: `10`
- Move Speed: `12`
- Attack Range: `1.1`
- Attack Cooldown: `1`

### EnemyBase

`EnemyBase` la component trung tam cua enemy moi.

No lam cac viec:

1. Doc `EnemyData`.
2. Tim Player bang tag `Player` neu chua gan target.
3. Khoi tao HP tu `EnemyData`.
4. Theo doi health death event.
5. Doi state giua `Idle`, `Chase`, `Attack`, `Dead`.
6. Goi `EnemyMovement` khi dang chase.

`EnemyBase` ho tro ca:

- `Health`
- `SimpleHealth`

Ly do: prefab enemy hien tai dang dung `SimpleHealth`, nen phase nay khong ep thay component health de tranh rui ro prefab.

### EnemyMovement

`EnemyMovement` di chuyen enemy ve phia target:

- Dung `Rigidbody2D.MovePosition`.
- Tu tim `ArenaBounds`.
- Clamp vi tri trong arena.
- Co the xoay theo huong di chuyen neu `EnemyData.RotateToMoveDirection = true`.

### EnemyAttackBase va EnemyMeleeAttack

`EnemyAttackBase` la lop nen cho attack:

- Giu cooldown.
- Kiem tra target tag.
- Doc damage/cooldown tu `EnemyBase`.

`EnemyMeleeAttack` hien tai gay damage khi enemy va Player dang collision/trigger stay.

Damage duoc gui qua:

```text
DamageInfo(enemyBase.Damage, gameObject, false, 0, 1, hitDirection)
```

### SimpleHealth

`SimpleHealth` duoc them:

```text
Initialize(float newMaxHealth, bool refill = true)
```

Muc dich la de `EnemyBase` co the set HP theo `EnemyData` ma khong can thay component health cu tren prefab.

### EnemySpawner

`EnemySpawner` duoc nang cap:

- Giu field cu `enemyPrefab` kieu `EnemyChaseAI2D` de khong mat reference.
- Them field moi `enemyBasePrefab` kieu `EnemyBase`.
- Neu co `enemyBasePrefab`, spawner uu tien spawn prefab moi.
- Neu chua co `enemyBasePrefab`, spawner van fallback ve prefab cu.
- Alive list track theo `GameObject` de tinh dung ca enemy cu va enemy moi.

Dieu nay giup `CombatManager.RemainingEnemyCount` van hoat dong dung khi chuyen sang enemy base.

### Enemy prefab

`Assets/Prefabs/Enemy.prefab` da duoc gan them:

- `EnemyBase`
- `EnemyMovement`
- `EnemyMeleeAttack`

Component cu van duoc giu:

- `EnemyChaseAI2D`
- `EnemyContactDamage2D`

Nhung hai component cu nay da duoc tat tren prefab de tranh:

- Di chuyen hai lan.
- Gay damage hai lan.

`SimpleHealth.maxHealth` tren prefab duoc dat thanh `100` de khop `EnemyData_KhoiLang`.

## Cach kiem tra trong Unity

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Chon `GameManager`.
3. Kiem tra `EnemySpawner`:
   - `Enemy Prefab` van tro toi `Enemy`.
   - `Enemy Base Prefab` tro toi component `EnemyBase` cua prefab `Enemy`.
4. Mo `Assets/Prefabs/Enemy.prefab`.
5. Kiem tra prefab co component:
   - `EnemyBase`
   - `EnemyMovement`
   - `EnemyMeleeAttack`
   - `SimpleHealth`
   - `EnemyHitFeedback2D`
6. Kiem tra `EnemyChaseAI2D` va `EnemyContactDamage2D` dang tat.
7. Bam Play.

Ket qua dung:

- Enemy spawn tu rìa arena.
- Enemy chase Player bang he EnemyBase moi.
- Enemy cham Player thi gay damage theo cooldown.
- Enemy bi danh chet thi bien mat va co death effect.
- Khi het gio va clear het enemy, CombatManager chuyen `Victory`.

## Test case cu the

### Test 1 - Enemy spawn va chase Player

Buoc test:

1. Mo `CombatScene`.
2. Bam Play.
3. Doi enemy spawn.
4. Di chuyen Player ra xa enemy.

Ket qua dung:

- Enemy di chuyen ve phia Player.
- Enemy khong di ra ngoai arena.

### Test 2 - Enemy melee damage

Buoc test:

1. De enemy cham Player.
2. Quan sat HP tren HUD.

Ket qua dung:

- Player mat HP moi `1s` mot lan theo `EnemyData.attackCooldown`.
- Damage moi lan la `10`.
- Player khong bi tru mau hai lan lien tuc trong cung mot cooldown.

### Test 3 - Enemy death

Buoc test:

1. Tan cong enemy bang weapon.
2. Quan sat enemy khi HP ve 0.

Ket qua dung:

- Enemy bien mat.
- Death effect hien neu sprite co san.
- `EnemySpawner.AliveCount` giam.

### Test 4 - Victory sau khi clear enemy

Buoc test:

1. Dat `CombatManager.normalCombatDuration = 5`.
2. Bam Play.
3. Doi het gio.
4. Diet het enemy con lai.

Ket qua dung:

- Khi het gio, spawner dung spawn.
- HUD hien `Clear Enemies: n` neu con enemy.
- Khi clear het enemy, CombatManager chuyen `Victory`.

### Test 5 - Fallback prefab cu

Buoc test thu cong:

1. Tam thoi xoa reference `EnemySpawner.enemyBasePrefab`.
2. Bam Play.

Ket qua dung:

- Spawner van co the spawn bang `Enemy Prefab` cu.
- Khong Missing Script.

Sau khi test xong, gan lai `Enemy Base Prefab`.

## Neu enemy khong chase

Kiem tra:

1. Enemy prefab co `EnemyBase`.
2. Enemy prefab co `EnemyMovement`.
3. `EnemyBase.enemyData` da gan `EnemyData_KhoiLang`.
4. Player co tag `Player`.
5. Console khong co Missing Script.

## Neu enemy spawn nhung khong hien tren man hinh

Kiem tra:

1. `Enemy.prefab` co `SpriteRenderer` dang bat.
2. `SpriteRenderer.sprite` khong bi trong.
3. `SpriteRenderer.color.a` lon hon 0.
4. Enemy co the spawn o ria arena nen neu camera dang o gan Player, doi vai giay de enemy chay vao tam nhin.
5. Kiem tra `EnemySpawner.AliveCount` bang `DebugCombatUI.showDebugUI` de biet enemy co ton tai nhung khong thay hinh hay khong.

## Neu enemy khong gay damage

Kiem tra:

1. Enemy prefab co `EnemyMeleeAttack`.
2. `EnemyMeleeAttack.targetTag = Player`.
3. Player co component implement `IDamageable`, hien tai la `PlayerHealth2D`.
4. Collider enemy va player co va cham voi nhau.
5. `EnemyContactDamage2D` cu dang tat de tranh damage trung.

## Neu enemy gay damage hai lan

Kiem tra:

1. `EnemyContactDamage2D` tren Enemy prefab dang tat.
2. Chi dung `EnemyMeleeAttack` cho enemy melee cua Phase 6.

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
