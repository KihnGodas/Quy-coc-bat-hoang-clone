# Phase 14 - Huong dan Stage Type va Boss Combat

## Muc tieu

Phase nay tach ro 2 loai man cho project:

- `NormalCombat`: man danh quai thuong co timer nhu hien tai.
- `BossCombat`: man danh boss, khong tinh thoi gian thang. Dieu kien victory la boss bi diet.

Phase nay chi dung nen boss mode va boss prototype de test dieu kien win/lose. Chua trien khai day du skill boss theo GD.

## Da tao file nao

- `Assets/Script/Bosses/BossBase.cs`
- `Assets/Script/Bosses/BossController.cs`
- `Assets/Script/Bosses/BossVisual2D.cs`
- `Assets/Script/UI/BossHealthUI.cs`
- `Assets/Docs/Phase14_HuongDan_StageType_BossCombat.md`

## Da sua file nao

- `Assets/Script/Manager/CombatManager.cs`
- `Assets/Script/Manager/CombatBootstrap.cs`
- `Assets/Script/Spawning/EnemySpawner.cs`
- `Assets/Script/UI/CombatHUD2D.cs`
- `Assets/Scenes/CombatScene.unity`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong di chuyen file nao.

## Mechanic hoat dong ra sao

### NormalCombat

Day la logic man thuong hien tai:

- `CombatManager.Mode = NormalCombat`.
- Combat co timer.
- `EnemySpawner` duoc bat khi combat start.
- Het gio thi spawner dung sinh enemy moi.
- Victory chi hien khi da het gio va clear het enemy con lai tren arena.
- Player chet thi Defeat.

### BossCombat

Day la logic man boss moi:

- `CombatManager.Mode = BossCombat`.
- Combat khong dung timer de thang.
- `EnemySpawner` khong sinh quai thuong.
- `BossController` spawn boss prototype neu chua co boss trong scene.
- `CombatManager` bind `bossHealth` tu boss.
- Victory khi boss chet.
- Player chet thi Defeat.

Boss prototype hien tai:

- Ten: `Boss Prototype - Co Thu Ton Gia`.
- HP: `4500`.
- Damage co ban: `60`.
- Vi tri spawn: tam map `(0, 0)`.
- Layer/tag: `Enemy`, nen vu khi, cong phap va ultimate cua Player co the danh trung.
- Visual: hinh vuong xanh la, pulse nhe, doi mau dan sang do khi sap het mau.

## Cach chuyen loai man trong Unity

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Chon `GameManager`.
3. Trong `CombatManager`, doi `Combat Mode`:
   - `NormalCombat`: test man quai thuong.
   - `BossCombat`: test man boss.
4. Bam Play.

## Cach kiem tra NormalCombat

1. Set `CombatManager.Mode = NormalCombat`.
2. Bam Play.
3. Enemy thuong spawn theo wave/difficulty scaling.
4. Het timer thi enemy khong spawn them.
5. Diet het enemy con lai de hien Victory.

## Cach kiem tra BossCombat

1. Set `CombatManager.Mode = BossCombat`.
2. Bam Play.
3. Kiem tra enemy thuong khong spawn.
4. Boss prototype xuat hien o giua map.
5. HUD hien `Boss Stage`.
6. Thanh mau boss hien ben trai man hinh.
7. Dung danh thuong, `Q`, `E`, `R` de gay damage boss.
8. Khi boss HP ve 0, hien Victory.
9. Neu Player chet truoc boss, hien Defeat.

## Test case cu the

- Test 1: `NormalCombat` van chay nhu phase 13, co timer va wave enemy.
- Test 2: `BossCombat` khong co Time Left de quyet dinh victory.
- Test 3: Trong `BossCombat`, `EnemySpawner.IsSpawningEnabled` khong lam sinh quai thuong.
- Test 4: Boss spawn dung layer `Enemy`, player danh trung boss bang vu khi can chien va tam xa.
- Test 5: Boss chet thi `CombatManager.Result = Victory`.
- Test 6: Player chet thi `CombatManager.Result = Defeat`.

## Luu y

- Boss prototype chi la nen test stage type, chua phai boss GD day du.
- Neu can test nhanh hon, co the giam `Prototype Boss HP` tren `BossController`.
- Neu muon gan boss prefab rieng, gan vao field `Boss Prefab` trong `BossController`; neu de trong, he thong se tu tao boss prototype runtime.
