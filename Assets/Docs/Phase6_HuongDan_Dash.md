# Phase 6 - Hướng dẫn Dash

## Mục tiêu

Phase này thêm dash cho Player.

Kết quả mong muốn:

- Nhấn `Space` để dash.
- Dash theo hướng WASD hiện tại.
- Nếu không nhấn WASD, dash theo hướng di chuyển cuối cùng.
- Dash có cooldown, không spam liên tục.
- Trong lúc dash, Player không nhận contact damage từ Enemy.
- Trong lúc dash, Player đi xuyên qua Enemy nhưng vẫn bị Wall chặn.

## File đã tạo hoặc sửa

- Tạo `Assets/Script/Player/PlayerDash2D.cs`
- Sửa `Assets/Script/Player/PlayerMovement2D.cs`
- Sửa `Assets/Scenes/CombatPrototype.unity`
- Tạo file hướng dẫn này: `Assets/Docs/Phase6_HuongDan_Dash.md`

Không đổi tên class có sẵn. Không di chuyển file nào. Không sửa prefab Enemy hoặc Projectile.

## Nội dung đã thêm

### PlayerMovement2D

`PlayerMovement2D` được mở rộng để script dash đọc được hướng di chuyển:

- `MoveInput`: hướng WASD hiện tại.
- `LastMoveDirection`: hướng di chuyển cuối cùng khác zero.
- `CanMove`: cho phép script khác khóa movement thường trong lúc dash.

Logic movement cũ vẫn giữ nguyên.

### PlayerDash2D

Script `PlayerDash2D` được gắn vào object `Player`.

Thông số hiện tại:

- Dash Speed: `12`
- Dash Duration: `0.15`
- Dash Cooldown: `1`
- Player Layer Name: `Player`
- Enemy Layer Name: `Enemy`

Khi dash bắt đầu:

- Khóa movement thường qua `PlayerMovement2D.CanMove = false`.
- Bật invincible qua `PlayerHealth2D.SetInvincible(true)`.
- Tạm ignore collision giữa layer `Player` và `Enemy`.
- Di chuyển Rigidbody2D theo hướng dash.

Khi dash kết thúc:

- Mở lại movement thường.
- Tắt invincible.
- Khôi phục collision giữa layer `Player` và `Enemy`.

## Cách kiểm tra trong Unity

1. Mở scene `Assets/Scenes/CombatPrototype.unity`.
2. Chọn object `Player`.
3. Kiểm tra `Player` có component `PlayerDash2D`.
4. Kiểm tra các field:
   - `Player Movement` trỏ tới `PlayerMovement2D`.
   - `Player Health` trỏ tới `PlayerHealth2D`.
   - Dash Speed `12`.
   - Dash Duration `0.15`.
   - Dash Cooldown `1`.
5. Bấm Play.

## Test case

### Test 1 - Dash sang phải

Thao tác:

- Giữ `D`.
- Nhấn `Space`.

Mong đợi:

- Player lao nhanh sang phải một đoạn ngắn.
- Sau dash, Player tiếp tục di chuyển bình thường.

### Test 2 - Dash lên trên

Thao tác:

- Giữ `W`.
- Nhấn `Space`.

Mong đợi:

- Player dash lên trên.

### Test 3 - Dash chéo

Thao tác:

- Giữ `W + D`.
- Nhấn `Space`.

Mong đợi:

- Player dash chéo lên phải.
- Tốc độ dash chéo không nhanh bất thường so với dash thẳng.

### Test 4 - Không spam dash

Thao tác:

- Nhấn `Space` liên tục.

Mong đợi:

- Player chỉ dash một lần.
- Phải chờ khoảng `1` giây mới dash tiếp được.

### Test 5 - Dash theo hướng cuối cùng

Thao tác:

- Giữ `A` để đi sang trái.
- Thả `A`.
- Nhấn `Space` khi không giữ WASD.

Mong đợi:

- Player dash sang trái, theo hướng di chuyển cuối cùng.

### Test 6 - Invincible khi dash

Thao tác:

- Để Enemy chạm Player.
- Khi Enemy đang áp sát, nhấn `Space` để dash xuyên khỏi vùng nguy hiểm.
- Quan sát `Current Health` trong `PlayerHealth2D`.

Mong đợi:

- Trong thời gian dash, Player không nhận contact damage.
- Sau dash, Player nhận damage lại bình thường nếu tiếp tục chạm Enemy.

### Test 7 - Dash xuyên qua Enemy

Thao tác:

- Để Enemy đứng chắn trước mặt Player.
- Dash thẳng vào hướng Enemy.

Mong đợi:

- Player đi xuyên qua Enemy trong thời gian dash.
- Enemy không chặn Player lại.
- Player không mất HP trong lúc dash qua Enemy.
- Sau dash, nếu Player tiếp tục chạm Enemy thì damage hoạt động lại bình thường.

### Test 8 - Không dash xuyên wall

Thao tác:

- Đứng gần wall.
- Dash về phía wall.

Mong đợi:

- Player bị wall chặn.
- Nếu có trường hợp xuyên wall, đổi `Rigidbody2D > Collision Detection` của Player sang `Continuous` trong Unity.

## Nếu dash không hoạt động

Hãy kiểm tra:

1. Player có component `PlayerDash2D`.
2. `Player Movement` không bị trống.
3. `Player Health` không bị trống.
4. `Dash Speed` lớn hơn `0`.
5. `Dash Duration` lớn hơn `0`.
6. Game view đang nhận input bàn phím.
7. Không có lỗi `Missing Script` trong Console.
8. Nếu Player đã chết, `PlayerHealth2D` sẽ tắt `PlayerDash2D`, nên cần Play lại hoặc reset HP để test dash.

## Nếu Player vẫn mất máu khi dash

Hãy kiểm tra:

1. Enemy gây damage thông qua `EnemyContactDamage2D`.
2. Player có `PlayerHealth2D`.
3. Trong `PlayerDash2D`, field `Player Health` trỏ đúng component `PlayerHealth2D`.
4. Dash Duration không quá ngắn khiến bạn khó quan sát invincible.

## Nếu dash không xuyên qua Enemy

Hãy kiểm tra:

1. Player đang ở layer `Player`.
2. Enemy prefab đang ở layer `Enemy`.
3. Trong `PlayerDash2D`, `Player Layer Name` là `Player`.
4. Trong `PlayerDash2D`, `Enemy Layer Name` là `Enemy`.
5. Enemy collider không nằm trên layer khác do object con hoặc prefab override.
6. Nếu bạn đổi tên layer trong Project Settings, cập nhật lại hai field layer name trong `PlayerDash2D`.
