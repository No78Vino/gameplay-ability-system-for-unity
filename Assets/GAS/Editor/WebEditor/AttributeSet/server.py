#!/usr/bin/env python3  
# -*- coding: utf-8 -*-  
"""  
EX-GAS AttributeSet 网页编辑器 - 本地 HTTP 服务  
依赖: pip install openpyxl  
用法: python server.py --xlsx "path/to/#exgas.attributeSet.xlsx"  
"""  
  
import argparse, json, os, sys, threading, webbrowser  
from http.server import BaseHTTPRequestHandler, HTTPServer  
from urllib.parse import urlparse, parse_qs  
  
try:  
    import openpyxl  
except ImportError:  
    print("[ERROR] 请先运行: pip install openpyxl")  
    sys.exit(1)  
  
# ── 常量 ────────────────────────────────────────────────────────────────────  
DATA_START_ROW = 5   # 数据从第5行开始（1-3行为表头+Luban类型定义）  
COL_ID         = 2   # 第2列: ID  
COL_NAME       = 3   # 第3列: Name  
COL_DESC       = 4   # 第4列: Desc  
COL_ATTRIBUTE  = 5   # 第5列: Attribute（分号分隔，每项格式: ID,InitValue,MinValue,MaxValue,UseMinValue,UseMaxValue）  
  
XLSX_PATH = ""       # 由命令行参数注入  
  
  
# ── Excel 读写 ───────────────────────────────────────────────────────────────  
def read_attrsets():  
    wb = openpyxl.load_workbook(XLSX_PATH)  
    ws = wb.active  
    result = []  
    for row in ws.iter_rows(min_row=DATA_START_ROW, values_only=True):  
        id_val = row[COL_ID - 1]  
        if id_val is None:  
            break  
        # 跳过非整数行（如残留的表头行）  
        try:  
            id_int = int(id_val)  
        except (ValueError, TypeError):  
            continue  
            
        name_val  = row[COL_NAME - 1] or ""  
        desc_val  = row[COL_DESC - 1] or ""  
        attr_val  = row[COL_ATTRIBUTE - 1] or ""  
  
        # 解析 Attribute 列：分号分隔，每项逗号分隔  
        attributes = []  
        if attr_val:  
            for item in str(attr_val).split(";"):  
                item = item.strip()  
                if not item:  
                    continue  
                parts = item.split(",")  
                if len(parts) >= 6:  
                    try:  
                        attributes.append({  
                            "id":           int(parts[0]),  
                            "initValue":    float(parts[1]),  
                            "minValue":     float(parts[2]),  
                            "maxValue":     float(parts[3]),  
                            "useMinValue":  parts[4].strip().lower() in ("true", "1"),  
                            "useMaxValue":  parts[5].strip().lower() in ("true", "1"),  
                        })  
                    except ValueError:  
                        pass  
  
        result.append({  
            "id":         int(id_val),  
            "name":       str(name_val),  
            "desc":       str(desc_val),  
            "attributes": attributes,  
        })  
    return result  
  
  
def write_attrsets(attrsets):  
    """将 attrsets 写回 Excel，按 ID 升序排列，只覆盖数据区。"""  
    wb = openpyxl.load_workbook(XLSX_PATH)  
    ws = wb.active  
  
    # 清空旧数据行  
    max_row = ws.max_row  
    for r in range(DATA_START_ROW, max_row + 1):  
        for c in [COL_ID, COL_NAME, COL_DESC, COL_ATTRIBUTE]:  
            ws.cell(row=r, column=c).value = None  
  
    for i, attrset in enumerate(sorted(attrsets, key=lambda s: s["id"])):  
        r = DATA_START_ROW + i  
        ws.cell(row=r, column=COL_ID).value   = attrset["id"]  
        ws.cell(row=r, column=COL_NAME).value = attrset["name"]  
        ws.cell(row=r, column=COL_DESC).value = attrset["desc"]  
  
        # 序列化 Attribute 列  
        parts = []  
        for a in attrset.get("attributes", []):  
            use_min = "true" if a.get("useMinValue") else "false"  
            use_max = "true" if a.get("useMaxValue") else "false"  
            parts.append(  
                f"{a['id']},{a['initValue']},{a['minValue']},{a['maxValue']},{use_min},{use_max}"  
            )  
        ws.cell(row=r, column=COL_ATTRIBUTE).value = ";".join(parts)  
  
    wb.save(XLSX_PATH)  
  
  
# ── 校验 ─────────────────────────────────────────────────────────────────────  
def validate_attrsets(attrsets):  
    """校验：name不为空、不重复、ID不重复。返回 error string 或 None。"""  
    names = [s["name"].strip() for s in attrsets]  
    if any(not n for n in names):  
        return "属性集名称不能为空"  
    if len(names) != len(set(names)):  
        return "存在重复的属性集名称"  
    ids = [s["id"] for s in attrsets]  
    if len(ids) != len(set(ids)):  
        return "存在重复的属性集 ID"  
    # 校验每个 AttributeSet 内部的 Attribute ID 不重复  
    for s in attrsets:  
        attr_ids = [a["id"] for a in s.get("attributes", [])]  
        if len(attr_ids) != len(set(attr_ids)):  
            return f"属性集「{s['name']}」内存在重复的 Attribute ID"  
    return None  
  
  
# ── HTTP Handler ─────────────────────────────────────────────────────────────  
class Handler(BaseHTTPRequestHandler):  
    def log_message(self, format, *args):  
        pass  # 静默日志  
  
    def send_json(self, data, status=200):  
        body = json.dumps(data, ensure_ascii=False).encode()  
        self.send_response(status)  
        self.send_header("Content-Type", "application/json; charset=utf-8")  
        self.send_header("Content-Length", str(len(body)))  
        self.send_header("Access-Control-Allow-Origin", "*")  
        self.end_headers()  
        self.wfile.write(body)  
  
    def send_file(self, path, mime):  
        with open(path, "rb") as f:  
            data = f.read()  
        self.send_response(200)  
        self.send_header("Content-Type", mime)  
        self.send_header("Content-Length", str(len(data)))  
        self.end_headers()  
        self.wfile.write(data)  
  
    def do_OPTIONS(self):  
        self.send_response(204)  
        self.send_header("Access-Control-Allow-Origin", "*")  
        self.send_header("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")  
        self.send_header("Access-Control-Allow-Headers", "Content-Type")  
        self.end_headers()  
  
    def do_GET(self):  
        p = urlparse(self.path).path  
        base = os.path.dirname(os.path.abspath(__file__))  
  
        if p == "/" or p == "/index.html":  
            self.send_file(os.path.join(base, "attrset_editor.html"), "text/html; charset=utf-8")  
        elif p == "/attrset_editor.css":  
            self.send_file(os.path.join(base, "attrset_editor.css"), "text/css; charset=utf-8")  
        elif p == "/attrset_editor.js":  
            self.send_file(os.path.join(base, "attrset_editor.js"), "application/javascript; charset=utf-8")  
        elif p == "/api/attrsets":  
            try:  
                self.send_json({"ok": True, "attrsets": read_attrsets()})  
            except Exception as e:  
                self.send_json({"ok": False, "error": str(e)}, 500)  
        elif p == "/api/info":  
            self.send_json({"ok": True, "xlsx": XLSX_PATH})  
        else:  
            self.send_json({"ok": False, "error": "Not Found"}, 404)  
  
    def read_body(self):  
        length = int(self.headers.get("Content-Length", 0))  
        return json.loads(self.rfile.read(length)) if length else {}  
  
    def do_POST(self):  
        p = urlparse(self.path).path  
        if p == "/api/attrsets":  
            try:  
                data    = self.read_body()  
                attrsets = read_attrsets()  
                # 自动分配 ID  
                new_id = data.get("id")  
                if not new_id:  
                    new_id = max((s["id"] for s in attrsets), default=0) + 1  
                new_set = {  
                    "id":         int(new_id),  
                    "name":       data.get("name", "").strip(),  
                    "desc":       data.get("desc", ""),  
                    "attributes": data.get("attributes", []),  
                }  
                attrsets.append(new_set)  
                err = validate_attrsets(attrsets)  
                if err:  
                    self.send_json({"ok": False, "error": err}, 400)  
                    return  
                write_attrsets(attrsets)  
                self.send_json({"ok": True, "attrset": new_set})  
            except Exception as e:  
                self.send_json({"ok": False, "error": str(e)}, 500)  
        else:  
            self.send_json({"ok": False, "error": "Not Found"}, 404)  
  
    def do_PUT(self):  
        p = urlparse(self.path).path  
        # /api/attrsets/{id}  
        if p.startswith("/api/attrsets/"):  
            try:  
                old_id   = int(p.split("/")[-1])  
                data     = self.read_body()  
                attrsets = read_attrsets()  
                idx = next((i for i, s in enumerate(attrsets) if s["id"] == old_id), None)  
                if idx is None:  
                    self.send_json({"ok": False, "error": "未找到该属性集"}, 404)  
                    return  
                attrsets[idx] = {  
                    "id":         int(data.get("id", old_id)),  
                    "name":       data.get("name", "").strip(),  
                    "desc":       data.get("desc", ""),  
                    "attributes": data.get("attributes", []),  
                }  
                err = validate_attrsets(attrsets)  
                if err:  
                    self.send_json({"ok": False, "error": err}, 400)  
                    return  
                write_attrsets(attrsets)  
                self.send_json({"ok": True, "attrset": attrsets[idx]})  
            except Exception as e:  
                self.send_json({"ok": False, "error": str(e)}, 500)  
        else:  
            self.send_json({"ok": False, "error": "Not Found"}, 404)  
  
    def do_DELETE(self):  
        p = urlparse(self.path).path  
        if p.startswith("/api/attrsets/"):  
            try:  
                del_id   = int(p.split("/")[-1])  
                attrsets = read_attrsets()  
                attrsets = [s for s in attrsets if s["id"] != del_id]  
                write_attrsets(attrsets)  
                self.send_json({"ok": True})  
            except Exception as e:  
                self.send_json({"ok": False, "error": str(e)}, 500)  
        else:  
            self.send_json({"ok": False, "error": "Not Found"}, 404)  
  
  
# ── 入口 ─────────────────────────────────────────────────────────────────────  
def main():  
    global XLSX_PATH  
    ap = argparse.ArgumentParser(description="EX-GAS AttributeSet 网页编辑器服务")  
    ap.add_argument("--xlsx", required=True, help="#exgas.attributeSet.xlsx 路径")  
    ap.add_argument("--port", type=int, default=8767)  
    ap.add_argument("--no-browser", action="store_true")  
    args = ap.parse_args()  
  
    XLSX_PATH = os.path.abspath(args.xlsx)  
    if not os.path.exists(XLSX_PATH):  
        print(f"[ERROR] 文件不存在: {XLSX_PATH}")  
        sys.exit(1)  
  
    url = f"http://127.0.0.1:{args.port}"  
    print(f"[EX-GAS AttributeSet Editor] {url}")  
    print(f"  Excel: {XLSX_PATH}")  
    print(f"  Ctrl+C 停止")  
  
    if not args.no_browser:  
        threading.Timer(0.8, lambda: webbrowser.open(url)).start()  
  
    try:  
        HTTPServer(("127.0.0.1", args.port), Handler).serve_forever()  
    except KeyboardInterrupt:  
        print("\n[停止]")  
  
  
if __name__ == "__main__":  
    main()