using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S6Patcher.Source.Forms
{
    public partial class modFrm : Form
    {
        public modFrm()
        {
            InitializeComponent();
        }
        private void modFrm_Load(object sender, EventArgs e)
        {
            cdMain.ShowDialog();
        }
    }
}
