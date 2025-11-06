"""
English Trie 交互式演示程式
可以讓使用者即時測試 Trie 的各種功能
"""

import sys
import io
from EnglishTrie import EnglishTrie

# 設置 Windows 控制台編碼為 UTF-8
if sys.platform == 'win32':
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8')
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8')


def print_menu():
    """顯示主菜單"""
    print("\n" + "=" * 60)
    print("English Trie 交互式菜單")
    print("=" * 60)
    print("1. 搜索單詞")
    print("2. 檢查前綴")
    print("3. 自動補全")
    print("4. 插入新單詞")
    print("5. 刪除單詞")
    print("6. 統計前綴單詞數")
    print("7. 顯示統計資訊")
    print("8. 批量測試單詞")
    print("0. 退出")
    print("=" * 60)


def search_word(trie: EnglishTrie):
    """搜索單詞"""
    word = input("\n請輸入要搜索的單詞: ").strip()
    if not word:
        print("❌ 輸入為空")
        return
    
    result = trie.search(word)
    if result:
        print(f"✅ 單詞 '{word}' 存在於字典中")
    else:
        print(f"❌ 單詞 '{word}' 不存在於字典中")
        # 提供建議
        suggestions = trie.autocomplete(word[:3], max_suggestions=5)
        if suggestions:
            print(f"\n💡 您是否要找:")
            for i, suggestion in enumerate(suggestions, 1):
                print(f"   {i}. {suggestion}")


def check_prefix(trie: EnglishTrie):
    """檢查前綴"""
    prefix = input("\n請輸入前綴: ").strip()
    if not prefix:
        print("❌ 輸入為空")
        return
    
    exists = trie.starts_with(prefix)
    count = trie.count_words_with_prefix(prefix)
    
    if exists:
        print(f"✅ 存在以 '{prefix}' 開頭的單詞")
        print(f"📊 共有 {count} 個單詞以此前綴開頭")
    else:
        print(f"❌ 不存在以 '{prefix}' 開頭的單詞")


def autocomplete(trie: EnglishTrie):
    """自動補全"""
    prefix = input("\n請輸入前綴: ").strip()
    if not prefix:
        print("❌ 輸入為空")
        return
    
    try:
        max_count = int(input("最多顯示幾個建議 (預設 10): ").strip() or "10")
    except ValueError:
        max_count = 10
    
    suggestions = trie.autocomplete(prefix, max_suggestions=max_count)
    
    if suggestions:
        print(f"\n💡 以 '{prefix}' 開頭的單詞建議:")
        for i, word in enumerate(suggestions, 1):
            print(f"   {i:2d}. {word}")
        print(f"\n共找到 {len(suggestions)} 個建議")
    else:
        print(f"❌ 找不到以 '{prefix}' 開頭的單詞")


def insert_word(trie: EnglishTrie):
    """插入新單詞"""
    word = input("\n請輸入要插入的單詞: ").strip()
    if not word:
        print("❌ 輸入為空")
        return
    
    if not word.isalpha():
        print("❌ 單詞只能包含字母")
        return
    
    if trie.search(word):
        print(f"⚠️  單詞 '{word}' 已經存在於字典中")
    else:
        trie.insert(word)
        print(f"✅ 成功插入單詞 '{word}'")


def delete_word(trie: EnglishTrie):
    """刪除單詞"""
    word = input("\n請輸入要刪除的單詞: ").strip()
    if not word:
        print("❌ 輸入為空")
        return
    
    if trie.delete(word):
        print(f"✅ 成功刪除單詞 '{word}'")
    else:
        print(f"❌ 單詞 '{word}' 不存在，無法刪除")


def count_prefix_words(trie: EnglishTrie):
    """統計前綴單詞數"""
    prefix = input("\n請輸入前綴: ").strip()
    if not prefix:
        print("❌ 輸入為空")
        return
    
    count = trie.count_words_with_prefix(prefix)
    print(f"📊 共有 {count} 個單詞以 '{prefix}' 開頭")


def show_statistics(trie: EnglishTrie):
    """顯示統計資訊"""
    print("\n" + "=" * 60)
    print("Trie 統計資訊")
    print("=" * 60)
    print(f"總單詞數: {trie.total_words:,}")
    
    # 顯示各字母開頭的單詞數量
    print("\n各字母開頭的單詞分佈:")
    for letter in 'abcdefghijklmnopqrstuvwxyz':
        count = trie.count_words_with_prefix(letter)
        bar = '█' * (count // 1000)  # 每 1000 個單詞顯示一個方塊
        print(f"  {letter.upper()}: {count:6,} {bar}")


def batch_test(trie: EnglishTrie):
    """批量測試單詞"""
    print("\n請輸入要測試的單詞，用空格或逗號分隔:")
    input_text = input().strip()
    
    if not input_text:
        print("❌ 輸入為空")
        return
    
    # 分割單詞
    words = input_text.replace(',', ' ').split()
    
    print("\n" + "=" * 60)
    print("批量測試結果")
    print("=" * 60)
    
    found = 0
    not_found = 0
    
    for word in words:
        word = word.strip()
        if not word:
            continue
        
        result = trie.search(word)
        status = "✅ 找到" if result else "❌ 未找到"
        print(f"{status:12} : {word}")
        
        if result:
            found += 1
        else:
            not_found += 1
    
    print("=" * 60)
    print(f"統計: 找到 {found} 個，未找到 {not_found} 個")


def main():
    """主程式"""
    print("\n" + "=" * 60)
    print("歡迎使用 English Trie 交互式演示程式")
    print("=" * 60)
    
    # 初始化 Trie
    print("\n正在初始化 Trie 並載入 NLTK 語料庫...")
    trie = EnglishTrie()
    trie.load_from_nltk_corpus('words')
    print(f"✅ 成功載入 {trie.total_words:,} 個單詞")
    
    # 主循環
    while True:
        print_menu()
        choice = input("\n請選擇功能 (0-8): ").strip()
        
        if choice == '1':
            search_word(trie)
        elif choice == '2':
            check_prefix(trie)
        elif choice == '3':
            autocomplete(trie)
        elif choice == '4':
            insert_word(trie)
        elif choice == '5':
            delete_word(trie)
        elif choice == '6':
            count_prefix_words(trie)
        elif choice == '7':
            show_statistics(trie)
        elif choice == '8':
            batch_test(trie)
        elif choice == '0':
            print("\n👋 感謝使用，再見！")
            break
        else:
            print("❌ 無效的選擇，請重新輸入")
        
        input("\n按 Enter 繼續...")


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\n\n👋 程式被中斷，再見！")
    except Exception as e:
        print(f"\n❌ 發生錯誤: {e}")

