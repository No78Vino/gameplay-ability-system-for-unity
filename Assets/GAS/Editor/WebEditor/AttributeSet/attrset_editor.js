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

// ── 全局：attribute choices 缓存 ─────────────────────────────────────────────  
let attrChoices = []; // [{id, name}, ...]  

async function loadAttrChoices() {
    try {
        const r = await fetch(`${API}/api/choices/attrs`).then(resp => resp.json());
        if (r.ok) attrChoices = r.attrs;
    } catch (e) {}
}

// ── 生成单个属性卡片 ──────────────────────────────────────────────────────────  
function buildAttrCard(a = { id: 0, initValue: 0, minValue: 0, maxValue: 999, useMinValue: true, useMaxValue: true }) {
    const options = attrChoices.map(ch =>
        `<option value="${ch.id}" ${ch.id === a.id ? 'selected' : ''}>${ch.name} #${ch.id}</option>`
    ).join('');

    const card = document.createElement('div');
    card.className = 'attr-card';
    card.innerHTML = `  
        <div class="attr-card-header">  
            <select class="attr-select" data-field="id">  
                <option value="0">-- 选择 Attribute --</option>  
                ${options}  
            </select>  
            <button class="attr-card-del" onclick="this.closest('.attr-card').remove()">✕</button>  
        </div>  
        <div class="attr-card-body">  
            <label>初始值  
                <input type="number" step="any" data-field="initValue" value="${a.initValue}">  
            </label>  
            <label class="attr-minmax">  
                <input type="checkbox" data-field="useMinValue" ${a.useMinValue ? 'checked' : ''}>  
                最小值  
                <input type="number" step="any" data-field="minValue" value="${a.minValue}">  
            </label>  
            <label class="attr-minmax">  
                <input type="checkbox" data-field="useMaxValue" ${a.useMaxValue ? 'checked' : ''}>  
                最大值  
                <input type="number" step="any" data-field="maxValue" value="${a.maxValue}">  
            </label>  
        </div>  
    `;
    return card;
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
    return Array.from(document.querySelectorAll('.attr-card')).map(card => ({
        id:          parseInt(card.querySelector('[data-field="id"]').value) || 0,
        initValue:   parseFloat(card.querySelector('[data-field="initValue"]').value) || 0,
        minValue:    parseFloat(card.querySelector('[data-field="minValue"]').value) || 0,
        maxValue:    parseFloat(card.querySelector('[data-field="maxValue"]').value) || 0,
        useMinValue: card.querySelector('[data-field="useMinValue"]').checked,
        useMaxValue: card.querySelector('[data-field="useMaxValue"]').checked,
    }));
}

// ── 渲染右侧表单 ──────────────────────────────────────────────────────────────  
function renderForm(attrset) {
    const body = document.getElementById('form-body');
    body.className = '';
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
            <label>属性列表</label>  
            <div id="attr-cards"></div>  
            <button class="btn btn-success btn-add-attr" onclick="App.addAttrRow()">＋ 新增属性</button>  
        </div>  
    `;
    const container = document.getElementById('attr-cards');
    (attrset.attributes || []).forEach(a => container.appendChild(buildAttrCard(a)));
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
    async reload() {
        selectedId = null;
        clearForm();
        loadInfo();
        await loadAttrChoices();
        await loadAttrSets();
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

// 在 App 对象中：  
    addAttrRow() {
        const container = document.getElementById('attr-cards');
        if (!container) return;
        container.appendChild(buildAttrCard());
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
(async function init() {
    loadInfo();
    await loadAttrChoices();  // 先等 choices 加载完，确保 attrChoices 有数据  
    await loadAttrSets();     // 再加载属性集列表  
})();