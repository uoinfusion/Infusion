// Statuses window outside the game (replacement of the WPF statuses window).
// Infusion serves a small page on http://127.0.0.1:47318/ and opens it in Chrome app mode
// (a small window without the browser toolbar), otherwise in the default browser.
// Clicking a party member targets it (same as on Windows). Command: ,status-window
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Infusion.LegacyApi;

namespace Infusion.Scripts.UOErebor.Extensions.StatusBars
{
    public static class StatusWindow
    {
        public const int Port = 47318;
        public static string Url => $"http://127.0.0.1:{Port}/";

        private static readonly object startLock = new object();
        private static bool started;
        private static HttpListener listener;

        public static void Start()
        {
            lock (startLock)
            {
                if (started)
                    return;
                started = true;
            }

            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add(Url);
                listener.Start();
            }
            catch (Exception ex)
            {
                try { UO.Log($"Status window: cannot start the server on {Url}: {ex.Message}"); } catch { }
                return;
            }

            var thread = new Thread(Serve) { IsBackground = true, Name = "StatusWindow" };
            thread.Start();

            try
            {
                UO.RegisterCommand("status-window", OpenWindow);
            }
            catch { }

            try { StatusGump.Register(); } catch { }
        }

        /// <summary>Opens the window (Chrome in app mode, otherwise the default browser).</summary>
        public static void OpenWindow()
        {
            Start();
            try
            {
                if (Directory.Exists("/Applications/Google Chrome.app"))
                {
                    var psi = new ProcessStartInfo("/usr/bin/open") { UseShellExecute = false };
                    foreach (var a in new[] { "-na", "Google Chrome", "--args", "--app=" + Url, "--window-size=300,560" })
                        psi.ArgumentList.Add(a);
                    Process.Start(psi);
                }
                else
                {
                    Process.Start("/usr/bin/open", new[] { Url });
                }
            }
            catch (Exception ex)
            {
                try { UO.Log("Status window: " + ex.Message); } catch { }
            }
        }

        private static void Serve()
        {
            while (listener != null && listener.IsListening)
            {
                HttpListenerContext ctx;
                try { ctx = listener.GetContext(); }
                catch { break; }

                try { Handle(ctx); }
                catch { try { ctx.Response.StatusCode = 500; ctx.Response.Close(); } catch { } }
            }
        }

        private static void Handle(HttpListenerContext ctx)
        {
            var path = ctx.Request.Url.AbsolutePath;
            string body;
            string type = "text/html; charset=utf-8";

            if (path == "/data")
            {
                body = BuildJson();
                type = "application/json; charset=utf-8";
            }
            else if (path == "/target")
            {
                if (uint.TryParse(ctx.Request.QueryString["id"], out var id))
                {
                    var owner = Statuses.AllInstances().FirstOrDefault(s => s.Contains((ObjectId)id));
                    if (owner != null)
                        Task.Run(() => { try { owner.OnMobileTargeted((ObjectId)id); } catch (Exception ex) { UO.Log(ex.Message); } });
                }
                body = "ok";
                type = "text/plain";
            }
            else
            {
                body = Page;
            }

            var bytes = Encoding.UTF8.GetBytes(body);
            ctx.Response.ContentType = type;
            ctx.Response.Headers["Cache-Control"] = "no-store";
            ctx.Response.ContentLength64 = bytes.Length;
            ctx.Response.OutputStream.Write(bytes, 0, bytes.Length);
            ctx.Response.Close();
        }

        private static string BuildJson()
        {
            object me = null;
            try
            {
                var m = UO.Me;
                if (m != null)
                    me = new
                    {
                        name = m.Name,
                        hp = (int)m.CurrentHealth, maxHp = (int)m.MaxHealth,
                        mana = (int)m.CurrentMana, maxMana = (int)m.MaxMana,
                        stam = (int)m.CurrentStamina, maxStam = (int)m.MaxStamina,
                        str = (int)m.Strength, dex = (int)m.Dexterity, @int = (int)m.Intelligence,
                        armor = (int)m.Armor,
                        weight = (int)m.Weight, maxWeight = (int)m.Strength * 4,
                        poisoned = m.IsPoisoned, dead = m.IsDead, hidden = m.IsHidden,
                    };
            }
            catch { }

            var groups = Statuses.AllInstances().Select(s => new
            {
                title = s.Title,
                bars = s.Snapshot().Select(b =>
                {
                    Mobile mob = null;
                    try { mob = UO.Mobiles[(ObjectId)b.Id]; } catch { }
                    return new
                    {
                        id = b.Id,
                        name = (b.NamePrefix ?? "") + (string.IsNullOrEmpty(b.Name) ? mob?.Name ?? "?" : b.Name),
                        hp = mob != null ? (int)mob.CurrentHealth : b.CurrentHealth,
                        maxHp = mob != null ? (int)mob.MaxHealth : b.MaxHealth,
                        dead = mob != null ? mob.IsDead : b.IsDead,
                        poisoned = mob != null ? mob.IsPoisoned : b.IsPoisoned,
                        away = mob == null || b.IsOutOfSight,
                        type = b.Type.ToString(),
                    };
                }).ToArray(),
            }).Where(g => g.bars.Length > 0).ToArray();

            var lines = ScriptStatus.Current().Select(p => new { text = p.Text, remaining = p.Remaining, s = p.Seconds }).ToArray();
            var warnings = ScriptStatus.Warnings();

            return JsonSerializer.Serialize(new { me, lines, warnings, groups });
        }

        private const string Page = @"<!doctype html>
<html lang=""en""><head><meta charset=""utf-8""><title>Infusion - statuses</title>
<meta name=""viewport"" content=""width=device-width"">
<style>
:root{--bg:#15171c;--panel:#1d2027;--text:#e6e6e6;--dim:#8b909c;--hp:#3fae5a;--poison:#9acd32;--mana:#3b7dd8;--stam:#d8a23b;--dead:#555;--line:#2a2e37}
*{box-sizing:border-box}body{margin:0;background:var(--bg);color:var(--text);font:13px/1.3 -apple-system,system-ui,sans-serif;user-select:none}
.box{background:var(--panel);margin:6px;border-radius:6px;padding:6px 8px}
h2{font-size:11px;letter-spacing:.06em;text-transform:uppercase;color:var(--dim);margin:2px 0 6px}
.name{font-weight:600;display:flex;justify-content:space-between}
.bar{position:relative;height:14px;background:#2b2f39;border-radius:3px;margin:3px 0;overflow:hidden}
.bar>i{position:absolute;inset:0 auto 0 0;background:var(--hp)}
.bar>span{position:absolute;inset:0;text-align:center;font-size:11px;line-height:14px;color:#fff;text-shadow:0 1px 1px #000}
.mana>i{background:var(--mana)}.stam>i{background:var(--stam)}
.grid{display:grid;grid-template-columns:repeat(3,1fr);gap:2px 8px;margin-top:4px;color:var(--dim)}
.grid b{color:var(--text);font-weight:600}
.m{padding:3px 2px;border-top:1px solid var(--line);cursor:pointer}.m:first-of-type{border-top:0}
.m:hover{background:#252933}
.m.away{opacity:.45}.m.dead .bar>i{background:var(--dead)}.m.poison .bar>i,.me.poison .hpb>i{background:var(--poison)}
.tag{font-size:10px;color:var(--dim);font-weight:400}
.empty{color:var(--dim);font-size:12px}
.st{display:flex;justify-content:space-between;padding:2px 0}.st b{font-weight:600;font-variant-numeric:tabular-nums}.st b.soon{color:#e8913a}
.off{color:#d86a3b;text-align:center;padding:20px}
body.fail,body.fail .box{color:#ff5a5a}.warn{color:#ff5a5a;font-weight:600;padding:2px 0}
</style></head><body>
<div id=""app""><div class=""off"">Waiting for Infusion...</div></div>
<script>
const pct=(a,b)=>b>0?Math.max(0,Math.min(100,a*100/b)):0;
const bar=(cls,a,b)=>`<div class=""bar ${cls}""><i style=""width:${pct(a,b)}%""></i><span>${a} / ${b}</span></div>`;
const esc=s=>String(s).replace(/[&<>""]/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','""':'&quot;'}[c]));
function render(d){
 let h='';
 document.body.classList.toggle('fail',!!(d.warnings&&d.warnings.length));
 if(d.warnings&&d.warnings.length){h+=`<div class=""box"">`;for(const w of d.warnings)h+=`<div class=""warn"">! ${esc(w)}</div>`;h+=`<div class=""empty"">try ,reload and check the log</div></div>`;}
 if(d.me){const m=d.me;
  h+=`<div class=""box me ${m.poisoned?'poison':''}""><div class=""name""><span>${esc(m.name||'')}</span><span class=""tag"">${m.dead?'dead':m.hidden?'hidden':''}</span></div>`+
   bar('hpb',m.hp,m.maxHp)+bar('mana',m.mana,m.maxMana)+bar('stam',m.stam,m.maxStam)+
   `<div class=""grid""><span>Str <b>${m.str}</b></span><span>Dex <b>${m.dex}</b></span><span>Int <b>${m.int}</b></span>`+
   `<span>Armor <b>${m.armor}</b></span><span style=""grid-column:span 2"">Weight <b>${m.weight} / ${m.maxWeight}</b></span></div></div>`;}
 if(d.lines&&d.lines.length){h+=`<div class=""box""><h2>Active</h2>`;for(const s of d.lines)h+=`<div class=""st""><span>${esc(s.text)}</span><b class=""${s.s>=0&&s.s<=10?'soon':''}"">${esc(s.remaining||'on')}</b></div>`;h+=`</div>`;}
 if(!d.groups.length) h+=`<div class=""box""><h2>Party</h2><div class=""empty"">Empty - add members with ,party-add</div></div>`;
 for(const g of d.groups){
  h+=`<div class=""box""><h2>${esc(g.title)}</h2>`;
  for(const b of g.bars){
   const st=b.dead?'dead':b.away?'out of sight':b.poisoned?'poisoned':'';
   h+=`<div class=""m ${b.away?'away':''} ${b.dead?'dead':''} ${b.poisoned?'poison':''}"" data-id=""${b.id}"" title=""Click = target"">`+
    `<div class=""name""><span>${esc(b.name)}</span><span class=""tag"">${st}</span></div>`+bar('',b.hp,b.maxHp)+`</div>`;}
  h+=`</div>`;}
 document.getElementById('app').innerHTML=h;}
document.addEventListener('click',e=>{const m=e.target.closest('.m');if(m)fetch('/target?id='+m.dataset.id,{method:'POST'});});
async function tick(){try{const r=await fetch('/data',{cache:'no-store'});render(await r.json());}
 catch(e){document.getElementById('app').innerHTML='<div class=""off"">Infusion is not running</div>';}
 setTimeout(tick,400);}
tick();
</script></body></html>";
    }
}
