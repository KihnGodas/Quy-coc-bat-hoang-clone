# Phase 10.1 - Auto Attack va cap nhat Moc Yeu

## Muc tieu

Phase chen ngang nay cap nhat combat theo yeu cau gameplay moi:

- Danh thuong cua Player chuyen thanh tu dong.
- Chuot trai khong con la input danh thuong.
- Q van la ky nang vu khi.
- E van la Cong Phap.
- R van la Ultimate.
- Visual vu khi duoc lam sinh dong hon de auto attack khong bi cam giac may moc.
- Moc Yeu doi chi root sang snapshot vi tri Player luc cast, warning khong bam theo Player.

## File da tao

- `Assets/Docs/Phase10_1_HuongDan_AutoAttack_MocYeu.md`

## File da sua

- `Assets/Script/Weapons/WeaponController.cs`
- `Assets/Script/Weapons/WeaponSpriteVisual2D.cs`
- `Assets/Script/Weapons/WeaponVisualEffect2D.cs`
- `Assets/Script/Enemies/EnemyData.cs`
- `Assets/Script/Enemies/EnemyRootAttack.cs`
- `Assets/ScriptableObjects/EnemyData_MocYeu.asset`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

Khong sua scene/prefab YAML trong phase nay.

## Auto normal attack

`WeaponController` tu tim enemy gan nhat trong tam auto attack.

Neu co enemy hop le:

1. Xac dinh huong tu Player toi enemy gan nhat.
2. Neu normal attack het cooldown thi tu danh.
3. Visual vu khi xoay theo huong target auto.

Neu khong co enemy trong tam:

- Khong danh thuong.
- Visual vu khi van xoay theo aim/move direction nhu truoc.

Input hien tai:

- Chuot trai: khong dung de danh thuong.
- Q: skill vu khi.
- E: Cong Phap.
- R: Ultimate.

## Tam auto attack

Tam auto attack duoc tinh tu reach cua normal attack va them padding mac dinh:

```text
auto range = max normal reach + 2.5m
```

Voi projectile weapon, reach tinh them:

```text
projectile speed * projectile lifetime
```

Dieu nay giup Phi Kiem tu danh o tam xa hon melee, con Kiem/Thuong/Riu chi auto khi enemy o gan.

## Visual vu khi moi

`WeaponSpriteVisual2D`:

- Idle co hover nhe quanh Player.
- Co sway angle nhe theo thoi gian.
- Attack extension khac nhau theo vu khi:
  - Thuong dam thang dai hon.
  - Riu swing nang hon.
  - Phi Kiem dao dong/pho phong nhanh hon.
  - Kiem giu cam giac slash gon.

`WeaponVisualEffect2D`:

- Cone co them inner arc de slash day hon.
- Rectangle/thrust co them impact line o dau don.
- Projectile co them glow ray ngan de ban ra ro hon.

## Moc Yeu moi

Thong so trong `EnemyData_MocYeu.asset`:

- HP: 120.
- Damage co ban: 11.
- Speed: 5 m/s.
- Root range: 10m.
- Root warning: 0.5s.
- Root radius: 1.5m.
- Root duration/trap lifetime: 1s.
- Root cooldown: 1.3s.
- Damage khi re troi dam trung: 11.
- Trap touch damage multiplier: 0.5.
- Trap touch damage: 5.5.

Logic moi:

1. Moc Yeu tien lai Player.
2. Khi Player vao `RootRange`, Moc Yeu cam re va dung yen.
3. Luc bat dau cast, Moc Yeu snapshot vi tri Player.
4. Vong warning do hien tai vi tri snapshot trong 0.5s.
5. Warning khong di chuyen theo Player.
6. Sau delay, re troi tai vi tri snapshot.
7. Neu Player con trong vung, Player nhan 11 damage va root.
8. Vung re ton tai 1s, neu Player cham trap thi nhan them 5.5 damage mot lan.

## Ly do khong dung range 28m trong prototype

Theo mo ta GD, Moc Yeu co plant range 28m. Tuy nhien camera gameplay hien tai nhin thay khoang 18m chieu cao. Neu de 28m, Player co the bi cast tu enemy ngoai man hinh, tao cam giac khong cong bang.

Vi vay prototype dung 10m de:

- Player thay duoc Moc Yeu truoc khi bi cast.
- Warning co y nghia de ne.
- Van giu duoc fantasy phap su khong che tam xa.

## Cach kiem tra trong Unity

### Test auto attack

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Bam Play.
3. Khong bam chuot trai.
4. Di chuyen Player lai gan enemy.

Ket qua dung:

- Vu khi tu danh khi enemy vao tam.
- Vu khi xoay ve phia enemy gan nhat.
- Q/E/R van dung nhu cu.

### Test visual vu khi

1. Doi lan luot cac vu khi trong `WeaponController.Current Weapon`.
2. Bam Play.
3. Quan sat idle va auto attack.

Ket qua dung:

- Vu khi co hover/sway nhe khi idle.
- Kiem/Thuong/Riu/Phi Kiem co nhịp attack khac nhau.
- Effect attack ro hon so voi truoc.

### Test Moc Yeu snapshot

1. Chon `EnemySpawner.Spawn Mode = Single Test Enemy`.
2. Chon `EnemyData_MocYeu`.
3. Bam Play.
4. Khi vong warning do xuat hien, chay ra khoi vung.

Ket qua dung:

- Vong warning dung yen tai vi tri snapshot.
- Vong warning khong bam theo Player.
- Neu Player ne ra ngoai truoc khi re troi, Player khong nhan hit chinh.
- Neu Player cham trap sau khi re troi, Player nhan 5.5 damage.

## Ghi chu verify

Can chay compile sau khi sua:

```text
dotnet build Assembly-CSharp.csproj --no-restore
```

Can verify Play Mode truc tiep trong Unity Editor de danh gia cam giac auto attack va visual vu khi.
