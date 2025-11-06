"""
English Trie 視覺化工具
展示 Trie 的樹狀結構
"""

import sys
import io
from EnglishTrie import EnglishTrie, TrieNode

# 設置 Windows 控制台編碼為 UTF-8
if sys.platform == 'win32':
    try:
        sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
        sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')
    except (AttributeError, ValueError):
        pass  # 已經設置過了


class TrieVisualizer:
    """Trie 視覺化工具類"""
    
    def __init__(self, trie: EnglishTrie):
        self.trie = trie
    
    def visualize_subtree(self, prefix: str = "", max_depth: int = 3, 
                         max_children: int = 5) -> None:
        """
        視覺化顯示 Trie 的子樹結構
        
        Args:
            prefix: 子樹的前綴
            max_depth: 最大顯示深度
            max_children: 每個節點最多顯示的子節點數
        """
        print(f"\n顯示以 '{prefix}' 開頭的 Trie 結構:")
        print("=" * 60)
        
        node = self.trie._find_node(prefix)
        if node is None:
            print(f"❌ 找不到前綴 '{prefix}'")
            return
        
        self._visualize_node(node, prefix, "", True, 0, max_depth, max_children)
    
    def _visualize_node(self, node: TrieNode, word: str, prefix: str, 
                       is_last: bool, depth: int, max_depth: int, 
                       max_children: int) -> None:
        """
        遞歸視覺化節點
        
        Args:
            node: 當前節點
            word: 當前單詞
            prefix: 顯示前綴
            is_last: 是否是最後一個子節點
            depth: 當前深度
            max_depth: 最大深度
            max_children: 最多顯示的子節點數
        """
        # 構建當前行的顯示
        connector = "└── " if is_last else "├── "
        end_marker = " ✓" if node.is_end_of_word else ""
        count_info = f" ({node.word_count})" if hasattr(node, 'word_count') else ""
        
        print(f"{prefix}{connector}{word}{end_marker}{count_info}")
        
        # 如果達到最大深度，停止遞歸
        if depth >= max_depth:
            if node.children:
                next_prefix = prefix + ("    " if is_last else "│   ")
                print(f"{next_prefix}...")
            return
        
        # 獲取子節點
        children = sorted(node.children.items())
        total_children = len(children)
        
        # 限制顯示的子節點數量
        if total_children > max_children:
            children = children[:max_children]
        
        # 遞歸顯示子節點
        for i, (char, child_node) in enumerate(children):
            is_last_child = (i == len(children) - 1) and (total_children <= max_children)
            next_prefix = prefix + ("    " if is_last else "│   ")
            self._visualize_node(child_node, char, next_prefix, is_last_child, 
                               depth + 1, max_depth, max_children)
        
        # 如果有更多子節點未顯示
        if total_children > max_children:
            next_prefix = prefix + ("    " if is_last else "│   ")
            print(f"{next_prefix}... (還有 {total_children - max_children} 個子節點)")
    
    def show_statistics(self) -> None:
        """顯示 Trie 的統計資訊"""
        print("\n" + "=" * 60)
        print("Trie 統計資訊")
        print("=" * 60)
        print(f"總單詞數: {self.trie.total_words:,}")
        
        # 計算樹的深度和節點數
        max_depth, total_nodes = self._calculate_tree_stats(self.trie.root, 0)
        print(f"最大深度: {max_depth}")
        print(f"總節點數: {total_nodes:,}")
        print(f"平均單詞長度: {max_depth / 2:.2f}")
        
        # 顯示前幾個字母的分佈
        print("\n字母分佈 (前 10 個):")
        letter_counts = []
        for letter in 'abcdefghijklmnopqrstuvwxyz':
            count = self.trie.count_words_with_prefix(letter)
            if count > 0:
                letter_counts.append((letter, count))
        
        letter_counts.sort(key=lambda x: x[1], reverse=True)
        for i, (letter, count) in enumerate(letter_counts[:10], 1):
            percentage = (count / self.trie.total_words) * 100
            bar = '█' * int(percentage / 2)
            print(f"{i:2d}. {letter.upper()}: {count:6,} ({percentage:5.2f}%) {bar}")
    
    def _calculate_tree_stats(self, node: TrieNode, depth: int) -> tuple:
        """
        計算樹的統計資訊
        
        Returns:
            (最大深度, 總節點數)
        """
        if not node.children:
            return depth, 1
        
        max_depth = depth
        total_nodes = 1
        
        for child_node in node.children.values():
            child_depth, child_nodes = self._calculate_tree_stats(child_node, depth + 1)
            max_depth = max(max_depth, child_depth)
            total_nodes += child_nodes
        
        return max_depth, total_nodes
    
    def compare_words(self, word1: str, word2: str) -> None:
        """
        比較兩個單詞在 Trie 中的路徑
        
        Args:
            word1: 第一個單詞
            word2: 第二個單詞
        """
        print(f"\n比較單詞 '{word1}' 和 '{word2}':")
        print("=" * 60)
        
        # 找到共同前綴
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
    """主函數"""
    print("=" * 60)
    print("English Trie 視覺化工具")
    print("=" * 60)
    
    # 創建小型示例 Trie
    print("\n創建示例 Trie...")
    trie = EnglishTrie()
    
    # 插入一些示例單詞
    sample_words = [
        'hello', 'help', 'helper', 'helpful', 'helpless',
        'world', 'word', 'work', 'worker', 'working',
        'cat', 'catch', 'car', 'card', 'care',
        'dog', 'door', 'down'
    ]
    
    print(f"插入 {len(sample_words)} 個示例單詞...")
    for word in sample_words:
        trie.insert(word)
    
    # 創建視覺化工具
    visualizer = TrieVisualizer(trie)
    
    # 顯示不同前綴的 Trie 結構
    print("\n" + "=" * 60)
    print("視覺化示例 1: 完整 Trie 結構")
    print("=" * 60)
    visualizer.visualize_subtree("", max_depth=4, max_children=10)
    
    print("\n" + "=" * 60)
    print("視覺化示例 2: 以 'hel' 開頭的子樹")
    print("=" * 60)
    visualizer.visualize_subtree("hel", max_depth=5, max_children=10)
    
    print("\n" + "=" * 60)
    print("視覺化示例 3: 以 'wor' 開頭的子樹")
    print("=" * 60)
    visualizer.visualize_subtree("wor", max_depth=5, max_children=10)
    
    # 顯示統計資訊
    visualizer.show_statistics()
    
    # 比較單詞
    print("\n" + "=" * 60)
    print("單詞比較示例")
    print("=" * 60)
    visualizer.compare_words("hello", "helpful")
    visualizer.compare_words("work", "worker")
    visualizer.compare_words("cat", "dog")
    
    # 載入完整語料庫的選項
    print("\n" + "=" * 60)
    print("載入完整 NLTK 語料庫測試")
    print("=" * 60)
    
    try:
        response = input("\n是否要載入完整的 NLTK words 語料庫？(y/n): ").strip().lower()
    except (EOFError, KeyboardInterrupt):
        response = 'n'
    
    if response == 'y':
        print("\n正在載入 NLTK words 語料庫...")
        full_trie = EnglishTrie()
        full_trie.load_from_nltk_corpus('words')
        
        full_visualizer = TrieVisualizer(full_trie)
        full_visualizer.show_statistics()
        
        # 顯示一些有趣的子樹
        interesting_prefixes = ['prog', 'comp', 'alg', 'data']
        for prefix in interesting_prefixes:
            print(f"\n以 '{prefix}' 開頭的單詞結構:")
            full_visualizer.visualize_subtree(prefix, max_depth=2, max_children=5)
    else:
        print("\n跳過完整語料庫載入。")
    
    print("\n" + "=" * 60)
    print("視覺化演示完成！")
    print("=" * 60)


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\n\n👋 程式被中斷，再見！")
    except Exception as e:
        print(f"\n❌ 發生錯誤: {e}")
        import traceback
        traceback.print_exc()

