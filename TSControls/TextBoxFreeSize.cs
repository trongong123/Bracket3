using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Windows.Forms;

namespace TopEng.Controls
{
    class TextBoxFreeSize : TextBox
    {
        public override bool AutoSize { get => base.AutoSize; set => base.AutoSize = value; }
        public TextBoxFreeSize()
        {
            this.AutoSize = false;
        }
    }
}
