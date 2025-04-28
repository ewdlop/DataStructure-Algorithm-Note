function* mergeNodes(left, right) {
  if (!left && !right) return;
  if (!left)  { yield right; return; }
  if (!right) { yield left;  return; }

  if (left.nodeName !== right.nodeName) {
    yield left; yield right; return;
  }

  const merged = left.cloneNode(false);                    // 只複製節點本身
  Array.from(right.attributes).forEach(attr =>
    merged.setAttribute(attr.name, attr.value));           // 屬性覆蓋

  merged.textContent = (left.textContent || '') +
                       (right.textContent || '');

  const lChildren = left.childNodes;
  const rChildren = right.childNodes;
  const len = Math.max(lChildren.length, rChildren.length);

  for (let i = 0; i < len; ++i) {
    for (const sub of mergeNodes(lChildren[i], rChildren[i])) {
      merged.appendChild(sub);
    }
  }
  yield merged;
}
