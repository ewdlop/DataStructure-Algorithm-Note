from lxml import html, etree
from itertools import zip_longest

def merge_elements(a: etree._Element, b: etree._Element):
    """
    Lazy-merge 兩個 <Element>，相同標籤才往下比對。
    每次合併完成後立即 yield，讓呼叫端自行處理。
    """
    if a is None and b is None:
        return                     # 兩邊都沒東西
    if a is None:
        yield b                    # 只有右邊
        return
    if b is None:
        yield a                    # 只有左邊
        return

    # 標籤一致才真正合併；否則視為差異，各自輸出
    if a.tag != b.tag:
        yield a
        yield b
        return

    # ---------- 1) 建立新的節點 ----------
    merged = etree.Element(a.tag)

    # ---------- 2) 合併屬性 ----------
    merged.attrib.update(a.attrib)         # 先左
    merged.attrib.update(b.attrib)         # 再右（右邊覆蓋）

    # ---------- 3) 合併文字 ----------
    merged.text = (a.text or '') + (b.text or '')

    # ---------- 4) 逐子節點遞迴 ----------
    for left_child, right_child in zip_longest(list(a), list(b)):
        for sub in merge_elements(left_child, right_child):
            merged.append(sub)

    # ---------- 5) 把結果交出去 ----------
    yield merged


# --------- 使用範例 ---------
html1 = """<ul>
  <li id="a">apple</li>
  <li id="b">banana</li>
</ul>"""

html2 = """<ul>
  <li id="b">BANANA</li>
  <li id="c">cherry</li>
</ul>"""

dom1 = html.fromstring(html1)
dom2 = html.fromstring(html2)

# 只取第一層 <ul>
merged_ul, = merge_elements(dom1, dom2)    # 注意逗號 (unpack generator)

print(html.tostring(merged_ul,
                    encoding="unicode",
                    pretty_print=True))
