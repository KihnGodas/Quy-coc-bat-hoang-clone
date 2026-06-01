# Phase 2 - Hướng dẫn Aim và FirePoint

## Mục tiêu

Phase này thêm logic aim theo chuột cho Player và tạo `FirePoint` để sau này dùng làm vị trí spawn projectile.

## File đã tạo hoặc sửa

- Tạo `Assets/Script/Player/PlayerAim2D.cs`
- Sửa `Assets/Scenes/CombatPrototype.unity`
- Tạo file hướng dẫn này: `Assets/Docs/Phase2_HuongDan_Aim_FirePoint.md`

Không đổi tên class có sẵn. Không sửa `PlayerMovement2D.cs`. Không sửa prefab hiện có.

## Nội dung đã thêm

### PlayerAim2D

Script `PlayerAim2D` được gắn vào object `Player`.

Script này làm các việc:

- Lấy vị trí chuột trên màn hình bằng Input System.
- Chuyển vị trí chuột sang world position bằng `Main Camera`.
- Tính hướng aim từ `Player` đến chuột.
- Lưu hướng aim vào `AimDirection`.
- Cập nhật vị trí và rotation của `FirePoint`.

### FirePoint

`FirePoint` là child object của `Player`.

Vai trò:

- Hiển thị điểm bắn để nhìn rõ Player đang aim về đâu.
- Làm điểm spawn projectile trong Phase 3.

Thông số hiện tại:

- `FirePoint` nằm cách tâm Player khoảng `1` unit theo hướng chuột.
- `FirePoint` có sprite nhỏ màu vàng để dễ nhìn trong Game view.

## Cách kiểm tra trong Unity

1. Mở scene `Assets/Scenes/CombatPrototype.unity`.
2. Chọn object `Player`.
3. Kiểm tra `Player` có component `PlayerAim2D`.
4. Mở foldout của `Player` trong Hierarchy.
5. Kiểm tra có child object tên `FirePoint`.
6. Chọn `FirePoint`, kiểm tra nó có `SpriteRenderer`.
7. Bấm Play.
8. Di chuyển chuột quanh Player.

Kết quả đúng:

- `FirePoint` di chuyển quanh Player theo hướng chuột.
- `FirePoint` luôn nằm phía trước Player.
- Khi đưa chuột bên phải Player, `FirePoint` nằm bên phải.
- Khi đưa chuột bên trái Player, `FirePoint` nằm bên trái.
- Khi đưa chuột lên trên Player, `FirePoint` nằm phía trên.
- Khi đưa chuột xuống dưới Player, `FirePoint` nằm phía dưới.
- Player vẫn di chuyển WASD bình thường như Phase 1.

## Test case

### Test 1 - Aim sang phải

Thao tác:

- Bấm Play.
- Đưa chuột sang bên phải Player.

Mong đợi:

- `FirePoint` nằm bên phải Player.
- Hướng của `FirePoint` xoay sang phải.

### Test 2 - Aim sang trái

Thao tác:

- Đưa chuột sang bên trái Player.

Mong đợi:

- `FirePoint` nằm bên trái Player.
- Hướng của `FirePoint` xoay sang trái.

### Test 3 - Aim theo đường chéo

Thao tác:

- Đưa chuột lên góc trên phải hoặc trên trái Player.

Mong đợi:

- `FirePoint` nằm trên đường chéo từ Player đến chuột.
- Khoảng cách từ Player đến `FirePoint` gần như không đổi.

### Test 4 - Movement không bị ảnh hưởng

Thao tác:

- Vừa giữ WASD vừa di chuyển chuột quanh Player.

Mong đợi:

- Player vẫn di chuyển mượt.
- `FirePoint` vẫn bám theo hướng chuột.
- Player không bị xoay lung tung.

## Nếu FirePoint không hiển thị

Nếu trong Game view không thấy `FirePoint`, hãy làm thủ công:

1. Chọn `FirePoint` trong Hierarchy.
2. Kiểm tra có component `SpriteRenderer`.
3. Tại field `Sprite`, gán sprite có sẵn trong Unity, ví dụ `Circle` hoặc `Square`.
4. Đổi `Color` sang màu vàng hoặc xanh lá để dễ nhìn.
5. Đặt `Sorting Order` cao hơn Player, ví dụ `2`.
6. Nếu vẫn quá nhỏ, tăng `Scale` của `FirePoint`, ví dụ `0.3, 0.3, 1`.

Miễn là `FirePoint` vẫn là child của `Player` và field `Fire Point` trong `PlayerAim2D` vẫn trỏ đúng vào object này.
