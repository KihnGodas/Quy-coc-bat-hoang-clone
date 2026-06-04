# Phase 9 - He thong Cong Phap bang phim E

## Muc tieu

Phase nay bo sung he thong Cong Phap rieng voi vu khi:

- Chuot trai: danh thuong cua vu khi.
- Q: ky nang cua vu khi.
- E: thi trien Cong Phap dang chon.
- 1/2/3/4/5: doi nhanh Cong Phap trong Play Mode de test.
- R: de danh cho Ultimate o phase sau.

He thong duoc gan runtime thong qua `CombatBootstrap`, khong sua scene/prefab YAML de tranh mat reference khi merge code.

## File da tao

- `Assets/Script/Spells/SpellType.cs`
- `Assets/Script/Spells/SpellData.cs`
- `Assets/Script/Spells/SpellVisualEffect2D.cs`
- `Assets/Script/Spells/PlayerSpellController.cs`
- `Assets/Script/Enemies/EnemyStatus2D.cs`
- `Assets/Docs/Phase9_HuongDan_CongPhap_E.md`

## File da sua

- `Assets/Script/Combat/Projectile2D.cs`
- `Assets/Script/Enemies/EnemyBase.cs`
- `Assets/Script/Enemies/EnemyAttackBase.cs`
- `Assets/Script/Manager/CombatBootstrap.cs`
- `Assembly-CSharp.csproj`

## Cong Phap hien co

### 1 - Moc / WoodVine

- Phim cast: E.
- Target: vi tri chuot.
- Damage: 130% BaseDamage = 65 khi BaseDamage = 50.
- Cooldown: 10s.
- Ban kinh: 2m.
- Hieu ung: vong xanh la + cac duong re/cay bo ra tu tam.
- Tac dung: gay damage AoE va Root enemy 1.5s.
- Root chi khoa di chuyen, khong khoa hoan toan attack.

### 2 - Hoa / Fireball

- Phim cast: E.
- Target: huong nham cua player.
- Damage: 230% BaseDamage = 115 khi BaseDamage = 50.
- Cooldown: 8s.
- Toc do dan: 15 m/s.
- Hieu ung: tia/loang do cam khi ban, projectile co hit flash mau lua khi cham muc tieu.
- Tac dung: dan don muc tieu, khong xuyen.

### 3 - Tho / EarthSpike

- Phim cast: E.
- Target: vi tri chuot.
- Damage: 180% BaseDamage = 90 khi BaseDamage = 50.
- Cooldown: 9s.
- Ban kinh: 2m.
- Hieu ung: vong vang nau + cac duong gai dat noi len.
- Tac dung: AoE damage, moi enemy chi nhan damage 1 lan du co nhieu collider.

### 4 - Thuy / WaterArrows

- Phim cast: E.
- Target: huong nham cua player.
- Damage: 3 vien x 75% BaseDamage = 37.5 moi vien khi BaseDamage = 50.
- Cooldown: 7s.
- Toc do dan: 14 m/s.
- Hieu ung: 3 tia xanh nuoc ban noi tiep, moi vien co hit flash xanh khi cham muc tieu.
- Tac dung: sat thuong don muc tieu, nhan manh nhip ban lien tiep.

### 5 - Kim / MetalBlade

- Phim cast: E.
- Target: huong nham cua player.
- Damage: 170% BaseDamage = 85 khi BaseDamage = 50.
- Cooldown: 8s.
- Toc do dan: 16 m/s.
- Hieu ung: vet chem/vang trang hinh luoi cong + tia phong thang.
- Tac dung: projectile xuyen nhieu enemy tren mot duong thang.

## Cach hoat dong trong code

- `CombatBootstrap` tu them `PlayerSpellController` vao Player khi Play Mode neu Player chua co component nay.
- `PlayerSpellController` doc phim `E` de cast va phim `1-5` de doi nhanh Cong Phap.
- Projectile Cong Phap dung lai `ProjectileSpawner` va `Projectile2D` hien co de khong tao he thong dan rieng.
- `Projectile2D` duoc bo sung `ConfigureImpactEffect` de tao hit flash theo mau tung Cong Phap.
- `EnemyStatus2D` quan ly Root/Stun. Hien tai Phase 9 dung Root cho Moc.
- `EnemyBase` doc `EnemyStatus2D.BlocksMovement` de dung di chuyen khi bi Root.
- `EnemyAttackBase` da san sang chan attack khi sau nay co Stun, nhung Root hien tai chi khoa di chuyen.

## Cach kiem tra trong Unity

1. Mo scene combat hien tai.
2. Bam Play.
3. Chon Cong Phap bang phim:
   - 1 = Moc.
   - 2 = Hoa.
   - 3 = Tho.
   - 4 = Thuy.
   - 5 = Kim.
4. Dua chuot vao enemy hoac huong muon ban.
5. Bam E de cast.
6. Quan sat:
   - Moc/Tho xuat hien tai vi tri chuot.
   - Hoa/Thuy/Kim bay theo huong player dang aim.
   - Moc lam enemy dung yen trong thoi gian ngan.
   - Kim co the trung nhieu enemy tren mot duong.

## Test case cu the

### Test 1 - Moc root enemy

- Chon phim 1.
- De chuot vao cum enemy.
- Bam E.
- Ky vong:
  - Co vong va re xanh la.
  - Enemy trong ban kinh bi mat mau.
  - Enemy bi doi tint xanh va dung di chuyen khoang 1.5s.

### Test 2 - Hoa damage don muc tieu

- Chon phim 2.
- Aim vao mot enemy.
- Bam E.
- Ky vong:
  - Co tia do cam khi ban.
  - Projectile cham enemy tao flash mau lua.
  - Dan khong xuyen qua enemy dau tien.

### Test 3 - Tho AoE

- Chon phim 3.
- De chuot giua nhieu enemy.
- Bam E.
- Ky vong:
  - Co vong/gai dat mau vang nau.
  - Enemy trong ban kinh 2m bi damage.
  - Enemy co nhieu collider khong bi tinh damage lap.

### Test 4 - Thuy ban lien tiep

- Chon phim 4.
- Aim vao enemy.
- Bam E.
- Ky vong:
  - Co 3 tia xanh ban lien tiep.
  - Moi vien gay damage rieng.
  - Cooldown ngan hon Hoa/Tho.

### Test 5 - Kim xuyen tuyen

- Chon phim 5.
- Dung sao cho nhieu enemy nam gan cung mot duong thang.
- Bam E.
- Ky vong:
  - Co visual luoi chem vang trang.
  - Projectile xuyen va gay damage nhieu enemy.

## Thao tac thu cong neu can

He thong hien tu gan runtime nen khong bat buoc sua scene. Neu muon Player luon hien `PlayerSpellController` truoc khi Play:

1. Chon Player trong Hierarchy.
2. Add Component `PlayerSpellController`.
3. Co the de trong reference, script se tu tim `PlayerStats`, `PlayerAim2D`, `ProjectileSpawner`.
4. Neu muon doi thong so can bang, sua list `Spells` tren component nay.

## Luu y can bang

- Damage tinh theo `PlayerStats.BaseDamage`, hien tai BaseDamage mac dinh la 50.
- Moc uu tien khong che nen damage vua phai va cooldown cao.
- Hoa la burst don muc tieu nen damage cao nhat.
- Tho la AoE nen damage thap hon Hoa.
- Thuy chia damage thanh 3 vien, hop de test nhieu hit.
- Kim co xuyen muc tieu nen damage moi hit thap hon Hoa.
