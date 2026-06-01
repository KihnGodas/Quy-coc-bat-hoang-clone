# Fix EnemySpawner - Khoi phuc script Enemy bi thieu

## Van de

`EnemySpawner.cs` dang reference class `EnemyChaseAI2D`, nhung file script `EnemyChaseAI2D.cs` khong ton tai trong project.

`Projectile2D.cs` va prefab `Enemy` cung dang can `SimpleHealth`, nhung file `SimpleHealth.cs` cung khong ton tai.

Ket qua co the gap trong Unity:

- Loi compile trong `EnemySpawner.cs` vi khong tim thay `EnemyChaseAI2D`.
- Prefab `Enemy` co component Missing Script.
- Projectile khong gay damage dung cho Enemy.
- EnemySpawner khong spawn duoc enemy.

## File da tao

- `Assets/Script/Enemies/EnemyChaseAI2D.cs`
- `Assets/Script/Core/SimpleHealth.cs`
- `Assets/Docs/Fix_EnemySpawner_MissingScripts.md`

## File da sua

- `Assets/Script/Spawning/EnemySpawner.cs`

## Diem quan trong ve reference

Da tao lai file `.meta` voi dung GUID ma prefab `Enemy` dang reference:

- `EnemyChaseAI2D`: `7168da36d3794ac458a0d0dd69ba03db`
- `SimpleHealth`: `fdf45cb4d677b804eb8ffb856f3c131c`

Viec nay giup prefab `Enemy` khong bi mat component da gan san.

## Cach kiem tra trong Unity

1. Quay lai Unity va doi project recompile xong.
2. Mo prefab `Assets/Prefabs/Enemy.prefab`.
3. Kiem tra khong con component `Missing Script`.
4. Mo scene `CombatPrototype`.
5. Bam Play.
6. Kiem tra `GameManager > EnemySpawner` khong bao loi.
7. Cho enemy spawn tu canh arena va di ve phia Player.
8. Ban enemy bang chuot trai hoac Q, enemy phai mat mau va chet khi het HP.

## Neu van con loi

Neu Unity van hien Missing Script tren prefab `Enemy`:

1. Chon prefab `Enemy`.
2. Remove component Missing Script.
3. Add component `EnemyChaseAI2D`.
4. Add component `SimpleHealth`.
5. Dat lai thong so:
   - `EnemyChaseAI2D Move Speed = 2.5`
   - `SimpleHealth Max Health = 50`
   - `SimpleHealth Destroy On Death = true`
