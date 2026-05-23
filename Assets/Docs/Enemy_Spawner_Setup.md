# Enemy Spawner Setup

Hướng dẫn này dùng cho vertical slice thứ hai: enemy tự đuổi theo player, nhận damage từ projectile và spawner tạo enemy quanh player hoặc quanh spawner.

## 1. Tạo Enemy GameObject

1. Trong scene, tạo một `GameObject` mới tên `Enemy`.
2. Gắn sprite hoặc placeholder 2D để dễ nhìn.
3. Đặt `Enemy` ở gần player để test chase trước khi tạo prefab.
4. Chỉnh scale hoặc màu khác player để dễ phân biệt.

## 2. Component cần có trên Enemy

Gắn các component sau vào `Enemy`:

- `Rigidbody2D`
- `Collider2D`, ví dụ `CircleCollider2D` hoặc `BoxCollider2D`
- `EnemyChaseAI2D`
- `SimpleHealth`

Thiết lập khuyến nghị:

- `Rigidbody2D > Gravity Scale`: `0`
- `Rigidbody2D > Constraints > Freeze Rotation Z`: bật
- `EnemyChaseAI2D > Move Speed`: bắt đầu với `3`
- `EnemyChaseAI2D > Target`: có thể để trống nếu scene có `PlayerMovement2D` trên player
- `SimpleHealth > Max Health`: bắt đầu với `3`
- `SimpleHealth > Destroy On Death`: bật

`EnemyChaseAI2D` sẽ tự tìm player theo thứ tự:

1. Object có component `PlayerMovement2D`
2. Object có tag `Player`

Vì vậy tag `Player` không bắt buộc nếu player đã có `PlayerMovement2D`, nhưng vẫn nên đặt tag `Player` để scene rõ ràng hơn.

## 3. Tạo Enemy Prefab

1. Kéo `Enemy` từ Hierarchy vào `Assets/_Project/Prefabs`.
2. Đặt tên prefab là `Enemy`.
3. Sau khi tạo prefab, có thể xóa `Enemy` khỏi scene nếu muốn chỉ spawn bằng spawner.

Kiểm tra prefab trước khi dùng:

- Có `Rigidbody2D`
- Có collider 2D
- Có `EnemyChaseAI2D`
- Có `SimpleHealth`

## 4. Tạo EnemySpawner

1. Trong scene, tạo empty `GameObject` tên `EnemySpawner`.
2. Gắn script `EnemySpawner`.
3. Kéo prefab `Enemy` vào field `Enemy Prefab`.
4. Kéo `Player` vào field `Player`, hoặc để trống để spawner tự tìm `PlayerMovement2D`.
5. Chỉnh các field:
   - `Spawn Interval`: thời gian giữa mỗi lần spawn, ví dụ `2`
   - `Max Alive Enemies`: số enemy tối đa còn sống, ví dụ `10`
   - `Spawn Radius`: bán kính spawn, ví dụ `8`
   - `Min Distance From Player`: khoảng cách tối thiểu tránh spawn sát player, ví dụ `2`
   - `Spawn Around Player`: bật nếu muốn enemy spawn quanh player

Nếu `Spawn Around Player` tắt, enemy sẽ spawn quanh vị trí của `EnemySpawner`.

## 5. Cách test spawn và enemy chase

1. Mở scene có `Player`, `Projectile` prefab và `EnemySpawner`.
2. Nhấn Play.
3. Enemy sẽ xuất hiện theo `Spawn Interval`.
4. Enemy tự chạy về phía player.
5. Dùng chuột trái bắn projectile vào enemy.
6. Khi enemy nhận đủ damage, `SimpleHealth` sẽ destroy enemy.
7. Spawner sẽ tiếp tục spawn thêm enemy nếu số enemy còn sống thấp hơn `Max Alive Enemies`.

## Ghi chú damage

`Projectile2D` ưu tiên tìm `SimpleHealth` trên object bị va chạm. Nếu có component này, projectile gọi `TakeDamage(damage)`.

Nếu object không có `SimpleHealth`, projectile vẫn fallback gọi method `TakeDamage(float)` bằng `SendMessage` để tiện test nhanh.

Không cần hard-code tag `Enemy` cho damage ở bước này. Tag `Player` chỉ là fallback để enemy hoặc spawner tìm player nếu không tìm thấy `PlayerMovement2D`.
