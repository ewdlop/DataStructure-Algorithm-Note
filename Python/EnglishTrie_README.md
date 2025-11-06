# English Trie 英文字典樹

使用 NLTK 英文語料庫構建的高效 Trie（字典樹）數據結構實現。

## 功能特點

- ✅ 從 NLTK words 語料庫自動加載 234,375+ 個英文單詞
- ✅ 快速單詞搜索（O(m) 時間複雜度，m 為單詞長度）
- ✅ 前綴檢查和匹配
- ✅ 自動補全功能
- ✅ 單詞插入和刪除
- ✅ 統計功能（計算特定前綴的單詞數量）
- ✅ 支持 Windows/Linux/Mac 跨平台

## 安裝依賴

```bash
pip install -r requirements.txt
```

或直接安裝：

```bash
pip install nltk
```

## 快速開始

### 基本使用

```python
from EnglishTrie import EnglishTrie

# 創建 Trie 實例
trie = EnglishTrie()

# 從 NLTK 語料庫加載單詞
trie.load_from_nltk_corpus('words')

# 搜索單詞
print(trie.search('hello'))  # True
print(trie.search('xyz'))    # False

# 檢查前綴
print(trie.starts_with('hel'))  # True

# 自動補全
suggestions = trie.autocomplete('prog', max_suggestions=5)
print(suggestions)  # ['prog', 'progambling', 'progamete', ...]
```

### 高級用法

```python
# 計算以特定前綴開頭的單詞數量
count = trie.count_words_with_prefix('pro')
print(f"以 'pro' 開頭的單詞數: {count}")

# 手動插入自定義單詞
trie.insert('myCustomWord')

# 刪除單詞
trie.delete('myCustomWord')

# 獲取所有單詞（謹慎使用，單詞量大時可能較慢）
# all_words = trie.get_all_words()
```

### 使用不同的 NLTK 語料庫

```python
# 使用 Brown 語料庫
trie.load_from_nltk_corpus('brown')

# 使用 Gutenberg 語料庫
trie.load_from_nltk_corpus('gutenberg')
```

## API 參考

### EnglishTrie 類

#### 方法

- `insert(word: str) -> None`
  - 插入一個單詞到 Trie 中
  
- `search(word: str) -> bool`
  - 搜索單詞是否存在，存在返回 True

- `starts_with(prefix: str) -> bool`
  - 檢查是否有單詞以給定前綴開頭

- `autocomplete(prefix: str, max_suggestions: int = 10) -> List[str]`
  - 根據前綴返回自動補全建議

- `delete(word: str) -> bool`
  - 刪除一個單詞，成功返回 True

- `count_words_with_prefix(prefix: str) -> int`
  - 計算有多少單詞以給定前綴開頭

- `get_all_words() -> List[str]`
  - 獲取 Trie 中的所有單詞（按字母順序）

- `load_from_nltk_corpus(corpus_name: str = 'words') -> None`
  - 從 NLTK 語料庫加載單詞

#### 屬性

- `total_words: int`
  - Trie 中的總單詞數

## 時間複雜度

| 操作 | 時間複雜度 | 說明 |
|------|-----------|------|
| 插入 | O(m) | m 為單詞長度 |
| 搜索 | O(m) | m 為單詞長度 |
| 前綴檢查 | O(m) | m 為前綴長度 |
| 刪除 | O(m) | m 為單詞長度 |
| 自動補全 | O(m + n) | m 為前綴長度，n 為建議數量 |

## 空間複雜度

O(ALPHABET_SIZE × N × M)
- ALPHABET_SIZE：字母表大小（英文為 26）
- N：單詞數量
- M：平均單詞長度

實際空間使用會因共享前綴而優化。

## 運行示例

```bash
cd Python
python EnglishTrie.py
```

## 輸出示例

```
============================================================
英文 Trie 字典樹示例
============================================================

正在從 NLTK words 語料庫加載單詞...
成功從 words 語料庫加載 234375 個單詞

============================================================
測試單詞搜索功能
============================================================
搜索 'hello': 找到
搜索 'world': 找到
搜索 'python': 找到

============================================================
測試前綴檢查功能
============================================================
前綴 'pro': 存在 (共 2451 個單詞)
前綴 'hel': 存在 (共 314 個單詞)

============================================================
測試自動補全功能
============================================================

以 'prog' 開頭的單詞建議:
  1. prog
  2. progambling
  3. progamete
  4. progamic
  5. proganosaur
...
```

## 應用場景

1. **搜索引擎**：實現搜索建議和自動補全
2. **拼寫檢查**：快速驗證單詞拼寫
3. **文本編輯器**：提供智能補全功能
4. **遊戲開發**：單詞遊戲（如拼字遊戲、填字遊戲）
5. **自然語言處理**：詞典查詢和文本分析

## 注意事項

1. 首次運行時會自動下載 NLTK 語料庫
2. NLTK words 語料庫包含 234,375+ 個英文單詞
3. 所有單詞會自動轉換為小寫以標準化
4. Windows 用戶：程式已處理控制台編碼問題

## 擴展建議

- 添加詞頻統計功能
- 實現模糊搜索（編輯距離）
- 添加單詞定義和例句
- 支持多語言
- 實現序列化/反序列化（保存和加載 Trie）

## 授權

MIT License

## 貢獻

歡迎提交 Issue 和 Pull Request！

