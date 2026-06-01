# Phase 3 - Hướng dẫn Basic Projectile

## Mục tiêu

Phase này thêm đòn đánh cơ bản bằng projectile cho Player.

Kết quả mong muốn:

- Player có thể bắn projectile bằng chuột trái.
- Giữ chuột trái có thể bắn liên tục.
- Tốc độ bắn bị giới hạn bởi cooldown `0.25` giây.
- Projectile spawn từ `FirePoint`.
- Projectile bay thẳng theo hướng aim chuột.
- Projectile tự hủy sau `1.5` giây.

## File đã tạo hoặc sửa

- Tạo `Assets/Script/Player/PlayerCombat2D.cs`
- Sửa `Assets/Script/Combat/Projectile2D.cs`
- Sửa `Assets/Prefabs/Projectile.prefab`
- Sửa `Assets/Scenes/CombatPrototype.unity`
- Tạo file hướng dẫn này: `Assets/Docs/Phase3_HuongDan_BasicProjectile.md`

Không đổi tên class có sẵn. Không xóa `PlayerShooter2D.cs`. Không di chuyển file nào.

## Nội dung đã thêm

### PlayerCombat2D

Script `PlayerCombat2D` được gắn vào object `Player`.

Script này làm các việc:

- Đọc input chuột trái bằng Input System.
- Dùng `PlayerAim2D.AimDirection` làm hướng bắn.
- Dùng `PlayerAim2D.FirePoint` làm vị trí spawn projectile.
- Instantiate `Projectile.prefab`.
- Gọi `Projectile2D.Launch(direction, owner)` để projectile bay đúng hướng.
- Giới hạn tốc độ bắn bằng `basicFireCooldown = 0.25`.

### Projectile2D

Script `Projectile2D` đã được chỉnh default:

- `speed = 12`
- `damage = 10`
- `lifetime = 1.5`

Script cũng tự đảm bảo khi validate:

- `Rigidbody2D` là Kinematic.
- Gravity Scale bằng `0`.
- Collider là Trigger.

### Projectile prefab

Prefab `Assets/Prefabs/Projectile.prefab` đã được chỉnh:

- Tag: `Projectile`
- Layer: `Projectile`
- `Rigidbody2D` Body Type: Kinematic
- `Rigidbody2D` Gravity Scale: `0`
- Collider: `CircleCollider2D`
- Collider Is Trigger: bật
- `Projectile2D`:
  - Speed: `12`
  - Damage: `10`
  - Lifetime: `1.5`

## Cách kiểm tra trong Unity

1. Mở scene `Assets/Scenes/CombatPrototype.unity`.
2. Chọn object `Player`.
3. Kiểm tra `Player` có component `PlayerCombat2D`.
4. Trong `PlayerCombat2D`, kiểm tra:
   - `Player Aim` trỏ tới component `PlayerAim2D` trên Player.
   - `Basic Projectile Prefab` trỏ tới `Assets/Prefabs/Projectile.prefab`.
   - `Basic Fire Cooldown` là `0.25`.
5. Mở prefab `Assets/Prefabs/Projectile.prefab`.
6. Kiểm tra prefab có:
   - Tag `Projectile`.
   - Layer `Projectile`.
   - `Rigidbody2D`.
   - `CircleCollider2D`.
   - `Projectile2D`.
7. Bấm Play trong scene `CombatPrototype`.
8. Di chuyển chuột để đổi hướng `FirePoint`.
9. Click chuột trái để bắn.
10. Giữ chuột trái để bắn liên tục.

Kết quả đúng:

- Click chuột trái bắn ra 1 projectile.
- Projectile spawn tại vị trí `FirePoint`.
- Projectile bay theo hướng chuột.
- Giữ chuột trái bắn liên tục nhưng không quá nhanh.
- Projectile tự biến mất sau khoảng `1.5` giây.
- Player vẫn di chuyển WASD bình thường.

## Test case

### Test 1 - Bắn từng viên

Thao tác:

- Bấm Play.
- Đưa chuột sang bên phải Player.
- Click chuột trái một lần.

Mong đợi:

- Một projectile xuất hiện tại `FirePoint`.
- Projectile bay sang phải.
- Sau khoảng `1.5` giây, projectile tự biến mất.

### Test 2 - Bắn theo hướng chuột

Thao tác:

- Đưa chuột lên trên Player rồi click chuột trái.
- Đưa chuột sang trái Player rồi click chuột trái.

Mong đợi:

- Projectile đầu bay lên trên.
- Projectile sau bay sang trái.
- Projectile không bay lệch khỏi hướng `FirePoint`.

### Test 3 - Giữ chuột trái

Thao tác:

- Giữ chuột trái trong khoảng 2 giây.

Mong đợi:

- Player bắn liên tục.
- Tốc độ bắn bị giới hạn, không bắn ra quá dày mỗi frame.
- Với cooldown `0.25`, trong 2 giây sẽ có khoảng 8 viên nếu giữ liên tục.

### Test 4 - Vừa di chuyển vừa bắn

Thao tác:

- Giữ WASD để di chuyển.
- Di chuột quanh Player.
- Giữ chuột trái để bắn.

Mong đợi:

- Player vẫn di chuyển bình thường.
- `FirePoint` vẫn xoay theo chuột.
- Projectile vẫn spawn từ `FirePoint`.
- Projectile bay đúng hướng aim tại thời điểm bắn.

## Nếu projectile không hiển thị

Nếu projectile có spawn nhưng không nhìn thấy trong Game view, hãy kiểm tra thủ công:

1. Mở `Assets/Prefabs/Projectile.prefab`.
2. Chọn object gốc `Projectile`.
3. Kiểm tra có component `SpriteRenderer`.
4. Tại field `Sprite`, gán sprite có sẵn trong Unity, ví dụ `Circle` hoặc `Square`.
5. Đổi `Color` sang màu dễ nhìn, ví dụ vàng, xanh lam hoặc tím.
6. Đặt `Sorting Order` cao hơn nền và tường, ví dụ `3`.
7. Kiểm tra scale của prefab, nếu quá nhỏ có thể tăng lên `0.3, 0.3, 1` hoặc theo ý bạn.

## Nếu bấm chuột không bắn

Hãy kiểm tra các bước sau:

1. Chọn `Player`.
2. Kiểm tra có component `PlayerCombat2D`.
3. Kiểm tra field `Player Aim` không bị trống.
4. Kiểm tra field `Basic Projectile Prefab` không bị trống.
5. Chọn child `FirePoint`, kiểm tra object này vẫn tồn tại.
6. Kiểm tra project đang dùng Input System và chuột hoạt động trong Game view.

Nếu vẫn không bắn, mở Console để xem có lỗi `MissingReference`, `Missing Script`, hoặc prefab reference bị mất không.
