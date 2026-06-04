# Phase 10 - He thong Ultimate bang phim R

## Muc tieu

Phase 10 cap nhat lai toan bo logic Ultimate theo design spec moi cua nhom.

Input tong quan:

- Chuot trai: danh thuong cua vu khi.
- Q: ky nang rieng cua vu khi.
- E: Cong Phap dang chon.
- R: Ultimate tuong ung voi Cong Phap dang chon.
- 1/2/3/4/5: doi Cong Phap trong Play Mode, Ultimate se doi theo Cong Phap do.

Dieu kien kich hoat:

- Player canh gioi = `KimDan`.
- Cong Phap canh gioi = `KimDan`.
- Cooldown chung: 25s cho tat ca Ultimate.

Prototype hien tai tu gan `PlayerCultivationState` vao Player khi Play, va mac dinh ca Player/Cong Phap deu la `KimDan` de co the test Ultimate ngay. Sau nay khi co phase progression that, state nay se duoc dieu khien boi he level/canh gioi.

## File da tao

- `Assets/Script/Progression/CultivationRealm.cs`
- `Assets/Script/Progression/PlayerCultivationState.cs`
- `Assets/Script/Ultimates/UltimateType.cs`
- `Assets/Script/Ultimates/UltimateData.cs`
- `Assets/Script/Ultimates/PlayerUltimateController.cs`
- `Assets/Docs/Phase10_HuongDan_Ultimate_R.md`

## File da sua

- `Assets/Script/Spells/SpellVisualEffect2D.cs`
- `Assets/Script/Enemies/EnemyStatus2D.cs`
- `Assets/Script/Manager/CombatBootstrap.cs`
- `Assets/Script/UI/CombatHUD2D.cs`
- `Assets/Script/Ultimates/UltimateData.cs`
- `Assets/Script/Ultimates/PlayerUltimateController.cs`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

Khong sua scene/prefab YAML. `CombatBootstrap` tu them runtime component vao Player:

- `PlayerSpellController`
- `PlayerCultivationState`
- `PlayerUltimateController`

## Status system

`EnemyStatus2D` phan biet ro:

- `ROOT`: enemy khong the di chuyen, van co the tan cong neu attack logic cho phep.
- `STUN`: enemy khong the di chuyen va khong the tan cong.
- `KNOCKBACK`: enemy bi day theo vector trong thoi gian ngan, movement bi khoa trong luc day.

`EnemyBase` da doc `EnemyStatus2D.BlocksMovement`, nen Root/Stun/Knockback deu chan chase movement.

`EnemyAttackBase` doc `EnemyStatus2D.BlocksAttack`, nen Stun chan attack, Root khong chan attack.

## Bang Ultimate moi

BaseDamage Player hien tai la 50.

### Moc - ULT_WOOD / Van Dang Phong Thien

- Phim test: chon `1`, bam `R`.
- Loai: AoE xung quanh nguoi choi.
- Input: vi tri Player tai thoi diem cast.
- Hinh dang: hinh tron.
- Ban kinh: 8m.
- Damage: 400% BaseDamage = 200.
- Hieu ung: Root.
- Root duration: 3s.
- Ap dung: tat ca enemy trong vung.
- Visual:
  - Dai tran re xanh xung quanh Player.
  - Vong ngoai lon ban kinh 8m.
  - Nhieu day leo toa tu tam den enemy/vung ngoai.
  - Enemy bi root doi tint xanh va co re quanh than.

Logic:

1. Cast.
2. Lay vi tri Player lam tam.
3. Spawn day leo quanh vung.
4. Enemy trong ban kinh 8m nhan 400% damage.
5. Enemy trong vung nhan ROOT 3s.

### Hoa - ULT_FIRE / Liet Duong Thien Van

- Phim test: chon `2`, bam `R`.
- Loai: AoE tai vi tri chi dinh.
- Input: vi tri chuot.
- Hinh dang: hinh tron.
- Ban kinh: 5m.
- Damage: 600% BaseDamage = 300.
- Hieu ung: khong co status.
- Delay animation: 0.8s.
- Visual:
  - Warning ring do/cam tai vi tri target.
  - Duong thien thach roi tu tren xuong.
  - Sau delay co vong no lua va tia lua toa ra.

Logic:

1. Player chon vi tri.
2. Hien warning indicator 0.8s.
3. Sau delay, apply 600% damage len tat ca enemy trong ban kinh 5m.
4. Enemy chay khoi vung truoc khi no se khong bi hit.

### Tho - ULT_EARTH / Thien Nham Tran Nguc

- Phim test: chon `3`, bam `R`.
- Loai: AoE tai vi tri nguoi choi.
- Input: vi tri Player tai thoi diem cast.
- Hinh dang: hinh tron.
- Ban kinh: 6m.
- Damage: 500% BaseDamage = 250.
- Hieu ung: Stun.
- Stun duration: 2s.
- Ap dung: tat ca enemy trong vung.
- Visual:
  - Dia chan quanh Player.
  - Nhieu vong nut dat dong tam.
  - Nhieu gai da moc len theo tung lop.

Logic:

1. Cast.
2. Lay vi tri Player lam tam.
3. Apply 500% damage cho enemy trong ban kinh 6m.
4. Apply STUN 2s.
5. STUN chan ca di chuyen va tan cong.

### Thuy - ULT_WATER / Thuong Hai No Trieu

- Phim test: chon `4`, bam `R`.
- Loai: AoE hinh chu nhat ban thang ve phia truoc.
- Input: huong Player dang aim tai thoi diem cast.
- Hinh dang: chu nhat.
- Chieu dai: 15m.
- Chieu rong: 8m.
- Goc vung: vi tri Player.
- Damage: 450% BaseDamage = 225.
- Hieu ung: Knockback.
- Knockback speed: 12 m/s.
- Knockback duration: 0.25s.
- Tong khoang day uoc tinh: 3m.
- Huong knockback: cung huong voi song.
- Visual:
  - Khung chu nhat 15x8m theo huong aim.
  - Nhieu duong song nuoc song song/luon song ben trong.
  - Enemy bi day co duong knockback xanh ngan.

Logic:

1. Cast.
2. Lay huong aim hien tai.
3. Tao hitbox chu nhat 15x8m bat dau tu Player.
4. Enemy trong hitbox nhan 450% damage.
5. Enemy trong hitbox bi KNOCKBACK theo vector huong cast.

### Kim - ULT_METAL / Van Nhan Quy Tong

- Phim test: chon `5`, bam `R`.
- Loai: AoE tai vi tri chi dinh, multi-hit.
- Input: vi tri chuot.
- Hinh dang: hinh tron.
- Ban kinh: 10m.
- Tong damage toi da: 700% BaseDamage = 350 neu enemy trung toan bo hit.
- So luong kim nhan: N = 20.
- Damage moi kim nhan: 700% / 20 = 35% BaseDamage = 17.5.
- Hit radius moi kim nhan: 1m.
- Delay animation: 0.6s.
- Hieu ung: khong co status.
- Visual:
  - Vong warning vang/trang ban kinh 10m.
  - Nhieu duong sight line trong vung.
  - Sau delay co 20 vet kim nhan roi xuong ngau nhien trong vung.

Logic:

1. Player chon vi tri.
2. Hien warning indicator 0.6s.
3. Spawn 20 kim nhan tai diem ngau nhien trong ban kinh 10m.
4. Moi kim nhan tao hit radius 1m.
5. Enemy co the trung nhieu kim nhan va nhan damage cong don.
6. Tong toi da 700% neu trung du 20 hit.

## HUD

HUD them dong:

- `Ult: <TenUltimate> Ready`
- `Ult: 12.3s`
- `Ult: Locked`

Neu `PlayerCultivationState.CanUseUltimate == false`, bam `R` se khong cast.

## Cach kiem tra trong Unity

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Bam Play.
3. Doi Cong Phap bang phim:
   - 1 = Moc.
   - 2 = Hoa.
   - 3 = Tho.
   - 4 = Thuy.
   - 5 = Kim.
4. Bam `R`.
5. Quan sat HUD dong `Ult`.
6. Sau khi cast, doi Cong Phap khac va thu bam R tiep.
7. Ket qua dung: cooldown chung 25s, doi Cong Phap khong reset cooldown.

## Test case cu the

### Test 1 - Dieu kien Kim Dan

Buoc test:

1. Play scene.
2. Chon Player runtime.
3. Tim `PlayerCultivationState`.
4. Doi `Player Realm` hoac `Spell Realm` ve duoi `KimDan`.
5. Bam `R`.

Ket qua dung:

- HUD hien `Ult: Locked`.
- Bam `R` khong cast Ultimate.
- Doi ca hai ve `KimDan`, Ultimate cast duoc.

### Test 2 - Moc quanh Player

Buoc test:

1. Chon `1`.
2. De enemy quanh Player.
3. Bam `R`.

Ket qua dung:

- Vong re lon nam quanh Player, khong nam tai chuot.
- Enemy trong 8m nhan damage va bi root 3s.
- Enemy root dung di chuyen nhung neu dang trong tam attack thi van co the tan cong.

### Test 3 - Hoa delay target

Buoc test:

1. Chon `2`.
2. Dua chuot vao cum enemy.
3. Bam `R`.
4. Di chuyen/quan sat enemy chay khoi vung.

Ket qua dung:

- Warning ring xuat hien truoc.
- Sau 0.8s moi no.
- Damage chi apply luc no, khong apply ngay khi bam R.

### Test 4 - Tho stun

Buoc test:

1. Chon `3`.
2. De enemy vay quanh Player.
3. Bam `R`.

Ket qua dung:

- Dia chan quanh Player.
- Enemy trong 6m nhan damage.
- Enemy bi stun 2s: khong di chuyen va khong tan cong.

### Test 5 - Thuy rectangle knockback

Buoc test:

1. Chon `4`.
2. Aim theo mot huong co enemy.
3. Bam `R`.

Ket qua dung:

- Vung visual la hinh chu nhat 15x8m ve phia truoc Player.
- Enemy trong vung nhan damage.
- Enemy bi day theo huong song khoang ngan.

### Test 6 - Kim multi-hit

Buoc test:

1. Chon `5`.
2. Dua chuot vao cum enemy lon.
3. Bam `R`.

Ket qua dung:

- Warning ring 10m hien 0.6s.
- Sau do 20 kim nhan roi ngau nhien trong vung.
- Enemy co the nhan nhieu hit neu dung tai diem nhieu kim nhan roi.

## Thao tac thu cong neu can

Neu muon test khoa Ultimate:

1. Play scene.
2. Chon Player runtime.
3. Trong `PlayerCultivationState`, doi:
   - `Player Realm` = `TrucCo`
   - hoac `Spell Realm` = `TrucCo`
4. Bam R de kiem tra lock.

Neu muon test balance:

1. Chon Player runtime.
2. Tim `PlayerUltimateController`.
3. Sua list `Ultimates`.
4. Cac field quan trong:
   - `Damage Multiplier`
   - `Cooldown`
   - `Radius`
   - `Cast Delay`
   - `Rectangle Length`
   - `Rectangle Width`
   - `Knockback Speed`
   - `Knockback Duration`
   - `Hit Radius`
   - `Projectile Count`

## Ghi chu verify

Can chay compile sau khi sua:

```text
dotnet build Assembly-CSharp.csproj --no-restore
```

Can verify Play Mode truc tiep trong Unity Editor vi visual, delay va knockback chi danh gia dung khi nhin trong Game/Scene view.
