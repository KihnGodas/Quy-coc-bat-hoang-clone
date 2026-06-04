# Phase 13 - Huong dan Wave / Difficulty Scaling

## Muc tieu

Phase nay them he thong tang ap luc tran dau theo thoi gian de combat khong bi deu nhip. He thong nay tam thoi khong dung toi progression chon nang cap khi len level, vi phan do nhom chua chot huong thiet ke.

## Da tao file nao

- `Assets/Script/Spawning/CombatDifficultyScaler.cs`
- `Assets/Docs/Phase13_HuongDan_Wave_DifficultyScaling.md`

## Da sua file nao

- `Assets/Script/Spawning/EnemySpawner.cs`
- `Assets/Script/Manager/CombatBootstrap.cs`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong di chuyen file nao.

## Mechanic hoat dong ra sao

`CombatDifficultyScaler` doc thoi gian tu `CombatManager`, sau do tinh do kho theo ti le:

```text
difficulty01 = elapsedTime / combatDuration
```

Tu ti le nay, he thong noi suy:

- Dau tran: spawn interval 2.2s, max alive enemy 7.
- Cuoi tran: spawn interval 0.85s, max alive enemy 14.

`EnemySpawner` van giu nguyen cac thong so goc tren Inspector, nhung khi dang play se dung thong so runtime do `CombatDifficultyScaler` cap vao:

- `EffectiveSpawnInterval`
- `EffectiveMaxAliveEnemies`

Enemy unlock theo `EnemySpawnEntry.unlockTime` van giu nguyen. `CombatScene` dang de `EnemySpawner.Spawn Mode = MixedTable` de khi test co the thay roster enemy mo dan theo thoi gian. Phase nay chi tang nhip spawn va gioi han so enemy song cung luc, khong thay doi AI, sat thuong, HP, visual, hay cach ket thuc victory.

Trong `CombatScene`, `CombatDifficultyScaler` da duoc gan truc tiep len `GameManager` de de nhin thay va test trong Inspector. Component nay co them cac field runtime `Current Difficulty01`, `Current Spawn Interval`, `Current Max Alive Enemies` de kiem tra scaling co dang chay hay khong. `CombatBootstrap` van giu co che tu gan runtime neu scene khac chua co component nay.

## Rui ro can luu y

- Neu tran dau chi dai 20-30s, late phase se day enemy len kha nhanh. Neu thay qua ngop, giam `lateMaxAliveEnemies` ve 10-12 hoac tang `lateSpawnInterval` len 1.1-1.3s.
- Khi `EnemySpawner` dang o `SingleTestEnemy`, scaling van tang so luong enemy test. Neu chi muon test tung con mot cach cham rai, co the tam thoi tat `Scaling Enabled` tren `CombatDifficultyScaler`.
- He thong nay khong sua `spawnLockBeforeCombatEnd`, nen luc gan het gio spawner van dung spawn truoc khi combat chuyen sang clear enemy nhu phase truoc.

## Cach kiem tra trong Unity

1. Mo scene combat hien tai.
2. Bam Play.
3. Chon `GameManager`.
4. Kiem tra co component `CombatDifficultyScaler` tren `GameManager`. Neu dang test scene khac chua gan san component nay, `CombatBootstrap` se tu gan runtime khi bam Play.
5. Chay tran dau tu dau den cuoi:
   - 0-10s: enemy xuat hien thua hon, chu yeu la cac enemy unlock dau bang.
   - Giua tran: enemy bat dau day nhanh hon, roster enemy mo rong hon theo `unlockTime`.
   - Gan cuoi tran: max alive cao hon, spawn interval ngan hon, ap luc ro hon.
7. Trong luc Play, chon `GameManager` va nhin `CombatDifficultyScaler`:
   - `Current Difficulty01` tang dan tu 0 len gan 1.
   - `Current Spawn Interval` giam dan tu 2.2 ve 0.85.
   - `Current Max Alive Enemies` tang dan tu 7 len 14.
6. Doi den het thoi gian:
   - Spawner dung sinh enemy moi.
   - Victory chi hien khi tat ca enemy con lai da bi diet het.

## Test case cu the

- Test 1: Combat bat dau, enemy khong spawn lien tuc ngay lap tuc ma theo interval dau tran khoang 2.2s.
- Test 2: Khi elapsed time tang, khoang cach giua cac lan spawn ngan dan.
- Test 3: So enemy song cung luc khong vuot qua `EffectiveMaxAliveEnemies`.
- Test 4: Neu tat `Scaling Enabled`, `EnemySpawner` quay ve dung `spawnInterval` va `maxAliveEnemies` goc.
- Test 5: Khi het gio, khong co enemy moi xuat hien sau moc stop-spawn, va victory doi den khi clear het enemy con tren arena.

## Thao tac thu cong neu can

Neu muon can bang nhanh trong Inspector:

1. Bam Play.
2. Chon object co `CombatDifficultyScaler`.
3. Chinh:
   - `Early Spawn Interval`
   - `Late Spawn Interval`
   - `Early Max Alive Enemies`
   - `Late Max Alive Enemies`
4. Test lai cam giac ap luc theo tung moc thoi gian.
