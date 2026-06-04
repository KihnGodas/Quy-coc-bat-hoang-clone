# Phase 7 - Huong dan Enemy Types va Visual Skill

## Muc tieu

Phase 7 tao day du nhom enemy thuong theo bang thiet ke moi. Moi enemy phai khac nhau ve chi so, kich thuoc, mau sac, silhouette va cach the hien skill tren man hinh.

Phase nay uu tien giu `Enemy.prefab` lam prefab goc de tranh mat reference khi merge code nhom. Cac loai enemy duoc cau hinh bang `EnemyData` rieng.

## File da tao

- `Assets/Script/Enemies/EnemySpawnEntry.cs`
- `Assets/Script/Enemies/EnemyVisual2D.cs`
- `Assets/Script/Enemies/EnemyRoleController.cs`
- `Assets/Script/Enemies/EnemyChargeAttack.cs`
- `Assets/Script/Enemies/EnemyProjectileAttack.cs`
- `Assets/Script/Enemies/EnemyRockThrowAttack.cs`
- `Assets/Script/Enemies/EnemyRootAttack.cs`
- `Assets/Script/Enemies/EnemyTelegraph2D.cs`
- `Assets/Script/Player/PlayerStatus2D.cs`
- `Assets/ScriptableObjects/EnemyData_HacLang.asset`
- `Assets/ScriptableObjects/EnemyData_DaiLang.asset`
- `Assets/ScriptableObjects/EnemyData_ThietTru.asset`
- `Assets/ScriptableObjects/EnemyData_OaiHung.asset`
- `Assets/ScriptableObjects/EnemyData_KimVien.asset`
- `Assets/ScriptableObjects/EnemyData_XaYeu.asset`
- `Assets/ScriptableObjects/EnemyData_UngYeu.asset`
- `Assets/ScriptableObjects/EnemyData_MocYeu.asset`
- `Assets/Docs/Phase7_HuongDan_EnemyTypes.md`

## File da sua

- `Assets/Script/Enemies/EnemyData.cs`
- `Assets/Script/Enemies/EnemyBase.cs`
- `Assets/Script/Enemies/EnemyMeleeAttack.cs`
- `Assets/Script/Combat/Projectile2D.cs`
- `Assets/Script/Player/PlayerMovement2D.cs`
- `Assets/Script/Player/PlayerDash2D.cs`
- `Assets/Script/Spawning/EnemySpawner.cs`
- `Assets/ScriptableObjects/EnemyData_KhoiLang.asset`
- `Assets/ScriptableObjects/EnemyData_HacLang.asset`
- `Assets/ScriptableObjects/EnemyData_DaiLang.asset`
- `Assets/ScriptableObjects/EnemyData_ThietTru.asset`
- `Assets/ScriptableObjects/EnemyData_OaiHung.asset`
- `Assets/ScriptableObjects/EnemyData_KimVien.asset`
- `Assets/ScriptableObjects/EnemyData_XaYeu.asset`
- `Assets/ScriptableObjects/EnemyData_UngYeu.asset`
- `Assets/ScriptableObjects/EnemyData_MocYeu.asset`
- `Assets/Scenes/CombatScene.unity`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong co file nao bi di chuyen.

Khong doi ten class va khong tao prefab enemy moi. Cac runtime component duoc add khi spawn neu prefab goc chua co component do.

## Mechanic tong quan

### EnemyData

`EnemyData` la noi cau hinh chinh cho tung loai enemy:

- Chi so co ban: HP, damage, move speed, attack range, cooldown.
- Visual: mau, scale, collider size, collider offset.
- Low health phase: nguong HP thap, multiplier toc do, damage, attack rate va max HP.
- Charge: range, cooldown, windup, distance, speed, damage multiplier.
- Projectile: prefab, range, retreat range, resume range, damage, speed, lifetime, cooldown, charge time, flash time, piercing, poison.
- Rock throw: warning time, fall time, AoE radius.
- Root: plant range, cooldown, warning time, duration, radius, damage.

### EnemySpawner

`EnemySpawner` co 3 mode:

- `SingleTestEnemy`: chi spawn 1 loai enemy de test ro tung vu khi voi tung enemy.
- `CycleTestEnemies`: spawn lan luot tung enemy trong bang.
- `MixedTable`: spawn tron theo unlock time va weight.

Mac dinh de `SingleTestEnemy` de ban test vu khi va enemy ro rang hon. Khi can gameplay tong hop, doi sang `MixedTable`.

### EnemyVisual2D

`EnemyVisual2D` lam enemy khac nhau ve mau, scale, collider va phu kien visual runtime.

Moi enemy co them label nho tren dau khi Play de de debug:

- `Khoi Lang`: soi xam baseline, than nho, dau an trang.
- `Hac Lang`: soi den to hon, mat do, duoi toi mau.
- `Dai Lang`: soi trang khong lo, than lon, sung vang, loi charge cam.
- `Thiet Tru`: lon rung giap kim loai, than be ngang, dai giap xam sang.
- `Oai Hung`: gau nau berserker, than lon, biem vang, loi do.
- `Kim Vien`: khi dot long vang, hai tay lon, tui da, dai vang.
- `Xa Yeu`: ran doc xanh, than doc, dau ran, duoi dai, dau hieu doc.
- `Ung Yeu`: chim ung xanh, hai canh rong, mo vang, loi xanh.
- `Moc Yeu`: cay kho, than cao, nhanh cay, la va re xanh.

Tat ca phu kien nay duoc tao bang `SpriteRenderer` con luc Play, khong tao prefab rieng.

### EnemyTelegraph2D

`EnemyTelegraph2D` tao visual canh bao va skill:

- Vong canh bao AoE.
- Duong chi huong dash/projectile.
- Flash nhap nhay khi skill sap kich hoat.
- Chuyen dong arc/parabol cho vat the nhu da cua Kim Vien.
- Hieu ung fade/pulse de nguoi choi nhin thay ro.

Day la visual runtime, khong can tao object thu cong trong scene.

### PlayerStatus2D

`PlayerStatus2D` hien quan ly status bat loi cho Player:

- `Root`: khoa di chuyen/dash trong thoi gian ngan.
- `Poison`: gay damage theo tick va phu mau xanh doc len sprite Player.

Poison hien dung cho `Xa Yeu`: thoi gian 5s, moi 1s gay 3 damage. Damage poison di qua `IDamageable`, nen van ton trong logic HP/chet/invincible hien co cua Player.

## Thang can bang theo camera

Sau khi test visual, cac chi so enemy duoc can bang lai theo camera hien tai:

- Player speed hien tai: 12 m/s.
- Camera orthographic size hien tai: 9, tuc chieu cao nhin thay khoang 18m.
- Ranged/root range khong nen vuot qua 8m - 9m de Player nhin thay enemy truoc khi bi tan cong.
- Enemy melee thuong nen cham hon Player, khoang 65% - 85% toc do Player.
- Enemy lon/nang khong nen nhanh hon Player; do nguy hiem nen den tu skill, HP hoac telegraph.

Vi ly do do, cac range cu nhu 22m va 28m da duoc giam xuong de tranh tinh huong enemy o ngoai camera nhung da tan cong Player.

## Chi tiet tung enemy

### Khoi Lang

- Role: `MeleeChaser`
- HP: 100
- Damage: 10
- Speed: 8.5 m/s
- Size: 1.0 x 0.7
- AI: CHASE truc tiep, khong re nhanh.
- Damage: contact damage khi hitbox cham Player.
- Visual: soi xam baseline, nho va de nhan biet lam enemy co ban.

### Hac Lang

- Role: `SpeedChaser`
- HP: 160
- Damage: 11.5
- Speed phase 1: 9.5 m/s
- Speed phase 2: khoang 11.8 m/s khi HP <= 60%.
- Size: 1.15 x 0.805
- Phase transition: one-way, khong quay lai phase 1 neu HP thay doi.
- Visual: soi den to hon, mat do, than toi mau.

### Dai Lang

- Role: `Charger`
- HP: 300
- Damage dash: 20
- Speed chase: 8 m/s
- Size: 1.5 x 1.05
- Trigger: distance <= 4.5m.
- Windup: 0.4s dung yen.
- Dash: lao thang theo huong Player tai thoi diem ket thuc windup.
- Dash distance: 7.5m.
- Dash speed: 24 m/s.
- Damage chi active trong pha DASH.
- Post-dash: quay ve CHASE sau khi het 7.5m hoac hit Player.
- Visual skill: vong do/cam tren than, duong do chi huong dash, flash vang luc bat dau lao.

### Thiet Tru

- Role: `Tank`
- HP: 200
- Damage: 11
- Speed: 6.8 m/s
- Size: 0.85 x 0.7
- AI: CHASE cham, contact damage.
- Special: khong co skill rieng, HP cao bu cho co che don gian.
- Visual: mau xam thep, dai giap va tam giap.

### Oai Hung

- Role: `Berserker`
- HP base: 160
- Damage base: 16
- Speed base: 8.2 m/s
- Size: 1.1 x 1.05
- Enrage trigger: HP <= 50%.
- Enrage stat: speed len khoang 9.8 m/s, damage x1.2, attack rate x1.2, max HP x1.2.
- Current HP khong duoc heal khi enrage.
- Phase transition: one-way.
- Visual: gau nau, biem vang, loi do berserk.

### Kim Vien

- Role: `HybridThrower`
- HP: 170
- Melee damage: 17
- Rock damage: 30
- Speed: 8 m/s
- Size: 0.9 x 1.05
- Neu distance > 7m: CHASE truc tiep.
- Neu distance <= 7m: dung lai va nem da.
- Rock target: snapshot vi tri Player luc nem, khong tracking.
- Cooldown nem da: 2.5s.
- Visual skill: vong AoE do tai diem roi, da xam bay theo cung parabol tu enemy toi diem snapshot, flash vang/cam khi impact.

### Xa Yeu

- Role: `RangedProjectile`
- HP: 70
- Damage: 11
- Speed ban than: 6.8 m/s
- Projectile speed: 8.5 m/s
- Fire rate: moi 4s
- Maintain range: 8.5m.
- Neu distance > 8.5m: tien lai.
- Neu distance <= 8.5m: dung lai va cast projectile.
- Tell sequence: charge do 3.5s, flash trang 0.5s, sau do fire.
- Projectile lifetime: 1s, tam toi da khoang 8.5m.
- Projectile khong xuyen.
- Poison: neu projectile trung Player, gay doc 5s, moi 1s gay 3 damage.
- Keep distance: neu Player ap sat duoi 4.5m, Xa Yeu lui ra; khi dat khoang 6.5m thi co the dung lai cast tiep.
- Visual skill: vong charge do dam dan tren than, duong ngam do, flash trang nhap nhay truoc khi ban, Player bi phu mau xanh khi dang doc.

### Ung Yeu

- Role: `RangedProjectile`
- HP: 70
- Damage: 11
- Speed ban than: 9.2 m/s
- Projectile speed: 11 m/s
- Fire rate: moi 3s
- Maintain range: 9m.
- Tell sequence: charge do 2.5s, flash trang 0.5s, sau do fire.
- Projectile lifetime: 0.82s, tam toi da khoang 9m.
- Projectile xuyen qua muc tieu va khong gay damage lap lai tren cung mot root.
- Keep distance: neu Player ap sat duoi 5m, Ung Yeu lui ra; khi dat khoang 7m thi co the dung lai cast tiep.
- Visual skill: vong charge do dam dan tren than, duong ngam do, flash trang nhap nhay truoc khi ban.

### Moc Yeu

- Role: `RootMage`
- HP: 120
- Root damage: 11
- Trap touch damage: 5.5
- Speed: 5 m/s
- Plant range: 10m
- Size: 0.6 x 1.0
- Neu distance > 10m: WALK cham toi Player.
- Neu distance <= 10m: ROOT, cam re va khong di chuyen nua.
- Root cycle: snapshot vi tri Player luc bat dau cast, vong do canh bao 0.5s dung yen tai vi tri snapshot, re troi len, gay damage 11, root/trap ton tai 1s.
- Trap damage: neu Player cham vung re trong 1s ton tai thi nhan them 50% damage co ban = 5.5 damage mot lan.
- Cooldown giua cac chu ky: 1.3s sau khi root ket thuc.
- Visual skill: vong warning do khong tracking Player, burst re xanh tai vi tri snapshot.

## Cach kiem tra trong Unity

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Chon `GameManager`.
3. Kiem tra component `EnemySpawner`.
4. De test tung enemy:
   - Doi `Spawn Mode` thanh `Single Test Enemy`.
   - Keo asset enemy can test vao `Test Enemy Data`.
   - Bam Play.
5. De test nhieu enemy:
   - Doi `Spawn Mode` thanh `Cycle Test Enemies` hoac `Mixed Table`.
   - Bam Play va quan sat enemy spawn.
6. De test vu khi voi tung enemy:
   - Chon Player.
   - Doi vu khi trong `WeaponController`.
   - Chon lai enemy can test trong `EnemySpawner.Test Enemy Data`.
   - Bam Play.

## Test case cu the

### Test 1 - Visual phan biet enemy

Buoc test:

1. Chon `Spawn Mode = Cycle Test Enemies`.
2. Bam Play.
3. Quan sat tung enemy spawn.

Ket qua dung:

- Enemy khac nhau ve mau, kich thuoc va phu kien.
- Label tren dau enemy hien dung ten.
- Khong co enemy nao nhin giong het nhau.

### Test 2 - Hac Lang phase 2

Buoc test:

1. Chon `Test Enemy Data = EnemyData_HacLang`.
2. Danh Hac Lang xuong <= 96 HP.

- Hac Lang tang toc tu 9.5 len khoang 11.8 m/s.
- Toc do khong giam lai ve phase 1.

### Test 3 - Dai Lang charge

Buoc test:

1. Chon `Test Enemy Data = EnemyData_DaiLang`.
2. Dung trong khoang 5m.
3. Quan sat warning va thu ne sang ngang.

Ket qua dung:

- Dai Lang dung yen 0.4s.
- Co vong charge va duong dash do.
- Dai Lang lao 7.5m hoac dung sau khi hit Player.
- Damage chi xay ra trong luc dash.

### Test 4 - Oai Hung enrage

Buoc test:

1. Chon `Test Enemy Data = EnemyData_OaiHung`.
2. Danh Oai Hung xuong <= 80 HP.

Ket qua dung:

- Damage va attack rate tang x1.2.
- Toc do tang tu 8.2 len khoang 9.8 m/s.
- Max HP tang len 192.
- Current HP khong duoc heal them.

### Test 5 - Kim Vien nem da

Buoc test:

1. Chon `Test Enemy Data = EnemyData_KimVien`.
2. Dung gan trong khoang 7m.
3. Quan sat vong warning va di chuyen khoi vung.

Ket qua dung:

- Kim Vien dung lai khi vao tam nem.
- Vong do hien tai vi tri Player luc nem.
- Da bay theo cung parabol tu enemy toi vi tri snapshot, khong tracking Player sau khi da snapshot.
- Neu Player con trong AoE khi impact thi nhan 30 damage.

### Test 6 - Xa Yeu ban doc

Buoc test:

1. Chon `Test Enemy Data = EnemyData_XaYeu`.
2. Dung trong khoang 8.5m.

Ket qua dung:

- Xa Yeu dung lai.
- Charge do 3.5s.
- Flash trang nhap nhay 0.5s.
- Ban projectile toc do 8.5 m/s, ton tai 1s.
- Projectile khong xuyen.
- Khi projectile trung Player, Player nhan hit damage 11 va bi poison.
- Trong luc poison, Player bi phu mau xanh va mat 3 HP moi giay trong 5s.

### Test 7 - Ung Yeu ban xuyen

Buoc test:

1. Chon `Test Enemy Data = EnemyData_UngYeu`.
2. Dung trong khoang 9m.

Ket qua dung:

- Ung Yeu charge nhanh hon Xa Yeu.
- Charge do 2.5s, flash trang nhap nhay 0.5s.
- Projectile toc do 11 m/s, ton tai 0.82s.
- Projectile xuyen qua target va khong damage lap lai cung mot root.
- Khong gay poison.

### Test 8 - Moc Yeu root

Buoc test:

1. Chon `Test Enemy Data = EnemyData_MocYeu`.
2. Dung trong khoang 8m.
3. Di chuyen lien tuc de xem warning co tracking Player khong.

Ket qua dung:

- Moc Yeu cam re va dung yen sau khi vao range.
- Vong canh bao do dung yen tai vi tri Player snapshot luc bat dau cast trong 0.5s.
- Re xanh hien tai vi tri cast.
- Player nhan 11 damage va bi root 1s neu con nam trong vung luc re troi.
- Player nhan them 5.5 damage neu cham vung re trong 1s ton tai.
- Sau cooldown 1.3s, Moc Yeu lap lai root cycle.

### Test 9 - Ranged maintain range

Buoc test:

1. Chon `Xa Yeu` hoac `Ung Yeu`.
2. Dung xa hon tam cast cua enemy.
3. Sau do tien lai gan vao tam cast.

Ket qua dung:

- Xa Yeu tien lai khi ngoai 8.5m va cast khi trong 8.5m.
- Ung Yeu tien lai khi ngoai 9m va cast khi trong 9m.
- Neu Player ap sat qua gan, Xa Yeu lui ra toi khoang 6.5m truoc khi cast tiep.
- Neu Player ap sat qua gan, Ung Yeu lui ra toi khoang 7m truoc khi cast tiep.

## Huong dan thu cong neu scene chua hien du field

Neu Unity Inspector chua hien field moi:

1. Luu scene.
2. Reimport `Assets/Script/Enemies/EnemyData.cs`.
3. Bam chuot phai tung `EnemyData_*.asset` va chon Reimport.
4. Mo lai `CombatScene`.
5. Kiem tra lai `EnemySpawner` va cac asset enemy data.

Neu projectile cua Xa Yeu/Ung Yeu khong hien:

1. Mo `EnemyData_XaYeu.asset` hoac `EnemyData_UngYeu.asset`.
2. Kiem tra `Projectile Prefab` co tro toi `Assets/Prefabs/Projectile.prefab`.
3. Kiem tra Player nam layer `Player` va co tag `Player`.

Neu visual warning khong hien:

1. Kiem tra enemy co `SpriteRenderer`.
2. Kiem tra sorting order cua sprite enemy khong bi qua thap so voi background.
3. Vao Scene view khi Play de quan sat object runtime co ten `DaiLangChargeCircle`, `ProjectileCharge`, `KimVienRockAoEWarning` hoac `MocYeuRootWarning`.

## Ghi chu verify

Can chay compile sau khi sua:

```text
dotnet build Assembly-CSharp.csproj --no-restore
```

Can verify Play Mode truc tiep trong Unity Editor vi visual runtime chi nhin ro khi bam Play.
