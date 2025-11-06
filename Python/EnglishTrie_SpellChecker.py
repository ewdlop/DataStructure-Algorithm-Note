"""
使用 English Trie 實現簡單的拼寫檢查器
支持編輯距離計算和拼寫建議
"""

import sys
import io
from typing import List, Tuple
from EnglishTrie import EnglishTrie

# 設置 Windows 控制台編碼為 UTF-8
if sys.platform == 'win32':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')


class SpellChecker:
    """基於 Trie 的拼寫檢查器"""
    
    def __init__(self, trie: EnglishTrie):
        self.trie = trie
    
    def check_spelling(self, word: str) -> bool:
        """
        檢查單詞拼寫是否正確
        
        Args:
            word: 要檢查的單詞
            
        Returns:
            True 如果拼寫正確，False 否則
        """
        return self.trie.search(word.lower())
    
    def suggest_corrections(self, word: str, max_suggestions: int = 5) -> List[str]:
        """
        為拼寫錯誤的單詞提供修正建議
        
        Args:
            word: 拼寫可能錯誤的單詞
            max_suggestions: 最大建議數量
            
        Returns:
            建議的正確拼寫列表
        """
        word = word.lower()
        suggestions = []
        
        # 策略 1: 前綴匹配（可能是未完成輸入）
        prefix_suggestions = self.trie.autocomplete(word, max_suggestions=max_suggestions)
        suggestions.extend(prefix_suggestions)
        
        # 策略 2: 單字符錯誤（少一個字符）
        if len(suggestions) < max_suggestions:
            for i in range(len(word)):
                candidate = word[:i] + word[i+1:]
                if self.trie.search(candidate) and candidate not in suggestions:
                    suggestions.append(candidate)
                    if len(suggestions) >= max_suggestions:
                        break
        
        # 策略 3: 單字符錯誤（多一個字符）
        if len(suggestions) < max_suggestions:
            for i in range(len(word) + 1):
                for c in 'abcdefghijklmnopqrstuvwxyz':
                    candidate = word[:i] + c + word[i:]
                    if self.trie.search(candidate) and candidate not in suggestions:
                        suggestions.append(candidate)
                        if len(suggestions) >= max_suggestions:
                            break
                if len(suggestions) >= max_suggestions:
                    break
        
        # 策略 4: 單字符替換
        if len(suggestions) < max_suggestions:
            for i in range(len(word)):
                for c in 'abcdefghijklmnopqrstuvwxyz':
                    if c != word[i]:
                        candidate = word[:i] + c + word[i+1:]
                        if self.trie.search(candidate) and candidate not in suggestions:
                            suggestions.append(candidate)
                            if len(suggestions) >= max_suggestions:
                                break
                if len(suggestions) >= max_suggestions:
                    break
        
        # 策略 5: 相鄰字符交換
        if len(suggestions) < max_suggestions:
            for i in range(len(word) - 1):
                candidate = word[:i] + word[i+1] + word[i] + word[i+2:]
                if self.trie.search(candidate) and candidate not in suggestions:
                    suggestions.append(candidate)
                    if len(suggestions) >= max_suggestions:
                        break
        
        return suggestions[:max_suggestions]
    
    def check_text(self, text: str) -> List[Tuple[str, bool, List[str]]]:
        """
        檢查一段文本中的所有單詞
        
        Args:
            text: 要檢查的文本
            
        Returns:
            列表，每個元素為 (單詞, 是否正確, 建議列表)
        """
        # 簡單的單詞分割（實際應用中可能需要更複雜的處理）
        words = text.split()
        results = []
        
        for word in words:
            # 移除標點符號
            clean_word = ''.join(c for c in word if c.isalpha())
            if not clean_word:
                continue
            
            is_correct = self.check_spelling(clean_word)
            suggestions = [] if is_correct else self.suggest_corrections(clean_word)
            
            results.append((clean_word, is_correct, suggestions))
        
        return results


def demonstrate_spell_checker():
    """演示拼寫檢查器的功能"""
    print("\n" + "=" * 60)
    print("英文拼寫檢查器演示")
    print("=" * 60)
    
    # 初始化 Trie 和拼寫檢查器
    print("\n正在初始化字典...")
    trie = EnglishTrie()
    trie.load_from_nltk_corpus('words')
    checker = SpellChecker(trie)
    print(f"✅ 字典載入完成，共 {trie.total_words:,} 個單詞")
    
    # 測試單個單詞
    print("\n" + "=" * 60)
    print("測試 1: 單個單詞拼寫檢查")
    print("=" * 60)
    
    test_words = [
        'hello',      # 正確
        'helo',       # 錯誤（少一個 l）
        'wrold',      # 錯誤（world 的錯誤拼寫）
        'programing', # 錯誤（programming 的錯誤拼寫）
        'recieve',    # 錯誤（receive 的常見錯誤）
        'python',     # 正確
        'teh',        # 錯誤（the 的常見錯誤）
    ]
    
    for word in test_words:
        is_correct = checker.check_spelling(word)
        print(f"\n單詞: '{word}'")
        
        if is_correct:
            print("  ✅ 拼寫正確")
        else:
            print("  ❌ 拼寫錯誤")
            suggestions = checker.suggest_corrections(word, max_suggestions=5)
            if suggestions:
                print("  💡 建議修正:")
                for i, suggestion in enumerate(suggestions, 1):
                    print(f"     {i}. {suggestion}")
    
    # 測試整段文本
    print("\n" + "=" * 60)
    print("測試 2: 文本拼寫檢查")
    print("=" * 60)
    
    test_text = "Helo wrold, this is a smple test of teh speling cheker."
    print(f"\n原文: {test_text}")
    print("\n檢查結果:")
    
    results = checker.check_text(test_text)
    error_count = 0
    
    for word, is_correct, suggestions in results:
        if is_correct:
            print(f"  ✅ {word}")
        else:
            error_count += 1
            print(f"  ❌ {word}")
            if suggestions:
                print(f"     建議: {', '.join(suggestions[:3])}")
    
    print(f"\n統計: 共檢查 {len(results)} 個單詞，發現 {error_count} 個錯誤")
    
    # 互動模式
    print("\n" + "=" * 60)
    print("互動模式")
    print("=" * 60)
    print("您可以輸入單詞或句子進行拼寫檢查")
    print("輸入 'quit' 或 'exit' 退出\n")
    
    while True:
        try:
            user_input = input("請輸入要檢查的文本: ").strip()
            
            if user_input.lower() in ['quit', 'exit', '']:
                print("\n👋 再見！")
                break
            
            # 判斷是單個單詞還是多個單詞
            words = user_input.split()
            
            if len(words) == 1:
                # 單個單詞檢查
                word = words[0]
                is_correct = checker.check_spelling(word)
                
                if is_correct:
                    print(f"✅ '{word}' 拼寫正確\n")
                else:
                    print(f"❌ '{word}' 拼寫錯誤")
                    suggestions = checker.suggest_corrections(word, max_suggestions=5)
                    if suggestions:
                        print("💡 建議修正:")
                        for i, suggestion in enumerate(suggestions, 1):
                            print(f"   {i}. {suggestion}")
                    print()
            else:
                # 多個單詞檢查
                results = checker.check_text(user_input)
                correct_count = sum(1 for _, is_correct, _ in results if is_correct)
                error_count = len(results) - correct_count
                
                print(f"\n檢查結果 (✅ {correct_count} 正確 / ❌ {error_count} 錯誤):")
                for word, is_correct, suggestions in results:
                    if is_correct:
                        print(f"  ✅ {word}")
                    else:
                        print(f"  ❌ {word} → 建議: {', '.join(suggestions[:3])}")
                print()
        
        except KeyboardInterrupt:
            print("\n\n👋 程式被中斷，再見！")
            break
        except Exception as e:
            print(f"\n❌ 發生錯誤: {e}\n")


def main():
    """主函數"""
    try:
        demonstrate_spell_checker()
    except KeyboardInterrupt:
        print("\n\n👋 程式被中斷，再見！")
    except Exception as e:
        print(f"\n❌ 發生錯誤: {e}")


if __name__ == "__main__":
    main()

