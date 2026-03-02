const API = 'http://127.0.0.1:8767';

let allAttrSets = [];
let selectedId = null;
let searchText = '';

// ── 状态栏 ───────────────────────────────────────────────────────────────────  
function setStatus(msg, type = '') {
    const el = document.getElementById('status-msg');
    el.textContent = msg;
    el.className = type;
}
function setCount(n) {
    document.getElementById('status-count').textContent = `共 ${n} 个属性集`;
}

// ── 数据加载 ─────────────────────────────────────────────────────────────────  
async function loadInfo() {
    try {
        const r = await fetch(`${API}/api/info`).then(resp => resp.json());
        if (r.ok) document.getElementById('xlsx-path').textContent = r.xlsx;
    } catch (e) {}
}

async function loadAttrSets() {
    setStatus('加载中...');
    try {
        const r = await fetch(`${API}/api/attrsets`).then(resp => resp.json());
        if (!r.ok) throw new Error(r.error);
        allAttrSets = r.attrsets;
        setCount(allAttrSets.length);
        renderList();
        setStatus('加载完成', 'ok');
    } catch (e) {
        setStatus('加载失败: ' + e.message, 'err');
    }
}

// ── 渲染左侧列表 ──────────────────────────────────────────────────────────────  
function renderList() {
    const container = document.getElementById('attrset-list');
    const filtered = allAttrSets.filter(s =>
        !searchText || s.name.toLowerCase().includes(searchText.toLowerCase())
    );
    container.innerHTML = '';
    filtered.forEach(attrset => {
        const div = document.createElement('div');
        div.className = 'attrset-item' + (attrset.id === selectedId ? ' selected' : '');
        div.innerHTML = `<span class="attrset-item-name">${attrset.name}</span><span class="attrset-item-id">#${attrset.id}</span>`;
        div.onclick = () => App.selectAttrSet(attrset.id);
        container.appendChild(div);
    });
}

// ── 读取当前表单中的 attributes 子行数据 ──────────────────────────────────────  
function collectAttrRows() {
    const rows = document.querySelectorAll('.attr-subrow');
    const result = [];
    rows.forEach(row => {
        const inputs = row.querySelectorAll('input');
        // inputs: [id, initValue, minValue, maxValue, useMinValue(checkbox), useMaxValue(checkbox)]  
        result.push({
            id:          parseInt(inputs[0].value) || 0,
            initValue:   parseFloat(inputs[1].value) || 0,
            minValue:    parseFloat(inputs[2].value) || 0,
            maxValue:    parseFloat(inputs[3].value) || 0,
            useMinValue: inputs[4].checked,
            useMaxValue: inputs[5].checked,
        });
    });
    return result;
}

// ── 渲染右侧表单 ──────────────────────────────────────────────────────────────  
function renderForm(attrset) {
    const body = document.getElementById('form-body');
    body.className = '';

    const attrRows = (attrset.attributes || []).map((a, idx) => `  
        <div class="attr-subrow">  
            <input class="attr-subrow-id" type="number" value="${a.id}" placeholder="AttrID" title="Attribute ID">  
            <input type="number" step="any" value="${a.initValue}" placeholder="初始值" title="初始值">  
            <input type="number" step="any" value="${a.minValue}" placeholder="最小值" title="最小值">  
            <input type="number" step="any" value="${a.maxValue}" placeholder="最大值" title="最大值">  
            <input type="checkbox" title="启用最小值" ${a.useMinValue ? 'checked' : ''}>  
            <input type="checkbox" title="启用最大值" ${a.useMaxValue ? 'checked' : ''}>  
            <button class="attr-subrow-del" onclick="this.closest('.attr-subrow').remove()">✕</button>  
        </div>  
    `).join('');

    body.innerHTML = `  
        <div class="field-group">  
            <label>ID</label>  
            <input class="field-readonly" type="number" value="${attrset.id}" readonly>  
        </div>  
        <div class="field-group">  
            <label>属性集名称</label>  
            <input id="edit-name" type="text" value="${attrset.name}" autocomplete="off">  
        </div>  
        <div class="field-group">  
            <label>描述</label>  
            <textarea id="edit-desc">${attrset.desc}</textarea>  
        </div>  
        <div class="field-group">  
            <label>属性列表 <small>（ID · 初始值 · 最小值 · 最大值 · 启用Min · 启用Max）</small></label>  
            <div class="attr-sublist">  
                <div class="attr-sublist-header">  
                    <span>AttrID</span>  
                    <span>初始值</span>  
                    <span>最小值</span>  
                    <span>最大值</span>  
                    <span title="启用最小值">Min✓</span>  
                    <span title="启用最大值">Max✓</span>  
                    <span style="width:28px"></span>  
                </div>  
                <div id="attr-subrows">${attrRows}</div>  
            </div>  
            <button class="btn btn-success btn-add-attr" onclick="App.addAttrRow()">＋ 新增属性</button>  
        </div>  
    `;

    document.getElementById('form-title').textContent = `编辑: ${attrset.name}`;
    document.getElementById('btn-delete').style.display = '';
    document.getElementById('btn-save').style.display = '';
}

function clearForm() {
    const body = document.getElementById('form-body');
    body.className = 'empty-hint';
    body.textContent = '← 从左侧选择一个属性集进行编辑';
    document.getElementById('form-title').textContent = '请从左侧选择一个属性集';
    document.getElementById('btn-delete').style.display = 'none';
    document.getElementById('btn-save').style.display = 'none';
}

// ── App 命名空间（供 HTML onclick 调用）──────────────────────────────────────  
const App = {
    reload() {
        selectedId = null;
        clearForm();
        loadInfo();
        loadAttrSets();
    },

    onSearch(val) {
        searchText = val;
        renderList();
    },

    selectAttrSet(id) {
        selectedId = id;
        renderList();
        const attrset = allAttrSets.find(s => s.id === id);
        if (attrset) renderForm(attrset);
    },

    addAttrRow() {
        const container = document.getElementById('attr-subrows');
        if (!container) return;
        const div = document.createElement('div');
        div.className = 'attr-subrow';
        div.innerHTML = `  
            <input class="attr-subrow-id" type="number" value="0" placeholder="AttrID" title="Attribute ID">  
            <input type="number" step="any" value="0" placeholder="初始值" title="初始值">  
            <input type="number" step="any" value="0" placeholder="最小值" title="最小值">  
            <input type="number" step="any" value="999" placeholder="最大值" title="最大值">  
            <input type="checkbox" title="启用最小值" checked>  
            <input type="checkbox" title="启用最大值" checked>  
            <button class="attr-subrow-del" onclick="this.closest('.attr-subrow').remove()">✕</button>  
        `;
        container.appendChild(div);
    },

    async saveSelected() {
        if (selectedId == null) return;
        const nameEl = document.getElementById('edit-name');
        const descEl = document.getElementById('edit-desc');
        if (!nameEl || !descEl) return;
        const name = nameEl.value.trim();
        const desc = descEl.value.trim();
        if (!name) { setStatus('属性集名称不能为空', 'err'); return; }

        const attributes = collectAttrRows();
        setStatus('保存中...');
        try {
            const r = await fetch(`${API}/api/attrsets/${selectedId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name, desc, attributes })
            }).then(resp => resp.json());
            if (!r.ok) throw new Error(r.error);
            setStatus('保存成功', 'ok');
            await loadAttrSets();
            App.selectAttrSet(selectedId);
        } catch (e) {
            setStatus('保存失败: ' + e.message, 'err');
        }
    },

    async deleteSelected() {
        if (selectedId == null) return;
        const attrset = allAttrSets.find(s => s.id === selectedId);
        if (!attrset) return;
        if (!confirm(`确认删除属性集「${attrset.name}」(ID: ${attrset.id})？`)) return;
        setStatus('删除中...');
        try {
            const r = await fetch(`${API}/api/attrsets/${selectedId}`, {
                method: 'DELETE'
            }).then(resp => resp.json());
            if (!r.ok) throw new Error(r.error);
            selectedId = null;
            clearForm();
            setStatus('删除成功', 'ok');
            await loadAttrSets();
        } catch (e) {
            setStatus('删除失败: ' + e.message, 'err');
        }
    },

    addAttrSet() {
        document.getElementById('modal-id').value = '';
        document.getElementById('modal-name').value = '';
        document.getElementById('modal-desc').value = '';
        document.getElementById('modal-overlay').classList.add('show');
        setTimeout(() => document.getElementById('modal-name').focus(), 50);
    },

    closeModal() {
        document.getElementById('modal-overlay').classList.remove('show');
    },

    async confirmAdd() {
        const idVal = document.getElementById('modal-id').value.trim();
        const name  = document.getElementById('modal-name').value.trim();
        const desc  = document.getElementById('modal-desc').value.trim();
        if (!name) { setStatus('属性集名称不能为空', 'err'); return; }
        const payload = { name, desc, attributes: [] };
        if (idVal) payload.id = parseInt(idVal);
        setStatus('新增中...');
        try {
            const r = await fetch(`${API}/api/attrsets`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            }).then(resp => resp.json());
            if (!r.ok) throw new Error(r.error);
            App.closeModal();
            setStatus('新增成功', 'ok');
            await loadAttrSets();
            App.selectAttrSet(r.attrset.id);
        } catch (e) {
            setStatus('新增失败: ' + e.message, 'err');
        }
    },
};

// ── 键盘快捷键 ───────────────────────────────────────────────────────────────  
document.addEventListener('keydown', e => {
    if (e.ctrlKey && e.key === 's') {
        e.preventDefault();
        App.saveSelected();
    }
    if (e.key === 'Escape') {
        App.closeModal();
    }
    if (e.key === 'Enter' && document.getElementById('modal-overlay').classList.contains('show')) {
        App.confirmAdd();
    }
});

// ── 初始化 ───────────────────────────────────────────────────────────────────  
loadInfo();
loadAttrSets();