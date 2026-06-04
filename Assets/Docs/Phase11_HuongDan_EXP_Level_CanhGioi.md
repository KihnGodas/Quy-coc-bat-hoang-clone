# Phase 11 - EXP, Level va Canh Gioi

## Muc tieu

Phase 11 bo sung progression that cho combat:

- Enemy chet roi EXP orb.
- Player nhat orb de tang EXP.
- Du EXP thi len level.
- Level quyet dinh canh gioi.
- Ultimate chi mo khi Player va Cong phap cung dat `KimDan`.

## File da tao

- `Assets/Script/Progression/CultivationRealm.cs`
- `Assets/Script/Progression/PlayerCultivationState.cs`
- `Assets/Script/Progression/PlayerExperience.cs`
- `Assets/Script/Progression/ExperienceOrb.cs`
- `Assets/Script/Enemies/EnemyExperienceDropper.cs`
- `Assets/Docs/Phase11_HuongDan_EXP_Level_CanhGioi.md`

## File da sua

- `Assets/Script/Enemies/EnemyData.cs`
- `Assets/Script/Enemies/EnemySpawner.cs`
- `Assets/Script/Manager/CombatBootstrap.cs`
- `Assets/Script/UI/CombatHUD2D.cs`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

Khong sua scene/prefab YAML. `CombatBootstrap` tu gan component progression vao Player khi Play.

## Logic progression

### EXP

- Enemy chet se spawn 1 EXP orb.
- Orb tu bay ve Player khi vao gan.
- Cham orb se cong EXP.

### Level

EXP duoc tinh theo cong thuc exponential nhe:

```text
EXP next level = 35 * 1.15^(level - 1)
```

Day la logic ban dau de test; sau nay co the can bang lai rat de.

### Canh gioi

- Level 1-4: `LuyenKhi`
- Level 5-9: `TrucCo`
- Level 10+: `KimDan`

`PlayerCultivationState.CanUseUltimate` chi true khi ca `PlayerRealm` va `SpellRealm` deu dat `KimDan`.

## EXP cua enemy

`EnemyData` co them field `ExperienceReward`.

Neu field nay = 0, game se tu tinh reward tu:

- HP cua enemy.
- Damage cua enemy.
- Role cua enemy.

Day giup project van chay duoc ngay ca khi chua phai sua het cac asset enemy.

## HUD

HUD them dong progression:

- `Lv x | Realm | EXP a/b`

Va dong Ultimate van giu:

- `Ult: Locked`
- `Ult: ... Ready`
- `Ult: ...s`

## Cach hoat dong trong code

### PlayerExperience

- Quan ly EXP hien tai.
- Tu tinh EXP can de len level tiep theo.
- Khi level doi, no cap nhat `PlayerCultivationState`.

### ExperienceOrb

- Tao orb runtime bang sprite don gian.
- Orb co co che attract ve Player.
- Khi cham Player thi cong EXP.

### EnemyExperienceDropper

- Gan tren enemy runtime.
- Lang nghe su kien chet cua `Health` hoac `SimpleHealth`.
- Spawn orb khi enemy chet.

### CombatBootstrap

- Tu gan `PlayerExperience`, `PlayerCultivationState`, `PlayerSpellController`, `PlayerUltimateController` vao Player.
- Khong can sua scene tay.

## Cach kiem tra trong Unity

### Test 1 - Drop EXP

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Play.
3. Giet 1 enemy.

Ket qua dung:

- EXP orb xuat hien tai cho enemy chet.

### Test 2 - Hut EXP

1. De orb nam gan Player.
2. Di lai gan orb.

Ket qua dung:

- Orb tu bay ve Player.
- EXP va Level tren HUD tang len.

### Test 3 - Canh gioi

1. Tang level len 5.
2. Kiem tra HUD.
3. Tang level len 10.

Ket qua dung:

- Level 5 -> `TrucCo`.
- Level 10 -> `KimDan`.
- Ultimate chi mo khi `KimDan`.

### Test 4 - Ultimate lock

1. Doi `Player Realm` hoac `Spell Realm` xuong `TrucCo`.
2. Bam `R`.

Ket qua dung:

- HUD hien `Ult: Locked`.
- `R` khong cast Ultimate.

## Thao tac thu cong neu can

Neu muon debug nhanh:

1. Chon Player runtime.
2. Tim `PlayerExperience`.
3. Doi `Level`/`Current Experience` trong Inspector neu can test nhanh.
4. Hoac cho enemy chet nhieu lan de level len tu nhien.

Neu muon test reward tung enemy:

1. Mo `EnemyData_*.asset`.
2. Sua `Experience Reward` neu muon override.
3. Neu de 0, he thong se dung reward fallback tu HP/Damage/Role.

## Ghi chu verify

Can chay compile sau khi sua:

```text
dotnet build Assembly-CSharp.csproj --no-restore
```

Can verify Play Mode truc tiep trong Unity Editor vi EXP orb, pickup va HUD progression chi danh gia dung khi chay scene.
