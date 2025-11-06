# English Trie 專案總覽

## 📁 專案結構

```
Python/
├── EnglishTrie.py                    # 核心 Trie 實現
├── EnglishTrie_Interactive.py        # 互動式演示程式
├── EnglishTrie_SpellChecker.py       # 拼寫檢查器應用
├── test_trie.py                      # 簡單測試程式
├── requirements.txt                  # Python 依賴項
├── EnglishTrie_README.md            # 詳細使用文檔
└── EnglishTrie_Overview.md          # 本文件
```

## 📚 文件說明

### 1. EnglishTrie.py
**核心 Trie 數據結構實現**

- ✅ 完整的 Trie 類實現
- ✅ 從 NLTK 語料庫自動加載 234,375+ 英文單詞
- ✅ 支持插入、搜索、刪除、前綴匹配等基本操作
- ✅ 提供自動補全功能
- ✅ 內建完整的示例和測試

**主要類:**
- `TrieNode`: Trie 節點類
- `EnglishTrie`: Trie 主類

**使用方法:**
```python
from EnglishTrie import EnglishTrie

trie = EnglishTrie()
trie.load_from_nltk_corpus('words')
print(trie.search('hello'))  # True
suggestions = trie.autocomplete('prog', max_suggestions=5)
```

**運行示例:**
```bash
python EnglishTrie.py
```

### 2. EnglishTrie_Interactive.py
**互動式命令列程式**

提供友好的互動式界面，讓使用者可以：
- 🔍 搜索單詞
- 📝 檢查前綴
- 💡 測試自動補全
- ➕ 插入新單詞
- ➖ 刪除單詞
- 📊 查看統計資訊
- 🔄 批量測試單詞

**運行方法:**
```bash
python EnglishTrie_Interactive.py
```

**功能菜單:**
```
1. 搜索單詞
2. 檢查前綴
3. 自動補全
4. 插入新單詞
5. 刪除單詞
6. 統計前綴單詞數
7. 顯示統計資訊
8. 批量測試單詞
0. 退出
```

### 3. EnglishTrie_SpellChecker.py
**拼寫檢查器應用**

基於 Trie 實現的智能拼寫檢查器，支持:
- ✅ 單詞拼寫驗證
- ✅ 拼寫錯誤修正建議
- ✅ 整段文本檢查
- ✅ 多種錯誤檢測策略

**錯誤檢測策略:**
1. 前綴匹配（未完成輸入）
2. 單字符缺失
3. 單字符多餘
4. 單字符替換
5. 相鄰字符交換

**運行方法:**
```bash
python EnglishTrie_SpellChecker.py
```

**使用示例:**
```python
from EnglishTrie_SpellChecker import SpellChecker
from EnglishTrie import EnglishTrie

trie = EnglishTrie()
trie.load_from_nltk_corpus('words')
checker = SpellChecker(trie)

# 檢查單詞
print(checker.check_spelling('hello'))  # True
print(checker.check_spelling('helo'))   # False

# 獲取建議
suggestions = checker.suggest_corrections('helo')
print(suggestions)  # ['hello', 'halo', 'help', ...]
```

### 4. test_trie.py
**簡單測試程式**

快速測試 Trie 基本功能的小程式，不需要載入完整語料庫。

**測試內容:**
- 手動插入單詞
- 搜索功能
- 前綴檢查
- 自動補全
- 刪除操作

**運行方法:**
```bash
python test_trie.py
```

## 🚀 快速開始

### 安裝依賴

```bash
pip install -r requirements.txt
```

或

```bash
pip install nltk
```

### 基本使用流程

1. **載入語料庫並創建 Trie**
```python
from EnglishTrie import EnglishTrie

trie = EnglishTrie()
trie.load_from_nltk_corpus('words')
print(f"已載入 {trie.total_words} 個單詞")
```

2. **搜索和前綴匹配**
```python
# 搜索完整單詞
if trie.search('hello'):
    print("單詞存在")

# 檢查前綴
if trie.starts_with('hel'):
    count = trie.count_words_with_prefix('hel')
    print(f"有 {count} 個單詞以 'hel' 開頭")
```

3. **自動補全**
```python
suggestions = trie.autocomplete('prog', max_suggestions=5)
for word in suggestions:
    print(word)
```

4. **修改 Trie**
```python
# 插入新單詞
trie.insert('myword')

# 刪除單詞
trie.delete('myword')
```

## 📊 性能特性

### 時間複雜度
| 操作 | 複雜度 | 說明 |
|------|--------|------|
| 插入 | O(m) | m = 單詞長度 |
| 搜索 | O(m) | m = 單詞長度 |
| 刪除 | O(m) | m = 單詞長度 |
| 前綴匹配 | O(m) | m = 前綴長度 |
| 自動補全 | O(m + n) | m = 前綴長度, n = 結果數 |

### 空間使用
- **原始資料**: NLTK words 語料庫約 4.5 MB
- **Trie 結構**: 約 20-30 MB (因共享前綴而優化)
- **總單詞數**: 234,375+ 個英文單詞

### 載入時間
- **首次載入**: 約 2-5 秒（下載語料庫）
- **後續載入**: 約 1-2 秒

## 💡 應用場景

### 1. 搜索引擎
```python
# 實時搜索建議
user_input = "prog"
suggestions = trie.autocomplete(user_input, max_suggestions=10)
```

### 2. 拼寫檢查
```python
from EnglishTrie_SpellChecker import SpellChecker

checker = SpellChecker(trie)
text = "Helo wrold"
results = checker.check_text(text)
```

### 3. 文字遊戲
```python
# 檢查玩家輸入的單詞是否有效
def is_valid_word(word):
    return trie.search(word.lower())
```

### 4. 自動完成
```python
# 文本編輯器的自動完成功能
def get_completions(partial_word):
    return trie.autocomplete(partial_word, max_suggestions=10)
```

## 🔧 進階功能

### 使用不同的語料庫

```python
# 使用 Brown 語料庫
trie.load_from_nltk_corpus('brown')

# 使用 Gutenberg 語料庫
trie.load_from_nltk_corpus('gutenberg')
```

### 獲取統計資訊

```python
# 總單詞數
print(f"總單詞數: {trie.total_words}")

# 各字母開頭的單詞數
for letter in 'abcdefghijklmnopqrstuvwxyz':
    count = trie.count_words_with_prefix(letter)
    print(f"{letter}: {count} 個單詞")
```

### 自定義字典

```python
# 創建自定義專業術語字典
tech_trie = EnglishTrie()
tech_words = ['algorithm', 'datastructure', 'python', 'javascript']
for word in tech_words:
    tech_trie.insert(word)
```

## 🐛 已知問題與限制

1. **記憶體使用**: 載入完整語料庫會佔用約 20-30 MB 記憶體
2. **首次運行**: 需要下載 NLTK 語料庫（約 4.5 MB）
3. **大小寫**: 所有單詞會轉換為小寫儲存
4. **特殊字符**: 目前只支持純字母單詞

## 🔮 未來改進方向

- [ ] 添加詞頻統計
- [ ] 實現模糊搜索（Levenshtein 距離）
- [ ] 支援序列化/反序列化（保存和載入 Trie）
- [ ] 添加單詞定義和例句
- [ ] 支援多語言
- [ ] 優化記憶體使用（壓縮 Trie）
- [ ] 添加並行處理支援

## 📖 參考資料

### Trie 數據結構
- [Wikipedia - Trie](https://en.wikipedia.org/wiki/Trie)
- [GeeksforGeeks - Trie](https://www.geeksforgeeks.org/trie-insert-and-search/)

### NLTK
- [NLTK 官方文檔](https://www.nltk.org/)
- [NLTK Corpora](https://www.nltk.org/nltk_data/)

## 📝 測試結果示例

### 成功載入語料庫
```
正在從 NLTK words 語料庫加載單詞...
成功從 words 語料庫加載 234375 個單詞
```

### 搜索測試
```
搜索 'hello': 找到
搜索 'world': 找到
搜索 'python': 找到
搜索 'programming': 未找到
```

### 前綴統計
```
前綴 'pro': 存在 (共 2451 個單詞)
前綴 'hel': 存在 (共 314 個單詞)
```

### 自動補全示例
```
以 'prog' 開頭的單詞建議:
  1. prog
  2. progambling
  3. progamete
  4. progamic
  5. proganosaur
```

## 🎯 結論

這個 English Trie 專案提供了完整的 Trie 數據結構實現，並整合了 NLTK 英文語料庫，可以用於各種實際應用場景。代碼清晰、功能完整、性能優秀，適合學習和實際使用。

---

**作者**: DataStructure-Algorithm-Note  
**日期**: 2025-11-06  
**版本**: 1.0  
**授權**: MIT License

