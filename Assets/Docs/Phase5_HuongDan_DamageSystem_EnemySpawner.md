# Phase 5 - Hướng dẫn Damage System và Enemy Spawner

## Mục tiêu

Phase này hoàn thiện vòng lặp combat cơ bản:

- Projectile gây damage cho Enemy.
- Enemy chết khi hết HP.
- Player có máu.
- Enemy chạm Player gây damage có cooldown.
- Enemy xuất hiện liên tục bằng spawner.

## File đã tạo hoặc sửa

- Tạo `Assets/Script/Player/PlayerHealth2D.cs`
- Tạo `Assets/Script/Enemies/EnemyContactDamage2D.cs`
- Sửa `Assets/Prefabs/Enemy.prefab`
- Sửa `Assets/Scenes/CombatPrototype.unity`
- Tạo file hướng dẫn này: `Assets/Docs/Phase5_HuongDan_DamageSystem_EnemySpawner.md`

Không đổi tên class có sẵn. Không di chuyển file nào. Enemy đặt tay trong scene đã được bỏ ra để chỉ dùng `EnemySpawner`.

## Nội dung đã thêm

### PlayerHealth2D

Script `PlayerHealth2D` được gắn vào object `Player`.

Thông số hiện tại:

- Max Health: `100`
- Current Health: `100`
- Disable Controls On Death: bật

Khi Player nhận damage:

- HP giảm theo lượng damage.
- Nếu HP về `0`, Console log `"Player Dead"`.
- Khi chết, Player tạm bị tắt `PlayerMovement2D`, `PlayerAim2D`, và `PlayerCombat2D`.

Script cũng có `SetInvincible(bool)` để dùng tiếp cho dash ở Phase 6.

### EnemyContactDamage2D

Script `EnemyContactDamage2D` được gắn vào prefab `Enemy`.

Thông số hiện tại:

- Contact Damage: `10`
- Attack Cooldown: `1`

Khi Enemy chạm Player:

- Nếu đã hết cooldown, Enemy gọi `PlayerHealth2D.TakeDamage(10)`.
- Sau đó chờ `1` giây mới gây damage tiếp.

### EnemySpawner

`GameManager` trong scene `CombatPrototype` đã được gắn `EnemySpawner`.

Thông số hiện tại:

- Enemy Prefab: `Assets/Prefabs/Enemy.prefab`
- Player: object `Player`
- Spawn Interval: `2`
- Max Alive Enemies: `8`
- Spawn Radius: `7`
- Min Distance From Player: `3`
- Spawn Around Player: bật
- Left Wall / Right Wall / Top Wall / Bottom Wall: trỏ tới 4 wall collider trong scene
- Spawn From Arena Edges: bật
- Edge Spawn Padding: `1`
- Spawn Position Attempts: `20`
- Wall Padding: `0.5`

Spawner sẽ tự sinh Enemy liên tục quanh Player, tối đa `8` Enemy còn sống.
Enemy sẽ spawn từ mép trong arena rồi tự chạy vào phía Player.
Khi Player đứng sát tường, spawner vẫn kiểm tra để Enemy không spawn ngoài wall và không spawn quá sát Player nếu còn vị trí hợp lệ.

## Cách kiểm tra trong Unity

1. Mở scene `Assets/Scenes/CombatPrototype.unity`.
2. Kiểm tra Hierarchy không còn Enemy đặt sẵn thủ công.
3. Chọn `GameManager`.
4. Kiểm tra có component `EnemySpawner`.
5. Kiểm tra `EnemySpawner` đã gán:
   - `Enemy Prefab`
   - `Player`
   - Spawn Interval `2`
   - Max Alive Enemies `8`
6. Chọn `Player`.
7. Kiểm tra có component `PlayerHealth2D`.
8. Mở `Assets/Prefabs/Enemy.prefab`.
9. Kiểm tra prefab có component `EnemyContactDamage2D`.
10. Bấm Play.

Kết quả đúng:

- Sau khoảng `2` giây, Enemy bắt đầu spawn.
- Enemy chạy về phía Player.
- Bắn Enemy bằng chuột trái làm Enemy mất máu.
- Với Enemy HP `50` và projectile damage `10`, bắn trúng khoảng 5 phát thì Enemy chết.
- Enemy chạm Player làm Player mất `10` HP.
- Player không mất máu mỗi frame; damage có cooldown khoảng `1` giây.

## Test case

### Test 1 - Spawner sinh Enemy từ mép arena

Thao tác:

- Bấm Play.
- Đợi khoảng `2` giây.

Mong đợi:

- Enemy đầu tiên xuất hiện gần một mép trong arena.
- Enemy tạo cảm giác đang đi từ ngoài biên vào.
- Enemy không xuất hiện ngay sát Player nếu Player đang ở giữa arena.
- Enemy tự chạy về phía Player.

### Test 2 - Giới hạn số Enemy sống

Thao tác:

- Bấm Play và không bắn trong vài giây.

Mong đợi:

- Enemy tiếp tục spawn theo chu kỳ.
- Số Enemy sống không vượt quá `8`.

### Test 3 - Projectile giết Enemy

Thao tác:

- Bấm Play.
- Bắn một Enemy bằng chuột trái.
- Cố gắng bắn trúng cùng Enemy khoảng 5 phát.

Mong đợi:

- Enemy nhận damage.
- Enemy biến mất sau khi HP về `0`.
- Sau đó spawner có thể sinh Enemy mới nếu số Enemy còn sống dưới giới hạn.

### Test 4 - Enemy gây damage Player

Thao tác:

- Bấm Play.
- Để Enemy chạm Player.
- Chọn Player trong Hierarchy và xem `PlayerHealth2D`.

Mong đợi:

- `Current Health` giảm từ `100` xuống `90`.
- Nếu tiếp tục bị chạm, HP giảm tiếp sau mỗi khoảng `1` giây.
- HP không tụt cực nhanh mỗi frame.

### Test 5 - Player chết

Thao tác:

- Để Enemy đánh Player nhiều lần đến khi HP về `0`.

Mong đợi:

- Console hiện log `"Player Dead"`.
- Player không còn di chuyển/bắn/aim sau khi chết.

### Test 6 - Enemy không spawn ngoài wall

Thao tác:

- Bấm Play.
- Di chuyển Player sát `Wall_Left`, `Wall_Right`, `Wall_Top`, hoặc `Wall_Bottom`.
- Đợi Enemy spawn vài lần.

Mong đợi:

- Enemy chỉ xuất hiện bên trong phạm vi 4 wall.
- Enemy không spawn ở phía ngoài wall.
- Nếu Player đứng sát góc, Enemy vẫn được clamp hoặc chọn vị trí hợp lệ bên trong arena.

### Test 7 - Enemy không pop ngay gần Player

Thao tác:

- Bấm Play.
- Đứng gần giữa arena.
- Quan sát vài lượt Enemy spawn.

Mong đợi:

- Enemy không xuất hiện ngay cạnh Player.
- Enemy xuất hiện gần rìa arena rồi chạy về phía Player.
- Cảm giác spawn giống Enemy tiến từ ngoài biên vào.

## Nếu Enemy không spawn

Hãy kiểm tra:

1. Chọn `GameManager`.
2. Kiểm tra có component `EnemySpawner`.
3. Field `Enemy Prefab` không bị trống.
4. Field `Player` không bị trống.
5. `Spawn Interval` lớn hơn `0`.
6. `Max Alive Enemies` lớn hơn `0`.
7. Mở Console để xem có lỗi `Missing Script` hoặc prefab reference bị mất không.

## Nếu Enemy vẫn spawn ngoài arena

Spawner hiện đã dùng 4 wall collider để giới hạn vùng spawn.

Hãy kiểm tra:

1. Chọn `GameManager`.
2. Trong component `EnemySpawner`, kiểm tra 4 field sau không bị trống:
   - `Left Wall`
   - `Right Wall`
   - `Top Wall`
   - `Bottom Wall`
3. Mỗi field phải trỏ đúng tới collider của wall tương ứng trong scene.
4. Nếu bạn thay wall hoặc tạo wall mới, hãy gán lại reference ở `GameManager`.
5. Kiểm tra collider của wall có phủ đúng vị trí visual wall.
6. Nếu Enemy spawn quá sát wall, tăng `Wall Padding`, ví dụ từ `0.5` lên `1`.

## Nếu Enemy vẫn xuất hiện quá gần Player

Hãy kiểm tra:

1. Chọn `GameManager`.
2. Trong `EnemySpawner`, kiểm tra `Spawn From Arena Edges` đang bật.
3. Tăng `Min Distance From Player`, ví dụ từ `3` lên `4`.
4. Tăng hoặc giảm `Edge Spawn Padding` để chỉnh vị trí Enemy nằm sâu vào trong arena hơn hoặc sát biên hơn.

## Nếu Player không mất máu

Hãy kiểm tra:

1. Player có `PlayerHealth2D`.
2. Enemy prefab có `EnemyContactDamage2D`.
3. Player và Enemy đều có collider không phải trigger.
4. Enemy có Rigidbody2D.
5. Enemy thật sự chạm vào Player trong Play Mode.

## Nếu Enemy không chết khi bị bắn

Hãy kiểm tra:

1. Enemy prefab có `SimpleHealth`.
2. `SimpleHealth.maxHealth` là `50`.
3. Projectile prefab có `Projectile2D.damage` là `10`.
4. Projectile collider là trigger.
5. Enemy collider đang bật.
