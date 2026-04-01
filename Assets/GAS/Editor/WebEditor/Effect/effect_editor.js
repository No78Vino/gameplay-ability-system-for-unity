// ═══════════════════════════════════════════════════════════════════  
// § 1  常量 · 枚举 · 全局状态  
// ═══════════════════════════════════════════════════════════════════  
// Use same-origin API to avoid hardcoded port mismatch.
const API = window.location.origin;

// ── 全局数据 ─────────────────────────────────────────────────────────  
let allEffects    = [];
let selectedId    = null;
let searchText    = '';
let _currentEffect = null;   // 当前正在编辑的 effect 对象（深拷贝）  

// ── Choices 缓存（从 /api/choices/* 拉取）────────────────────────────  
let tagChoices     = [];   // [{id, name}]  
let cueChoices     = [];   // [{id, name}]  
let abilityChoices = [];   // [{id, name}]  
let attrSetChoices = [];   // [{id, name, attrs:[{id,name}]}]  
let effectChoices  = [];   // [{id, name}]  
let mmcChoices     = [];   // [{id, name}]  

// ── 枚举（与 server.py ENUM_* 对齐）─────────────────────────────────  
const ENUM_TIME_UNIT        = ['Frame', 'Turn'];
const ENUM_OPERATION        = ['Add', 'Multiply', 'Override', 'Minus', 'Divide'];
const ENUM_STACKING_TYPE    = ['AggregateBySource', 'AggregateByTarget'];
const ENUM_DURATION_REFRESH = ['NeverRefresh', 'RefreshOnSuccessfulApplication'];
const ENUM_PERIOD_RESET     = ['NeverRefresh', 'ResetOnSuccessfulApplication'];
const ENUM_EXPIRATION       = ['ClearEntireStack', 'RemoveSingleStackAndRefreshDuration', 'RefreshDuration'];
const ENUM_ABILITY_ACTIVATE   = ['None', 'WhenAdded', 'SyncWithEffect'];
const ENUM_ABILITY_DEACTIVATE = ['None', 'SyncWithEffect'];
const ENUM_ABILITY_REMOVE     = ['None', 'SyncWithEffect', 'WhenEnd', 'WhenCancel', 'WhenCancelOrEnd'];

// ── 组件分类 ──────────────────────────────────────────────────────────  
const TAG_COMPONENTS = [
    'AssetTags', 'GrantedTags', 'ApplicationRequiredTags',
    'OngoingRequiredTags', 'RemoveGameplayEffectsWithTags', 'ImmunityTags'
];
const CUE_COMPONENTS = [
    'CueOnApply', 'CueOnTick', 'CueOnAdd',
    'CueOnRemove', 'CueOnActivate', 'CueOnDeactivate'
];
const FUNC_COMPONENTS = ['Duration', 'Period', 'Modifiers', 'GrantedAbility', 'Stacking'];

const TAG_PROTOCOL_FIELDS = [
    { comp: 'AssetTags', key: 'assetTags', label: 'AssetTags 描述标签' },
    { comp: 'GrantedTags', key: 'grantedTags', label: 'GrantedTags 授予标签' },
    { comp: 'ApplicationRequiredTags', key: 'applicationRequiredTags', label: 'ApplicationRequiredTags 应用需求标签' },
    { comp: 'OngoingRequiredTags', key: 'ongoingRequiredTags', label: 'OngoingRequiredTags 持续需求标签' },
    { comp: 'RemoveGameplayEffectsWithTags', key: 'removeEffectsWithTags', label: 'RemoveGEWithTags 移除GE标签' },
    { comp: 'ImmunityTags', key: 'immunityTags', label: 'ImmunityTags 免疫标签' },
];

// ── 组件中文标签（对齐 C# EffectEditComponent 的 [LabelText]）────────  
const COMP_LABELS = {
    'AssetTags': '描述标签',
    'GrantedTags': '获得标签',
    'ApplicationRequiredTags': '应用需求标签',
    'OngoingRequiredTags': '持续需求标签',
    'RemoveGameplayEffectsWithTags': '移除持有标签的buff',
    'ImmunityTags': '被免疫的标签',
    'Duration': '持续时间',
    'Period': '间隔执行',
    'Modifiers': '修改器',
    'CueOnApply': '应用时触发的Cue',
    'CueOnTick': '帧更新的Cue',
    'CueOnAdd': '添加时触发的Cue',
    'CueOnRemove': '移除时触发的Cue',
    'CueOnActivate': '激活时触发的Cue',
    'CueOnDeactivate': '失活时触发的Cue',
    'GrantedAbility': '获取技能',
    'Stacking': 'buff堆叠'
};

// ═══════════════════════════════════════════════════════════════════  
// § 2  工具函数  
// ═══════════════════════════════════════════════════════════════════  

function escHtml(s) {
    return String(s ?? '')
        .replace(/&/g,'&amp;').replace(/</g,'&lt;')
        .replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}

function setStatus(msg, type = '') {
    const el = document.getElementById('status-msg');
    el.textContent = msg;
    el.className = type;
}

function setCount(n) {
    document.getElementById('status-count').textContent = `共 ${n} 个Effect`;
}

/** 读取当前激活的组件列表（从 DOM 中 .component-chip.active 读取） */
function getActiveComponents() {
    return [...document.querySelectorAll('.component-chip.active')]
        .map(el => el.dataset.comp);
}

/** 枚举按钮组点击：同组互斥激活 */
function setEnumBtn(groupId, btn, value) {
    document.querySelectorAll(`#${groupId} button`).forEach(b => b.classList.remove('active'));
    btn.classList.add('active');
}

/** 读取枚举按钮组当前选中值 */
function getEnumBtnValue(groupId, defaultVal = '') {
    const active = document.querySelector(`#${groupId} button.active`);
    return active ? active.textContent.trim() : defaultVal;
}

/** 切换组件开关：chip 点击 → 重新渲染详情区 */
function toggleComponent(chip, compName) {
    chip.classList.toggle('active');
    const effect = buildEffectFromForm();
    renderCompDetails(effect);
}

/** Tab 切换 */
function switchTab(tabId, btn) {
    document.querySelectorAll('.tab-content').forEach(t => t.classList.remove('active'));
    document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
    document.getElementById(tabId).classList.add('active');
    btn.classList.add('active');
    // 切到详情 tab 时刷新详情  
    if (tabId === 'tab-details') {
        const effect = buildEffectFromForm();
        renderCompDetails(effect);
    }
}

// ── Tag Chip（主要显示 name，ID 为辅，对齐 C# GasXlsxChoice.Tags()）──  
function tagChipHtml(listId, id, choices) {
    const item = choices.find(c => c.id === id);
    const label = item
        ? `${escHtml(item.name)} <span style="opacity:.5;font-size:10px">#${id}</span>`
        : `#${id}`;
    return `<span class="chip" data-id="${id}">  
        ${label}  
        <button class="chip-del" onclick="removeChip('${listId}',${id})">✕</button>  
    </span>`;
}

/** 生成 Chip 的 HTML（Cue/Effect 通用，显示 [id] name） */
function chipHtml(listId, id, choices) {
    const item = choices.find(c => c.id === id);
    const label = item ? `[${id}] ${escHtml(item.name)}` : `[${id}]`;
    return `<span class="chip" data-id="${id}">  
        ${label}  
        <button class="chip-del" onclick="removeChip('${listId}',${id})">✕</button>  
    </span>`;
}

/** 从 chip-list 中移除一个 chip */
function removeChip(listId, id) {
    const container = document.getElementById(`chips-${listId}`);
    if (!container) return;
    const chip = container.querySelector(`.chip[data-id="${id}"]`);
    if (chip) chip.remove();
}

/** 把 select 中选中的值添加为 chip（支持 tag 格式和通用格式） */
function addChip(listId, selId, choices, useTagFormat) {
    const sel = document.getElementById(selId);
    if (!sel || !sel.value) return;
    const id = parseInt(sel.value);
    const container = document.getElementById(`chips-${listId}`);
    if (!container) return;
    // 防重复  
    if (container.querySelector(`.chip[data-id="${id}"]`)) return;
    const html = useTagFormat ? tagChipHtml(listId, id, choices) : chipHtml(listId, id, choices);
    container.insertAdjacentHTML('beforeend', html);
    sel.value = '';
}

/** 读取 chip-list 中所有 id */
function readChipIds(listId) {
    const container = document.getElementById(`chips-${listId}`);
    if (!container) return [];
    return [...container.querySelectorAll('.chip[data-id]')]
        .map(el => parseInt(el.dataset.id));
}

// ── 可搜索下拉组件 ────────────────────────────────────────────────────  
let _sdCounter = 0;

/**
 * 生成可搜索下拉 HTML
 * @param {string} selectId - 兼容旧 addChip 的 select id
 * @param {string} placeholder
 * @param {Array} choices - [{id, name}]
 * @param {string} displayMode - 'tag' 显示 name(#id), 'default' 显示 [id] name
 * @returns {string} HTML
 */
function searchableDropdownHtml(selectId, placeholder, choices, displayMode) {
    const uid = `sd-${_sdCounter++}`;
    const mode = displayMode || 'default';
    return `  
    <div class="search-dropdown" id="${uid}" data-sd-select-id="${selectId}" data-sd-mode="${mode}">  
        <input class="sd-input" type="text" placeholder="${escHtml(placeholder)}"  
            onfocus="sdOpen('${uid}')"  
            oninput="sdFilter('${uid}', this.value)">  
        <div class="sd-list">  
            ${choices.map(c => {
        const display = mode === 'tag'
            ? `${escHtml(c.name)} (#${c.id})`
            : `[${c.id}] ${escHtml(c.name)}`;
        return `<div class="sd-item" data-value="${c.id}" onclick="sdSelect('${uid}',${c.id})">${display}</div>`;
    }).join('')}  
        </div>  
        <select id="${selectId}" style="display:none">  
            <option value=""></option>  
            ${choices.map(c => `<option value="${c.id}">${c.id}</option>`).join('')}  
        </select>  
    </div>`;
}

function sdOpen(uid) {
    // 关闭其他  
    document.querySelectorAll('.search-dropdown.open').forEach(d => {
        if (d.id !== uid) d.classList.remove('open');
    });
    const el = document.getElementById(uid);
    if (el) el.classList.add('open');
}

function sdFilter(uid, keyword) {
    const el = document.getElementById(uid);
    if (!el) return;
    el.classList.add('open');
    const kw = keyword.toLowerCase();
    el.querySelectorAll('.sd-item').forEach(item => {
        const text = item.textContent.toLowerCase();
        item.style.display = text.includes(kw) ? '' : 'none';
    });
}

function sdSelect(uid, value) {
    const el = document.getElementById(uid);
    if (!el) return;
    // 设置隐藏 select 的值  
    const selId = el.dataset.sdSelectId;
    const hiddenSel = document.getElementById(selId);
    if (hiddenSel) hiddenSel.value = value;
    // 清空输入 & 关闭  
    el.querySelector('.sd-input').value = '';
    el.classList.remove('open');
}

// 全局点击关闭下拉  
document.addEventListener('click', e => {
    if (!e.target.closest('.search-dropdown')) {
        document.querySelectorAll('.search-dropdown.open').forEach(d => d.classList.remove('open'));
    }
});

// ═══════════════════════════════════════════════════════════════════  
// § 3  数据加载（API 通信）  
// ═══════════════════════════════════════════════════════════════════  

async function loadInfo() {
    try {
        const r = await fetch(`${API}/api/info`).then(resp => resp.json());
        if (r.ok) document.getElementById('xlsx-path').textContent = r.xlsx;
    } catch(e) {}
}

async function loadChoices() {
    try {
        const [tags, cues, abilities, attrsets, effects, mmcs] = await Promise.all([
            fetch(`${API}/api/choices/tags`).then(r => r.json()),
            fetch(`${API}/api/choices/cues`).then(r => r.json()),
            fetch(`${API}/api/choices/abilities`).then(r => r.json()),
            fetch(`${API}/api/choices/attrsets`).then(r => r.json()),
            fetch(`${API}/api/choices/effects`).then(r => r.json()),
            fetch(`${API}/api/choices/mmcs`).then(r => r.json()).catch(() => ({ok:false})),
        ]);
        if (tags.ok)      tagChoices     = tags.tags       || [];
        if (cues.ok)      cueChoices     = cues.cues       || [];
        if (abilities.ok) abilityChoices = abilities.abilities || [];
        if (attrsets.ok)  attrSetChoices = attrsets.attrsets  || [];
        if (effects.ok)   effectChoices  = effects.effects    || [];
        if (mmcs.ok)      mmcChoices     = mmcs.mmcs          || [];
    } catch(e) { console.warn('loadChoices failed', e); }
}

async function loadEffects() {
    try {
        const r = await fetch(`${API}/api/effects`).then(resp => resp.json());
        if (!r.ok) throw new Error(r.error);
        allEffects = r.effects;
        // 同步 effectChoices（供 Period.effects / Stacking.overflowEffects 下拉）  
        effectChoices = allEffects.map(e => ({ id: e.id, name: e.name }));
        renderList();
        setCount(allEffects.length);
        setStatus('已加载', 'ok');
    } catch(e) {
        setStatus('加载失败: ' + e.message, 'err');
    }
}

// ═══════════════════════════════════════════════════════════════════  
// § 4  列表渲染 + 表单骨架渲染  
// ═══════════════════════════════════════════════════════════════════  

function renderList() {
    const container = document.getElementById('effect-list');
    const kw = searchText.toLowerCase();
    const filtered = allEffects.filter(e =>
        e.name.toLowerCase().includes(kw) || String(e.id).includes(kw)
    );
    container.innerHTML = filtered.map(e => `  
        <div class="effect-item ${e.id === selectedId ? 'selected' : ''}"  
             onclick="App.selectEffect(${e.id})">  
            <span class="effect-item-name">${escHtml(e.name)}</span>  
            <span class="effect-item-id">#${e.id}</span>  
        </div>  
    `).join('');
}

function renderForm(effect) {
    _currentEffect = JSON.parse(JSON.stringify(effect));
    document.getElementById('form-title').textContent = `[${effect.id}] ${effect.name}`;
    document.getElementById('btn-delete').style.display = '';
    document.getElementById('btn-save').style.display   = '';

    const body = document.getElementById('form-body');
    body.className = '';
    const active = new Set(effect.components || []);

    body.innerHTML = `  
<div class="panel">  
    <div class="panel-header">基础信息</div>  
    <div class="panel-body">  
        <div class="inline-fields">  
            <div class="field-group">  
                <label>ID</label>  
                <input id="f-id" type="number" value="${effect.id}" class="field-readonly" readonly>  
            </div>  
            <div class="field-group" style="flex:3">  
                <label>名称</label>  
                <input id="f-name" type="text" value="${escHtml(effect.name)}">  
            </div>  
        </div>  
        <div class="field-group">  
            <label>描述</label>  
            <textarea id="f-desc">${escHtml(effect.desc)}</textarea>  
        </div>  
    </div>  
</div>  
  
<div class="tab-bar">  
    <button class="tab-btn active" onclick="switchTab('tab-components', this)">组件列表</button>  
    <button class="tab-btn" onclick="switchTab('tab-details', this)">详情</button>  
</div>  
  
<div id="tab-components" class="tab-content active">  
    <div class="panel">  
        <div class="panel-header">组件开关（点击切换）</div>  
        <div class="panel-body">  
            <div class="component-section">  
                <h3>Tag 组件</h3>  
                <div class="component-grid">  
                    ${TAG_COMPONENTS.map(c => `  
                        <div class="component-chip ${active.has(c)?'active':''}"  
                             onclick="toggleComponent(this,'${c}')" data-comp="${c}">${c}<small>${COMP_LABELS[c]||''}</small></div>  
                    `).join('')}  
                </div>  
            </div>  
            <div class="component-section">  
                <h3>功能组件</h3>  
                <div class="component-grid">  
                    ${FUNC_COMPONENTS.map(c => `  
                        <div class="component-chip ${active.has(c)?'active':''}"  
                             onclick="toggleComponent(this,'${c}')" data-comp="${c}">${c}<small>${COMP_LABELS[c]||''}</small></div>  
                    `).join('')}  
                </div>  
            </div>  
            <div class="component-section">  
                <h3>Cue 组件</h3>  
                <div class="component-grid">  
                    ${CUE_COMPONENTS.map(c => `  
                        <div class="component-chip ${active.has(c)?'active':''}"  
                             onclick="toggleComponent(this,'${c}')" data-comp="${c}">${c}<small>${COMP_LABELS[c]||''}</small></div>  
                    `).join('')}  
                </div>  
            </div>  
        </div>  
    </div>  
</div>  
  
<div id="tab-details" class="tab-content">  
    <div id="comp-details"></div>  
</div>  
`;
    renderCompDetails(effect);
}

// ── Modifier 单项 HTML ────────────────────────────────────────────────  
function modifierItemHtml(m, i) {
    const attrSetOpts = attrSetChoices.map(as =>
        `<option value="${as.id}" ${as.id===m.attrSet?'selected':''}>[${as.id}] ${escHtml(as.name)}</option>`
    ).join('');
    const as = attrSetChoices.find(a => a.id === m.attrSet);
    const attrOpts = (as ? as.attrs || [] : []).map(a =>
        `<option value="${a.id}" ${a.id===m.attr?'selected':''}>[${a.id}] ${escHtml(a.name)}</option>`
    ).join('');

    const mmcVal = m.mmc ?? 0;
    const mmcOpts = mmcChoices.map(mc =>
        `<option value="${mc.id}" ${mc.id===mmcVal?'selected':''}>[${mc.id}] ${escHtml(mc.name)}</option>`
    ).join('');

    return `  
<div class="modifier-item" data-idx="${i}">  
    <button class="remove-btn" onclick="removeModifier(${i})">✕</button>  
    <div class="inline-fields">  
        <div class="field-group">  
            <label>属性集 AttrSet</label>  
            <select onchange="onAttrSetChange(this,${i})" data-mod-attrset="${i}">  
                <option value="">-- 选择 --</option>  
                ${attrSetOpts}  
            </select>  
        </div>  
        <div class="field-group">  
            <label>属性 Attr</label>  
            <select data-mod-attr="${i}">  
                <option value="">-- 选择 --</option>  
                ${attrOpts}  
            </select>  
        </div>  
    </div>  
    <div class="inline-fields">  
        <div class="field-group">  
            <label>数值 Magnitude</label>  
            <input type="number" step="any" data-mod-magnitude="${i}" value="${m.magnitude ?? 0}">  
        </div>  
        <div class="field-group">  
            <label>MMC</label>  
            <select data-mod-mmc="${i}">  
                <option value="0">-- 无MMC --</option>  
                ${mmcOpts}  
            </select>  
        </div>  
    </div>  
    <div class="field-group">  
        <label>运算 Operation</label>  
        <div class="enum-btns" id="mod-op-btns-${i}">  
            ${ENUM_OPERATION.map(op =>
        `<button onclick="setEnumBtn('mod-op-btns-${i}',this,'${op}')" class="${m.operation===op?'active':''}">${op}</button>`
    ).join('')}  
        </div>  
    </div>  
</div>`;
}

function onAttrSetChange(sel, idx) {
    const asId = parseInt(sel.value);
    const as = attrSetChoices.find(a => a.id === asId);
    const attrSel = document.querySelector(`[data-mod-attr="${idx}"]`);
    if (!attrSel) return;
    attrSel.innerHTML = '<option value="">-- 选择 --</option>' +
        (as ? as.attrs || [] : []).map(a =>
            `<option value="${a.id}">[${a.id}] ${escHtml(a.name)}</option>`
        ).join('');
}

function addModifier() {
    const list = document.getElementById('modifier-list');
    if (!list) return;
    const idx = list.querySelectorAll('.modifier-item').length;
    list.insertAdjacentHTML('beforeend', modifierItemHtml({
        attrSet:0, attr:0, magnitude:0, operation:'Add', mmc:0
    }, idx));
}

function removeModifier(i) {
    const el = document.querySelector(`.modifier-item[data-idx="${i}"]`);
    if (el) el.remove();
}

// ── GrantedAbility 单项 HTML ──────────────────────────────────────────  
function abilityItemHtml(a, i) {
    const abilityOpts = abilityChoices.map(ab =>
        `<option value="${ab.id}" ${ab.id===a.abilityId?'selected':''}>[${ab.id}] ${escHtml(ab.name)}</option>`
    ).join('');

    return `  
<div class="ability-item" data-idx="${i}">  
    <button class="remove-btn" onclick="removeGrantedAbility(${i})">✕</button>  
    <div class="inline-fields">  
        <div class="field-group" style="flex:3">  
            <label>Ability</label>  
            <select data-ab-id="${i}">  
                <option value="">-- 选择 --</option>  
                ${abilityOpts}  
            </select>  
        </div>  
        <div class="field-group">  
            <label>等级 Level</label>  
            <input type="number" data-ab-level="${i}" value="${a.level ?? 1}" min="1">  
        </div>  
    </div>  
    <div class="field-group">  
        <label>激活策略 ActivationPolicy</label>  
        <div class="enum-btns" id="ab-act-btns-${i}">  
            ${ENUM_ABILITY_ACTIVATE.map(v =>
        `<button onclick="setEnumBtn('ab-act-btns-${i}',this,'${v}')" class="${a.activationPolicy===v?'active':''}">${v}</button>`
    ).join('')}  
        </div>  
    </div>  
    <div class="field-group">  
        <label>失活策略 DeactivationPolicy</label>  
        <div class="enum-btns" id="ab-deact-btns-${i}">  
            ${ENUM_ABILITY_DEACTIVATE.map(v =>
        `<button onclick="setEnumBtn('ab-deact-btns-${i}',this,'${v}')" class="${a.deactivationPolicy===v?'active':''}">${v}</button>`
    ).join('')}  
        </div>  
    </div>  
    <div class="field-group">  
        <label>移除策略 RemovePolicy</label>  
        <div class="enum-btns" id="ab-rem-btns-${i}">  
            ${ENUM_ABILITY_REMOVE.map(v =>
        `<button onclick="setEnumBtn('ab-rem-btns-${i}',this,'${v}')" class="${a.removePolicy===v?'active':''}">${v}</button>`
    ).join('')}  
        </div>  
    </div>  
</div>`;
}

function addGrantedAbility() {
    const list = document.getElementById('ability-list');
    if (!list) return;
    const idx = list.querySelectorAll('.ability-item').length;
    list.insertAdjacentHTML('beforeend', abilityItemHtml({
        abilityId:0, level:1,
        activationPolicy:'None', deactivationPolicy:'None', removePolicy:'None'
    }, idx));
}

function removeGrantedAbility(i) {
    const el = document.querySelector(`.ability-item[data-idx="${i}"]`);
    if (el) el.remove();
}

// ═══════════════════════════════════════════════════════════════════  
// § 5-A  renderCompDetails（Tag类 → Stacking → Cue类）  
// ═══════════════════════════════════════════════════════════════════  

function renderCompDetails(effect) {
    const active = new Set(getActiveComponents());
    const container = document.getElementById('comp-details');
    if (!container) return;
    let html = '';

    // ── Tag 类（使用 searchableDropdown + tagChipHtml）──────────────────  
    for (const field of TAG_PROTOCOL_FIELDS) {
        const comp = field.comp;
        if (!active.has(comp)) continue;
        const ids = effect[field.key] || [];
        const filteredChoices = tagChoices.filter(t => !ids.includes(t.id));
        html += `  
<div class="panel" id="panel-${comp}">  
    <div class="panel-header">${field.label}</div>  
    <div class="panel-body">  
        <div class="chip-list" id="chips-${comp}">  
            ${ids.map(id => tagChipHtml(comp, id, tagChoices)).join('')}  
        </div>  
        <div class="add-chip-row">  
            ${searchableDropdownHtml(`sel-${comp}`, '搜索Tag...', filteredChoices, 'tag')}  
            <button class="btn btn-success btn-sm"  
                onclick="addChip('${comp}','sel-${comp}',tagChoices,true)">添加</button>  
        </div>  
    </div>  
</div>`;
    }

    // ── Duration ──────────────────────────────────────────────────────  
    if (active.has('Duration')) {
        const dur = effect.duration || { unit:'Frame', time:0, resetStartTimeWhenActivated:false };
        html += `  
<div class="panel" id="panel-Duration">  
    <div class="panel-header">Duration 持续时间</div>  
    <div class="panel-body">  
        <div class="warn-box ${dur.time===-1?'show':''}" id="dur-inf-warn">  
            ⚠ time=-1 表示 Infinite（无限持续）  
        </div>  
        <div class="inline-fields">  
            <div class="field-group">  
                <label>时间单位</label>  
                <div class="enum-btns" id="dur-unit-btns">  
                    ${ENUM_TIME_UNIT.map(u =>
            `<button onclick="setEnumBtn('dur-unit-btns',this,'${u}')"  
                            class="${dur.unit===u?'active':''}">${u}</button>`
        ).join('')}  
                </div>  
            </div>  
            <div class="field-group">  
                <label>时长（-1=无限）</label>  
                <input id="dur-time" type="number" value="${dur.time}"  
                    oninput="document.getElementById('dur-inf-warn').classList.toggle('show',+this.value===-1)">  
            </div>  
        </div>  
        <div class="field-group">  
            <label>  
                <input type="checkbox" id="dur-reset" ${dur.resetStartTimeWhenActivated?'checked':''}>  
                &nbsp;激活时重置计时 (ResetStartTimeWhenActivated)  
            </label>  
        </div>  
    </div>  
</div>`;
    }

    // ── Period ────────────────────────────────────────────────────────  
    if (active.has('Period')) {
        const per = effect.period || { time:0, effects:[], firstTrigger:false };
        const filteredEffects = effectChoices.filter(e => !(per.effects||[]).includes(e.id));
        html += `  
<div class="panel" id="panel-Period">  
    <div class="panel-header">Period 周期执行</div>  
    <div class="panel-body">  
        <div class="warn-box ${!active.has('Duration')?'show':''}">  
            ⚠ Period 需要 Duration 组件才会生效！  
        </div>  
        <div class="inline-fields">  
            <div class="field-group">  
                <label>周期间隔（帧/回合）</label>  
                <input id="per-time" type="number" value="${per.time}">  
            </div>  
            <div class="field-group" style="align-self:flex-end">  
                <label>  
                    <input type="checkbox" id="per-first" ${per.firstTrigger?'checked':''}>  
                    &nbsp;首次立即触发 (FirstTrigger)  
                </label>  
            </div>  
        </div>  
        <div class="field-group">  
            <label>周期执行的 Effect IDs</label>  
            <div class="chip-list" id="chips-Period-effects">  
                ${(per.effects||[]).map(id => chipHtml('Period-effects', id, effectChoices)).join('')}  
            </div>  
            <div class="add-chip-row">  
                ${searchableDropdownHtml('sel-Period-effects', '搜索Effect...', filteredEffects, 'default')}  
                <button class="btn btn-success btn-sm"  
                    onclick="addChip('Period-effects','sel-Period-effects',effectChoices,false)">添加</button>  
            </div>  
        </div>  
    </div>  
</div>`;
    }

    // ── Modifiers ─────────────────────────────────────────────────────  
    if (active.has('Modifiers')) {
        const mods = effect.modifiers || [];
        html += `  
<div class="panel" id="panel-Modifiers">  
    <div class="panel-header">  
        Modifiers 属性修改器  
        <button class="btn btn-success btn-sm" onclick="addModifier()">＋ 新增</button>  
    </div>  
    <div class="panel-body" id="modifier-list">  
        ${mods.map((m, i) => modifierItemHtml(m, i)).join('')}  
    </div>  
</div>`;
    }

    // ── GrantedAbility ────────────────────────────────────────────────  
    if (active.has('GrantedAbility')) {
        const abs = effect.grantedAbilities || [];
        html += `  
<div class="panel" id="panel-GrantedAbility">  
    <div class="panel-header">  
        GrantedAbility 授予技能  
        <button class="btn btn-success btn-sm" onclick="addGrantedAbility()">＋ 新增</button>  
    </div>  
    <div class="panel-body" id="ability-list">  
        ${abs.map((a, i) => abilityItemHtml(a, i)).join('')}  
    </div>  
</div>`;
    }

    // ── Stacking ──────────────────────────────────────────────────────  
    if (active.has('Stacking')) {
        const st = effect.stacking || {
            code: 0, stackingType: 'AggregateBySource', limitCount: 1,
            durationRefreshPolicy: 'NeverRefresh', periodResetPolicy: 'NeverRefresh',
            expirationPolicy: 'ClearEntireStack',
            denyOverflowApplication: false, clearStackOnOverflow: false, overflowEffects: []
        };
        const filteredOverflow = effectChoices.filter(e => !(st.overflowEffects || []).includes(e.id));
        html += `  
<div class="panel" id="panel-Stacking">  
    <div class="panel-header">Stacking 层数叠加</div>  
    <div class="panel-body">  
        <div class="warn-box ${!active.has('Duration') ? 'show' : ''}">  
            ⚠ Stacking 需要 Duration 组件才会生效！  
        </div>  
        <div class="inline-fields">  
            <div class="field-group">  
                <label>堆叠码 (code)</label>  
                <input id="st-code" type="number" value="${st.code}">  
            </div>  
            <div class="field-group">  
                <label>上限 (limitCount)</label>  
                <input id="st-limit" type="number" value="${st.limitCount}">  
            </div>  
        </div>  
        <div class="field-group">  
            <label>堆叠类型 (stackingType)</label>  
            <div class="enum-btns" id="st-type-btns">  
                ${ENUM_STACKING_TYPE.map(v =>
            `<button onclick="setEnumBtn('st-type-btns',this,'${v}')"  
                        class="${st.stackingType === v ? 'active' : ''}">${v}</button>`
        ).join('')}  
            </div>  
        </div>  
        <div class="field-group">  
            <label>Duration刷新策略 (durationRefreshPolicy)</label>  
            <div class="enum-btns" id="st-dur-btns">  
                ${ENUM_DURATION_REFRESH.map(v =>
            `<button onclick="setEnumBtn('st-dur-btns',this,'${v}')"  
                        class="${st.durationRefreshPolicy === v ? 'active' : ''}">${v}</button>`
        ).join('')}  
            </div>  
        </div>  
        <div class="field-group">  
            <label>Period重置策略 (periodResetPolicy)</label>  
            <div class="enum-btns" id="st-per-btns">  
                ${ENUM_PERIOD_RESET.map(v =>
            `<button onclick="setEnumBtn('st-per-btns',this,'${v}')"  
                        class="${st.periodResetPolicy === v ? 'active' : ''}">${v}</button>`
        ).join('')}  
            </div>  
        </div>  
        <div class="field-group">  
            <label>过期策略 (expirationPolicy)</label>  
            <div class="enum-btns" id="st-exp-btns">  
                ${ENUM_EXPIRATION.map(v =>
            `<button onclick="setEnumBtn('st-exp-btns',this,'${v}')"  
                        class="${st.expirationPolicy === v ? 'active' : ''}">${v}</button>`
        ).join('')}  
            </div>  
        </div>  
        <div class="field-group">  
            <label>  
                <input type="checkbox" id="st-deny" ${st.denyOverflowApplication ? 'checked' : ''}>  
                &nbsp;拒绝溢出应用 (denyOverflowApplication)  
            </label>  
        </div>  
        <div class="field-group">  
            <label>  
                <input type="checkbox" id="st-clear" ${st.clearStackOnOverflow ? 'checked' : ''}>  
                &nbsp;溢出时清空层数 (clearStackOnOverflow)  
            </label>  
        </div>  
        <div class="field-group">  
            <label>溢出触发的 Effect IDs</label>  
            <div class="chip-list" id="chips-st-overflow">  
                ${(st.overflowEffects || []).map(id => chipHtml('st-overflow', id, effectChoices)).join('')}  
            </div>  
            <div class="add-chip-row">  
                ${searchableDropdownHtml('sel-st-overflow', '搜索Effect...', filteredOverflow, 'default')}  
                <button class="btn btn-success btn-sm"  
                    onclick="addChip('st-overflow','sel-st-overflow',effectChoices,false)">添加</button>  
            </div>  
        </div>  
    </div>  
</div>`;
    }

    // ── Cue 类（使用 searchableDropdown）──────────────────────────────  
    const cueFieldMap = {
        'CueOnApply':      { key:'cueOnApply',      label:'CueOnApply（Instant触发）' },
        'CueOnTick':       { key:'cueOnTick',        label:'CueOnTick（每帧）' },
        'CueOnAdd':        { key:'cueOnAdd',         label:'CueOnAdd（添加时）' },
        'CueOnRemove':     { key:'cueOnRemove',      label:'CueOnRemove（移除时）' },
        'CueOnActivate':   { key:'cueOnActivate',    label:'CueOnActivate（激活时）' },
        'CueOnDeactivate': { key:'cueOnDeactivate',  label:'CueOnDeactivate（失活时）' },
    };
    for (const [comp, meta] of Object.entries(cueFieldMap)) {
        if (!active.has(comp)) continue;
        const ids = effect[meta.key] || [];
        const filteredCues = cueChoices.filter(c => !ids.includes(c.id));
        html += `  
<div class="panel" id="panel-${comp}">  
    <div class="panel-header">${meta.label}</div>  
    <div class="panel-body">  
        <div class="chip-list" id="chips-${comp}">  
            ${ids.map(id => chipHtml(comp, id, cueChoices)).join('')}  
        </div>  
        <div class="add-chip-row">  
            ${searchableDropdownHtml(`sel-${comp}`, '搜索Cue...', filteredCues, 'default')}  
            <button class="btn btn-success btn-sm"  
                onclick="addChip('${comp}','sel-${comp}',cueChoices,false)">添加</button>  
        </div>  
    </div>  
</div>`;
    }

    // ── 写入 DOM ──────────────────────────────────────────────────────  
    container.innerHTML = html;
}
// ═══ § 5 结束 ═══════════════════════════════════════════════════════  

// ═══════════════════════════════════════════════════════════════════  
// § 6  buildEffectFromForm + App 对象 + 初始化  
// ═══════════════════════════════════════════════════════════════════  

/** 从当前 DOM 表单中收集数据，构建 effect 对象 */
function buildEffectFromForm() {
    const effect = _currentEffect
        ? JSON.parse(JSON.stringify(_currentEffect))
        : { id: 0, name: '', desc: '', components: [] };

    // 基础信息  
    const nameEl = document.getElementById('f-name');
    const descEl = document.getElementById('f-desc');
    if (nameEl) effect.name = nameEl.value.trim();
    if (descEl) effect.desc = descEl.value.trim();

    // 激活组件  
    effect.components = getActiveComponents();
    const active = new Set(effect.components);

    // ── Tag 类 ────────────────────────────────────────────────────────  
    for (const field of TAG_PROTOCOL_FIELDS) {
        effect[field.key] = active.has(field.comp) ? readChipIds(field.comp) : [];
    }

    // ── Duration ──────────────────────────────────────────────────────  
    if (active.has('Duration')) {
        effect.duration = {
            unit: getEnumBtnValue('dur-unit-btns', 'Frame'),
            time: parseInt(document.getElementById('dur-time')?.value ?? 0),
            resetStartTimeWhenActivated: document.getElementById('dur-reset')?.checked ?? false,
        };
    } else {
        effect.duration = null;
    }

    // ── Period ────────────────────────────────────────────────────────  
    if (active.has('Period')) {
        effect.period = {
            time: parseInt(document.getElementById('per-time')?.value ?? 0),
            effects: readChipIds('Period-effects'),
            firstTrigger: document.getElementById('per-first')?.checked ?? false,
        };
    } else {
        effect.period = null;
    }

    // ── Modifiers ─────────────────────────────────────────────────────  
    if (active.has('Modifiers')) {
        const items = document.querySelectorAll('.modifier-item');
        effect.modifiers = Array.from(items).map(el => {
            const idx = el.dataset.idx;
            return {
                attrSet:   parseInt(el.querySelector(`[data-mod-attrset="${idx}"]`)?.value || 0),
                attr:      parseInt(el.querySelector(`[data-mod-attr="${idx}"]`)?.value || 0),
                magnitude: parseFloat(el.querySelector(`[data-mod-magnitude="${idx}"]`)?.value || 0),
                operation: getEnumBtnValue(`mod-op-btns-${idx}`, 'Add'),
                mmc:       parseInt(el.querySelector(`[data-mod-mmc="${idx}"]`)?.value || 0),
            };
        });
    } else {
        effect.modifiers = [];
    }

    // ── GrantedAbility ────────────────────────────────────────────────  
    if (active.has('GrantedAbility')) {
        const items = document.querySelectorAll('.ability-item');
        effect.grantedAbilities = Array.from(items).map(el => {
            const idx = el.dataset.idx;
            return {
                abilityId:          parseInt(el.querySelector(`[data-ab-id="${idx}"]`)?.value || 0),
                level:              parseInt(el.querySelector(`[data-ab-level="${idx}"]`)?.value || 1),
                activationPolicy:   getEnumBtnValue(`ab-act-btns-${idx}`, 'None'),
                deactivationPolicy: getEnumBtnValue(`ab-deact-btns-${idx}`, 'None'),
                removePolicy:       getEnumBtnValue(`ab-rem-btns-${idx}`, 'None'),
            };
        });
    } else {
        effect.grantedAbilities = [];
    }

    // ── Stacking ──────────────────────────────────────────────────────  
    if (active.has('Stacking')) {
        effect.stacking = {
            code:                    parseInt(document.getElementById('st-code')?.value || 0),
            stackingType:            getEnumBtnValue('st-type-btns', 'AggregateBySource'),
            limitCount:              parseInt(document.getElementById('st-limit')?.value || 1),
            durationRefreshPolicy:   getEnumBtnValue('st-dur-btns', 'NeverRefresh'),
            periodResetPolicy:       getEnumBtnValue('st-per-btns', 'NeverRefresh'),
            expirationPolicy:        getEnumBtnValue('st-exp-btns', 'ClearEntireStack'),
            denyOverflowApplication: document.getElementById('st-deny')?.checked ?? false,
            clearStackOnOverflow:    document.getElementById('st-clear')?.checked ?? false,
            overflowEffects:         readChipIds('st-overflow'),
        };
    } else {
        effect.stacking = null;
    }

    // ── Cue 类 ────────────────────────────────────────────────────────  
    const cueKeyMap = {
        'CueOnApply':      'cueOnApply',
        'CueOnTick':       'cueOnTick',
        'CueOnAdd':        'cueOnAdd',
        'CueOnRemove':     'cueOnRemove',
        'CueOnActivate':   'cueOnActivate',
        'CueOnDeactivate': 'cueOnDeactivate',
    };
    for (const [comp, key] of Object.entries(cueKeyMap)) {
        effect[key] = active.has(comp) ? readChipIds(comp) : [];
    }

    return effect;
}

// ── clearForm ─────────────────────────────────────────────────────────  
function clearForm() {
    document.getElementById('form-title').textContent = '请从左侧选择一个Effect';
    document.getElementById('btn-delete').style.display = 'none';
    document.getElementById('btn-save').style.display   = 'none';
    const body = document.getElementById('form-body');
    body.className = 'empty-hint';
    body.textContent = '← 从左侧选择一个Effect进行编辑';
    _currentEffect = null;
}

// ── App 命名空间（供 HTML onclick 调用）──────────────────────────────  
const App = {
    async reload() {
        selectedId = null;
        clearForm();
        loadInfo();
        await loadChoices();
        await loadEffects();
    },

    onSearch(val) {
        searchText = val;
        renderList();
    },

    selectEffect(id) {
        selectedId = id;
        renderList();
        const effect = allEffects.find(e => e.id === id);
        if (effect) renderForm(effect);
    },

    async saveSelected() {
        if (selectedId == null) return;
        const effect = buildEffectFromForm();
        if (!effect.name) { setStatus('Effect名称不能为空', 'err'); return; }

        setStatus('保存中...');
        try {
            const r = await fetch(`${API}/api/effects/${selectedId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(effect),
            }).then(resp => resp.json());
            if (!r.ok) throw new Error(r.error);
            setStatus('保存成功', 'ok');
            await loadEffects();
            App.selectEffect(selectedId);
        } catch (e) {
            setStatus('保存失败: ' + e.message, 'err');
        }
    },

    async deleteSelected() {
        if (selectedId == null) return;
        const effect = allEffects.find(e => e.id === selectedId);
        if (!effect) return;
        if (!confirm(`确认删除 Effect「${effect.name}」(ID: ${effect.id})？`)) return;

        setStatus('删除中...');
        try {
            const r = await fetch(`${API}/api/effects/${selectedId}`, {
                method: 'DELETE',
            }).then(resp => resp.json());
            if (!r.ok) throw new Error(r.error);
            selectedId = null;
            clearForm();
            setStatus('删除成功', 'ok');
            await loadEffects();
        } catch (e) {
            setStatus('删除失败: ' + e.message, 'err');
        }
    },

    addEffect() {
        document.getElementById('modal-id').value   = '';
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
        if (!name) { setStatus('Effect名称不能为空', 'err'); return; }

        const payload = { name, desc, components: [] };
        if (idVal !== '') payload.id = parseInt(idVal);

        setStatus('新增中...');
        try {
            const r = await fetch(`${API}/api/effects`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload),
            }).then(resp => resp.json());
            if (!r.ok) throw new Error(r.error);
            App.closeModal();
            setStatus('新增成功', 'ok');
            await loadEffects();
            App.selectEffect(r.effect.id);
        } catch (e) {
            setStatus('新增失败: ' + e.message, 'err');
        }
    },
};

// ── 键盘快捷键 ────────────────────────────────────────────────────────  
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

// ── 初始化 ────────────────────────────────────────────────────────────  
(async function init() {
    loadInfo();
    await loadChoices();
    await loadEffects();
})();
