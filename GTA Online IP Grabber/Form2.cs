using LiteDB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTA_Online_IP_Grabber
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public class IP_Address
        {
            public string IP { get; set; }
            public string Country { get; set; }
            public string City { get; set; }

            public string State { get; set; }

            public string ZipCode { get; set; }

            public string TimeZone { get; set; }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            using (var db = new LiteDatabase(@"IP_Addresses.db"))
            {

                var col = db.GetCollection<IP_Address>("IP_Address");
                dataGridView1.DataSource = col.FindAll().ToList();
            }
        }
    }
}
