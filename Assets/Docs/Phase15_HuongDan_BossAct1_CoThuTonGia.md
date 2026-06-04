# Phase 15 - Huong dan Boss Act I: Thoi Han Tung / Co Thu Ton Gia

## Muc tieu

Phase nay thay boss prototype don gian bang boss Act I co mechanic va visual ro rang hon:

- Boss co tao hinh rieng bang nhieu sprite primitive ghep lai.
- Boss co khien moc mien nhiem sat thuong.
- Spawn 8 re cay bao ve quanh arena.
- Pha re cay de mo khien boss.
- Boss co cac skill telegraph ro truoc khi gay sat thuong.

## Da tao file nao

- `Assets/Script/Bosses/Act1WoodBossController.cs`
- `Assets/Script/Bosses/Act1WoodBossVisual.cs`
- `Assets/Script/Bosses/BossRootNode.cs`
- `Assets/Script/Bosses/BossRuntimeSprites.cs`
- `Assets/Script/Bosses/BossSkillTelegraph2D.cs`
- `Assets/Script/Bosses/BossHazard2D.cs`
- `Assets/Docs/Phase15_HuongDan_BossAct1_CoThuTonGia.md`

## Da sua file nao

- `Assets/Script/Bosses/BossBase.cs`
- `Assets/Script/Bosses/BossController.cs`
- `Assets/Script/UI/BossHealthUI.cs`
- `Assets/Scenes/CombatScene.unity`
- `Assembly-CSharp.csproj`

## Co di chuyen file khong

Khong di chuyen file nao.

## Boss hoat dong ra sao

Khi `CombatManager.Mode = BossCombat`:

1. `BossController` spawn boss `Boss Prototype - Co Thu Ton Gia`.
2. Neu `Use Act1 Wood Boss Prototype` bat, boss duoc gan:
   - `Act1WoodBossVisual`
   - `Act1WoodBossController`
3. Boss dung yen o tam map.
4. Boss spawn 8 re cay quanh arena.
5. Khi con re cay, boss bat khien va mien nhiem sat thuong.
6. Pha 1 re cay se mo khien trong 8s.
7. Neu het 8s ma van con re cay, khien bat lai.
8. Pha het re cay thi khien mat vinh vien.
9. Boss chet thi Victory theo logic BossCombat cua phase 14.

## Visual boss

Boss duoc ghep bang primitive runtime:

- Khiem aura tron xanh quanh boss.
- Than nguoi xanh ngoc/tai nhe.
- Ao choang xanh reu.
- Mat/tran sang mau xanh.
- Nhieu re cay moc len tu dau/lung.
- Core sang o nguc.
- Khi HP thap, khien/than boss co xu huong chuyen sang sac canh bao.

Re cay bao ve:

- La object rieng tren layer/tag `Enemy`.
- Co HP rieng `700`.
- Khi bi danh se nhay do.
- Khi chet se bien mat va bao ve boss bi mo khien.

## Skill boss hien tai

### Dan dao xoay 8 huong

- Boss ban dan xanh theo 8 huong.
- Truc ban xoay dan qua tung dot.
- Khi boss HP <= 40%, dan nhanh hon va doi mau do.
- Damage:
  - Thuong: `150% base damage`.
  - Enrage: `200% base damage`.

### Bay re va trieu hoi

- Tao 5 vung canh bao do quanh vi tri player.
- Sau warning, bay re ton tai trong thoi gian ngan.
- Cham bay se nhan damage nho va bi root.
- Sau do 4 re cheo dap vao vi tri bay.

### Re troi duoi chan

- 8 lan lien tiep.
- Moi lan co vong do warning duoi vi tri player snapshot.
- Sau warning, re troi len gay damage.

### Re khong lo quet ngang

- Tao vung warning hinh chu nhat dai.
- Sau 1s, re khong lo quet qua vung do va gay damage.

## Cach kiem tra trong Unity

1. Mo `Assets/Scenes/CombatScene.unity`.
2. Chon `GameManager`.
3. Set `CombatManager.Mode = BossCombat`.
4. Kiem tra `BossController.Use Act1 Wood Boss Prototype = true`.
5. Bam Play.
6. Boss xuat hien o tam map voi tao hinh xanh/re cay.
7. Kiem tra 8 re cay xuat hien quanh arena.
8. Danh boss khi khien dang bat:
   - Boss khong mat HP.
   - Boss Health UI hien `Shield: ACTIVE`.
9. Pha 1 re cay:
   - Shield UI chuyen `Shield: OPEN`.
   - Boss co the mat HP trong 8s.
10. Pha het re cay:
   - Shield khong bat lai.
11. Ne cac skill co telegraph do/xanh.
12. Giet boss de hien Victory.

## Test case cu the

- Test 1: BossCombat khong spawn quai thuong.
- Test 2: Boss spawn dung visual Act I, khong con la hinh vuong prototype cu.
- Test 3: Khi shield active, danh boss khong tru HP.
- Test 4: Pha re cay mo shield boss.
- Test 5: Re cay nhay do khi bi danh va bien mat khi HP ve 0.
- Test 6: Dan xoay 8 huong gay damage player khi cham.
- Test 7: Bay re co warning, root player khi cham.
- Test 8: Re duoi chan player dung snapshot vi tri, khong tracking lien tuc sau warning.
- Test 9: Re quet ngang co warning truoc khi gay damage.
- Test 10: Boss chet thi Victory.

## Luu y

- Visual la primitive runtime de test gameplay, chua phai final art.
- Logic skill hien dang nam trong `Act1WoodBossController` de prototype gon va de test. Sau khi gameplay on dinh moi nen tach tung skill thanh script rieng.
- Neu boss qua trau khi test, giam `Prototype Boss HP` tren `BossController`.
