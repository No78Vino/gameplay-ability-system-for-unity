const API = 'http://127.0.0.1:8765';

// ── 状态 ─────────────────────────────────────────────────────────────────    
let allTags = [];
let selectedId = null;
let searchText = '';

// ── 工具 ─────────────────────────────────────────────────────────────────    
function setStatus(msg, type = '') {
    const el = document.getElementById('status-msg');
    el.textContent = msg;
    el.className = type;
}
function setCount(n) {
    document.getElementById('status-count').textContent = `共 ${n} 个 Tag`;
}

// ── 数据加载 ─────────────────────────────────────────────────────────────    
async function loadInfo() {
    try {
        const r = await fetch(`${API}/api/info`).then(res => res.json());
        if (r.ok) document.getElementById('xlsx-path').textContent = r.xlsx;
    } catch (e) {}
}

async function loadTags() {
    setStatus('加载中...');
    try {
        const r = await fetch(`${API}/api/tags`).then(res => res.json());
        if (!r.ok) throw new Error(r.error);
        allTags = r.tags;
        setCount(allTags.length);
        renderTree();
        setStatus('加载完成', 'ok');

        // 客户端警告检测（移入 try 块内，删除重复的 allTags = r.tags）  
        const nameSet = new Set(allTags.map(t => t.name));
        const idCount = {};
        allTags.forEach(t => idCount[t.id] = (idCount[t.id] || 0) + 1);
        const warns = [];
        allTags.forEach(t => {
            if (idCount[t.id] > 1) warns.push(`重复ID ${t.id}: 「${t.name}」`);
            const parts = t.name.split('.');
            for (let i = 1; i < parts.length; i++) {
                const parent = parts.slice(0, i).join('.');
                if (!nameSet.has(parent)) warns.push(`「${t.name}」缺少父类「${parent}」`);
            }
        });
        if (warns.length > 0) showWarnings(warns); else clearWarnings();
    } catch (e) {
        setStatus('加载失败: ' + e.message, 'err');
    }
}

// ── 树构建 ───────────────────────────────────────────────────────────────    
function buildTree(tags) {
    const root = { children: new Map() };
    for (const tag of tags) {
        const parts = tag.name.split('.');
        let cur = root, fullName = '';
        for (const seg of parts) {
            fullName = fullName ? fullName + '.' + seg : seg;
            if (!cur.children.has(seg))
                cur.children.set(seg, { segment: seg, fullName, tag: null, children: new Map() });
            cur = cur.children.get(seg);
        }
        cur.tag = tag;
    }
    return root;
}

function renderTree() {
    const container = document.getElementById('tree');
    const filter = searchText.toLowerCase();
    const filtered = filter
        ? allTags.filter(t => t.name.toLowerCase().includes(filter) || String(t.id).includes(filter))
        : allTags;
    const tree = buildTree(filtered);
    container.innerHTML = '';
    renderTreeNode(tree, container);
}

function renderTreeNode(node, container) {
    const entries = [...node.children.entries()].sort((a, b) => a[0].localeCompare(b[0]));
    for (const [seg, child] of entries) {
        const hasChildren = child.children.size > 0;
        const isSelected = child.tag && child.tag.id === selectedId;

        const nodeEl = document.createElement('div');
        nodeEl.className = 'tree-node';

        const inner = document.createElement('div');
        inner.className = 'tree-node-inner' + (isSelected ? ' selected' : '');

        const toggle = document.createElement('span');
        toggle.className = 'tree-toggle';
        toggle.textContent = hasChildren ? '▾' : '';

        const label = document.createElement('span');
        label.className = 'tree-label';
        label.textContent = seg;
        label.title = child.fullName;

        const idBadge = document.createElement('span');
        idBadge.className = 'tree-id';
        idBadge.textContent = child.tag ? '#' + child.tag.id : '';

        inner.appendChild(toggle);
        inner.appendChild(label);
        inner.appendChild(idBadge);
        nodeEl.appendChild(inner);

        if (child.tag) {
            inner.onclick = () => App.selectTag(child.tag.id);
        }

        if (hasChildren) {
            const childContainer = document.createElement('div');
            childContainer.className = 'tree-children';
            renderTreeNode(child, childContainer);
            nodeEl.appendChild(childContainer);

            toggle.onclick = (e) => {
                e.stopPropagation();
                const collapsed = childContainer.style.display === 'none';
                childContainer.style.display = collapsed ? '' : 'none';
                toggle.textContent = collapsed ? '▾' : '▸';
            };
        }

        container.appendChild(nodeEl);
    }
}

// ── 右侧表单 ─────────────────────────────────────────────────────────────    
function showForm(tag) {
    const formBody = document.getElementById('form-body');
    formBody.className = '';
    document.getElementById('form-title').textContent = tag.name;
    document.getElementById('btn-delete').style.display = '';
    document.getElementById('btn-save').style.display = '';

    // 层级路径预览    
    const parts = tag.name.split('.');
    const pathHtml = parts.map((p, i) =>
        i < parts.length - 1
            ? `<span>${p}</span> › `
            : `<span style="color:#cdd6f4">${p}</span>`
    ).join('');

    formBody.innerHTML = `    
    <div class="field-group">    
      <label>ID</label>    
      <input id="edit-id" type="number" value="${tag.id}">    
    </div>    
    <div class="field-group">    
      <label>Tag 名称 <small>（点分层级，如 State.Debuff.Stun）</small></label>    
      <input id="edit-name" type="text" value="${escHtml(tag.name)}">    
      <div class="tag-path-preview">${pathHtml}</div>    
    </div>    
    <div class="field-group">    
      <label>描述</label>    
      <textarea id="edit-desc">${escHtml(tag.desc)}</textarea>    
    </div>    
  `;

    // 实时更新路径预览    
    document.getElementById('edit-name').addEventListener('input', function () {
        const ps = this.value.split('.');
        const html = ps.map((p, i) =>
            i < ps.length - 1
                ? `<span>${escHtml(p)}</span> › `
                : `<span style="color:#cdd6f4">${escHtml(p)}</span>`
        ).join('');
        formBody.querySelector('.tag-path-preview').innerHTML = html;
    });
}

function clearForm() {
    document.getElementById('form-title').textContent = '请从左侧选择一个 Tag';
    document.getElementById('btn-delete').style.display = 'none';
    document.getElementById('btn-save').style.display = 'none';
    const formBody = document.getElementById('form-body');
    formBody.className = 'empty-hint';
    formBody.textContent = '← 从左侧选择一个 Tag 进行编辑';
}

function escHtml(s) {
    return String(s)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;');
}

function showWarnings(warnings) {
    let el = document.getElementById('warning-bar');
    if (!el) {
        el = document.createElement('div');
        el.id = 'warning-bar';
        el.style.cssText = 'background:#fab387;color:#1e1e2e;padding:6px 20px;font-size:12px;';
        document.getElementById('statusbar').after(el);
    }
    el.innerHTML = '⚠️ ' + warnings.join('<br>⚠️ ');
}
function clearWarnings() {
    const el = document.getElementById('warning-bar');
    if (el) el.remove();
}

// ── App 命名空间（供 HTML onclick 调用）──────────────────────────────────    
const App = {

    reload() {
        selectedId = null;
        clearForm();
        loadTags();
    },

    onSearch(val) {
        searchText = val;
        renderTree();
    },

    selectTag(id) {
        selectedId = id;
        renderTree(); // 刷新选中高亮    
        const tag = allTags.find(t => t.id === id);
        if (tag) showForm(tag);
    },

    async saveSelected() {
        if (selectedId == null) return;
        const nameEl = document.getElementById('edit-name');
        const descEl = document.getElementById('edit-desc');
        const idEl   = document.getElementById('edit-id');
        if (!nameEl || !descEl || !idEl) return;

        const name  = nameEl.value.trim();
        const desc  = descEl.value.trim();
        const newId = parseInt(idEl.value);
        if (!name) { setStatus('Tag名称不能为空', 'err'); return; }

        setStatus('保存中...');
        try {
            const r = await fetch(`${API}/api/tags/${selectedId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: newId, name, desc })
            }).then(res => res.json());

            if (!r.ok) throw new Error(r.error);
            // 显示警告  
            if (r.warnings && r.warnings.length > 0) {
                setStatus('保存成功（有警告）', 'ok');
                showWarnings(r.warnings);
            } else {
                setStatus('保存成功', 'ok');
                clearWarnings();
            }
            await loadTags();
            App.selectTag(newId);  // ID 可能已变，用新 ID 选中  
        } catch (e) {
            setStatus('保存失败: ' + e.message, 'err');
        }
    },

    async deleteSelected() {
        if (selectedId == null) return;
        const tag = allTags.find(t => t.id === selectedId);
        if (!tag) return;
        if (!confirm(`确认删除 Tag「${tag.name}」(ID: ${tag.id})？\n\n注意：如果其他配置引用了此 Tag，请手动检查。`)) return;

        setStatus('删除中...');
        try {
            const r = await fetch(`${API}/api/tags/${selectedId}`, {
                method: 'DELETE'
            }).then(res => res.json());

            if (!r.ok) throw new Error(r.error);
            setStatus('删除成功', 'ok');
            selectedId = null;
            clearForm();
            await loadTags();
        } catch (e) {
            setStatus('删除失败: ' + e.message, 'err');
        }
    },

    addTag() {
        document.getElementById('modal-name').value = '';
        document.getElementById('modal-desc').value = '';
        document.getElementById('modal-id').value = '';  // ← 新增：清空 ID 输入框  
        document.getElementById('modal-overlay').classList.add('show');
        setTimeout(() => document.getElementById('modal-name').focus(), 50);
    },

    closeModal() {
        document.getElementById('modal-overlay').classList.remove('show');
    },

    async confirmAdd() {
        const name = document.getElementById('modal-name').value.trim();
        const desc = document.getElementById('modal-desc').value.trim();
        const idVal = document.getElementById('modal-id').value.trim();  // ← 新增：读取 ID  
        if (!name) { setStatus('Tag名称不能为空', 'err'); return; }

        setStatus('新增中...');
        try {
            const payload = { name, desc };
            if (idVal !== '') payload.id = parseInt(idVal);  // ← 新增：有值才传 id  
            const r = await fetch(`${API}/api/tags`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            }).then(res => res.json());

            if (!r.ok) throw new Error(r.error);
            App.closeModal();
            setStatus('新增成功', 'ok');
            await loadTags();
            App.selectTag(r.tag.id);
        } catch (e) {
            setStatus('新增失败: ' + e.message, 'err');
        }
    }
};

// ── 键盘快捷键 ───────────────────────────────────────────────────────────    
document.addEventListener('keydown', e => {
    // Ctrl+S 保存    
    if (e.ctrlKey && e.key === 's') {
        e.preventDefault();
        App.saveSelected();
    }
    // Escape 关闭对话框    
    if (e.key === 'Escape') {
        App.closeModal();
    }
    // Enter 确认新增（对话框内）    
    if (e.key === 'Enter' && document.getElementById('modal-overlay').classList.contains('show')) {
        App.confirmAdd();
    }
});

// ── 初始化 ───────────────────────────────────────────────────────────────    
loadInfo();
loadTags();