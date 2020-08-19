using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Add_unit_formation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            numericUpDown1.Value = 42;
        }
        private Byte[] exe;
        private string GamePath;
        public void Injection(UInt32 Addresse, string value)
        {
            string Value = string.Empty;
            string[] MybyteValue;
            UInt32 cpt = 0;
            Byte[] aocExe = exe;

            if (aocExe.Length < 0x293040)
            {
                ExtendExeFile();
                //remap aoc byetes
                aocExe = File.ReadAllBytes(GamePath);
            }

            if (Addresse > 0x2FFFFF)
            {
                if (Addresse < 0x7A5000)
                {
                    Addresse -= 0x400000;
                }
                else
                {
                    Addresse -= 0x512000;
                }
            }

            Value = Regex.Replace(value, ".{2}", "$0 ");

            MybyteValue = Value.Split(' ');
            foreach (var item in MybyteValue)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int val = int.Parse(item, System.Globalization.NumberStyles.HexNumber);
                    var b = aocExe[Addresse + cpt];
                    aocExe[Addresse + cpt] = (byte)val;
                    var bb = aocExe[Addresse + cpt];
                }
                cpt++;
            }
            cpt = 0;
            exe = aocExe;
        }
        public void ExtendExeFile()
        {
            UInt32 CurrentAddresse = 0;
            Byte[] aocExe = File.ReadAllBytes(GamePath);
            Byte[] NewExe = new byte[0x300000];
            //copy old in new 
            foreach (var bytes in aocExe)
            {
                if (CurrentAddresse < 0x29302D)
                {
                    NewExe[CurrentAddresse] = bytes;// aocExe[CurrentAddresse];
                    CurrentAddresse++;
                }
                else
                {
                    break;
                }
            }
            File.WriteAllBytes(GamePath, NewExe);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string gameFolder = Path.GetDirectoryName(this.GamePath);
            exe = File.ReadAllBytes(GamePath);
            //.pdata .patch
            Injection(0x100, "504500004C010900");
            Injection(0x150, "00F03E00");
            Injection(0x310, "2E7064617461000000100000002038000010000000102700000000000000000000000000E00000E02E7061746368000000C006000030380000C0060000202700000000000000000000000000E00000E0");

            //004E5C57    -E9 A4F32F00    JMP empires2.007E5000
            Injection(0x4E5C57, "E9A4F32F00");

            // 007E5000
            Injection(0x2D3ffd, "906690E84BAFCFFF6A006A008B961C1000006A006A006A006AFF683FA000006A0068AE0000006A2A6A0E528BCEE821AFCFFFE9280CD0FF90");
            //007E2046 0x2D1046
            //007E5000       2FBA‬
            //007E5020   6A 2A            PUSH 2A
            int nbToAdd = (int)numericUpDown1.Value;
            string ImageToChange = nbToAdd.ToString("X").PadLeft(2, '0');
            Injection(0x2D4023, "6A"+ ImageToChange);
            //0043A963 

            //005B30B1   > 8B5424 14      MOV EDX,DWORD PTR SS:[ESP+14]            ;  Case 66 of switch 005B2F2E
            // 005B30B5. 8D42 FF LEA EAX,DWORD PTR DS:[EDX - 1]; Switch(cases 1..AB)
            //005B30B8. 3D AA000000 CMP EAX,0AA
            //2A jump in 57

            //select unit even initialisation ?
            //0053B110 |.C2 0400        RETN 4
            //0053B113 |> 8D5424 03      LEA EDX, DWORD PTR SS:[ESP+3]             ;  Case 54 of switch 0053AE3A
            //0053B117  |. 8D4424 03      LEA EAX, DWORD PTR SS:[ESP+3]
            //0053B11B  |. 52             PUSH EDX
            //0053B11C  |. 8D5424 07      LEA EDX, DWORD PTR SS:[ESP+7]
            //0053B120  |. 50             PUSH EAX
            //0053B121  |. 52             PUSH EDX
            //0053B122  |. 6A 01          PUSH 1
            //0053B124  |. 6A 0C PUSH 0C
            //0053B126  |. E8 257DF3FF CALL empires2.00472E50
            //0053B12B  |. 59             POP ECX
            //0053B12C  |. C2 0400        RETN 4
            //0053B12F  |> 8D4424 03      LEA EAX, DWORD PTR SS:[ESP+3]             ;  Case 55 of switch 0053AE3A
            //0053B133  |. 8D5424 03      LEA EDX, DWORD PTR SS:[ESP+3]
            //0053B137  |. 50             PUSH EAX
            //0053B138  |. 8D4424 07      LEA EAX, DWORD PTR SS:[ESP+7]
            //0053B13C  |. 52             PUSH EDX
            //0053B13D  |. 50             PUSH EAX
            //0053B13E  |. 6A 02          PUSH 2
            //0053B140  |. 6A 0C PUSH 0C
            //0053B142  |. E8 097DF3FF CALL empires2.00472E50
            //0053B147  |. 59             POP ECX
            //0053B148  |. C2 0400        RETN 4
            //0053B14B  |> 8D5424 03      LEA EDX, DWORD PTR SS:[ESP+3]             ;  Case 56 of switch 0053AE3A
            //0053B14F  |. 8D4424 03      LEA EAX, DWORD PTR SS:[ESP+3]
            //0053B153  |. 52             PUSH EDX
            //0053B154  |. 8D5424 07      LEA EDX, DWORD PTR SS:[ESP+7]
            //0053B158  |. 50             PUSH EAX
            //0053B159  |. 52             PUSH EDX
            //0053B15A  |. 6A 03          PUSH 3
            //0053B15C  |. 6A 0C PUSH 0C
            //0053B15E  |. E8 ED7CF3FF    CALL empires2.00472E50
            //0053B163  |. 59             POP ECX
            //0053B164  |. C2 0400        RETN 4
            //0053B167  |> 8D4424 03      LEA EAX, DWORD PTR SS:[ESP+3]             ;  Case 57 of switch 0053AE3A


            //push button event
            //4DE71C |. 83C4 0C ADD ESP,0C
            //004DE71F |.C2 0C00 RETN 0C
            //004DE722 |> 6A 04          PUSH 4; Case 54 of switch 004DE079
            //004DE724 |. 8BCE MOV ECX,ESI
            //004DE726 |.E8 45790000    CALL empires2.004E6070
            //004DE72B |. 8B4C24 04      MOV ECX, DWORD PTR SS:[ESP+4]
            //004DE72F  |. 64:890D 000000>MOV DWORD PTR FS:[0],ECX
            //004DE736  |. 5E             POP ESI
            //004DE737  |. 83C4 0C ADD ESP,0C
            //004DE73A  |. C2 0C00 RETN 0C
            //004DE73D  |> 6A 02          PUSH 2                                   ;  Case 55 of switch 004DE079
            //004DE73F  |. 8BCE MOV ECX,ESI
            //004DE741  |. E8 2A790000 CALL empires2.004E6070
            //004DE746  |. 8B4C24 04      MOV ECX, DWORD PTR SS:[ESP+4]
            //004DE74A  |. 64:890D 000000>MOV DWORD PTR FS:[0],ECX
            //004DE751  |. 5E             POP ESI
            //004DE752  |. 83C4 0C ADD ESP,0C
            //004DE755  |. C2 0C00 RETN 0C
            //004DE758  |> 6A 07          PUSH 7                                   ;  Case 56 of switch 004DE079
            //004DE75A  |. 8BCE MOV ECX,ESI
            //004DE75C  |. E8 0F790000    CALL empires2.004E6070
            //004DE761  |. 8B4C24 04      MOV ECX, DWORD PTR SS:[ESP+4]
            //004DE765  |. 64:890D 000000>MOV DWORD PTR FS:[0],ECX
            //004DE76C  |. 5E             POP ESI
            //004DE76D  |. 83C4 0C ADD ESP,0C
            //004DE770  |. C2 0C00 RETN 0C
            //004DE773  |> 6A 08          PUSH 8                                   ;  Case 57 of switch 004DE079
            //004DE775  |. 8BCE MOV ECX,ESI
            //004DE777  |. E8 F4780000    CALL empires2.004E6070
            //004DE77C  |. 8B4C24 04      MOV ECX, DWORD PTR SS:[ESP+4]
            //004DE780  |. 64:890D 000000>MOV DWORD PTR FS:[0],ECX
            //004DE787  |. 5E             POP ESI
            //004DE788  |. 83C4 0C ADD ESP,0C
            //004DE78B  |. C2 0C00 RETN 0C


            //formation fonction
            //0043CEC0  /$ 83EC 0C        SUB ESP,0C


            //conclusion  4 function for 4 formation:
            //0046CD1D
            //0046CD50
            //0046CDAD
            //0046CCF6

            //add button click event fuction

            //004DEE9A    -E9 9C613000    JMP empires2.007E503F
            Injection(0x4DEE9A, "E9A0613000");
            //added formation function
            //007E503F   5E               POP ESI
            Injection(0x2D403F, "9090909090909090909090909090903DAE000000751B6A038BCEE81210D0FF8B4C240464890D000000005E83C40CC20C003DAF00000074588B4C24045E64890D0000000083C40CE91E9ECFFF");

            //007E50CF   6A 05            PUSH 5
            Injection(0x2D40cF, "6A05EB84");


            //007E502F   EB 62            JMP SHORT Empires2.007E5093
            Injection(0x2D402F, "EB62909090");
            //
            Injection(0x2D4093, "90909090906A006A008B961C1000006A006A006A006AFF683FA000006A0068AF0000006A2C6A03528BCEE88EAECFFFE9950BD0FF9060");


            //add description to the button
            //defaultswitc case

            Injection(0x2D40DD, "81FAAE00000075168B5424208B01525668589C0000FF50245F5E5DC2140081FAAF00000075168B5424208B01525668599C0000FF50245F5E5DC214003EC64435FF008BC65F5E5DC21400");
            //005B30BD    -0F87 1A202300  JA Empires2.007E50DD
            Injection(0x5B30BD, "0F871A202300");


            //005B30BD   . 0F87 60070000  JA Empires2.005B3823

            //007E50DD   81FA AE000000    CMP EDX,0AE

            //hotkey
            //0053B14B |> 8D5424 03      LEA EDX, DWORD PTR SS:[ESP+3]             ;  Case 56 of switch 0053AE3A
            //0053B14F  |. 8D4424 03      LEA EAX, DWORD PTR SS:[ESP+3]
            //0053B153  |. 52             PUSH EDX
            //0053B154  |. 8D5424 07      LEA EDX, DWORD PTR SS:[ESP+7]
            //0053B158  |. 50             PUSH EAX
            //0053B159  |. 52             PUSH EDX
            //0053B15A     6A 03          PUSH 3
            //0053B15C  |. 6A 0C PUSH 0C
            //0053B15E  |. E8 ED7CF3FF    CALL Empires2.00472E50
            //0053B163  |. 59             POP ECX
            //0053B164  |. C2 0400        RETN 4

            //
            //0053AE42  |. 0F87 37040000  JA Empires2.0053B27F
            Injection(0x53AE42, "0F87E5A22A00");
            //007E512D   0000             ADD BYTE PTR DS:[EAX],AL
            Injection(0x2D412D, "81FAAF000000751C8D4424038D542403508D44240752506A056A0CE803DDC8FF59C2040081FAAE000000751C8D4424038D542403508D44240752506A066A0CE8DFDCC8FF59C2040033C059C20400");


            File.WriteAllBytes(GamePath, exe);



            string result = "STRINGTABLE" + Environment.NewLine;
            if (File.Exists("StringTable2502.rc"))
            {
                File.Delete("StringTable2502.rc");
            }
            if (File.Exists("resource.rc"))
            {
                File.Delete("resource.rc");
            }
            if (File.Exists("StringTable2502.res"))
            {
                File.Delete("StringTable2502.res");
            }

            
            string languagedll = Path.Combine(gameFolder, "language.dll");

            //extract ressources from language dll to know exactly the name of language table
            Process.Start(@"Resource_hack\ResourceHacker.exe", "-open \"" + languagedll + "\"  -save resource.rc  -action extract -mask STRINGTABLE,,");
            int compter = 0;
            do
            {
                Thread.Sleep(200);
                compter++;
                if (compter == 20)
                {
                    break;
                }
            }
            while (!File.Exists("resource.rc"));
            Thread.Sleep(1000);
            var fileResContent = File.ReadAllLines("resource.rc");

            string languageTable = fileResContent.ElementAt(1);

            result += languageTable + Environment.NewLine;// "LANGUAGE LANG_ENGLISH, SUBLANG_ENGLISH_US" + Environment.NewLine;
            result += "{" + Environment.NewLine;
            result += "  40016, 	\"About Face\"" + Environment.NewLine;
            result += "  40017, 	\"Wheel Right\"" + Environment.NewLine;
            result += "  40018, 	\"Wheel Left\"" + Environment.NewLine;
            result += "  40019, 	\"Horde\"" + Environment.NewLine;
            result += "  40020, 	\"Box Formation\"" + Environment.NewLine;
            result += "  40021, 	\"Line Formation\"" + Environment.NewLine;
            result += "  40022, 	\"Staggered Formation\"" + Environment.NewLine;
            result += "  40023, 	\"Flank Formation\"" + Environment.NewLine;
            result += "  40024, 	\"Triangle Formation\"" + Environment.NewLine;
            result += "  40025, 	\"Fusion Formation\"" + Environment.NewLine;
            result += "}";

            File.WriteAllText("StringTable2502.rc", result);
            //compile the new combobox Random map items names
            Process.Start(@"Resource_hack\ResourceHacker.exe", "-open StringTable2502.rc -save StringTable2502.res -action compile");

            Thread.Sleep(200);
            //add ressources to language.dll
            Process.Start(@"Resource_hack\ResourceHacker.exe", "-open \"" + languagedll + "\" -save \"" + languagedll + "\" -action addoverwrite -res StringTable2502.res -mask STRINGTABLE,,");

            //004AE650   . 83EC 08        SUB ESP,8
            //56 57 
            //004DE777   . E8 F4780000    CALL Empires2.004E6070




            //Test  004E60F9     83FF 03        CMP EDI,3


            MessageBox.Show("Done");
        }

        private void btn_Brw_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                GamePath = openFileDialog.FileName;
            }
        }
    }
}
