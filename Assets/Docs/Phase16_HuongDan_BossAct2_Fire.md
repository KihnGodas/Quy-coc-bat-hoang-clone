# Phase 16 - Huong dan Boss Act II: Huyet Diem Ton Gia

## Muc tieu

Phase nay them boss Act II he Hoa de test mot kieu boss khac boss Moc Act I:

- Boss co visual do/cam, loi lua va aura lua ro rang.
- Boss tap trung vao di chuyen chu dong, AoE, vung chay va dash ep player di chuyen.
- `BossController` co the chon boss prototype theo type, khong ghi de boss Act I.

## Da tao file nao

- `Assets/Script/Bosses/Act2FireBossController.cs`
- `Assets/Script/Bosses/Act2FireBossVisual.cs`
- `Assets/Docs/Phase16_HuongDan_BossAct2_Fire.md`

## Da sua file nao

- `Assets/Script/Bosses/BossController.cs`
- `Assets/Script/UI/BossHealthUI.cs`
- `Assets/Scenes/CombatScene.unity`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong di chuyen file nao.

## Boss hoat dong ra sao

Khi `CombatManager.Mode = BossCombat` va `BossController.Prototype Boss Type = Act2Fire`:

1. `BossController` tao boss `Boss Act II - Huyet Diem Ton Gia`.
2. Boss duoc gan `Act2FireBossVisual` va `Act2FireBossController`.
3. Boss co HP mac dinh `9000`, damage co ban `95`.
4. Boss di chuyen chu dong khi khong dash:
   - Qua xa player thi tien lai.
   - Qua gan player thi lui ra.
   - Dung khoang hop ly thi di vong quanh player.
5. Boss dung skill theo vong lap:
   - `Fire Ring`
   - `Meteor Rain`
   - `Flame Dash`
   - `Burning Zone`
6. Khi HP <= 45%, boss vao trang thai Enraged:
   - Skill ra nhanh hon.
   - Mot so skill co them hit/meteor va damage cao hon.
   - Boss di chuyen nhanh hon.
   - Dash 3 lan lien tiep va `Ember Pulse` day hon.
   - UI hien `State: ENRAGED`.
7. Boss chet thi Victory theo logic BossCombat.

## Visual boss

Boss Hoa duoc ghep bang primitive runtime:

- Aura lua tron mau do/cam.
- Than ao choang do sam.
- Mat va loi sang vang/cam.
- Hai tay nhu ngon lua.
- Vuong mien lua va cac ember quanh than.
- Khi HP thap, aura va core chuyen sac do manh hon.

## Skill boss hien tai

### Fire Ring

- Tao vong canh bao tron quanh boss.
- Sau warning, nhieu cum lua no thanh vong tron.
- Khi Enraged, them mot vong lua phu sau vong dau.
- Boss Act II moi co hai vong lua o trang thai thuong, va ba vong khi Enraged.
- Muc dich: ep player khong dung sat boss qua lau.

### Meteor Rain

- Lay snapshot vi tri player luc cast.
- Tao nhieu vong canh bao do/cam quanh snapshot.
- Co mot vai meteor duoc dat theo huong di chuyen hien tai cua player de chan duong chay.
- So meteor nhieu hon ban dau, normal khoang 15 va Enraged khoang 22.
- Sau delay, thien thach roi xuong va de lai vung chay ngan.
- Meteor khong tracking player sau khi warning da tao.

### Flame Dash

- Boss canh bao mot vung chu nhat theo huong player.
- Sau delay, boss lao thang theo huong do.
- De lai vet lua tick damage trong thoi gian ngan.
- Diem ket thuc dash co vu no lua nho.
- Khi Enraged, boss dash 3 lan lien tiep, moi lan van co warning rieng.

### Burning Zone

- Tao nhieu vung canh bao quanh player snapshot.
- Sau delay, vung lua ton tai 3.8s va gay damage theo tick neu player dung ben trong.
- Khi Enraged, so vung chay tang len.
- Ban moi tang len 7 vung thuong va 10 vung khi Enraged, ton tai 4.5s.

### Ember Pulse

- Boss tu dong ban tia lua nho theo vong tron moi khoang 1.4s.
- Khi Enraged, so tia tang va nhip ban nhanh hon.
- Damage thap hon skill chinh, muc dich la khong cho player dung yen danh boss qua lau.

## Cach kiem tra trong Unity

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Chon `GameManager`.
3. Set `CombatManager.Mode = BossCombat`.
4. Trong `BossController`, set `Prototype Boss Type = Act2Fire`.
5. Kiem tra `Use Boss Type Default Stats = true`.
6. Bam Play.
7. Boss xuat hien o tam map voi visual do/cam.
8. Quan sat boss lan luot dung:
   - Di chuyen giu khoang cach hoac di vong quanh player.
   - Vong lua quanh boss.
   - Mua thien thach co warning truoc.
   - Dash lua co hinh chu nhat canh bao.
   - Vung chay ton tai va tick damage.
   - Ember Pulse ban tia lua quanh boss theo nhip.
9. Danh boss den HP <= 45% de test Enraged.
10. Giet boss de hien Victory.

## Test case cu the

- Test 1: BossCombat spawn boss Hoa khi chon `Act2Fire`.
- Test 2: Chuyen lai `Act1Wood` thi boss Moc van hoat dong.
- Test 3: Fire Ring co warning truoc khi gay damage.
- Test 4: Boss khong dung yen qua lau, co the tien/lui/orbit quanh player.
- Test 5: Meteor Rain danh vao vi tri snapshot, khong tracking player sau khi warning da tao.
- Test 6: Meteor Rain co meteor chan huong di chuyen hien tai cua player.
- Test 7: Flame Dash co warning huong truoc, boss lao theo duong thang.
- Test 8: Flame Dash co no lua o diem dung.
- Test 9: Burning Zone gay damage theo tick neu player dung trong vung.
- Test 10: HP <= 45% thi UI hien `State: ENRAGED`.
- Test 11: Khi Enraged, boss di chuyen nhanh hon va dash 3 lan lien tiep.
- Test 12: Boss chet thi Victory.
- Test 13: Level 1 khong nen ha boss qua nhanh neu khong ne tot va duy tri DPS lien tuc.

## Luu y

- Visual hien tai la primitive runtime de test gameplay, chua phai final art.
- Neu boss qua kho, giam `Prototype Boss HP` hoac tat `Use Boss Type Default Stats` de dung thong so custom tren Inspector.
