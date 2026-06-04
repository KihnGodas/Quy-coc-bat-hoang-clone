# Phase 5 - Combat Manager

## Muc tieu

Phase nay tao he thong quan ly mot tran combat co trang thai ro rang:

- Chua bat dau.
- Dang chay.
- Het gio va dang don enemy con lai.
- Victory.
- Defeat.

Combat Manager la nen de sau nay noi enemy wave, boss, reward va man choi.

## File da tao

- `Assets/Script/Manager/CombatResult.cs`
- `Assets/Script/Manager/CombatTimer.cs`
- `Assets/Script/Manager/CombatManager.cs`
- `Assets/Script/Camera/CameraFollow2D.cs`
- `Assets/Docs/Phase5_HuongDan_CombatManager.md`

## File da sua

- `Assets/Script/Manager/CombatBootstrap.cs`
- `Assets/Script/Spawning/EnemySpawner.cs`
- `Assets/Script/UI/CombatHUD2D.cs`
- `Assets/Script/UI/DebugCombatUI.cs`
- `Assets/Scenes/CombatScene.unity`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

File doc cu `Assets/Docs/Phase5_HuongDan_DamageSystem_EnemySpawner.md` da duoc thay bang doc Phase 5 dung theo roadmap goc: Combat Manager.

## Mechanic hoat dong ra sao

### CombatResult

`CombatResult` gom:

```text
None
Victory
Defeat
```

Dung de cac he thong khac biet ket qua tran dau.

### Dieu kien Victory cua normal combat

Normal combat khong Victory ngay khi het thoi gian.

Dieu kien Victory dung la:

```text
Timer da het
EnemySpawner da dung spawn enemy moi
Enemy con lai trong arena = 0
```

Khi timer ve 0 nhung van con enemy:

- `CombatManager.State = ClearingEnemies`
- HUD hien `Clear Enemies: n`
- EnemySpawner khong sinh enemy moi nua
- Player phai diet het enemy con lai de Victory

### CombatTimer

`CombatTimer` la class dem thoi gian cho normal combat:

- `Start(duration)`
- `Tick(deltaTime)`
- `Stop()`
- `Elapsed`
- `Remaining`
- `IsComplete`

Class nay khong la `MonoBehaviour`; no duoc `CombatManager` quan ly truc tiep.

### CombatManager

`CombatManager` duoc gan tren object `GameManager` trong `CombatScene`.

Mode hien co:

- `NormalCombat`
- `BossCombat`

State hien co:

- `NotStarted`
- `Running`
- `ClearingEnemies`
- `Victory`
- `Defeat`

Event hien co:

- `OnCombatStarted`
- `OnCombatVictory`
- `OnCombatDefeat`

Normal combat:

1. `CombatManager` auto start khi Play neu `autoStart = true`.
2. Bat `EnemySpawner`.
3. Chay timer `normalCombatDuration`.
4. Het timer thi tat `EnemySpawner` va chuyen sang `ClearingEnemies`.
5. Khi enemy con lai trong arena ve 0 thi Victory.
6. Neu Player chet truoc hoac chet trong luc `ClearingEnemies` thi Defeat.
7. Khi Victory/Defeat, dam bao `EnemySpawner` da tat.

Boss combat:

1. Khong dung timer de Victory.
2. Neu Player chet thi Defeat.
3. Neu boss co `Health` va boss chet thi Victory.
4. Phase nay chi tao nen boss mode, chua tao boss cu the.

### EnemySpawner

`EnemySpawner` duoc them:

- `spawningEnabled`
- `IsSpawningEnabled`
- `SetSpawningEnabled(bool)`
- `combatManager`
- `spawnLockBeforeCombatEnd`

Khi `spawningEnabled = false`, spawner khong sinh enemy moi. Enemy dang ton tai khong bi xoa.

Trong `CombatScene`, `spawningEnabled` mac dinh de `false`; `CombatManager` se bat spawner khi combat start.

Spawner chi sinh enemy khi combat dang `Running`. Voi normal combat, spawner se ngung sinh enemy moi trong khoang `spawnLockBeforeCombatEnd` truoc khi het gio. Mac dinh scene dang de `0.5s`.

Muc dich:

- Tranh truong hop dung frame cuoi cung timer ve 0 nhung spawner vua tao enemy moi.
- Tranh cam giac nguoi choi thay Victory roi nhung enemy moi van di tu ria map vao.

### CombatBootstrap

`CombatBootstrap` duoc them reference `CombatManager` de cac phase sau co the truy cap manager trung tam.

### CombatHUD2D

`CombatHUD2D` hien them dong:

- `Time Left: xs` khi dang o `NormalCombat` va combat dang chay.
- `Clear Enemies: n` khi het gio nhung van con enemy trong arena.
- `Combat: Victory` hoac `Combat: Defeat` khi tran dau ket thuc.
- `Combat: Running | Boss Mode` neu sau nay doi sang boss combat.

Dong nay nam duoi HP tren HUD chinh, de nguoi choi nhin duoc thoi gian con lai ngay trong man hinh gameplay.

HUD gameplay hien tai duoc rut gon thanh 3 dong:

- HP.
- Time Left, Clear Enemies hoac Combat Result.
- Skill va Dash tren cung mot dong.

Muc dich la tranh viec HUD va debug UI de len nhau.

### DebugCombatUI

`DebugCombatUI` hien them:

- Combat state.
- Timer con lai neu la normal combat.
- Boss mode neu dang dung boss combat.

`DebugCombatUI` co them `showDebugUI`. Trong `CombatScene`, gia tri mac dinh la `false` de HUD gameplay khong bi de chu. Khi can debug, co the bat lai trong Inspector tren object `GameManager`.

### CameraFollow2D

`CameraFollow2D` duoc gan vao `Main Camera`.

Hoat dong:

1. Theo doi `target`, mac dinh scene tro toi Player.
2. Di chuyen trong `LateUpdate` de camera theo sau player muot hon.
3. Giu nguyen truc z cua camera.
4. Neu co `ArenaBounds`, camera duoc clamp trong bien arena dua theo kich thuoc orthographic hien tai.

Trong `CombatScene`:

- `Target` tro toi Player.
- `Arena Bounds` tro toi `ArenaBounds`.
- `Smooth Time = 0.12`.
- `Clamp To Arena = true`.

## Cach kiem tra trong Unity

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Chon object `GameManager`.
3. Kiem tra co component:
   - `EnemySpawner`
   - `CombatHUD2D`
   - `CombatBootstrap`
   - `DebugCombatUI`
   - `CombatManager`
4. Kiem tra `CombatManager`:
   - `Combat Mode = NormalCombat`
   - `Normal Combat Duration = 20`
   - `Auto Start = true`
   - `Control Enemy Spawner = true`
   - `Player Health` tro den Player.
   - `Enemy Spawner` tro den `EnemySpawner`.
5. Bam Play.

Ket qua dung:

- Combat state hien `Running`.
- HUD chinh hien `Time Left` ngay duoi HP.
- HUD chinh khong bi de chu.
- DebugCombatUI mac dinh khong hien; neu can debug thi bat `showDebugUI`.
- Enemy bat dau spawn sau khi combat start.
- Khi timer ve 0, combat state chuyen `ClearingEnemies` neu con enemy.
- Sau khi het gio, spawner khong sinh enemy moi.
- Trong 0.5s cuoi tran normal combat, spawner khong tao enemy moi.
- Khi enemy con lai ve 0, combat state moi chuyen `Victory`.
- Camera di chuyen theo Player va khong vuot ra ngoai `ArenaBounds`.

## Test case cu the

### Test 1 - Normal combat auto start

Buoc test:

1. Mo `CombatScene`.
2. Bam Play.
3. Quan sat `DebugCombatUI`.

Ket qua dung:

- `Combat: Running`.
- Time left giam dan.
- HUD chinh hien dong `Time Left: xs`.
- Enemy spawn theo `EnemySpawner`.

### Test 2 - Het timer thi dung spawn va doi clear enemy

Buoc test:

1. Dat `CombatManager.normalCombatDuration = 5` de test nhanh.
2. Bam Play.
3. Doi 5 giay.

Ket qua dung:

- Neu con enemy tren arena, combat state chuyen `ClearingEnemies`.
- EnemySpawner dung spawn enemy moi.
- Khong co enemy moi xuat hien dung khoanh khac timer ve 0.
- Enemy da spawn truoc do khong bi xoa bat ngo.

### Test 2b - Victory khi clear het enemy sau khi het gio

Buoc test:

1. Lam tiep tu Test 2.
2. Diet het enemy con lai tren arena.

Ket qua dung:

- HUD dang hien `Clear Enemies: n`.
- Moi enemy bi diet lam so dem giam xuong.
- Khi so enemy con lai ve 0, combat state chuyen `Victory`.
- EnemySpawner van khong spawn them enemy moi.

### Test 3 - Defeat khi Player chet

Buoc test:

1. Bam Play.
2. De enemy cham Player den khi HP ve 0.

Ket qua dung:

- Player goi `OnDeath`.
- Combat state chuyen `Defeat`.
- EnemySpawner dung spawn enemy moi.
- Player controls bi tat theo `PlayerHealth2D`.

### Test 4 - Boss mode nen

Buoc test thu cong:

1. Chon `GameManager`.
2. Doi `CombatManager.combatMode = BossCombat`.
3. Neu co boss prototype sau nay, gan `Boss Health`.
4. Bam Play.

Ket qua dung:

- Combat state chuyen `Running`.
- Timer khong dung de Victory.
- Player chet thi Defeat.
- Boss chet thi Victory neu da gan `Boss Health`.

### Test 5 - HUD khong de chu

Buoc test:

1. Mo `CombatScene`.
2. Bam Play.
3. Nhin goc tren trai man hinh.

Ket qua dung:

- Chi thay HUD gameplay gom HP, Time Left, Skill/Dash.
- Khong thay block debug gom Player HP, Timer, Enemy Count neu `showDebugUI = false`.
- Neu bat `DebugCombatUI.showDebugUI = true`, debug UI hien bat dau tu vi tri thap hon va co the dung de test.

### Test 6 - Camera follow map rong

Buoc test:

1. Bam Play.
2. Di chuyen Player sang trai/phai/tren/duoi bang WASD.
3. Quan sat camera.

Ket qua dung:

- Camera di chuyen theo Player.
- Player khong bi di ra ngoai tam nhin trong luc di chuyen binh thuong.
- Khi toi gan bien arena, camera dung lai theo bien map thay vi nhin ra ngoai vung choi.

## Neu Combat khong start

Kiem tra:

1. `GameManager` co `CombatManager`.
2. `CombatManager.autoStart` dang bat.
3. `CombatManager.playerHealth` khong bi trong.
4. Console khong co loi compile/Missing Script.

## Neu Enemy khong spawn

Kiem tra:

1. Combat state co dang `Running` khong.
2. `CombatManager.controlEnemySpawner` co bat khong.
3. `CombatManager.enemySpawner` co tro dung `EnemySpawner` khong.
4. `EnemySpawner.enemyPrefab` khong bi trong.
5. `EnemySpawner.maxAliveEnemies` lon hon 0.
6. Neu gan cuoi tran, kiem tra `EnemySpawner.spawnLockBeforeCombatEnd`; trong khoang nay enemy moi se khong spawn.

## Neu camera khong di theo Player

Kiem tra:

1. `Main Camera` co component `CameraFollow2D`.
2. `CameraFollow2D.target` tro toi Player.
3. `CameraFollow2D.arenaBounds` tro toi `ArenaBounds`.
4. Player co tag `Player` de script tu tim lai neu reference bi trong.

## Ghi chu verify

Da chay:

```text
dotnet build Assembly-CSharp.csproj
```

Ket qua:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

Chua verify truc tiep bang Play Mode trong Unity Editor.
