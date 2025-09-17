using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LibRimeDemo.Data;

namespace TerraJS.Contents.Utils
{
    public class IMEUtils
    {
        public static string GetInput(RimeSession session)
        {
            return session?.Context.Composition.Preedit.Replace(" ", "\'") ?? "";
        }

        public static List<string> GetCurrentPageCandidates(RimeSession session)
        {
            return [..session?.Context.Menu.Candidates.Select(c => c.Text) ?? []];
        }

        public static string GetCommit(RimeSession session)
        {
            return session?.CommitText ?? "";
        }

        public static int GetSelectCandidateIndex(RimeSession session)
        {
            return session?.Context.Menu.HighlightedCandidateIndex ?? -1;
        }

        public static void BackSpace(RimeSession session)
        {
            session.ProcessKey((int)X11KeySym.BackSpace);
        }

        public static void Escape(RimeSession session)
        {
            session.ProcessKey((int)X11KeySym.Escape);
        }

        public static void PrevPage(RimeSession session)
        {
            session.PrevPage();
        }

        public static void NextPage(RimeSession session)
        {
            session.NextPage();
        }

        public static void SelectPrev(RimeSession session)
        {
            session.ProcessKey((int)X11KeySym.Up);
        }

        public static void SelectNext(RimeSession session)
        {
            session.ProcessKey((int)X11KeySym.Down);
        }

        public static void DisableIME()
        {
            IntPtr hWnd = GetForegroundWindow();

            IntPtr himc = ImmGetContext(hWnd);

            if (himc != IntPtr.Zero)
            {
                try
                {
                    ImmSetOpenStatus(himc, false);

                    ImmSetConversionStatus(himc, IME_CMODE_ALPHANUMERIC, 0);
                }
                finally
                {
                    ImmReleaseContext(hWnd, himc);
                }
            }
        }

        private const int IME_CMODE_ALPHANUMERIC = 0x0000;
        private const int IME_CMODE_NATIVE = 0x0001;

        [DllImport("imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll")]
        public static extern bool ImmSetOpenStatus(IntPtr himc, bool bOpen);

        [DllImport("imm32.dll")]
        public static extern bool ImmSetConversionStatus(IntPtr himc, int dwConversion, int dwSentence);

        [DllImport("imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr himc);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetKeyboardLayout(int idThread);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetForegroundWindow();
    }
}
