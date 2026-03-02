const API = 'http://127.0.0.1:8768';

let allAscs       = [];
let selectedId    = null;
let searchText    = '';

// ── choices 缓存 ──────────────────────────────────────────────────────────────  
let tagChoices     = []; // [{id, name}, ...]  
let attrSetChoices = [];
let abilityChoices = [];

// ── 状态栏 ───────────────────────────────────────────────────────────────────  
function setStatus(msg, type = '') {
    const el = document.getElementById('status-msg');
    el.textContent = msg;
    el.className = type;
}
function setCount(n) {
    document.getElementById('status-count').textContent = `共 ${n} 个预设`;
}

// ── 数据加载 ─────────────────────────────────────────────────────────────────  
async function loadInfo() {
    try {
        const r = await fetch(`${API}/api/info`).then(resp => resp.json());
        if (r.ok) document.getElementById('xlsx-path').textContent = r.xlsx;
    } catch (e) {}
}

async function loadAllChoices() {
    try {
        const [tags, attrsets, abilities] = await Promise.all([
            fetch(`${API}/api/choices/tags`).then(r => r.json()),
            fetch(`${API}/api/choices/attrsets`).then(r => r.json()),
            fetch(`${API}/api/choices/abilities`).then(r => r.json()),
        ]);
        if (tags.ok)      tagChoices     = tags.tags;
        if (attrsets.ok)  attrSetChoices = attrsets.attrsets;
        if (abilities.ok) abilityChoices = abilities.abilities;
    } catch (e) {}
}

async function loadAscs() {
    setStatus('加载中...');
    try {
        const r = await fetch(`${API}/api/ascs`).then(resp => resp.json());
        if (!r.ok) throw new Error(r.error);
        allAscs = r.ascs;
        setCount(allAscs.length);
        renderList();
        setStatus('加载完成', 'ok');
    } catch (e) {
        setStatus('加载失败: ' + e.message, 'err');
    }
}

// ── 渲染左侧列表 ──────────────────────────────────────────────────────────────  
function renderList() {
    const container = document.getElementById('asc-list');
    const filtered = allAscs.filter(a =>
        !searchText || a.name.toLowerCase().includes(searchText.toLowerCase())
    );
    container.innerHTML = '';
    filtered.forEach(asc => {
        const div = document.createElement('div');
        div.className = 'asc-item' + (asc.id === selectedId ? ' selected' : '');
        div.innerHTML = `<span class="asc-item-name">${asc.name}</span><span class="asc-item-id">#${asc.id}</span>`;
        div.onclick = () => App.selectAsc(asc.id);
        container.appendChild(div);
    });
}

// ── Chip 工具（Tag / AttrSet / Ability 通用）──────────────────────────────────  
function buildChipList(containerId, ids, choices) {
    const container = document.getElementById(containerId);
    if (!container) return;
    container.innerHTML = '';
    ids.forEach(id => {
        const choice = choices.find(c => c.id === id);
        const label = choice ? `${choice.name} #${id}` : `#${id}`;
        const chip = document.createElement('span');
        chip.className = 'chip';
        chip.dataset.id = String(id);
        chip.innerHTML = `${label}<button class="chip-del" onclick="removeChip('${containerId}', ${id})">✕</button>`;
        container.appendChild(chip);
    });
}

function removeChip(containerId, id) {
    const container = document.getElementById(containerId);
    if (!container) return;
    const chip = container.querySelector(`.chip[data-id="${id}"]`);
    if (chip) chip.remove();
}

function getChipIds(containerId) {
    const container = document.getElementById(containerId);
    if (!container) return [];
    return Array.from(container.querySelectorAll('.chip')).map(c => parseInt(c.dataset.id));
}

function addChipFromSelect(selectId, containerId, choices) {
    const sel = document.getElementById(selectId);
    if (!sel) return;
    const id = parseInt(sel.value);
    if (!id) return;
    const container = document.getElementById(containerId);
    if (!container) return;
    // 避免重复  
    if (container.querySelector(`.chip[data-id="${id}"]`)) return;
    const choice = choices.find(c => c.id === id);
    const label = choice ? `${choice.name} #${id}` : `#${id}`;
    const chip = document.createElement('span');
    chip.className = 'chip';
    chip.dataset.id = String(id);
    chip.innerHTML = `${label}<button class="chip-del" onclick="removeChip('${containerId}', ${id})">✕</button>`;
    container.appendChild(chip);
    sel.value = '';
}

// ── 生成选择下拉框 options ────────────────────────────────────────────────────  
function buildOptions(choices) {
    return `<option value="">-- 选择 --</option>` +
        choices.map(c => `<option value="${c.id}">${c.name} #${c.id}</option>`).join('');
}

// ── 渲染右侧表单 ──────────────────────────────────────────────────────────────  
function renderForm(asc) {
    const body = document.getElementById('form-body');
    body.className = '';
    body.innerHTML = `  
        <!-- 基础信息：两列网格 -->  
        <div class="basic-info-grid">  
            <div class="field-group">  
                <label>ID</label>  
                <input class="field-readonly" type="number" value="${asc.id}" readonly>  
            </div>  
            <div class="field-group">  
                <label>等级</label>  
                <input id="edit-level" type="number" value="${asc.level}" min="1">  
            </div>  
            <div class="field-group">  
                <label>预设名称</label>  
                <input id="edit-name" type="text" value="${asc.name}" autocomplete="off">  
            </div>  
            <div class="field-group" style="grid-column:1/-1">  
                <label>描述</label>  
                <textarea id="edit-desc" style="min-height:52px">${asc.desc}</textarea>  
            </div>  
        </div>  
  
        <!-- 三栏并排 -->  
        <div class="refs-row">  
            <!-- Tag -->  
            <div class="ref-panel">  
                <div class="ref-panel-header">🏷 初始 Tag</div>  
                <div class="ref-panel-body">  
                    <div id="tag-chips" class="chip-list"></div>  
                    <div class="add-chip-row">  
                        <select id="tag-select" class="chip-select">${buildOptions(tagChoices)}</select>  
                        <button class="btn btn-success btn-add-chip"  
                            onclick="addChipFromSelect('tag-select','tag-chips',tagChoices)">＋</button>  
                    </div>  
                </div>  
            </div>  
            <!-- AttrSet -->  
            <div class="ref-panel">  
                <div class="ref-panel-header">📦 属性集 AttrSet</div>  
                <div class="ref-panel-body">  
                    <div id="attrset-chips" class="chip-list"></div>  
                    <div class="add-chip-row">  
                        <select id="attrset-select" class="chip-select">${buildOptions(attrSetChoices)}</select>  
                        <button class="btn btn-success btn-add-chip"  
                            onclick="addChipFromSelect('attrset-select','attrset-chips',attrSetChoices)">＋</button>  
                    </div>  
                </div>  
            </div>  
            <!-- Ability -->  
            <div class="ref-panel">  
                <div class="ref-panel-header">⚡ 初始技能 Ability</div>  
                <div class="ref-panel-body">  
                    <div id="ability-chips" class="chip-list"></div>  
                    <div class="add-chip-row">  
                        <select id="ability-select" class="chip-select">${buildOptions(abilityChoices)}</select>  
                        <button class="btn btn-success btn-add-chip"  
                            onclick="addChipFromSelect('ability-select','ability-chips',abilityChoices)">＋</button>  
                    </div>  
                </div>  
            </div>  
        </div>  
    `;
    buildChipList('tag-chips',     asc.tag     || [], tagChoices);
    buildChipList('attrset-chips', asc.attrSet || [], attrSetChoices);
    buildChipList('ability-chips', asc.ability || [], abilityChoices);
    document.getElementById('form-title').textContent = `编辑: ${asc.name}`;
    document.getElementById('btn-delete').style.display = '';
    document.getElementById('btn-save').style.display = '';
}

function clearForm() {
    const body = document.getElementById('form-body');
    body.className = 'empty-hint';
    body.textContent = '← 从左侧选择一个ASC预设进行编辑';
    document.getElementById('form-title').textContent = '请从左侧选择一个ASC预设';
    document.getElementById('btn-delete').style.display = 'none';
    document.getElementById('btn-save').style.display = 'none';
}

// ── App 命名空间（供 HTML onclick 调用）──────────────────────────────────────  
const App = {
    async reload() {
        selectedId = null;
        clearForm();
        loadInfo();
        await loadAllChoices();
        await loadAscs();
    },

    onSearch(val) {
        searchText = val;
        renderList();
    },

    selectAsc(id) {
        selectedId = id;
        renderList();
        const asc = allAscs.find(a => a.id === id);
        if (asc) renderForm(asc);
    },

    async saveSelected() {
        if (selectedId == null) return;
        const nameEl  = document.getElementById('edit-name');
        const descEl  = document.getElementById('edit-desc');
        const levelEl = document.getElementById('edit-level');
        if (!nameEl || !descEl || !levelEl) return;
        const name  = nameEl.value.trim();
        const desc  = descEl.value.trim();
        const level = parseInt(levelEl.value) || 1;
        if (!name) { setStatus('预设名称不能为空', 'err'); return; }

        const tag     = getChipIds('tag-chips');
        const attrSet = getChipIds('attrset-chips');
        const ability = getChipIds('ability-chips');

        setStatus('保存中...');
        try {
            const r = await fetch(`${API}/api/ascs/${selectedId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name, desc, level, tag, attrSet, ability })
            }).then(resp => resp.json());
            if (!r.ok) throw new Error(r.error);
            setStatus('保存成功', 'ok');
            await loadAscs();
            App.selectAsc(selectedId);
        } catch (e) {
            setStatus('保存失败: ' + e.message, 'err');
        }
    },

    async deleteSelected() {
        if (selectedId == null) return;
        const asc = allAscs.find(a => a.id === selectedId);
        if (!asc) return;
        if (!confirm(`确认删除ASC预设「${asc.name}」(ID: ${asc.id})？`)) return;
        setStatus('删除中...');
        try {
            const r = await fetch(`${API}/api/ascs/${selectedId}`, {
                method: 'DELETE'
            }).then(resp => resp.json());
            if (!r.ok) throw new Error(r.error);
            selectedId = null;
            clearForm();
            setStatus('删除成功', 'ok');
            await loadAscs();
        } catch (e) {
            setStatus('删除失败: ' + e.message, 'err');
        }
    },

    addAsc() {
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
        if (!name) { setStatus('预设名称不能为空', 'err'); return; }
        const payload = { name, desc, level: 1, tag: [], attrSet: [], ability: [] };
        if (idVal) payload.id = parseInt(idVal);
        setStatus('新增中...');
        try {
            const r = await fetch(`${API}/api/ascs`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            }).then(resp => resp.json());
            if (!r.ok) throw new Error(r.error);
            App.closeModal();
            setStatus('新增成功', 'ok');
            await loadAscs();
            App.selectAsc(r.asc.id);
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
    await loadAllChoices(); // 先等 choices 加载完  
    await loadAscs();       // 再加载预设列表  
})();