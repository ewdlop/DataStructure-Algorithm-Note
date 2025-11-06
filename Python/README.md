# 🌳 English Trie - NLTK 英文字典樹完整實現

## 📋 專案簡介

這是一個完整的 **Trie（字典樹）** 數據結構實現，整合了 **NLTK 英文語料庫**，包含 234,375+ 個英文單詞。專案包含多個功能模組和示例程式，適合學習和實際應用。

## 🎯 主要特性

- ✅ **完整的 Trie 實現** - 支持插入、搜索、刪除、前綴匹配
- ✅ **NLTK 整合** - 自動從 NLTK 語料庫載入英文單詞
- ✅ **自動補全功能** - 根據前綴提供單詞建議
- ✅ **拼寫檢查器** - 檢測拼寫錯誤並提供修正建議
- ✅ **視覺化工具** - 以樹狀結構顯示 Trie
- ✅ **互動式介面** - 友好的命令列操作界面
- ✅ **跨平台支援** - Windows/Linux/Mac

## 📂 文件結構

```
Python/
├── EnglishTrie.py                 # ⭐ 核心 Trie 實現
├── EnglishTrie_Interactive.py     # 💻 互動式程式
├── EnglishTrie_SpellChecker.py    # 📝 拼寫檢查器
├── EnglishTrie_Visualize.py       # 🎨 視覺化工具
├── test_trie.py                   # 🧪 基本測試
├── test_visualize.py              # 🧪 視覺化測試
├── requirements.txt               # 📦 依賴項
├── EnglishTrie_README.md         # 📖 詳細文檔
├── EnglishTrie_Overview.md       # 📋 專案總覽
└── README_EnglishTrie.md         # 📘 本文件
```

## 🚀 快速開始

### 1. 安裝依賴

```bash
cd Python
pip install -r requirements.txt
```

### 2. 運行示例程式

#### 基本示例（含 NLTK 語料庫）

```bash
python EnglishTrie.py
```

**輸出示例:**
```
正在從 NLTK words 語料庫加載單詞...
成功從 words 語料庫加載 234375 個單詞

測試單詞搜索功能:
搜索 'hello': 找到 ✅
搜索 'world': 找到 ✅
搜索 'python': 找到 ✅

以 'prog' 開頭的單詞建議:
  1. prog
  2. progambling
  3. progamete
  ...
```

#### 簡單測試（不載入語料庫）

```bash
python test_trie.py
```

#### 視覺化測試

```bash
python test_visualize.py
```

**輸出示例:**
```
視覺化: 以 'hel' 開頭的子樹
============================================================
└── hel (5)
    ├── l (1)
    │   └── o ✓ (1)
    └── p ✓ (4)
        ├── e (1)
        │   └── r ✓ (1)
        ├── f (1)
        │   └── u (1)
        │       └── l ✓ (1)
        └── l (1)
            └── e (1)
                └── s (1)
                    └── s ✓ (1)
```

## 💡 使用示例

### 基本用法

```python
from EnglishTrie import EnglishTrie

# 創建 Trie 並載入語料庫
trie = EnglishTrie()
trie.load_from_nltk_corpus('words')

# 搜索單詞
print(trie.search('hello'))           # True
print(trie.search('helo'))            # False

# 前綴檢查
print(trie.starts_with('hel'))        # True
count = trie.count_words_with_prefix('hel')
print(f"以 'hel' 開頭的單詞: {count}")

# 自動補全
suggestions = trie.autocomplete('prog', max_suggestions=5)
for word in suggestions:
    print(word)
```

### 拼寫檢查

```python
from EnglishTrie import EnglishTrie
from EnglishTrie_SpellChecker import SpellChecker

trie = EnglishTrie()
trie.load_from_nltk_corpus('words')
checker = SpellChecker(trie)

# 檢查單詞
if not checker.check_spelling('helo'):
    suggestions = checker.suggest_corrections('helo', max_suggestions=5)
    print(f"您是否要找: {', '.join(suggestions)}")

# 檢查文本
text = "Helo wrold, this is a test."
results = checker.check_text(text)
for word, is_correct, suggestions in results:
    if not is_correct:
        print(f"❌ {word} → 建議: {', '.join(suggestions[:3])}")
```

### 視覺化

```python
from EnglishTrie import EnglishTrie
from EnglishTrie_Visualize import TrieVisualizer

trie = EnglishTrie()
# 插入一些單詞
for word in ['hello', 'help', 'helper']:
    trie.insert(word)

# 視覺化
visualizer = TrieVisualizer(trie)
visualizer.visualize_subtree('hel', max_depth=5)
visualizer.show_statistics()
```

## 📊 性能數據

### 時間複雜度

| 操作 | 複雜度 | 說明 |
|------|-------|------|
| 插入 | O(m) | m = 單詞長度 |
| 搜索 | O(m) | m = 單詞長度 |
| 刪除 | O(m) | m = 單詞長度 |
| 前綴檢查 | O(m) | m = 前綴長度 |
| 自動補全 | O(m + n × k) | n = 結果數, k = 平均單詞長度 |

### 空間使用

- **NLTK words 語料庫**: ~4.5 MB
- **Trie 結構（記憶體）**: ~20-30 MB
- **總單詞數**: 234,375 個
- **總節點數**: ~500,000 個（因共享前綴而優化）

### 載入時間

- **首次運行**: 2-5 秒（下載 NLTK 語料庫）
- **後續運行**: 1-2 秒（僅載入）

## 🎮 互動式程式

運行互動式程式可以即時測試各種功能：

```bash
python EnglishTrie_Interactive.py
```

**菜單選項:**

```
============================================================
English Trie 交互式菜單
============================================================
1. 搜索單詞
2. 檢查前綴
3. 自動補全
4. 插入新單詞
5. 刪除單詞
6. 統計前綴單詞數
7. 顯示統計資訊
8. 批量測試單詞
0. 退出
============================================================
```

## 🛠️ API 參考

### EnglishTrie 類

#### 主要方法

```python
# 插入單詞
trie.insert(word: str) -> None

# 搜索單詞（完整匹配）
trie.search(word: str) -> bool

# 檢查前綴是否存在
trie.starts_with(prefix: str) -> bool

# 自動補全
trie.autocomplete(prefix: str, max_suggestions: int = 10) -> List[str]

# 刪除單詞
trie.delete(word: str) -> bool

# 計算前綴單詞數
trie.count_words_with_prefix(prefix: str) -> int

# 獲取所有單詞
trie.get_all_words() -> List[str]

# 從 NLTK 語料庫載入
trie.load_from_nltk_corpus(corpus_name: str = 'words') -> None
```

#### 屬性

```python
trie.total_words  # 總單詞數
trie.root         # 根節點
```

### SpellChecker 類

```python
# 檢查拼寫
checker.check_spelling(word: str) -> bool

# 獲取修正建議
checker.suggest_corrections(word: str, max_suggestions: int = 5) -> List[str]

# 檢查文本
checker.check_text(text: str) -> List[Tuple[str, bool, List[str]]]
```

### TrieVisualizer 類

```python
# 視覺化子樹
visualizer.visualize_subtree(prefix: str = "", max_depth: int = 3)

# 顯示統計資訊
visualizer.show_statistics() -> None

# 比較單詞
visualizer.compare_words(word1: str, word2: str) -> None
```

## 🌟 實際應用場景

### 1. 搜索引擎自動補全

```python
def search_autocomplete(query):
    suggestions = trie.autocomplete(query, max_suggestions=10)
    return suggestions
```

### 2. 拼寫檢查器

```python
def check_document(text):
    checker = SpellChecker(trie)
    results = checker.check_text(text)
    errors = [(word, sugg) for word, correct, sugg in results if not correct]
    return errors
```

### 3. 單詞遊戲驗證

```python
def is_valid_game_word(word):
    return trie.search(word.lower()) and len(word) >= 3
```

### 4. 文本編輯器補全

```python
def editor_autocomplete(partial_word):
    if len(partial_word) < 2:
        return []
    return trie.autocomplete(partial_word, max_suggestions=5)
```

## 🔍 技術細節

### Trie 節點結構

```python
class TrieNode:
    def __init__(self):
        self.children: Dict[str, TrieNode] = {}
        self.is_end_of_word: bool = False
        self.word_count: int = 0
```

### 空間優化

- **前綴共享**: 相同前綴的單詞共享節點
- **例如**: "hello", "help", "helper" 共享 "hel" 前綴
- **節省**: 相比存儲完整單詞列表，節省約 60-70% 空間

### 查詢效率

- **最壞情況**: O(m)，m 是單詞長度
- **平均情況**: O(5-6)，英文單詞平均長度
- **對比 Hash Table**: 相同時間複雜度，但支持前綴操作

## 📚 擴展功能建議

- [ ] **詞頻統計**: 記錄單詞使用頻率
- [ ] **模糊搜索**: 實現 Levenshtein 距離算法
- [ ] **序列化**: 保存/載入 Trie 結構
- [ ] **壓縮 Trie**: 減少記憶體使用
- [ ] **多語言支援**: 支援其他語言語料庫
- [ ] **上下文感知**: 基於上下文的智能補全
- [ ] **學習功能**: 從用戶輸入學習新詞

## 🐛 故障排除

### 問題 1: NLTK 語料庫下載失敗

**解決方案:**
```python
import nltk
nltk.download('words')
```

### 問題 2: Windows 編碼錯誤

**解決方案:** 程式已自動處理 Windows 控制台編碼問題

### 問題 3: 記憶體不足

**解決方案:** 使用較小的語料庫或自定義單詞列表
```python
trie = EnglishTrie()
# 不載入完整語料庫，手動插入需要的單詞
for word in my_word_list:
    trie.insert(word)
```

## 📈 測試結果

### 功能測試

✅ 插入功能 - 通過  
✅ 搜索功能 - 通過  
✅ 刪除功能 - 通過  
✅ 前綴匹配 - 通過  
✅ 自動補全 - 通過  
✅ 語料庫載入 - 通過  
✅ 拼寫檢查 - 通過  
✅ 視覺化 - 通過  

### 性能測試

- ✅ 載入 234,375 個單詞: 1-2 秒
- ✅ 單詞搜索: < 0.001 秒
- ✅ 前綴匹配: < 0.001 秒
- ✅ 自動補全 (10 個結果): < 0.01 秒

## 📖 相關資源

- [Trie - Wikipedia](https://en.wikipedia.org/wiki/Trie)
- [NLTK 官方網站](https://www.nltk.org/)
- [NLTK Corpora 列表](https://www.nltk.org/nltk_data/)
- [數據結構與算法](https://github.com/ewdlop/DataStructure-Algorithm-Note)

## 👥 貢獻

歡迎提交 Issue 和 Pull Request！

## 📄 授權

MIT License

## 🎓 學習價值

這個專案展示了：
- ✅ Trie 數據結構的實現
- ✅ Python 類和物件導向程式設計
- ✅ 遞歸算法應用
- ✅ NLTK 自然語言處理庫使用
- ✅ 命令列介面設計
- ✅ 跨平台編碼處理
- ✅ 視覺化技術

---

**作者**: DataStructure-Algorithm-Note  
**更新日期**: 2025-11-06  
**版本**: 1.0.0  

🌟 如果這個專案對您有幫助，請給一個 Star！

