# Phase 4 - Hướng dẫn Enemy cơ bản

## Mục tiêu

Phase này thêm Enemy cơ bản để test combat prototype.

Kết quả mong muốn:

- Có Enemy trong scene `CombatPrototype`.
- Enemy có tag/layer đúng.
- Enemy có Rigidbody2D và Collider2D.
- Enemy tự tìm Player.
- Enemy tự di chuyển về phía Player.
- Enemy không rơi vì gravity và không tự xoay vật lý lung tung.

## File đã tạo hoặc sửa

- Sửa `Assets/Prefabs/Enemy.prefab`
- Sửa `Assets/Scenes/CombatPrototype.unity`
- Tạo file hướng dẫn này: `Assets/Docs/Phase4_HuongDan_EnemyCoBan.md`

Không tạo class mới. Không đổi tên class có sẵn. Không sửa `Player`, `Wall`, `FirePoint`, hoặc projectile logic.

## Nội dung đã chỉnh

### Enemy prefab

Prefab `Assets/Prefabs/Enemy.prefab` đã được chỉnh:

- Tag: `Enemy`
- Layer: `Enemy`
- `Rigidbody2D`:
  - Gravity Scale: `0`
  - Freeze Rotation Z: bật
- `BoxCollider2D`: dùng để va chạm vật lý
- `EnemyChaseAI2D`:
  - Move Speed: `2.5`
  - Target để trống để Enemy tự tìm Player khi chạy game
  - Rotate To Move Direction: tắt để Enemy không xoay hình lung tung
- `SimpleHealth`:
  - Max Health: `50`
  - Destroy On Death: bật

### Enemy trong scene

Scene `Assets/Scenes/CombatPrototype.unity` đã được thêm một object `Enemy` để test ngay.

Thông số Enemy test trong scene:

- Vị trí ban đầu: `(4, 0, 0)`
- Tag: `Enemy`
- Layer: `Enemy`
- Có `SpriteRenderer`
- Có `Rigidbody2D`
- Có `BoxCollider2D`
- Có `EnemyChaseAI2D`
- Có `SimpleHealth`

## Cách kiểm tra trong Unity

1. Mở scene `Assets/Scenes/CombatPrototype.unity`.
2. Trong Hierarchy, kiểm tra có object `Enemy`.
3. Chọn `Enemy`.
4. Kiểm tra Inspector:
   - Tag là `Enemy`.
   - Layer là `Enemy`.
   - Có `Rigidbody2D`.
   - Có `BoxCollider2D`.
   - Có `EnemyChaseAI2D`.
   - Có `SimpleHealth`.
5. Mở prefab `Assets/Prefabs/Enemy.prefab`.
6. Kiểm tra prefab cũng có tag/layer/component tương tự.
7. Bấm Play.

Kết quả đúng:

- Enemy tự chạy về phía Player.
- Enemy không bị rơi xuống.
- Enemy không tự xoay vật lý lung tung.
- Enemy chạy với tốc độ vừa phải, không quá nhanh.
- Player vẫn di chuyển và bắn như Phase 1 đến Phase 3.

## Test case

### Test 1 - Enemy xuất hiện trong scene

Thao tác:

- Mở scene `CombatPrototype`.
- Quan sát Hierarchy.

Mong đợi:

- Có object `Enemy`.
- Enemy nằm trong vùng arena.
- Enemy có tag `Enemy`.

### Test 2 - Enemy tìm Player

Thao tác:

- Chọn `Enemy`.
- Trong component `EnemyChaseAI2D`, để field `Target` trống.
- Bấm Play.

Mong đợi:

- Enemy tự tìm Player thông qua `PlayerMovement2D` hoặc tag `Player`.
- Không cần gán target thủ công.

### Test 3 - Enemy đuổi Player

Thao tác:

- Bấm Play.
- Đứng yên để Enemy tiến lại gần.
- Di chuyển Player bằng WASD.

Mong đợi:

- Enemy liên tục đi về phía Player.
- Khi Player đổi vị trí, Enemy đổi hướng đuổi theo.
- Enemy không chạy quá nhanh.

### Test 4 - Enemy không bị xoay vật lý

Thao tác:

- Cho Enemy va chạm với Player hoặc tường.

Mong đợi:

- Enemy không tự quay vòng do vật lý.
- Rigidbody2D vẫn Freeze Rotation Z.

## Nếu Enemy không hiển thị

Nếu Enemy có trong Hierarchy nhưng không nhìn thấy trong Game view, hãy làm thủ công:

1. Chọn object `Enemy` trong scene hoặc mở `Assets/Prefabs/Enemy.prefab`.
2. Kiểm tra có component `SpriteRenderer`.
3. Tại field `Sprite`, gán sprite có sẵn trong Unity, ví dụ `Circle` hoặc `Square`.
4. Đổi `Color` sang màu đỏ hoặc cam để dễ nhìn.
5. Đặt `Sorting Order` cao hơn nền/tường, ví dụ `1` hoặc `2`.
6. Nếu Enemy quá nhỏ hoặc quá to, chỉnh `Scale` theo ý bạn.
7. Nếu chỉnh prefab, nhớ Apply để prefab lưu thay đổi.

## Nếu Enemy không đuổi Player

Hãy kiểm tra:

1. Player có tag `Player`.
2. Player có component `PlayerMovement2D`.
3. Enemy có component `EnemyChaseAI2D`.
4. Enemy có `Rigidbody2D`.
5. Trong `EnemyChaseAI2D`, `Move Speed` lớn hơn `0`.
6. Field `Target` có thể để trống. Script sẽ tự tìm Player khi Play.

Nếu vẫn không chạy, mở Console để xem có lỗi `Missing Script`, `MissingReference`, hoặc lỗi tag/layer không.

## Ghi chú cho Phase 5

Phase 4 chỉ làm Enemy xuất hiện, tìm Player và đuổi Player.

Damage/contact damage sẽ làm ở Phase 5:

- Projectile gây damage cho Enemy.
- Enemy chết khi hết máu.
- Player có máu.
- Enemy chạm Player gây damage có cooldown.
