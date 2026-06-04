# Phase 8 - Huong dan Weapon Skill Q

## Muc tieu

Phase 8 chuan hoa he vu khi cua Player:

- Danh thuong: tu dong danh enemy gan nhat trong tam vu khi.
- `Q`: ky nang rieng cua vu khi.
- `E`: Cong Phap.
- `R`: Ultimate.
- Base damage Player hien tai la 50.
- Can bang lai tam danh, vung danh, damage va cooldown de vu khi can chien khong thiet hon Phi Kiem.

## File da tao

- `Assets/Docs/Phase8_HuongDan_WeaponSkill_Q.md`

## File da sua

- `Assets/Script/Weapons/WeaponController.cs`
- `Assets/Script/Weapons/MeleeHitbox.cs`
- `Assets/Script/Weapons/WeaponData.cs`
- `Assets/Script/Combat/ProjectileSpawner.cs`
- `Assets/Docs/Phase8_HuongDan_WeaponSkill_Q.md`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

`CombatScene` hien dang de `WeaponController.weapons` rong, nen runtime se dung bang `CreateDefaultWeaponData()` trong code.

## Logic can bang

```text
Final damage = PlayerStats.BaseDamage * damageMultiplier
BaseDamage hien tai = 50
```

Ly do chinh cua dot can bang nay:

- Kiem, Thuong, Riu phai vao gan enemy hon Phi Kiem, nen can duoc bu bang tam danh/vung danh/damage.
- Phi Kiem co loi the tam xa va an toan, nen damage moi projectile giam de khong vuot DPS can chien.
- Auto attack can hitbox rong hon ban thu cong vi player khong bam chuot trai can tung don nua.
- Auto attack melee chi nen chon muc tieu gan dung tam hitbox. Neu vung chon qua rong, vu khi se danh khi enemy chua nam trong hitbox va bi ton cooldown.
- `MeleeHitbox` co them dung sai collider edge `0.35m` de collider enemy nam sat mep vung chem van duoc tinh trung.
- `WeaponController` co them kiem tra auto target theo shape that cua vu khi melee. Voi Thuong, enemy phai nam trong rectangle dam thi moi bi chon lam muc tieu auto attack.

## Bang vu khi hien tai

### Kiem

Danh thuong:

- Shape: Cone.
- Damage: 175% = 87.5 damage.
- Crit chance: 10%.
- Cooldown: 1.5s.
- Range: 3.8m.
- Angle: 200 do.

Ky nang `Q`:

- Shape: Projectile.
- Damage: 200% = 100 damage.
- Projectile speed: 16 m/s.
- Lifetime: 2s.
- Cooldown: 7s.
- Pierce: co, projectile xuyen qua nhieu enemy tren mot duong thang.

### Thuong

Danh thuong:

- Shape: Rectangle.
- Damage: 190% = 95 damage.
- Crit chance: 10%.
- Cooldown: 1.2s.
- Range: 6.2m.
- Width: 2.1m.

Ky nang `Q`:

- Shape: Circle.
- Damage: 210% = 105 damage.
- Radius: 6.2m.
- Cooldown: 5.2s.

### Riu

Danh thuong:

- Shape: Cone.
- Damage: 250% = 125 damage.
- Crit chance: 5%.
- Cooldown: 1.9s.
- Range: 3.4m.
- Angle: 135 do.

Ky nang `Q`:

- Shape: Rectangle.
- Damage: 315% = 157.5 damage.
- Range: 4.8m.
- Width: 2.8m.
- Cooldown: 8s.

### Phi Kiem

Danh thuong:

- Shape: ProjectileSpread.
- Projectile count: 3.
- Damage moi vien: 38% = 19 damage.
- Tong toi da moi lan ban: 57 damage neu 3 vien trung.
- Crit chance: 20%.
- Cooldown: 1.15s.
- Spread: 18 do.
- Projectile speed: 16 m/s.
- Lifetime: 1s.

Ky nang `Q`:

- Shape: ProjectileSpread.
- Projectile count: 7.
- Damage moi vien: 75% = 37.5 damage.
- Tong toi da neu ca 7 vien trung: 262.5 damage.
- Spread: 80 do.
- Cooldown: 7s.
- Projectile speed: 16 m/s.
- Lifetime: 1s.

## Cach kiem tra trong Unity

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Chon Player.
3. Trong `WeaponController`, doi `Current Weapon` lan luot:
   - `Sword`
   - `Spear`
   - `Axe`
   - `FlyingSword`
4. Chon `GameManager`.
5. Trong `EnemySpawner`, co the de `Spawn Mode = Single Test Enemy` de test tung loai.
6. Bam Play.

## Test case cu the

### Test 1 - Kiem

Buoc test:

1. Set `Current Weapon = Sword`.
2. Cho enemy vao tam auto attack.
3. Bam `Q`.

Ket qua dung:

- Auto attack tao vung chem ban nguyet truoc mat enemy gan nhat.
- Tam chem khoang 3.8m, co dung sai collider de danh enemy melee ma khong phai cham sat.
- `Q` phong kiem khi theo huong aim/move hien tai.
- `Q` xuyen qua nhieu enemy neu enemy nam tren cung mot duong thang.

### Test 2 - Thuong

Buoc test:

1. Set `Current Weapon = Spear`.
2. Cho enemy tien vao theo duong thang hoac hoi lech goc.
3. De enemy vay quanh Player roi bam `Q`.

Ket qua dung:

- Auto attack dam thang khoang 6.2m, width 2.1m.
- Thuong co loi the tam xa ro hon Kiem/Riu.
- `Q` tao vong tron quanh Player radius 6.2m.
- Auto target cua Thuong chi nen kich hoat khi enemy nam trong rectangle dam, tranh mat cooldown vi enemy lech ngoai truc.

### Test 3 - Riu

Buoc test:

1. Set `Current Weapon = Axe`.
2. Cho enemy vao truoc mat Player.
3. Bam `Q`.

Ket qua dung:

- Auto attack la cone 135 do, range 3.4m.
- Moi don danh cham hon nhung sat thuong cao hon.
- `Q` tao vung chu nhat truoc mat dai 4.8m rong 2.8m.

### Test 4 - Phi Kiem

Buoc test:

1. Set `Current Weapon = FlyingSword`.
2. Cho enemy vao tam ban.
3. Bam `Q` khi co nhom enemy truoc mat.

Ket qua dung:

- Auto attack ban 3 phi kiem, moi vien damage thap hon truoc.
- `Q` ban 7 phi kiem theo quat 80 do.
- Phi Kiem van an toan tam xa nhung khong con vuot DPS can chien qua nhieu.

## Ghi chu balance

Uoc tinh DPS danh thuong khi trung tot:

- Kiem: khoang 60.5 DPS, nhung de trung hon do range/goc chem lon hon.
- Thuong: khoang 80 DPS, tam danh xa va width rong hon de on dinh voi auto attack.
- Riu: khoang 65 DPS.
- Phi Kiem: khoang 50-60 DPS tuy so projectile trung.

Bang nay uu tien cam giac test:

- Kiem = can bang, de dung.
- Thuong = tam xa can-trung, can huong nhung khong qua kho.
- Riu = don nang, cham, vung danh lon.
- Phi Kiem = an toan tam xa, damage moi vien thap hon.

## Ghi chu verify

Can chay compile sau khi sua:

```text
dotnet build Assembly-CSharp.csproj
```

Can verify Play Mode truc tiep trong Unity Editor de danh gia cam giac vu khi.
