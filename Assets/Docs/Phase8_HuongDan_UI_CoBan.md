# Phase 8 - UI co ban

## Muc tieu

Phase nay them UI co ban de theo doi trang thai combat trong `CombatPrototype`:

- Mau Player.
- Cooldown skill Q.
- Cooldown dash Space.

## File da tao

- `Assets/Script/UI/CombatHUD2D.cs`
- `Assets/Docs/Phase8_HuongDan_UI_CoBan.md`

## File da sua

- `Assets/Scenes/CombatPrototype.unity`
- `Assets/Prefabs/Enemy.prefab`
- `Assets/Prefabs/Projectile.prefab`
- `Assets/Prefabs/SkillProjectile.prefab`

## Thay doi trong scene

`GameManager` duoc gan them component `CombatHUD2D`.

Component nay dang reference truc tiep toi cac component tren `Player`:

- `PlayerHealth2D`
- `PlayerCombat2D`
- `PlayerDash2D`

Neu reference bi mat, script se tu tim lai component trong scene khi Play.

## Cach UI hien thi

UI duoc ve bang `OnGUI`, nen chi hien trong Game View khi bam Play.

Vi tri mac dinh:

- Goc tren ben trai man hinh.
- Offset: `16, 16`.

Noi dung hien thi:

```text
HP: 100 / 100
Skill Ready
Dash Ready
```

Khi skill dang hoi:

```text
Skill CD: 2.4s
```

Khi dash dang hoi:

```text
Dash CD: 0.6s
```

## Test case

### 1. Kiem tra HP UI

1. Mo scene `CombatPrototype`.
2. Bam Play.
3. De Enemy cham Player.
4. Kiem tra dong HP co giam, vi du `HP: 90 / 100`.

Ket qua dung:

- HP UI cap nhat khi Player mat mau.
- Player khong mat mau qua nhanh vi cooldown damage cua Enemy van duoc giu.

### 2. Kiem tra Skill cooldown UI

1. Bam Play.
2. Nhan `Q`.
3. Quan sat dong Skill.

Ket qua dung:

- Ngay sau khi ban skill, UI hien `Skill CD: 3.0s` hoac gia tri gan 3 giay.
- Cooldown dem nguoc.
- Khi hoi xong, UI hien `Skill Ready`.

### 3. Kiem tra Dash cooldown UI

1. Bam Play.
2. Nhan `Space`.
3. Quan sat dong Dash.

Ket qua dung:

- Ngay sau khi dash, UI hien `Dash CD: 1.0s` hoac gia tri gan 1 giay.
- Cooldown dem nguoc.
- Khi hoi xong, UI hien `Dash Ready`.

### 4. Kiem tra gameplay cu khong bi anh huong

Trong Play Mode, kiem tra lai:

- WASD van di chuyen.
- Chuot trai van ban basic projectile.
- Q van ban skill projectile.
- Space van dash.
- Dash van di xuyen Enemy trong thoi gian dash.

## Ghi chu

Phase nay chua dung Canvas hoac TextMeshPro de tranh phat sinh package/reference UI khong can thiet trong prototype.

Neu sau nay can UI dep hon, co the chuyen `CombatHUD2D` sang Canvas + TextMeshPro o Phase polish hoac khi lam UI chinh thuc.

## Sua loi sprite bi tim

Sau khi reload scene, cac SpriteRenderer co the hien mau tim neu material dang tro toi asset/shader khong ton tai.

Da doi material cua cac SpriteRenderer prototype ve material mac dinh cua Unity:

```text
Sprites-Default
```

File da duoc cap nhat:

- `Assets/Scenes/CombatPrototype.unity`
- `Assets/Prefabs/Enemy.prefab`
- `Assets/Prefabs/Projectile.prefab`
- `Assets/Prefabs/SkillProjectile.prefab`

Neu van con object bi tim, hay chon object do trong Inspector va kiem tra:

1. Component `SpriteRenderer`.
2. Field `Material`.
3. Dat ve `Sprites-Default`.
