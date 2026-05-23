# Player Combat Setup

Hướng dẫn này dùng cho vertical slice đầu tiên: player di chuyển bằng WASD, aim bằng chuột và bắn projectile bằng chuột trái.

## 1. Tạo Player GameObject

1. Trong scene, tạo một `GameObject` mới tên `Player`.
2. Gắn sprite hoặc placeholder 2D, ví dụ `SpriteRenderer` với sprite hình vuông/tròn.
3. Đặt `Player` ở giữa arena, ví dụ vị trí `(0, 0, 0)`.
4. Tạo child object tên `ProjectileSpawnPoint`.
5. Đặt `ProjectileSpawnPoint` hơi lệch về phía trước sprite, ví dụ local position `(0.6, 0, 0)`.

## 2. Component cần có trên Player

Gắn các component sau vào `Player`:

- `Rigidbody2D`
- `Collider2D`, ví dụ `CircleCollider2D` hoặc `BoxCollider2D`
- `PlayerMovement2D`
- `PlayerShooter2D`

Thiết lập khuyến nghị:

- `Rigidbody2D > Gravity Scale`: `0`
- `Rigidbody2D > Constraints > Freeze Rotation Z`: bật
- `PlayerMovement2D > Move Speed`: bắt đầu với `5`
- `PlayerShooter2D > Fire Rate`: bắt đầu với `5`
- `PlayerShooter2D > Aim Camera`: gán Main Camera nếu không tự nhận
- `PlayerShooter2D > Projectile Spawn Point`: gán child `ProjectileSpawnPoint`

## 3. Tạo Projectile Prefab

1. Tạo một `GameObject` mới tên `Projectile`.
2. Gắn sprite hoặc placeholder nhỏ để dễ nhìn.
3. Gắn `Rigidbody2D`.
4. Gắn `Collider2D`, ví dụ `CircleCollider2D`.
5. Gắn script `Projectile2D`.
6. Kéo `Projectile` từ Hierarchy vào `Assets/_Project/Prefabs` để tạo prefab.
7. Xóa object `Projectile` khỏi scene nếu không cần đặt sẵn.

Thiết lập khuyến nghị:

- `Projectile2D > Speed`: `12`
- `Projectile2D > Lifetime`: `2`
- `Projectile2D > Damage`: `1`
- `Projectile2D > Destroy On Hit`: bật

Script `Projectile2D` sẽ tự đặt `Rigidbody2D` thành kinematic, tắt gravity và đặt collider thành trigger khi chạy.

## 4. Gán projectilePrefab và projectileSpawnPoint

1. Chọn `Player` trong scene.
2. Ở component `PlayerShooter2D`, kéo prefab `Projectile` vào field `Projectile Prefab`.
3. Kéo child `ProjectileSpawnPoint` vào field `Projectile Spawn Point`.
4. Nếu camera trong scene chưa có tag `MainCamera`, kéo camera vào field `Aim Camera`.

## 5. Cách test trong Unity

1. Mở scene prototype hoặc `SampleScene`.
2. Nhấn Play.
3. Dùng `WASD` hoặc phím mũi tên để di chuyển.
4. Di chuyển chuột quanh player để đổi hướng aim.
5. Giữ hoặc bấm chuột trái để bắn.
6. Kiểm tra projectile bay theo hướng chuột và tự hủy sau thời gian `Lifetime`.

## Ghi chú input

Project hiện đang có New Input System. Các script dùng trực tiếp `Keyboard.current` và `Mouse.current` để prototype chạy nhanh trong Play Mode.

Sau này có thể thay bằng `InputSystem_Actions.inputactions` và `PlayerInput` để hỗ trợ rebinding, gamepad, mobile input hoặc UI input rõ ràng hơn.
