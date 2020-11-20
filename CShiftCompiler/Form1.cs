using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CShiftCompiler
{
    public partial class Form1 : Form
    {
        private int maxLineNumberCharLength;

        public Form1()
        {
            InitializeComponent();
        }

        private void scintilla1_Click(object sender, EventArgs e)
        {

        }

        private void scintilla1_TextChanged(object sender, EventArgs e)
        {
            EditorConfigration();
        }

        private void EditorConfigration()
        {
            // Did the number of characters in the line number display change
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = scintilla.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            scintilla.Margins[0].Width = scintilla.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;


            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 12;
            scintilla.Styles[Style.Default].BackColor = Color.FromArgb(30, 30, 30);
            scintilla.Styles[Style.Default].ForeColor = Color.White;
            scintilla.CaretForeColor = Color.White;

            scintilla.StyleClearAll();
            //Configure the CPP (C#) lexer styles
            scintilla.Styles[Style.Cpp.Default].ForeColor = Color.White;
            scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
            scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            scintilla.Styles[Style.Cpp.Number].ForeColor = Color.FromArgb(184, 215, 163); //184, 215, 163
            scintilla.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
            scintilla.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(214, 157, 133); // Red
            scintilla.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            scintilla.Styles[Style.Cpp.StringEol].BackColor = Color.White;
            scintilla.Styles[Style.Cpp.Operator].ForeColor = Color.White;
            scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
            var nums = scintilla.Margins[0];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            scintilla.Styles[Style.LineNumber].BackColor = Color.FromArgb(60, 60, 60);
            scintilla.Styles[Style.LineNumber].ForeColor = Color.FromArgb(43, 145, 175);

            //scintilla.Styles[Style.Default].BackColor = Color.Red;

            scintilla.Lexer = Lexer.Cpp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorConfigration();
            scintilla.ScrollWidth = 0;
            scintilla.ScrollWidthTracking = false;
        }
    }
}
