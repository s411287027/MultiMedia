# 🍔 皮老闆的秘方爭奪戰

> Plankton's Secret Formula Heist · 期末專題 · Unity 3D 動作潛入小遊戲

---

## 📖 遊戲介紹

皮老闆趁蟹老闆不注意，潛入比奇堡偷取蟹堡王的神秘配方！

玩家扮演皮老闆，從比奇堡街頭出發，先躲開漂浮的水母群，再橫越車水馬龍的「船車」大道，最後闖過蟹堡王廚房裡瘋狂旋轉的料理棒，成功拿到 **Krabby Patty** 並帶回終點。

| 項目 | 內容 |
| ---- | ---- |
| **遊戲類型** | 3D 第三人稱動作潛入 |
| **操作方式** | WASD 移動（相對相機方向）、Space 跳躍、滑鼠拖動視角 |
| **勝利條件** | 拿到蟹堡（Patty）後，回到終點觸發區（WinZone） |
| **失敗條件** | 三顆愛心歸零 → Game Over；中途被打到會回到最近的檢查點 |

---

## 🎮 操作說明

| 按鍵 | 動作 |
| ---- | ---- |
| `W` / `A` / `S` / `D` | 依相機朝向前後左右移動 |
| `Space` | 跳躍（長按可跳更高，短按抑制跳躍弧） |
| `滑鼠左右移動` | 旋轉相機（Yaw） |

* 角色朝向會自動隨相機 yaw 同步，所以「往前走」永遠是螢幕的前方。
* 跳躍手感使用 `fallMultiplier` / `lowJumpMultiplier`，下落較快、放開跳鍵會提前下墜，避免「太飄」。

---

## 🗺️ 關卡設計

本遊戲共有 **三個區域**，全部整合在 `SampleScene.unity` 內依序串連：

### 場景一｜`SampleScene.unity` — 水母區 + 船車馬路 + 蟹堡王廚房

#### 第一段｜水母草原
* 場景：比奇堡的草地廣場。
* 障礙：由 `JellyfishSpawner` 在 `Start()` 一次生成 N 隻水母，每隻交由 `Jellyfish` 隨機漫遊於 `grassGround` 的 AABB 範圍內（X/Z 隨機目標 + Y 軸 minY~maxY 上下漂浮）。
* 撞到水母 → 觸發 `ShakeThenRespawn`：皮老闆被電到原地抽搐 3 秒、扣一顆心，再傳送回最近的檢查點。

#### 第二段｜船車馬路
* 場景：水母區後方的多線道馬路。
* 障礙：每條車道一個 `BoatSpawner`，協程 `SpawnRoutine` 每隔 `Random.Range(minSpawnTime, maxSpawnTime)` 秒生成一台船車。每台 `BoatMovement` 沿 `Vector3.forward` 前進，超過 `destroyDistance` 自動 `Destroy`。
* 撞到 `Car` tag → 直接扣血並 Respawn。

#### 第三段｜蟹堡王廚房

* 障礙：多組 `RotatingBar` 持續繞 `rotationAxis` 旋轉（每組可獨立調整 `rotationSpeed` 做難度遞進）。
* 撞到 `RotatingBar` tag → 扣血並 Respawn。
* 場景內放置 **Krabby Patty**：踩到時 `hasPatty = true`、外觀切換成 `holdingModel`、檢查點重設到 Patty 位置。
* 場景內放置 **WinZone**：帶著蟹堡進入即觸發 `HealthManager.Win()`，跳出勝利畫面並顯示耗時。

### 場景二｜`MainMenu.unity` — 標題畫面
* `MainMenuManager` 提供 `StartGame`（載入 `SampleScene`）、`ShowIntro` / `CloseIntro`（切換故事說明面板）。

> ⚠️ 場景以字串硬編碼切換，所有要播放的場景必須加入 **File ▸ Build Profiles ▸ Scene List**。

---

## 🧱 系統架構

沒有中央 GameManager，所有狀態由各自 MonoBehaviour 持有，並用 Inspector 的拖曳 reference 串接（不靠 `Find`）。

| 腳本 | 角色 |
| ---- | ---- |
| [`Plankton.cs`](Assets/Scripts/Plankton.cs) | 玩家本體：相機相對移動、跳躍、被電抽搐、Respawn、收集 Patty、處理 `Jellyfish` / `Car` / `Patty` / `WinZone` / `RotatingBar` 五種 tag 碰撞 |
| [`CameraController.cs`](Assets/Scripts/CameraController.cs) | 第三人稱環繞相機；負責鎖定游標；對外提供 `GetYaw` / `GetCameraForward` / `GetCameraRight` |
| [`HealthManager.cs`](Assets/Scripts/HealthManager.cs) | 3 顆愛心 UI、`TakeDamage` / `GameOver` / `Win` / `Restart`；同時是事實上的遊戲狀態中樞 |
| [`GameTimer.cs`](Assets/Scripts/GameTimer.cs) | 計時器，勝負結束時 `StopTimer`，並提供 `GetElapsedTime` 給勝利面板顯示 |
| [`WinPanelController.cs`](Assets/Scripts/WinPanelController.cs) | 把耗時格式化到勝利面板的 TextMeshPro |
| [`Checkpoint.cs`](Assets/Scripts/Checkpoint.cs) | 一次性觸發器；通過後呼叫 `Plankton.SetCheckpoint` 改寫重生點 |
| [`LevelExit.cs`](Assets/Scripts/LevelExit.cs) | 廚房出口的標記點（不結算遊戲，真正勝利由 `WinZone` 觸發） |
| [`MainMenuManager.cs`](Assets/Scripts/MainMenuManager.cs) | 主選單按鈕邏輯 |
| [`BoatSpawner.cs`](Assets/Scripts/BoatSpawner.cs) / [`BoatMovement.cs`](Assets/Scripts/BoatMovement.cs) | 馬路車流；協程隨機間隔生成、距離達標自毀 |
| [`JellyfishSpawner.cs`](Assets/Scripts/JellyfishSpawner.cs) / [`Jellyfish.cs`](Assets/Scripts/Jellyfish.cs) | 水母群；一次性生成 + 範圍內隨機漫遊 |
| [`RotatingBar.cs`](Assets/Scripts/RotatingBar.cs) | 廚房旋轉棒；單純 `Transform.Rotate` |

### Tag 契約

新增障礙或拾取物時，請務必：
1. 在 [`ProjectSettings/TagManager.asset`](ProjectSettings/TagManager.asset) 註冊 tag。
2. 在 `Plankton.cs` 的 `OnCollisionEnter`（實體撞擊）或 `OnTriggerEnter`（拾取／觸發區）內新增對應分支。

目前已被處理的 tag：`Jellyfish`、`Car`、`Patty`、`WinZone`、`RotatingBar`。

---

## 🛠️ 技術規格

| 項目 | 版本 / 設定 |
| ---- | ----------- |
| Unity Editor | **6000.4.5f1**（Unity 6，不可降版） |
| Render Pipeline | URP 17.4（`com.unity.render-pipelines.universal`） |
| Input | 新版 Input System 1.19（直接以 `Keyboard.current` / `Mouse.current` 輪詢，未綁定 `InputSystem_Actions`） |
| 物理 | 內建 3D 物理（Rigidbody + Collider，非 `physics2d`） |
| UI | TextMeshPro |

---

## ▶️ 開啟與遊玩

1. 用 Unity Hub 安裝 **Unity 6000.4.5f1**。
2. 開啟此資料夾為 Unity 專案（Unity 會自動還原 `Library/`、產生 `MultiMedia.slnx`）。
3. 確認 **File ▸ Build Profiles ▸ Scene List** 內依序有：`MainMenu`、`SampleScene`。
4. 開啟 `Assets/Scenes/MainMenu.unity`，按下 ▶️ Play。

> 沒有 headless build script、沒有自動化測試，所有遊玩驗證都在 Editor 進行。

---

## ⚠️ 注意事項（給後續維護者）

* **Time.timeScale 全域共用**：`HealthManager.GameOver()` 會把 `Time.timeScale` 設為 0，只有 `Restart()` 會把它還原為 1。任何新加入的暫停／轉場流程都要記得復原，否則下一個場景會「冰封」載入。`Win()` 故意不凍結時間，讓勝利面板覆蓋於還在跑的場景上。
* **Cursor 鎖定有兩個 owner**：`CameraController.Start` 鎖定游標，`HealthManager.Win / GameOver` 解鎖。新增任何選單／暫停狀態時請成對處理。
* **Respawn 點會被偷偷搬走**：撿到 Patty 時 `Plankton` 會把自己的檢查點重設成 Patty 所在位置；通過 `Checkpoint` 也會改寫。如果重生位置怪怪的，請優先檢查這兩處。
* **編碼**：[`HealthManager.cs`](Assets/Scripts/HealthManager.cs)、[`GameTimer.cs`](Assets/Scripts/GameTimer.cs)、[`MainMenuManager.cs`](Assets/Scripts/MainMenuManager.cs)、[`CameraController.cs`](Assets/Scripts/CameraController.cs) 內部分中文註解曾以非 UTF-8 編碼存檔，呈現亂碼（`�[�o��`）。除非能確認原意，否則不要盲改；新增註解請以 UTF-8 撰寫。
* **Library / Temp / Logs / UserSettings**：Unity 自動產生，不要 commit、不要手動編輯。`_Recovery/` 是編輯器崩潰回復資料夾，亦非真正來源碼。

---

## 👥 分工

| 組員 | 負責模組 | 主要產出 |
| ---- | -------- | -------- |
| A | 核心程式 | 玩家移動、跳躍手感、被電特效、Respawn、檢查點 |
| B | 障礙物機關 | 船車生成與移動、水母漫遊、旋轉棒 |
| C | 場景與關卡 | 三個區域的地圖搭建、Checkpoint 與 WinZone 配置、Patty 擺放 |
| D | 系統與 UI | 主選單、計時器、愛心 UI、勝利／失敗面板 |
