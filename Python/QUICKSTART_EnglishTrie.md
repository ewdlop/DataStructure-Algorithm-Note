# 🚀 English Trie 快速開始指南

## 📦 安裝（僅一個命令）

```bash
pip install nltk
```

## ⚡ 5 分鐘快速上手

### 1️⃣ 最簡單的使用方式

```python
from EnglishTrie import EnglishTrie

# 創建並載入
trie = EnglishTrie()
trie.load_from_nltk_corpus('words')  # 自動下載並載入 234,375 個單詞

# 搜索
print(trie.search('hello'))  # True

# 自動補全
print(trie.autocomplete('prog'))  # ['prog', 'progambling', ...]
```

### 2️⃣ 運行示例程式

```bash
# 完整示例（含視覺化和測試）
python EnglishTrie.py

# 簡單測試（快速）
python test_trie.py

# 視覺化測試
python test_visualize.py
```

## 📝 常用功能速查

### 搜索單詞

```python
trie.search('hello')  # True
trie.search('helo')   # False
```

### 前綴檢查

```python
trie.starts_with('hel')  # True
trie.count_words_with_prefix('hel')  # 314
```

### 自動補全

```python
suggestions = trie.autocomplete('prog', max_suggestions=5)
# ['prog', 'progambling', 'progamete', 'progamic', 'proganosaur']
```

### 插入和刪除

```python
trie.insert('myword')
trie.delete('myword')
```

### 拼寫檢查

```python
from EnglishTrie_SpellChecker import SpellChecker

checker = SpellChecker(trie)
is_correct = checker.check_spelling('hello')  # True
suggestions = checker.suggest_corrections('helo')  # ['hello', 'halo', ...]
```

## 🎯 核心文件

| 文件 | 用途 | 運行方式 |
|------|------|---------|
| `EnglishTrie.py` | 核心實現 + 示例 | `python EnglishTrie.py` |
| `test_trie.py` | 快速測試 | `python test_trie.py` |
| `test_visualize.py` | 視覺化測試 | `python test_visualize.py` |

## 📊 性能

- **載入時間**: 1-2 秒
- **搜索速度**: < 0.001 秒/單詞
- **記憶體使用**: ~25 MB
- **單詞數量**: 234,375 個

## 🔧 進階使用

### 使用不同語料庫

```python
trie.load_from_nltk_corpus('brown')      # 更口語化的單詞
trie.load_from_nltk_corpus('gutenberg')  # 文學作品單詞
```

### 自定義單詞列表

```python
trie = EnglishTrie()
for word in ['apple', 'banana', 'cherry']:
    trie.insert(word)
```

### 統計資訊

```python
print(f"總單詞數: {trie.total_words}")
print(f"以 'a' 開頭的單詞: {trie.count_words_with_prefix('a')}")
```

## ❓ 常見問題

**Q: 首次運行需要網路嗎？**  
A: 是的，需要下載 NLTK 語料庫（約 4.5 MB），之後就不需要了。

**Q: Windows 上中文顯示亂碼？**  
A: 程式已自動處理編碼問題，應該可以正常顯示。

**Q: 如何節省記憶體？**  
A: 不載入完整語料庫，只插入需要的單詞。

**Q: 支援中文嗎？**  
A: 目前僅支援英文，但可以輕鬆擴展到其他語言。

## 📚 完整文檔

- 📖 [完整 README](README_EnglishTrie.md)
- 📋 [專案總覽](EnglishTrie_Overview.md)
- 📘 [詳細文檔](EnglishTrie_README.md)

## 🎓 實際應用示例

### 搜索建議系統

```python
def search_suggestions(user_input):
    return trie.autocomplete(user_input, max_suggestions=10)
```

### 拼寫檢查器

```python
def check_text(text):
    checker = SpellChecker(trie)
    return checker.check_text(text)
```

### 單詞遊戲驗證

```python
def is_valid_word(word):
    return trie.search(word.lower())
```

## 🎉 完成！

現在您已經可以開始使用 English Trie 了！

如有問題，請查看 [完整 README](README_EnglishTrie.md) 或提交 Issue。

---

**提示**: 運行 `python EnglishTrie.py` 可以看到完整的功能演示！

