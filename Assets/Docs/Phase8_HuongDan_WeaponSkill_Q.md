# Phase 8 - Huong dan Weapon Skill Q

## Muc tieu

Phase 8 chuan hoa he vu khi cua Player theo tai lieu gameplay moi cua nhom:

- Chuot trai: danh thuong cua vu khi.
- `Q`: ky nang rieng cua vu khi.
- Khong chuyen ky nang vu khi thanh passive tu dong.
- Giu base damage Player hien tai la 50.
- Can bang lai he so damage/range/cooldown de hop voi enemy hien tai.

## File da tao

- `Assets/Docs/Phase8_HuongDan_WeaponSkill_Q.md`

## File da sua

- `Assets/Script/Weapons/WeaponController.cs`
- `Assets/Script/Weapons/WeaponData.cs`
- `Assets/Script/Combat/ProjectileSpawner.cs`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

Khong sua scene/prefab trong Phase 8 nay. `CombatScene` hien dang de `WeaponController.weapons` rong, nen runtime se dung bang `CreateDefaultWeaponData()` trong code.

## Input hien tai

- Chuot trai: danh thuong.
- `Q`: ky nang vu khi.
- `E`: de danh cho Cong Phap o phase sau.
- `R`: de danh cho Ultimate o phase sau.
- Dash hien tai van la `Space`.

## Logic can bang

Base damage cua Player hien tai la 50, nen he so damage duoc tinh nhu sau:

```text
Final damage = 50 * damageMultiplier
```

Voi enemy hien tai co HP khoang 70 - 300, Phase 8 khong dung toan bo he so qua cao cua tai lieu goc cho moi projectile. Dac biet Phi Kiem Q co 7 vien, neu moi vien 150% thi tong toi da la 1050%, qua manh cho prototype hien tai.

## Bang vu khi sau Phase 8

### Kiem

Danh thuong:

- Shape: Cone.
- Damage: 150% = 75 damage.
- Crit chance: 10%.
- Cooldown: 1.5s.
- Range: 2m.
- Angle: 180 do.

Ky nang `Q`:

- Shape: Projectile.
- Damage: 190% = 95 damage.
- Projectile speed: 16 m/s.
- Lifetime: 2s.
- Cooldown: 7s.
- Pierce: co, projectile xuyen qua nhieu enemy tren mot duong thang.
- Muc tieu gameplay: kiem khi bay xuyen/di xa, dung de danh nhieu enemy tren mot huong.

### Thuong

Danh thuong:

- Shape: Rectangle.
- Damage: 160% = 80 damage.
- Crit chance: 10%.
- Cooldown: 1.25s.
- Range: 4m.
- Width: 1.2m.

Ky nang `Q`:

- Shape: Circle.
- Damage: 180% = 90 damage.
- Radius: 5.5m.
- Cooldown: 5s.
- Muc tieu gameplay: clear vung quanh Player, manh khi bi vay.

### Riu

Danh thuong:

- Shape: Cone.
- Damage: 200% = 100 damage.
- Crit chance: 5%.
- Cooldown: 2s.
- Range: 2.5m.
- Angle: 120 do.

Ky nang `Q`:

- Shape: Rectangle.
- Damage: 280% = 140 damage.
- Range: 3.8m.
- Width: 2.3m.
- Cooldown: 8s.
- Muc tieu gameplay: don nang, sat thuong cao, vung danh hep hon va hoi chieu dai.

### Phi Kiem

Danh thuong:

- Shape: ProjectileSpread.
- Projectile count: 3.
- Damage moi vien: 50% = 25 damage.
- Crit chance: 20%.
- Cooldown: 1s.
- Spread: 18 do.
- Projectile speed: 16 m/s.
- Lifetime: 1s.

Ky nang `Q`:

- Shape: ProjectileSpread.
- Projectile count: 7.
- Damage moi vien: 90% = 45 damage.
- Tong toi da neu ca 7 vien trung: 315 damage.
- Spread: 80 do.
- Cooldown: 7s.
- Projectile speed: 16 m/s.
- Lifetime: 1s.
- Muc tieu gameplay: burst theo hinh quat, manh khi can clear nhom enemy nhung can aim dung.

## Cach kiem tra trong Unity

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Chon Player.
3. Trong `WeaponController`, doi `Current Weapon` lan luot:
   - `Sword`
   - `Spear`
   - `Axe`
   - `FlyingSword`
4. Chon `GameManager`.
5. Trong `EnemySpawner`, de `Spawn Mode = Single Test Enemy`.
6. Test voi cac enemy:
   - `EnemyData_KhoiLang`
   - `EnemyData_ThietTru`
   - `EnemyData_DaiLang`
   - `EnemyData_XaYeu`
7. Bam Play.

## Test case cu the

### Test 1 - Kiem

Buoc test:

1. Set `Current Weapon = Sword`.
2. Bam giu chuot trai.
3. Bam `Q`.

Ket qua dung:

- Chuot trai tao vung chem ban nguyet truoc mat.
- Tam chem khoang 2m, khong qua dai.
- `Q` phong kiem khi theo huong aim/move hien tai.
- `Q` xuyen qua nhieu enemy neu enemy nam tren cung mot duong thang.
- `Q` co cooldown 7s.

### Test 2 - Thuong

Buoc test:

1. Set `Current Weapon = Spear`.
2. Bam chuot trai.
3. De enemy vay quanh Player roi bam `Q`.

Ket qua dung:

- Chuot trai dam thang khoang 4m, width 1.2m, damage 160% va cooldown 1.25s de bu cho viec can huong thang.
- `Q` tao vong tron quanh Player radius 5.5m.
- Damage Q manh nhung khong nen xoa sach enemy tank.

### Test 3 - Riu

Buoc test:

1. Set `Current Weapon = Axe`.
2. Bam chuot trai vao enemy truoc mat.
3. Bam `Q`.

Ket qua dung:

- Chuot trai la cone 120 do, sat thuong cao hon cac vu khi khac.
- `Q` tao vung chu nhat truoc mat dai 3.8m rong 2.3m.
- `Q` co cooldown 8s, neu dung sai huong se de hut.

### Test 4 - Phi Kiem

Buoc test:

1. Set `Current Weapon = FlyingSword`.
2. Bam giu chuot trai.
3. Bam `Q` khi co nhom enemy truoc mat.

Ket qua dung:

- Chuot trai ban 3 phi kiem, moi vien damage vua phai.
- `Q` ban 7 phi kiem theo quat 80 do.
- `Q` manh khi enemy dung thanh nhom, nhung khong nen one-shot moi thu neu chi vai vien trung.

## Ghi chu balance

Bang hien tai la ban prototype can bang voi enemy Phase 7:

- Kiem va Thuong on dinh, de dung.
- Riu sat thuong cao nhung cham.
- Phi Kiem co tam xa va crit cao, nen damage moi projectile thap hon de tranh qua manh.

Sau khi co Cong Phap `E`, Ultimate `R`, XP/level va upgrade, cac he so nay can duoc balance lai o Phase 17.

## Ghi chu verify

Can chay compile sau khi sua:

```text
dotnet build Assembly-CSharp.csproj --no-restore
```

Can verify Play Mode truc tiep trong Unity Editor de danh gia cam giac vu khi.
