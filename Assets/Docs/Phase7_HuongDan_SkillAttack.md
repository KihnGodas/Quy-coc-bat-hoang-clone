# Phase 7 - Hướng dẫn Skill Attack

## Mục tiêu

Phase này thêm skill projectile cho Player.

Kết quả mong muốn:

- Nhấn `Q` để bắn skill projectile.
- Skill projectile spawn từ `FirePoint`.
- Skill projectile bay theo hướng aim chuột.
- Skill mạnh hơn basic projectile.
- Skill có cooldown `3` giây.

## File đã tạo hoặc sửa

- Sửa `Assets/Script/Player/PlayerCombat2D.cs`
- Tạo `Assets/Prefabs/SkillProjectile.prefab`
- Sửa `Assets/Scenes/CombatPrototype.unity`
- Tạo file hướng dẫn này: `Assets/Docs/Phase7_HuongDan_SkillAttack.md`

Không tạo script projectile mới. Không đổi tên class có sẵn. Không di chuyển file nào.

## Nội dung đã thêm

### PlayerCombat2D

`PlayerCombat2D` đã được mở rộng:

- Basic attack chuột trái vẫn giữ nguyên.
- Thêm `Skill Projectile Prefab`.
- Thêm `Skill Cooldown`.
- Thêm input `Q` để dùng skill.
- Thêm property để Phase 8 UI có thể dùng:
  - `SkillCooldown`
  - `IsSkillReady`
  - `SkillCooldownRemaining`

Thông số hiện tại:

- Basic Fire Cooldown: `0.25`
- Skill Cooldown: `3`

### SkillProjectile prefab

Prefab mới: `Assets/Prefabs/SkillProjectile.prefab`

Thông số hiện tại:

- Tag: `Projectile`
- Layer: `Projectile`
- Scale: `1, 1, 1`
- Rigidbody2D:
  - Body Type: Kinematic
  - Gravity Scale: `0`
- CircleCollider2D:
  - Is Trigger: bật
- Projectile2D:
  - Speed: `8`
  - Lifetime: `2`
  - Damage: `35`
  - Destroy On Hit: bật

## Cách kiểm tra trong Unity

1. Mở scene `Assets/Scenes/CombatPrototype.unity`.
2. Chọn object `Player`.
3. Trong component `PlayerCombat2D`, kiểm tra:
   - `Basic Projectile Prefab` trỏ tới `Projectile.prefab`.
   - `Skill Projectile Prefab` trỏ tới `SkillProjectile.prefab`.
   - `Skill Cooldown` là `3`.
4. Mở `Assets/Prefabs/SkillProjectile.prefab`.
5. Kiểm tra prefab có:
   - `SpriteRenderer`
   - `Rigidbody2D`
   - `CircleCollider2D`
   - `Projectile2D`
6. Bấm Play.

## Test case

### Test 1 - Bắn skill bằng Q

Thao tác:

- Bấm Play.
- Aim chuột sang một hướng bất kỳ.
- Nhấn `Q`.

Mong đợi:

- Skill projectile spawn từ `FirePoint`.
- Skill projectile bay theo hướng chuột.
- Skill projectile nhìn lớn hơn basic projectile.

### Test 2 - Cooldown skill

Thao tác:

- Nhấn `Q` liên tục.

Mong đợi:

- Skill chỉ bắn một lần.
- Sau khoảng `3` giây mới bắn được tiếp.

### Test 3 - Basic attack vẫn hoạt động

Thao tác:

- Giữ chuột trái.
- Sau đó nhấn `Q`.

Mong đợi:

- Basic projectile vẫn bắn bằng chuột trái.
- Skill projectile vẫn bắn bằng `Q`.
- Hai cooldown độc lập.

### Test 4 - Skill gây damage mạnh hơn

Thao tác:

- Bắn skill trúng Enemy.

Mong đợi:

- Skill gây damage `35`.
- Enemy HP `50`, nên một skill chưa giết Enemy đầy máu.
- Sau một skill, chỉ cần thêm basic projectile damage `10` và một hit nữa là Enemy gần hoặc sẽ chết tùy HP còn lại.

## Nếu nhấn Q không bắn

Hãy kiểm tra:

1. Game view đang focus.
2. Player có component `PlayerCombat2D`.
3. Field `Skill Projectile Prefab` không bị trống.
4. `Skill Cooldown` lớn hơn `0`.
5. Console không có lỗi `Missing Script` hoặc prefab reference bị mất.

## Nếu SkillProjectile không hiển thị

Nếu skill projectile có spawn nhưng không thấy trong Game view:

1. Mở `Assets/Prefabs/SkillProjectile.prefab`.
2. Kiểm tra component `SpriteRenderer`.
3. Gán sprite thủ công bằng `Circle` hoặc `Square` nếu field `Sprite` bị trống hoặc sprite không hiển thị.
4. Đổi màu sang màu khác basic projectile, ví dụ xanh lam hoặc tím.
5. Đặt `Sorting Order` cao hơn projectile thường, ví dụ `4`.
6. Tăng `Scale` nếu muốn skill nhìn mạnh hơn.

## Ghi chú

Skill hiện chưa xuyên nhiều Enemy. Projectile sẽ biến mất sau khi trúng mục tiêu đầu tiên vì `Destroy On Hit` đang bật.
