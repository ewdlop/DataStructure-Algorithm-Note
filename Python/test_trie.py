"""
簡單的測試程式，演示 EnglishTrie 的功能
"""

from EnglishTrie import EnglishTrie

def main():
    print("正在初始化 English Trie...")
    trie = EnglishTrie()
    
    # 測試不載入完整語料庫的情況
    print("\n測試 1: 手動插入單詞")
    test_words = ['hello', 'help', 'helper', 'world', 'word', 'work', 'worker']
    for word in test_words:
        trie.insert(word)
        print(f"插入: {word}")
    
    print(f"\n總單詞數: {trie.total_words}")
    
    print("\n測試 2: 搜索單詞")
    search_tests = ['hello', 'help', 'hel', 'world', 'xyz']
    for word in search_tests:
        result = trie.search(word)
        print(f"搜索 '{word}': {'✅ 找到' if result else '❌ 未找到'}")
    
    print("\n測試 3: 前綴檢查")
    prefix_tests = ['hel', 'wor', 'xyz']
    for prefix in prefix_tests:
        exists = trie.starts_with(prefix)
        count = trie.count_words_with_prefix(prefix)
        print(f"前綴 '{prefix}': {'存在' if exists else '不存在'} (共 {count} 個單詞)")
    
    print("\n測試 4: 自動補全")
    autocomplete_tests = ['hel', 'wor']
    for prefix in autocomplete_tests:
        suggestions = trie.autocomplete(prefix, max_suggestions=10)
        print(f"'{prefix}' 的建議: {', '.join(suggestions)}")
    
    print("\n測試 5: 刪除單詞")
    trie.delete('helper')
    print("刪除 'helper' 後:")
    print(f"搜索 'helper': {'找到' if trie.search('helper') else '未找到'}")
    print(f"前綴 'hel' 的單詞數: {trie.count_words_with_prefix('hel')}")
    
    print("\n測試完成！")

if __name__ == "__main__":
    main()

