using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TopEng.Controls
{
    public partial class ProductInfoJIG : UserControl
    {
        public int id;
        public ProductInfoJIG()
        {
            InitializeComponent();
        }
        public void SetName(string name) => labelName.Text = name;
        
    }
}
