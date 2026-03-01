const API = 'http://127.0.0.1:8766';

let allAttrs = [];
let selectedId = null;
let searchText = '';

function setStatus(msg, type = '') {
    const el = document.getElementById('status-msg');
    el.textContent = msg;
    el.className = type;
}
function setCount(n) {
    document.getElementById('status-count').textContent = `共 ${n} 个属性`;
}

async function loadInfo() {
    try {
        const r = await fetch(`${API}/api/info`).then(resp => resp.json());
        if (r.ok) document.getElementById('xlsx-path').textContent = r.xlsx;
    } catch (e) {}
}

async function loadAttrs() {
    setStatus('加载中...');
    try {
        const r = await fetch(`${API}/api/attrs`).then(resp => resp.json());
        if (!r.ok) throw new Error(r.error);
        allAttrs = r.attrs;
        setCount(allAttrs.length);
        renderList();
        setStatus('加载完成', 'ok');
    } catch (e) {
        setStatus('加载失败: ' + e.message, 'err');
    }
}

function renderList() {
    const container = document.getElementById('attr-list');
    const filtered = allAttrs.filter(a =>
        !searchText || a.name.toLowerCase().includes(searchText.toLowerCase())
    );
    container.innerHTML = '';
    filtered.forEach(attr => {
        const div = document.createElement('div');
        div.className = 'attr-item' + (attr.id === selectedId ? ' selected' : '');
        div.innerHTML = `<span class="attr-item-name">${attr.name}</span><span class="attr-item-id">#${attr.id}</span>`;
        div.onclick = () => App.selectAttr(attr.id);
        container.appendChild(div);
    });
}

function renderForm(attr) {
    const body = document.getElementById('form-body');
    body.className = '';
    body.innerHTML = `  
        <div class="field-group">  
            <label>ID</label>  
            <input class="field-readonly" type="number" value="${attr.id}" readonly>  
        </div>  
        <div class="field-group">  
            <label>属性名称</label>  
            <input id="edit-name" type="text" value="${attr.name}" autocomplete="off">  
        </div>  
        <div class="field-group">  
            <label>描述</label>  
            <textarea id="edit-desc">${attr.desc}</textarea>  
        </div>  
    `;
    document.getElementById('form-title').textContent = `编辑: ${attr.name}`;
    document.getElementById('btn-delete').style.display = '';
    document.getElementById('btn-save').style.display = '';
}

const App = {
    reload() { loadInfo(); loadAttrs(); },
    onSearch(v) { searchText = v; renderList(); },

    selectAttr(id) {
        selectedId = id;
        renderList();
        const attr = allAttrs.find(a => a.id === id);
        if (attr) renderForm(attr);
    },

    async saveSelected() {
        if (selectedId == null) return;
        const nameEl = document.getElementById('edit-name');
        const descEl = document.getElementById('edit-desc');
        if (!nameEl || !descEl) return;
        const name = nameEl.value.trim();
        const desc = descEl.value.trim();
        if (!name) { setStatus('属性名称不能为空', 'err'); return; }
        setStatus('保存中...');
        try {
            const r = await fetch(`${API}/api/attrs/${selectedId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name, desc })
            }).then(resp => resp.json());   // ← 修复变量名冲突bug  
            if (!r.ok) throw new Error(r.error);
            setStatus('保存成功', 'ok');
            await loadAttrs();
            App.selectAttr(selectedId);
        } catch (e) {
            setStatus('保存失败: ' + e.message, 'err');
        }
    },

    async deleteSelected() {
        if (selectedId == null) return;
        if (!confirm('确认删除该属性？')) return;
        setStatus('删除中...');
        try {
            const r = await fetch(`${API}/api/attrs/${selectedId}`, {
                method: 'DELETE'
            }).then(resp => resp.json());   // ← 修复变量名冲突bug  
            if (!r.ok) throw new Error(r.error);
            selectedId = null;
            document.getElementById('form-body').className = 'empty-hint';
            document.getElementById('form-body').textContent = '← 从左侧选择一个属性进行编辑';
            document.getElementById('form-title').textContent = '请从左侧选择一个属性';
            document.getElementById('btn-delete').style.display = 'none';
            document.getElementById('btn-save').style.display = 'none';
            setStatus('删除成功', 'ok');
            await loadAttrs();
        } catch (e) {
            setStatus('删除失败: ' + e.message, 'err');
        }
    },

    addAttr() {
        document.getElementById('modal-id').value = '';
        document.getElementById('modal-name').value = '';
        document.getElementById('modal-desc').value = '';
        document.getElementById('modal-overlay').style.display = 'flex';
    },
    closeModal() { document.getElementById('modal-overlay').style.display = 'none'; },

    async confirmAdd() {
        const idVal = document.getElementById('modal-id').value.trim();
        const name  = document.getElementById('modal-name').value.trim();
        const desc  = document.getElementById('modal-desc').value.trim();
        if (!name) { alert('属性名称不能为空'); return; }
        const payload = { name, desc };
        if (idVal) payload.id = parseInt(idVal);
        setStatus('新增中...');
        try {
            const r = await fetch(`${API}/api/attrs`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            }).then(resp => resp.json());   // ← 修复变量名冲突bug  
            if (!r.ok) throw new Error(r.error);
            App.closeModal();
            setStatus('新增成功', 'ok');
            await loadAttrs();
            App.selectAttr(r.attr.id);
        } catch (e) {
            setStatus('新增失败: ' + e.message, 'err');
        }
    },
};

// 初始化  
loadInfo();
loadAttrs();