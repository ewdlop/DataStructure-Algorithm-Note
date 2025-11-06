"""
測試視覺化功能（不需要用戶輸入）
"""

import sys
import os

# 添加當前目錄到路徑
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

# 確保只設置一次編碼
if not hasattr(sys.stdout, '_wrapped'):
    import io
    if sys.platform == 'win32':
        try:
            sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
            sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')
            sys.stdout._wrapped = True
        except (AttributeError, ValueError):
            pass

from EnglishTrie import EnglishTrie, TrieNode


class TrieVisualizer:
    """Trie 視覺化工具類"""
    
    def __init__(self, trie):
        self.trie = trie
    
    def visualize_subtree(self, prefix="", max_depth=3, max_children=5):
        print(f"\n顯示以 '{prefix}' 開頭的 Trie 結構:")
        print("=" * 60)
        
        node = self.trie._find_node(prefix)
        if node is None:
            print(f"❌ 找不到前綴 '{prefix}'")
            return
        
        self._visualize_node(node, prefix, "", True, 0, max_depth, max_children)
    
    def _visualize_node(self, node, word, prefix, is_last, depth, max_depth, max_children):
        connector = "└── " if is_last else "├── "
        end_marker = " ✓" if node.is_end_of_word else ""
        count_info = f" ({node.word_count})" if hasattr(node, 'word_count') else ""
        
        print(f"{prefix}{connector}{word}{end_marker}{count_info}")
        
        if depth >= max_depth:
            if node.children:
                next_prefix = prefix + ("    " if is_last else "│   ")
                print(f"{next_prefix}...")
            return
        
        children = sorted(node.children.items())
        total_children = len(children)
        
        if total_children > max_children:
            children = children[:max_children]
        
        for i, (char, child_node) in enumerate(children):
            is_last_child = (i == len(children) - 1) and (total_children <= max_children)
            next_prefix = prefix + ("    " if is_last else "│   ")
            self._visualize_node(child_node, char, next_prefix, is_last_child, 
                               depth + 1, max_depth, max_children)
        
        if total_children > max_children:
            next_prefix = prefix + ("    " if is_last else "│   ")
            print(f"{next_prefix}... (還有 {total_children - max_children} 個子節點)")
    
    def show_statistics(self):
        print("\n" + "=" * 60)
        print("Trie 統計資訊")
        print("=" * 60)
        print(f"總單詞數: {self.trie.total_words:,}")
        
        max_depth, total_nodes = self._calculate_tree_stats(self.trie.root, 0)
        print(f"最大深度: {max_depth}")
        print(f"總節點數: {total_nodes:,}")
    
    def _calculate_tree_stats(self, node, depth):
        if not node.children:
            return depth, 1
        
        max_depth = depth
        total_nodes = 1
        
        for child_node in node.children.values():
            child_depth, child_nodes = self._calculate_tree_stats(child_node, depth + 1)
            max_depth = max(max_depth, child_depth)
            total_nodes += child_nodes
        
        return max_depth, total_nodes
    
    def compare_words(self, word1, word2):
        print(f"\n比較單詞 '{word1}' 和 '{word2}':")
        print("=" * 60)
        
        common_prefix = ""
        for c1, c2 in zip(word1, word2):
            if c1 == c2:
                common_prefix += c1
            else:
                break
        
        print(f"共同前綴: '{common_prefix}' (長度 {len(common_prefix)})")
        print(f"'{word1}' 在 Trie 中: {'存在 ✓' if self.trie.search(word1) else '不存在 ✗'}")
        print(f"'{word2}' 在 Trie 中: {'存在 ✓' if self.trie.search(word2) else '不存在 ✗'}")
        
        if common_prefix:
            count = self.trie.count_words_with_prefix(common_prefix)
            print(f"共享前綴 '{common_prefix}' 的單詞數: {count}")

def main():
    print("=" * 60)
    print("English Trie 視覺化測試")
    print("=" * 60)
    
    # 創建示例 Trie
    print("\n創建示例 Trie...")
    trie = EnglishTrie()
    
    # 插入示例單詞
    sample_words = [
        'hello', 'help', 'helper', 'helpful', 'helpless',
        'world', 'word', 'work', 'worker', 'working',
        'cat', 'catch', 'car', 'card', 'care',
        'dog', 'door', 'down'
    ]
    
    for word in sample_words:
        trie.insert(word)
    print(f"✓ 已插入 {len(sample_words)} 個單詞")
    
    # 創建視覺化工具
    visualizer = TrieVisualizer(trie)
    
    # 顯示完整結構
    print("\n視覺化: 完整 Trie 結構")
    visualizer.visualize_subtree("", max_depth=4, max_children=10)
    
    # 顯示特定前綴
    print("\n視覺化: 以 'hel' 開頭的子樹")
    visualizer.visualize_subtree("hel", max_depth=5, max_children=10)
    
    print("\n視覺化: 以 'wor' 開頭的子樹")
    visualizer.visualize_subtree("wor", max_depth=5, max_children=10)
    
    # 顯示統計資訊
    visualizer.show_statistics()
    
    # 比較單詞
    print("\n單詞比較:")
    visualizer.compare_words("hello", "helpful")
    visualizer.compare_words("work", "worker")
    
    print("\n" + "=" * 60)
    print("測試完成！")
    print("=" * 60)

if __name__ == "__main__":
    main()

