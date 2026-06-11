# Phase 17 - Huong dan Tutorial Boss: Su Gia Thai Thuong Dao Vien

## Muc tieu

Phase nay them boss tutorial theo GDD:

- Boss co the bi gay damage, nhung damage bi giam rat manh nen khong the bi danh bai trong gameplay thuc te.
- Tran dau la bai kiem tra sinh ton 30 giay.
- Sau 30 giay, boss noi: "Mọi chuyện kết thúc ở đây thôi".
- Boss tung don ket thuc cuc manh, player bi ha guc va man ket thuc `Defeat` theo cot truyen.

## Da tao file nao

- `Assets/Script/Bosses/TutorialBossController.cs`
- `Assets/Script/Bosses/TutorialBossVisual.cs`
- `Assets/Script/Bosses/BossLineTelegraph2D.cs`
- `Assets/Docs/Phase17_HuongDan_TutorialBoss.md`

## Da sua file nao

- `Assets/Script/Bosses/BossController.cs`
- `Assets/Script/Core/Health.cs`
- `Assets/Script/Manager/CombatManager.cs`
- `Assets/Script/UI/CombatHUD2D.cs`
- `Assets/Script/UI/BossHealthUI.cs`
- `Assets/Scenes/CombatScene.unity`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong di chuyen file nao.

## Boss hoat dong ra sao

Boss tutorial dung mode rieng:

- `CombatManager.Mode = TutorialBossCombat`.
- `BossController.Prototype Boss Type = TutorialMessenger`.
- Boss spawn voi HP mac dinh `99999`, BaseATK `20`.
- Boss nhan damage voi multiplier rat thap nen player thay co phan hoi damage, nhung khong the giet boss trong 30 giay tutorial.
- Boss di chuyen tu do quanh player voi toc do `10 m/s`.
- Boss dung 3 skill trong trang thai `SAT HACH` voi nhip cast day hon ban GDD co ban de tutorial khong qua thua.
- Sau 30 giay boss chuyen sang `KET THUC`.
- Boss dung toan bo skill, hien cau thoai ket thuc va tung don `Thai Thuong Phan Quyet`.
- Player nhan damage cuc lon va man ket thuc `Defeat` theo cot truyen.

## Visual boss

Visual hien tai duoc ghep bang sprite primitive runtime:

- Hao quang xanh lam/trang quanh boss.
- Dao an xoay nhe o than boss.
- Dao bao trang vang/xanh.
- Loi dao phap sang o nguc.
- Nhieu thanh kiem noi quanh than boss.
- Vong canh bao, line warning va hieu ung strike co mau ro de playtest.

## Skill boss

### Kiem Khi

- Spawn 1 projectile thang.
- Huong tu boss den snapshot vi tri player luc cast.
- Toc do `20 m/s`.
- Ton tai `2s`.
- Sat thuong `20`.
- Moi lan cast ban 6 duong kiem khi lech goc nhe.
- Cooldown hien tai de test: `0.85s`.
- Rong xap xi `2m`.

### Phi Kiem

- Tao 10 tia do bang `BossLineTelegraph2D`.
- Moi tia cach nhau `0.2s`.
- Moi tia ton tai `0.85s`.
- Sau warning, boss ban 10 phi kiem theo dung huong da snapshot.
- Toc do `18 m/s`.
- Sat thuong moi kiem `15`.
- Cooldown hien tai de test: `2.1s`.

### Dao Van

- Chon 12 vi tri ngau nhien quanh player trong ban kinh `8m`.
- Cac vung co khoang cach toi thieu de tranh chong len nhau.
- Hien vong canh bao do ban kinh `1.5m` trong `0.85s`.
- Sau warning gay `30` damage tai tung vung.
- Cooldown hien tai de test: `2.8s`.

### Thai Thuong Phan Quyet

- Sau 30 giay boss noi: "Mọi chuyện kết thúc ở đây thôi".
- Hien vong canh bao lon duoi player.
- Sau delay, strike xuong player.
- Gay damage `99999`.
- Combat ket thuc `Defeat`.

## Cach kiem tra trong Unity

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Chon `GameManager`.
3. Set `CombatManager.Mode = TutorialBossCombat`.
4. Set `BossController.Prototype Boss Type = TutorialMessenger`.
5. Bam Play.
6. Kiem tra boss xuat hien voi visual trang/xanh/vang.
7. Kiem tra player khong spawn trung tam voi boss, boss o tam map va player lech sang trai.
8. Danh boss va xac nhan boss co mat mau rat it, khong the bi giet trong 30 giay.
9. Quan sat 3 skill:
   - Kiem Khi.
   - Phi Kiem co 10 tia do.
   - Dao Van co 12 vong canh bao.
10. Song sot den 30 giay.
11. Kiem tra boss hien cau "Mọi chuyện kết thúc ở đây thôi".
12. Kiem tra don ket thuc ha guc player va hien `DEFEAT`.

## Test case cu the

- Test 1: Tutorial boss spawn dung khi chon `TutorialBossCombat` va `TutorialMessenger`.
- Test 2: Player khong spawn cung vi tri voi boss trong boss mode.
- Test 3: Boss co nhan damage rat nho khi player tan cong.
- Test 4: Kiem Khi gay 20 damage khi trung.
- Test 5: Phi Kiem co 10 line warning va ban theo huong snapshot.
- Test 6: Dao Van sinh 12 vung khong chong len nhau.
- Test 7: Skill cast day hon ban dau, khong phai cho qua lau giua cac skill.
- Test 8: Sau 30 giay boss dung skill.
- Test 9: Cau thoai ket thuc hien dung noi dung.
- Test 10: Don ket thuc gay damage cuc lon va man ra `Defeat`.
- Test 11: Boss Act I va Act II van co the test bang `BossCombat`.

## Luu y

- Defeat trong phase nay la scripted defeat theo cot truyen, khong phai bug gameplay.
- Sau nay neu co dialogue/cutscene system, cau thoai ket thuc nen chuyen tu `OnGUI` sang he thong dialogue that.

## Cap nhat hieu ung ket thuc

- `Thai Thuong Phan Quyet` khong con la vong canh bao tinh.
- Khi het sat hach, phap an ket thuc tu dong bam theo vi tri player trong thoi gian niem.
- Visual gom nhieu lop: vong ngoai, dao an vuong xoay, vong trong, loi sang va 8 thanh kiem quanh phap an.
- Trong luc niem, co nhieu tia lock-line noi tu boss den vi tri player hien tai.
- Khi het delay, don ket thuc danh vao vi tri player hien tai, tao loi sang va cot sang tu tren roi xuong.
- Day van la scripted defeat theo cot truyen.
