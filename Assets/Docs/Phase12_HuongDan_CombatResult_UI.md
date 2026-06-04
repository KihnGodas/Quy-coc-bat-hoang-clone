# Phase 12 - Combat Result UI va Restart Button

## Muc tieu

Phase 12 bo sung man hinh ket qua sau combat:

- Khi Victory hoac Defeat, hien UI ket qua o giua man hinh.
- Hien thong tin co ban de playtest:
  - Ket qua Victory/Defeat.
  - Time.
  - Enemy defeated.
  - Enemy remaining.
  - Level.
  - EXP hien tai.
- Co button `Restart` de nguoi choi bam choi lai.
- Khong dung phim `R` de tranh xung dot Ultimate.
- Khong lam he thong level up chon 1 trong 3 nang cap.

## File da tao

- `Assets/Script/UI/CombatResultUI.cs`
- `Assets/Docs/Phase12_HuongDan_CombatResult_UI.md`

## File da sua

- `Assets/Script/Manager/CombatBootstrap.cs`
- `Assets/Script/Spawning/EnemySpawner.cs`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

Khong sua scene/prefab YAML. `CombatBootstrap` tu gan `CombatResultUI` vao GameManager khi Play neu scene chua co component nay.

## Cach hoat dong

### CombatResultUI

`CombatResultUI` dung `OnGUI` de tao UI prototype.

UI chi hien khi:

```text
CombatManager.State == Victory
hoac
CombatManager.State == Defeat
```

Khi hien UI:

- `Victory` dung mau xanh.
- `Defeat` dung mau do.
- Button `Restart` reload scene hien tai bang `SceneManager`.

### Restart button

Khi bam `Restart`:

```text
SceneManager.LoadScene(activeScene.buildIndex)
```

Neu scene khong co build index hop le, fallback ve:

```text
SceneManager.LoadScene(activeScene.name)
```

### Enemy defeated

`EnemySpawner` co them:

- `TotalSpawnedCount`
- `DefeatedEnemyCount`

`DefeatedEnemyCount` duoc tinh bang:

```text
TotalSpawnedCount - AliveCount
```

Day la thong ke prototype de playtest, khong phai analytics chinh thuc.

## Cach kiem tra trong Unity

### Test 1 - Victory result

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Play.
3. Song den khi het timer.
4. Clear het enemy con lai.

Ket qua dung:

- UI hien `VICTORY`.
- Co thong tin Time, Enemy defeated, Enemy remaining, Level, EXP.
- Button `Restart` hien o duoi.
- Bam `Restart` se load lai scene.

### Test 2 - Defeat result

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Play.
3. De Player bi enemy giet.

Ket qua dung:

- UI hien `DEFEAT`.
- Enemy spawner dung spawn them.
- Button `Restart` load lai scene.

### Test 3 - Khong xung dot Ultimate

1. Chon Cong Phap bat ky.
2. Khi combat dang chay, bam `R`.
3. Khi result UI hien, bam button `Restart`.

Ket qua dung:

- `R` van la Ultimate khi combat dang chay.
- Restart khong dung phim `R`, chi dung button UI.

## Thao tac thu cong neu can

Neu muon gan UI san trong scene thay vi runtime:

1. Chon object `GameManager`.
2. Add Component `CombatResultUI`.
3. Co the de trong reference, script se tu tim:
   - `CombatManager`
   - `EnemySpawner`
   - `PlayerExperience`

Mac dinh khong can thao tac nay vi `CombatBootstrap` da tu gan runtime.

## Ghi chu verify

Can chay compile sau khi sua:

```text
dotnet build Assembly-CSharp.csproj
```

Can verify Play Mode truc tiep trong Unity Editor de kiem tra button restart va reload scene.
