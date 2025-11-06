"""
English Trie 實現
使用 NLTK 的英文語料庫構建字典樹
"""

import sys
import io
import nltk
from typing import Dict, List, Optional

# 設置 Windows 控制台編碼為 UTF-8
if sys.platform == 'win32':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')


class TrieNode:
    """Trie 節點類"""
    
    def __init__(self):
        self.children: Dict[str, 'TrieNode'] = {}
        self.is_end_of_word: bool = False
        self.word_count: int = 0  # 記錄有多少個單詞經過這個節點


class EnglishTrie:
    """英文 Trie 字典樹實現"""
    
    def __init__(self):
        self.root = TrieNode()
        self.total_words = 0
    
    def insert(self, word: str) -> None:
        """
        插入一個單詞到 Trie 中
        
        Args:
            word: 要插入的單詞
        """
        if not word:
            return
        
        node = self.root
        word = word.lower()  # 轉換為小寫以標準化
        
        for char in word:
            if char not in node.children:
                node.children[char] = TrieNode()
            node = node.children[char]
            node.word_count += 1
        
        if not node.is_end_of_word:
            node.is_end_of_word = True
            self.total_words += 1
    
    def search(self, word: str) -> bool:
        """
        搜索單詞是否存在於 Trie 中
        
        Args:
            word: 要搜索的單詞
            
        Returns:
            如果單詞存在則返回 True，否則返回 False
        """
        node = self._find_node(word.lower())
        return node is not None and node.is_end_of_word
    
    def starts_with(self, prefix: str) -> bool:
        """
        檢查是否有單詞以給定前綴開頭
        
        Args:
            prefix: 前綴字符串
            
        Returns:
            如果存在以該前綴開頭的單詞則返回 True
        """
        return self._find_node(prefix.lower()) is not None
    
    def _find_node(self, prefix: str) -> Optional[TrieNode]:
        """
        找到對應前綴的節點
        
        Args:
            prefix: 前綴字符串
            
        Returns:
            對應的 TrieNode 或 None
        """
        node = self.root
        for char in prefix:
            if char not in node.children:
                return None
            node = node.children[char]
        return node
    
    def autocomplete(self, prefix: str, max_suggestions: int = 10) -> List[str]:
        """
        根據前綴自動補全
        
        Args:
            prefix: 前綴字符串
            max_suggestions: 最大建議數量
            
        Returns:
            建議的單詞列表
        """
        prefix = prefix.lower()
        node = self._find_node(prefix)
        
        if node is None:
            return []
        
        suggestions = []
        self._collect_words(node, prefix, suggestions, max_suggestions)
        return suggestions
    
    def _collect_words(self, node: TrieNode, current_word: str, 
                      suggestions: List[str], max_suggestions: int) -> None:
        """
        遞歸收集所有以當前前綴開頭的單詞
        
        Args:
            node: 當前節點
            current_word: 當前構建的單詞
            suggestions: 收集的建議列表
            max_suggestions: 最大建議數量
        """
        if len(suggestions) >= max_suggestions:
            return
        
        if node.is_end_of_word:
            suggestions.append(current_word)
        
        for char, child_node in sorted(node.children.items()):
            self._collect_words(child_node, current_word + char, 
                              suggestions, max_suggestions)
    
    def delete(self, word: str) -> bool:
        """
        從 Trie 中刪除一個單詞
        
        Args:
            word: 要刪除的單詞
            
        Returns:
            如果刪除成功返回 True，否則返回 False
        """
        word = word.lower()
        
        def _delete_helper(node: TrieNode, word: str, index: int) -> bool:
            if index == len(word):
                if not node.is_end_of_word:
                    return False
                node.is_end_of_word = False
                self.total_words -= 1
                return len(node.children) == 0
            
            char = word[index]
            if char not in node.children:
                return False
            
            should_delete_child = _delete_helper(node.children[char], word, index + 1)
            
            if should_delete_child:
                del node.children[char]
                return len(node.children) == 0 and not node.is_end_of_word
            
            return False
        
        return _delete_helper(self.root, word, 0)
    
    def count_words_with_prefix(self, prefix: str) -> int:
        """
        計算有多少單詞以給定前綴開頭
        
        Args:
            prefix: 前綴字符串
            
        Returns:
            單詞數量
        """
        node = self._find_node(prefix.lower())
        return node.word_count if node else 0
    
    def get_all_words(self) -> List[str]:
        """
        獲取 Trie 中的所有單詞
        
        Returns:
            所有單詞的列表
        """
        words = []
        self._collect_all_words(self.root, "", words)
        return words
    
    def _collect_all_words(self, node: TrieNode, current_word: str, 
                          words: List[str]) -> None:
        """
        遞歸收集所有單詞
        
        Args:
            node: 當前節點
            current_word: 當前構建的單詞
            words: 收集的單詞列表
        """
        if node.is_end_of_word:
            words.append(current_word)
        
        for char, child_node in sorted(node.children.items()):
            self._collect_all_words(child_node, current_word + char, words)
    
    def load_from_nltk_corpus(self, corpus_name: str = 'words') -> None:
        """
        從 NLTK 語料庫加載單詞
        
        Args:
            corpus_name: NLTK 語料庫名稱，默認為 'words'
        """
        try:
            # 嘗試獲取語料庫
            if corpus_name == 'words':
                from nltk.corpus import words
                word_list = words.words()
            elif corpus_name == 'brown':
                from nltk.corpus import brown
                word_list = set(brown.words())
            elif corpus_name == 'gutenberg':
                from nltk.corpus import gutenberg
                word_list = set(gutenberg.words())
            else:
                print(f"不支持的語料庫: {corpus_name}")
                return
            
            # 插入所有單詞
            for word in word_list:
                if word.isalpha():  # 只插入純字母單詞
                    self.insert(word)
            
            print(f"成功從 {corpus_name} 語料庫加載 {self.total_words} 個單詞")
            
        except LookupError:
            print(f"語料庫 {corpus_name} 未找到，正在下載...")
            nltk.download(corpus_name)
            # 重新嘗試加載
            self.load_from_nltk_corpus(corpus_name)


def main():
    """主函數 - 演示 Trie 的使用"""
    print("=" * 60)
    print("英文 Trie 字典樹示例")
    print("=" * 60)
    
    # 創建 Trie 實例
    trie = EnglishTrie()
    
    # 從 NLTK 語料庫加載單詞
    print("\n正在從 NLTK words 語料庫加載單詞...")
    trie.load_from_nltk_corpus('words')
    
    # 測試搜索功能
    print("\n" + "=" * 60)
    print("測試單詞搜索功能")
    print("=" * 60)
    test_words = ['hello', 'world', 'python', 'programming', 'xyz123']
    for word in test_words:
        result = trie.search(word)
        print(f"搜索 '{word}': {'找到' if result else '未找到'}")
    
    # 測試前綴檢查
    print("\n" + "=" * 60)
    print("測試前綴檢查功能")
    print("=" * 60)
    test_prefixes = ['pro', 'hel', 'xyz', 'comp']
    for prefix in test_prefixes:
        result = trie.starts_with(prefix)
        count = trie.count_words_with_prefix(prefix)
        print(f"前綴 '{prefix}': {'存在' if result else '不存在'} "
              f"(共 {count} 個單詞)")
    
    # 測試自動補全
    print("\n" + "=" * 60)
    print("測試自動補全功能")
    print("=" * 60)
    autocomplete_tests = ['prog', 'hel', 'comp', 'alg']
    for prefix in autocomplete_tests:
        suggestions = trie.autocomplete(prefix, max_suggestions=5)
        print(f"\n以 '{prefix}' 開頭的單詞建議:")
        for i, word in enumerate(suggestions, 1):
            print(f"  {i}. {word}")
    
    # 額外測試：手動插入和刪除
    print("\n" + "=" * 60)
    print("測試手動插入和刪除")
    print("=" * 60)
    custom_word = "datastructure"
    print(f"\n插入自定義單詞: '{custom_word}'")
    trie.insert(custom_word)
    print(f"搜索 '{custom_word}': {'找到' if trie.search(custom_word) else '未找到'}")
    
    print(f"\n刪除單詞: '{custom_word}'")
    trie.delete(custom_word)
    print(f"搜索 '{custom_word}': {'找到' if trie.search(custom_word) else '未找到'}")
    
    # 統計信息
    print("\n" + "=" * 60)
    print("Trie 統計信息")
    print("=" * 60)
    print(f"總單詞數: {trie.total_words}")
    print(f"以 'a' 開頭的單詞數: {trie.count_words_with_prefix('a')}")
    print(f"以 'z' 開頭的單詞數: {trie.count_words_with_prefix('z')}")
    print(f"以 'qu' 開頭的單詞數: {trie.count_words_with_prefix('qu')}")
    
    print("\n" + "=" * 60)
    print("演示完成！")
    print("=" * 60)


if __name__ == "__main__":
    main()

