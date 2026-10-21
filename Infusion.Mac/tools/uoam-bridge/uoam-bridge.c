/*
 * uoam-bridge.exe - runs under Wine next to uoam.exe (UO Automap).
 * It creates the "UOASSIST-TP-MSG-WND" window (the same as the uoam.csx script does on Windows),
 * so UOAM thinks a UO client with the UOAssist API is running.
 * The player position comes over TCP from Infusion for macOS (127.0.0.1:47317),
 * text messages from UOAM are sent back to Infusion, which shows them in game.
 *
 * Build (MinGW): i686-w64-mingw32-gcc -O2 -o uoam-bridge.exe uoam-bridge.c -lws2_32 -static
 *
 * Protocol (lines, ASCII/UTF-8):
 *   Infusion -> bridge:  "P <x> <y>\n"                    position
 *                        "S <msg> <wparam> <lparam>\n"   SendMessage to the UOAM window (CUOAMWindow)
 *   bridge -> Infusion:  "T <wparam> <text>\n"            message from UOAM (WM_USER+207)
 */
#define WIN32_LEAN_AND_MEAN
#include <winsock2.h>
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define API_KEY "UOASSIST-TP-MSG-WND"
#define PORT 47317

static volatile LONG g_x = 0, g_y = 0;
static volatile SOCKET g_sock = INVALID_SOCKET;
static CRITICAL_SECTION g_sendLock;
static int g_commandOffset = 0;
static HWND g_hwnd;

static void send_line(const char *line)
{
    EnterCriticalSection(&g_sendLock);
    SOCKET s = g_sock;
    if (s != INVALID_SOCKET)
        send(s, line, (int)strlen(line), 0);
    LeaveCriticalSection(&g_sendLock);
}

static void read_atom_and_delete(ATOM atom, char *buf, int size)
{
    buf[0] = 0;
    if (GlobalGetAtomNameA(atom, buf, size) == 0)
        buf[0] = 0;
    GlobalDeleteAtom(atom);
}

static LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    char text[512];
    char line[640];

    switch (msg)
    {
    case WM_USER + 200:
        return 1;
    case WM_USER + 202:
        return (LRESULT)((g_x & 0xFFFF) | ((g_y & 0xFFFF) << 16));
    case WM_USER + 207:
        read_atom_and_delete((ATOM)lParam, text, sizeof(text));
        /* text from UOAM is in CP1250 - convert to UTF-8 */
        {
            wchar_t wide[512];
            char utf8[1024];
            int n = MultiByteToWideChar(1250, 0, text, -1, wide, 512);
            if (n > 0 && WideCharToMultiByte(CP_UTF8, 0, wide, -1, utf8, sizeof(utf8), NULL, NULL) > 0)
                snprintf(line, sizeof(line), "T %u %s\n", (unsigned)(wParam & 0xFFFF), utf8);
            else
                snprintf(line, sizeof(line), "T %u %s\n", (unsigned)(wParam & 0xFFFF), text);
        }
        send_line(line);
        return 0;
    case WM_USER + 209:
        read_atom_and_delete((ATOM)lParam, text, sizeof(text));
        if (wParam == 0)
            return 0;
        g_commandOffset++;
        return WM_USER + 0x400 + g_commandOffset;
    case WM_APP + 1: /* SendMessage request to UOAM from the TCP thread */
        {
            int *args = (int *)lParam;
            HWND uoam = FindWindowA("CUOAMWindow", NULL);
            if (uoam)
                SendMessageA(uoam, (UINT)args[0], (WPARAM)args[1], (LPARAM)args[2]);
            free(args);
        }
        return 0;
    }
    return DefWindowProcA(hwnd, msg, wParam, lParam);
}

static void handle_line(char *line)
{
    if (line[0] == 'P')
    {
        int x, y;
        if (sscanf(line + 1, "%d %d", &x, &y) == 2)
        {
            InterlockedExchange(&g_x, x);
            InterlockedExchange(&g_y, y);
        }
    }
    else if (line[0] == 'S')
    {
        int m, w, l;
        if (sscanf(line + 1, "%d %d %d", &m, &w, &l) == 3)
        {
            int *args = (int *)malloc(3 * sizeof(int));
            args[0] = m; args[1] = w; args[2] = l;
            PostMessageA(g_hwnd, WM_APP + 1, 0, (LPARAM)args);
        }
    }
}

static DWORD WINAPI NetThread(LPVOID unused)
{
    (void)unused;
    for (;;)
    {
        SOCKET s = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        struct sockaddr_in addr;
        memset(&addr, 0, sizeof(addr));
        addr.sin_family = AF_INET;
        addr.sin_port = htons(PORT);
        addr.sin_addr.s_addr = htonl(INADDR_LOOPBACK);

        if (connect(s, (struct sockaddr *)&addr, sizeof(addr)) != 0)
        {
            closesocket(s);
            Sleep(2000);
            continue;
        }

        printf("Pripojeno k Infusionu.\n");
        fflush(stdout);
        EnterCriticalSection(&g_sendLock);
        g_sock = s;
        LeaveCriticalSection(&g_sendLock);

        char buf[1024];
        int len = 0;
        for (;;)
        {
            int r = recv(s, buf + len, (int)sizeof(buf) - 1 - len, 0);
            if (r <= 0)
                break;
            len += r;
            buf[len] = 0;
            char *start = buf, *nl;
            while ((nl = strchr(start, '\n')) != NULL)
            {
                *nl = 0;
                handle_line(start);
                start = nl + 1;
            }
            len = (int)strlen(start);
            memmove(buf, start, len + 1);
            if (len >= (int)sizeof(buf) - 1)
                len = 0;
        }

        EnterCriticalSection(&g_sendLock);
        g_sock = INVALID_SOCKET;
        LeaveCriticalSection(&g_sendLock);
        closesocket(s);
        printf("Infusion odpojen, cekam...\n");
        fflush(stdout);
        Sleep(1000);
    }
    return 0;
}

int main(void)
{
    WSADATA wsa;
    WSAStartup(MAKEWORD(2, 2), &wsa);
    InitializeCriticalSection(&g_sendLock);

    WNDCLASSA wc;
    memset(&wc, 0, sizeof(wc));
    wc.lpfnWndProc = WndProc;
    wc.hInstance = GetModuleHandleA(NULL);
    wc.lpszClassName = API_KEY;
    if (!RegisterClassA(&wc) && GetLastError() != ERROR_CLASS_ALREADY_EXISTS)
    {
        printf("RegisterClass selhal\n");
        return 1;
    }

    g_hwnd = CreateWindowExA(0, API_KEY, API_KEY, 0, 0, 0, 0, 0, NULL, NULL, wc.hInstance, NULL);
    if (!g_hwnd)
    {
        printf("CreateWindow selhal\n");
        return 1;
    }

    /* UOAM enables "UO -> Track Player" only when it finds a UO client window (class "Ultima Online").
       ClassicUO runs natively on macOS, so a hidden substitute window is created under Wine. */
    {
        WNDCLASSA uo;
        memset(&uo, 0, sizeof(uo));
        uo.lpfnWndProc = DefWindowProcA;
        uo.hInstance = wc.hInstance;
        uo.lpszClassName = "Ultima Online";
        RegisterClassA(&uo);
        CreateWindowExA(0, "Ultima Online", "Ultima Online", 0, 0, 0, 0, 0, NULL, NULL, wc.hInstance, NULL);
    }

    printf("UOAM most bezi (port %d).\n", PORT);
    fflush(stdout);
    CreateThread(NULL, 0, NetThread, NULL, 0, NULL);

    MSG msg;
    while (GetMessageA(&msg, NULL, 0, 0) > 0)
    {
        TranslateMessage(&msg);
        DispatchMessageA(&msg);
    }
    return 0;
}
